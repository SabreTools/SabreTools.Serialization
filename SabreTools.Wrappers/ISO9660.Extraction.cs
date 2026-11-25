using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Data.Models.ISO9660.SystemUseEntries;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class ISO9660 : IExtractable
    {
        #region Extraction State

        /// <summary>
        /// List of extracted directories by their sector offset
        /// </summary>
        private readonly HashSet<int> extractedDirectories = [];

        /// <summary>
        /// List of extracted files by their sector offset
        /// </summary>
        private readonly Dictionary<int, uint> extractedFiles = [];

        /// <summary>
        /// List of multi-extent files written, by their FileIdentifier
        /// </summary>
        /// <remarks>Used as last-extent has mutli-extent flag=0</remarks>
        private readonly List<byte[]> multiExtentFiles = [];

        /// <summary>
        /// List of CD-DA files written with a WAVE header, by their output path
        /// </summary>
        /// <remarks>Used to update the sizes in the header when a further extent is appended</remarks>
        private readonly List<string> waveFiles = [];

        /// <summary>
        /// List of extracted associated files by their output path, with their sector offset
        /// </summary>
        /// <remarks>Empty associated files can share their sector offset with each other and with an empty file</remarks>
        private readonly Dictionary<string, int> extractedAssociatedFiles = [];

        /// <summary>
        /// List of extracted associated files by their sector offset
        /// </summary>
        private readonly HashSet<int> extractedAssociatedFileLocations = [];

        /// <summary>
        /// List of output paths written with a Rock Ridge name, with the file identifier of their directory record
        /// </summary>
        /// <remarks>Paths are compared without case, so that a name taken on a case-insensitive file system is found</remarks>
        private readonly Dictionary<string, byte[]> rockRidgePaths = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// List of output paths written as Rock Ridge symbolic links
        /// </summary>
        /// <remarks>The extent of a symbolic link holds no data, and can be the extent of the file after it</remarks>
        private readonly HashSet<string> extractedLinks = [];

        /// <summary>
        /// Indicates if the directory hierarchy being extracted uses Rock Ridge
        /// </summary>
        private bool rockRidge;

        #endregion

        /// <summary>
        /// Source of the volume as raw 2352-byte CD-ROM sectors, if the volume was read from one
        /// </summary>
        /// <remarks>CD-DA files are extracted from it, as audio sectors have no 2048-byte user data</remarks>
        protected virtual Stream? RawSectorSource => null;

        /// <inheritdoc/>
        public virtual bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no volume or directory descriptors, there is nothing to extract
            if (VolumeDescriptorSet.Length == 0 || DirectoryDescriptors.Count == 0)
                return true;

            // Clear the extraction state
            extractedDirectories.Clear();
            extractedFiles.Clear();
            multiExtentFiles.Clear();
            waveFiles.Clear();
            extractedAssociatedFiles.Clear();
            extractedAssociatedFileLocations.Clear();
            extractedLinks.Clear();

            bool allExtracted = true;

            // Determine and validate sector length, default to 2048
            short sectorLength = (short)(SystemArea.Length / 16);
            if (sectorLength < 2048 || (sectorLength & (sectorLength - 1)) != 0)
                sectorLength = 2048;

            // Loop through all Volume Descriptors to extract files from each directory hierarchy
            // Note: This will prioritize the last volume descriptor directory hierarchies first (prioritises those filenames)
            // Note: Directory hierarchies with Rock Ridge names come before all others, as Linux uses them instead of Joliet names
            List<VolumeDescriptor> volumeDescriptors = [];
            int rockRidgeCount = 0;
            for (int i = VolumeDescriptorSet.Length - 1; i >= 0; i--)
            {
                var vd = VolumeDescriptorSet[i];
                if (GetRockRidgeRootDirectory(vd) is DirectoryExtent root && HasRockRidgeNames(root))
                    volumeDescriptors.Insert(rockRidgeCount++, vd);
                else
                    volumeDescriptors.Add(vd);
            }

            foreach (var vd in volumeDescriptors)
            {
                DirectoryRecord rootDirectoryRecord;
                if (vd is PrimaryVolumeDescriptor pvd)
                    rootDirectoryRecord = pvd.RootDirectoryRecord;
                else if (vd is SupplementaryVolumeDescriptor svd)
                    rootDirectoryRecord = svd.RootDirectoryRecord;
                else if (vd is HighSierraPrimaryVolumeDescriptor hspvd)
                    rootDirectoryRecord = hspvd.RootDirectoryRecord;
                else
                    continue;

                var blockLength = vd.GetLogicalBlockSize(sectorLength);
                rockRidge = GetRockRidgeRootDirectory(vd) is not null;

                // TODO: Better encoding detection (EscapeSequences)
                var encoding = Encoding.UTF8;
                if (vd is SupplementaryVolumeDescriptor)
                    encoding = Encoding.BigEndianUnicode;

                // Extract all files within root directory hierarchy
                allExtracted &= ExtractExtent(rootDirectoryRecord.ExtentLocation.LittleEndian, encoding, blockLength, outputDirectory, includeDebug);

                // If Big Endian extent location differs from Little Endian extent location, also extract that directory hierarchy
                if (!rootDirectoryRecord.ExtentLocation.IsValid)
                {
                    if (includeDebug) Console.WriteLine($"Extracting from volume descriptor (big endian root dir location)");
                    allExtracted &= ExtractExtent(rootDirectoryRecord.ExtentLocation.BigEndian, encoding, blockLength, outputDirectory, includeDebug);
                }

                // Also extract the directories that are only recorded in a path table
                ExtractPathTableDirectories(rootDirectoryRecord, encoding, blockLength, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a CD-DA file as a WAVE file of the raw sectors it is recorded in
        /// </summary>
        /// <remarks>
        /// The file starts with the 44-byte RIFF/WAVE header that Windows (CDFS) puts in front of CD-DA files,
        /// 16-bit stereo PCM at 44100 Hz, followed by the raw sectors. The extent length counts 2048-byte units,
        /// so every sector the extent touches is written in full (2352 bytes). Sectors past the end of the source
        /// are reported and left out. A further extent of the file is appended, and the sizes in the header are
        /// updated if the header was written by this extraction.
        /// </remarks>
        private bool ExtractCDDAFile(Stream rawSectorSource, DirectoryRecord dr, int extentLocation, long fileOffset, string filepath, bool includeDebug)
        {
            const long userDataSize = Data.Models.CDROM.Constants.Mode1DataSize;
            const long sectorSize = Data.Models.CDROM.Constants.CDROMSectorSize;
            const long sectorsPerChunk = 1024;

            // Get the sectors of the extent, and how many of them the source holds
            long firstSector = fileOffset / userDataSize;
            long sectorCount = ((fileOffset + dr.ExtentLength + userDataSize - 1) / userDataSize) - firstSector;
            long sectorsInSource = (rawSectorSource.Length / sectorSize) - firstSector;
            if (sectorsInSource <= 0)
            {
                Console.WriteLine($"CD-DA file starts past the end of the image, cannot extract: {filepath}");
                return false;
            }

            // Write the output file
            long sectorsToWrite = Math.Min(sectorCount, sectorsInSource);
            if (includeDebug) Console.WriteLine($"Extracting CD-DA file as WAVE, a 44-byte header added before the raw sectors: {filepath}");

            using (var fs = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                // A new file starts with the WAVE header, its sizes are set below
                if (fs.Length == 0)
                {
                    fs.WriteLittleEndian(0x46464952u); // "RIFF"
                    fs.WriteLittleEndian(0u); // Size of the file - 8
                    fs.WriteLittleEndian(0x45564157u); // "WAVE"
                    fs.WriteLittleEndian(0x20746D66u); // "fmt "
                    fs.WriteLittleEndian(16u); // Size of the format chunk
                    fs.WriteLittleEndian((ushort)1); // PCM
                    fs.WriteLittleEndian((ushort)2); // Channels
                    fs.WriteLittleEndian(44100u); // Samples per second
                    fs.WriteLittleEndian((uint)(sectorSize * 75)); // Bytes per second
                    fs.WriteLittleEndian((ushort)4); // Block align
                    fs.WriteLittleEndian((ushort)16); // Bits per sample
                    fs.WriteLittleEndian(0x61746164u); // "data"
                    fs.WriteLittleEndian(0u); // Size of the file - 44
                    waveFiles.Add(filepath);
                }

                fs.Seek(0, SeekOrigin.End);
                rawSectorSource.SeekIfPossible(firstSector * sectorSize, SeekOrigin.Begin);
                for (long written = 0; written < sectorsToWrite; written += sectorsPerChunk)
                {
                    int bytesToRead = (int)(Math.Min(sectorsToWrite - written, sectorsPerChunk) * sectorSize);

                    byte[] buffer = rawSectorSource.ReadBytes(bytesToRead);
                    fs.Write(buffer, 0, bytesToRead);
                }

                // Set the sizes in the header to the length of the file
                if (waveFiles.Contains(filepath))
                {
                    long fileLength = fs.Length;
                    fs.Seek(4, SeekOrigin.Begin);
                    fs.WriteLittleEndian((uint)(fileLength - 8));
                    fs.Seek(40, SeekOrigin.Begin);
                    fs.WriteLittleEndian((uint)(fileLength - 44));
                }
            }

            // Mark the file as extracted
            extractedFiles.Add(extentLocation, dr.ExtentLength);

            if (sectorsToWrite < sectorCount)
            {
                Console.WriteLine($"CD-DA file ends past the end of the image, extracted {sectorsToWrite} of {sectorCount} sectors: {filepath}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Extract all files from within a directory/file extent
        /// </summary>
        private bool ExtractExtent(int extentLocation, Encoding encoding, int blockLength, string outputDirectory, bool includeDebug)
        {
            // Check that directory exists in model
            if (!DirectoryDescriptors.TryGetValue(extentLocation, out FileExtent? value))
                return false;

            bool succeeded = true;
            if (value is not DirectoryExtent dir)
                return succeeded;

            var records = dir.DirectoryRecords;
            for (int i = 0; i < records.Length; i++)
            {
                var dr = records[i];

                // With Rock Ridge, a relocated directory is extracted in place of the file that links to it,
                // and not in the directory it was relocated to
                var childLink = rockRidge ? GetSystemUseEntry<ChildLink>(dr) : null;
                if (rockRidge && GetSystemUseEntry<RelocatedDirectory>(dr) is not null)
                    continue;

                // Recurse if record is directory
#if NET20 || NET35
                if ((dr.FileFlags & FileFlags.DIRECTORY) != 0 || childLink is not null)
#else
                if (dr.FileFlags.HasFlag(FileFlags.DIRECTORY) || childLink is not null)
#endif
                {
                    // Don't recurse up or self
                    if (dr.FileIdentifier.EqualsExactly(Constants.CurrentDirectory) || dr.FileIdentifier.EqualsExactly(Constants.ParentDirectory))
                        continue;

                    // Append directory name
                    string outputPath = GetOutputPath(dr, encoding, outputDirectory, GetSafeName(encoding.GetString(dr.FileIdentifier)));
                    if (includeDebug) Console.WriteLine($"Extracting to directory: {outputPath}");

                    // Create directory (even if empty)
                    if (!string.IsNullOrEmpty(outputPath) && !Directory.Exists(outputPath))
                        Directory.CreateDirectory(outputPath);

                    // Recursively extract from LittleEndian extent location
                    var location = childLink?.ChildDirectoryLocation ?? dr.ExtentLocation;
                    ExtractExtent(location.LittleEndian, encoding, blockLength, outputPath, includeDebug);

                    // Also extract from BigEndian values if ambiguous
                    if (!location.IsValid)
                        ExtractExtent(location.BigEndian, encoding, blockLength, outputPath, includeDebug);
                }
                else
                {
                    // Associated files are written to a file of their own, the file of the same name follows in its own record
#if NET20 || NET35
                    if ((dr.FileFlags & FileFlags.ASSOCIATED_FILE) != 0)
#else
                    if (dr.FileFlags.HasFlag(FileFlags.ASSOCIATED_FILE))
#endif
                    {
                        int last = GetLastExtentIndex(records, i);
                        succeeded &= ExtractAssociatedFile(records, i, last, encoding, blockLength, outputDirectory, includeDebug);
                        i = last;
                        continue;
                    }

                    // Record is a simple file extent, extract file
                    succeeded &= ExtractFile(dr, encoding, blockLength, false, outputDirectory, includeDebug);

                    // Also extract from BigEndian values if ambiguous
                    // TODO: How to treat files with same name but different location?
                    // if (!dr.ExtentLocation.IsValid)
                    //     succeeded &= ExtractFile(dr, encoding, blockLength, true, outputDirectory, includeDebug);
                }
            }

            // Mark the directory as extracted
            extractedDirectories.Add(extentLocation);

            return succeeded;
        }

        /// <summary>
        /// Extract the directories that are recorded in the path tables of a directory hierarchy but not extracted yet
        /// </summary>
        private void ExtractPathTableDirectories(DirectoryRecord rootDirectoryRecord, Encoding encoding, int blockLength, string outputDirectory, bool includeDebug)
        {
            foreach (var group in PathTableGroups)
            {
                foreach (var pathTable in new[] { group.PathTableL, group.OptionalPathTableL, group.PathTableM, group.OptionalPathTableM })
                {
                    // The path tables of a directory hierarchy start with its root directory
                    if (pathTable is null || pathTable[0].ExtentLocation != rootDirectoryRecord.ExtentLocation.LittleEndian)
                        continue;

                    // Each directory follows its parent directory, which is numbered from 1 in path table order
                    var paths = new string?[pathTable.Length];
                    paths[0] = string.Empty;
                    for (int i = 1; i < pathTable.Length; i++)
                    {
                        int parent = pathTable[i].ParentDirectoryNumber - 1;
                        if (parent < 0 || parent >= i || paths[parent] is not string parentPath)
                            continue;

                        string path = Path.Combine(parentPath, GetSafeName(encoding.GetString(pathTable[i].DirectoryIdentifier)));
                        paths[i] = path;

                        // Each directory is extracted once
                        int extentLocation = pathTable[i].ExtentLocation;
                        if (extractedDirectories.Contains(extentLocation) || !DirectoryDescriptors.TryGetValue(extentLocation, out FileExtent? value) || value is not DirectoryExtent)
                            continue;

                        string outputPath = Path.Combine(outputDirectory, path);
                        if (includeDebug) Console.WriteLine($"Extracting to directory: {outputPath}");

                        // Create directory (even if empty)
                        if (!Directory.Exists(outputPath))
                            Directory.CreateDirectory(outputPath);

                        ExtractExtent(extentLocation, encoding, blockLength, outputPath, includeDebug);
                    }
                }
            }
        }

        /// <summary>
        /// Extract file pointed to by a directory record
        /// </summary>
        private bool ExtractFile(DirectoryRecord dr, Encoding encoding, int blockLength, bool bigEndian, string outputDirectory, bool includeDebug)
        {
            // Cannot extract file if it is a directory
#if NET20 || NET35
            if ((dr.FileFlags & FileFlags.DIRECTORY) != 0)
#else
            if (dr.FileFlags.HasFlag(FileFlags.DIRECTORY))
#endif
                return false;

            int extentLocation = bigEndian ? dr.ExtentLocation.BigEndian : dr.ExtentLocation.LittleEndian;

            // Check that the file hasn't been extracted already
            if (extractedFiles.ContainsKey(extentLocation))
                return true;

            string filename = GetFileName(dr, encoding);

            // Get the output file path
            var filepath = GetOutputPath(dr, encoding, outputDirectory, filename);

            // Symbolic links don't mark their extent as extracted, so a record of the same path in another directory hierarchy is skipped by its path
            if (extractedLinks.Contains(filepath))
                return true;

            bool multiExtent = false;
            bool interleaved = false;

            // Check for multi-extent and interleaved files
#if NET20 || NET35
            if ((dr.FileFlags & FileFlags.MULTI_EXTENT) != 0)
#else
            if (dr.FileFlags.HasFlag(FileFlags.MULTI_EXTENT))
#endif
            {
                if (includeDebug) Console.WriteLine($"Extraction of multi-extent files is currently WIP: {filename}");
                multiExtentFiles.Add(dr.FileIdentifier);
                multiExtent = true;
            }

            // Each file section records its own interleaving
            if (dr.FileUnitSize != 0 || dr.InterleaveGapSize != 0)
            {
                // An interleaved file needs a file unit size
                if (dr.FileUnitSize == 0)
                {
                    Console.WriteLine($"Interleaved file has no file unit size, cannot extract: {filename}");
                    extractedFiles.Add(extentLocation, dr.ExtentLength);
                    return false;
                }

                interleaved = true;
            }

            // Check that the output file doesn't already exist
            if (!multiExtent && (File.Exists(filepath) || Directory.Exists(filepath)))
            {
                // If it's the last extent of a multi-extent file, continue to append
                if (!multiExtentFiles.Exists(item => item.EqualsExactly(dr.FileIdentifier)))
                {
                    if (includeDebug) Console.WriteLine($"File/Folder already exists, cannot extract: {filename}");
                    return false;
                }
            }

            const uint chunkSize = 2048 * 1024;
            lock (_dataSourceLock)
            {
                // An interleaved file is recorded in file units of FileUnitSize logical blocks,
                // each followed by a gap of InterleaveGapSize logical blocks (ECMA-119 6.4.3)
                long unitLength = interleaved ? (long)dr.FileUnitSize * blockLength : dr.ExtentLength;
                long unitDistance = interleaved ? (long)(dr.FileUnitSize + dr.InterleaveGapSize) * blockLength : 0;

                // The extended attribute record precedes the file data, in interleaved mode it fills the first file unit
                long fileOffset = (long)extentLocation * blockLength;
                if (interleaved && dr.ExtendedAttributeRecordLength != 0)
                    fileOffset += unitDistance;
                else
                    fileOffset += (long)dr.ExtendedAttributeRecordLength * blockLength;

                _dataSource.SeekIfPossible(fileOffset, SeekOrigin.Begin);

                // CD-DA files are extracted as WAVE files of their raw sectors, if the volume was read from raw sectors
                var rawSectorSource = RawSectorSource;
                if (rawSectorSource is not null && dr.IsCDDA())
                    return ExtractCDDAFile(rawSectorSource, dr, extentLocation, fileOffset, filepath, includeDebug);

                // A Rock Ridge symbolic link is written as a file with the path it points to, its extent holds no data
                byte[]? linkTarget = rockRidge ? GetSymbolicLinkTarget(dr) : null;

                // Get the length, and make sure it won't EOF
                uint length = linkTarget is null ? dr.ExtentLength : 0;
                long lastUnit = length == 0 ? 0 : (length - 1) / unitLength;
                if (fileOffset + (lastUnit * unitDistance) + (length - (lastUnit * unitLength)) > _dataSource.Length)
                    return false;

                // Write the output file
                if (includeDebug) Console.WriteLine($"Extracting: {filepath}");
                using var fs = File.Open(filepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                if (linkTarget is not null)
                {
                    // The extent is not marked as extracted, as it can be the extent of the file after the symbolic link
                    fs.Write(linkTarget, 0, linkTarget.Length);
                    extractedLinks.Add(filepath);
                    return true;
                }

                long unitRemaining = unitLength;
                while (length > 0)
                {
                    // Move to the next file unit
                    if (unitRemaining == 0)
                    {
                        fileOffset += unitDistance;
                        unitRemaining = unitLength;
                        _dataSource.SeekIfPossible(fileOffset, SeekOrigin.Begin);
                    }

                    int bytesToRead = (int)Math.Min(Math.Min(length, chunkSize), unitRemaining);

                    byte[] buffer = _dataSource.ReadBytes(bytesToRead);
                    fs.Write(buffer, 0, bytesToRead);
                    fs.Flush();

                    length -= (uint)bytesToRead;
                    unitRemaining -= bytesToRead;
                }

                // Mark the file as extracted
                extractedFiles.Add(extentLocation, dr.ExtentLength);
            }

            return true;
        }

        /// <summary>
        /// Extract an associated file to a file next to the file it belongs to
        /// </summary>
        /// <param name="records">Directory records of the directory</param>
        /// <param name="first">Index of the first record of the associated file</param>
        /// <param name="last">Index of the last record of the associated file</param>
        /// <remarks>
        /// With the Apple extensions to ISO 9660, the associated file is the resource fork of the file, and the
        /// System Use field holds its Finder type, creator and flags. The AppleDouble file is written in the layout
        /// macOS uses on volumes without forks, macOS reads both from it as attributes of the file.
        /// Without the Apple extensions, ISO 9660 does not specify what the associated file holds, so it is written
        /// as it is recorded, to the name of the file with ".associated" appended.
        /// </remarks>
        private bool ExtractAssociatedFile(DirectoryRecord[] records, int first, int last, Encoding encoding, int blockLength, string outputDirectory, bool includeDebug)
        {
            var dr = records[first];
            int extentLocation = dr.ExtentLocation.LittleEndian;

            // Take the Finder info from the file, which directly follows, or else from the associated file
            bool fileFollows = last + 1 < records.Length && records[last + 1].FileIdentifier.EqualsExactly(dr.FileIdentifier);
            byte[] finderInfo = new byte[32];
            bool appleExtensions = (fileFollows && ReadFinderInfo(records[last + 1], finderInfo))
                || ReadFinderInfo(dr, finderInfo);

            // Get the output file path
            string filename = appleExtensions ? $"._{GetFileName(dr, encoding)}" : $"{GetFileName(dr, encoding)}.associated";
            var filepath = Path.Combine(outputDirectory, filename);

            // Check that the associated file hasn't been extracted already
            if (extractedAssociatedFiles.TryGetValue(filepath, out int extractedLocation) && extractedLocation == extentLocation)
                return true;

            // Check that the associated file hasn't been extracted under another name, like in the other directory
            // hierarchy of a volume with Joliet, unless it has a file that wasn't extracted yet, as empty associated
            // files can share their sector offset
            if (extractedAssociatedFileLocations.Contains(extentLocation)
                && (!fileFollows || extractedFiles.ContainsKey(records[last + 1].ExtentLocation.LittleEndian)))
            {
                return true;
            }

            // Currently cannot extract interleaved files
            uint length = 0;
            for (int i = first; i <= last; i++)
            {
                if (records[i].FileUnitSize != 0 || records[i].InterleaveGapSize != 0)
                {
                    Console.WriteLine($"Extraction of interleaved files is currently not supported: {filename}");
                    return false;
                }

                length += records[i].ExtentLength;
            }

            // Check that the output file doesn't already exist
            if (File.Exists(filepath) || Directory.Exists(filepath))
            {
                if (includeDebug) Console.WriteLine($"File/Folder already exists, cannot extract: {filename}");
                return false;
            }

            const uint chunkSize = 2048 * 1024;
            lock (_dataSourceLock)
            {
                // Make sure no extent will EOF
                for (int i = first; i <= last; i++)
                {
                    uint extentLength = records[i].ExtentLength;
                    if (GetFileOffset(records[i], blockLength) + extentLength > _dataSource.Length)
                        return false;
                }

                // Write the AppleDouble header if there is one, followed by the associated file
                if (includeDebug) Console.WriteLine($"Extracting: {filepath}");
                using var fs = File.Open(filepath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                if (appleExtensions)
                {
                    byte[] header = GetAppleDoubleHeader(finderInfo, length);
                    fs.Write(header, 0, header.Length);
                }

                for (int i = first; i <= last; i++)
                {
                    _dataSource.SeekIfPossible(GetFileOffset(records[i], blockLength), SeekOrigin.Begin);
                    uint remaining = records[i].ExtentLength;
                    while (remaining > 0)
                    {
                        int bytesToRead = (int)Math.Min(remaining, chunkSize);

                        byte[] buffer = _dataSource.ReadBytes(bytesToRead);
                        fs.Write(buffer, 0, bytesToRead);

                        remaining -= (uint)bytesToRead;
                    }
                }

                // Mark the associated file as extracted
                extractedAssociatedFiles[filepath] = extentLocation;
                extractedAssociatedFileLocations.Add(extentLocation);
            }

            return true;
        }

        /// <summary>
        /// Get the index of the last record of a file recorded in multiple extents
        /// </summary>
        /// <param name="records">Directory records of the directory</param>
        /// <param name="index">Index of the first record of the file</param>
        private static int GetLastExtentIndex(DirectoryRecord[] records, int index)
        {
            // Every extent but the last has the multi-extent flag, the next one follows with the same name and kind
            while (index + 1 < records.Length
                && (records[index].FileFlags & FileFlags.MULTI_EXTENT) != 0
                && (records[index + 1].FileFlags & FileFlags.ASSOCIATED_FILE) == (records[index].FileFlags & FileFlags.ASSOCIATED_FILE)
                && records[index + 1].FileIdentifier.EqualsExactly(records[index].FileIdentifier))
            {
                index++;
            }

            return index;
        }

        /// <summary>
        /// Get the name of the file of a directory record, without its version number, as a name for the output file
        /// </summary>
        private static string GetFileName(DirectoryRecord dr, Encoding encoding)
        {
            // TODO: Decode properly (Use VD's separator characters and encoding)
            string filename = encoding.GetString(dr.FileIdentifier);
            int index = filename.LastIndexOf(';');
            if (index > 0)
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
                filename = filename[..index];
#else
                filename = filename.Substring(0, index);
#endif

            return GetSafeName(filename);
        }

        /// <summary>
        /// Get a name that can be used as one component of an output path
        /// </summary>
        /// <remarks>
        /// Characters that are invalid in a file name on the current platform, the path separators among them, are
        /// replaced with "_", and so are the dots of the names "." and "..", so that every name is written inside its
        /// directory. On Windows, periods and spaces at the end of a name are removed, as Windows does when it creates
        /// the file or directory, so that a directory is found again under that name, and a name of only periods and
        /// spaces is replaced with "_" characters.
        /// </remarks>
        private static string GetSafeName(string name)
        {
            if (name == "." || name == "..")
                return new string('_', name.Length);

            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalid, '_');
            }

            if (Path.DirectorySeparatorChar == '\\')
            {
                string trimmed = name.TrimEnd('.', ' ');
                name = trimmed.Length > 0 ? trimmed : new string('_', name.Length);
            }

            return name;
        }

        /// <summary>
        /// Get the offset of the data of a file
        /// </summary>
        private static long GetFileOffset(DirectoryRecord dr, int blockLength)
            => ((long)dr.ExtentLocation.LittleEndian + dr.ExtendedAttributeRecordLength) * blockLength;

        /// <summary>
        /// Get the root directory of the directory hierarchy of a volume descriptor, if the directory hierarchy uses Rock Ridge
        /// </summary>
        /// <param name="vd">Volume descriptor</param>
        /// <returns>Root directory if the volume descriptor is a primary volume descriptor whose directory hierarchy uses SUSP, null otherwise</returns>
        /// <remarks>
        /// Rock Ridge is recorded with the System Use Sharing Protocol (SUSP), which a directory hierarchy uses if the System Use
        /// entries of the "." record of its root directory start with the Sharing Protocol Indicator. Like Linux, only the
        /// directory hierarchy of the primary volume descriptor is read with Rock Ridge.
        /// </remarks>
        private DirectoryExtent? GetRockRidgeRootDirectory(VolumeDescriptor vd)
        {
            if (vd is not PrimaryVolumeDescriptor pvd)
                return null;

            if (!DirectoryDescriptors.TryGetValue(pvd.RootDirectoryRecord.ExtentLocation.LittleEndian, out FileExtent? value) || value is not DirectoryExtent root || root.DirectoryRecords.Length == 0)
                return null;

            var entries = root.DirectoryRecords[0].SystemUseEntries;
            if (entries.Length == 0 || entries[0] is not SharingProtocolIndicator sp || !sp.CheckBytes.EqualsExactly(Constants.SharingProtocolCheckBytes))
                return null;

            return root;
        }

        /// <summary>
        /// Indicates if a record of a root directory has a Rock Ridge name
        /// </summary>
        /// <param name="root">Root directory of a directory hierarchy that uses Rock Ridge</param>
        /// <returns>True if a record of the root directory has a Rock Ridge name, false otherwise</returns>
        /// <remarks>A directory hierarchy can use Rock Ridge without names, like only for the POSIX file attributes</remarks>
        private static bool HasRockRidgeNames(DirectoryExtent root)
        {
            foreach (var dr in root.DirectoryRecords)
            {
                if (GetRockRidgeName(dr, Encoding.UTF8) is not null)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get the output path of a directory record, from its Rock Ridge name if the directory hierarchy uses Rock Ridge
        /// </summary>
        /// <param name="dr">Directory record</param>
        /// <param name="encoding">Encoding of the names of the directory hierarchy</param>
        /// <param name="outputDirectory">Output directory of the directory that holds the record</param>
        /// <param name="name">Name from the file identifier, used if there is no Rock Ridge name</param>
        /// <remarks>
        /// A Rock Ridge name that is taken by the Rock Ridge name of another directory record, like a name that only differs
        /// in case on a case-insensitive file system, is replaced by the name from the file identifier, so both are extracted.
        /// </remarks>
        private string GetOutputPath(DirectoryRecord dr, Encoding encoding, string outputDirectory, string name)
        {
            string? rockRidgeName = rockRidge ? GetRockRidgeName(dr, encoding) : null;
            if (rockRidgeName is null)
                return Path.Combine(outputDirectory, name);

            // The records of a file in multiple extents share the file identifier and the path
            string path = Path.Combine(outputDirectory, GetSafeName(rockRidgeName));
            if (rockRidgePaths.TryGetValue(path, out byte[]? identifier) && !identifier.EqualsExactly(dr.FileIdentifier) && (File.Exists(path) || Directory.Exists(path)))
                return Path.Combine(outputDirectory, name);

            rockRidgePaths[path] = dr.FileIdentifier;
            return path;
        }

        /// <summary>
        /// Get the first System Use entry of a type of a directory record
        /// </summary>
        /// <param name="dr">Directory record</param>
        /// <returns>First System Use entry of the type, null if there is none</returns>
        private static T? GetSystemUseEntry<T>(DirectoryRecord dr) where T : BaseEntry
        {
            foreach (var entry in dr.SystemUseEntries)
            {
                if (entry is T typedEntry)
                    return typedEntry;
            }

            return null;
        }

        /// <summary>
        /// Get the Rock Ridge name of a directory record, the name contents of its "NM" entries
        /// </summary>
        /// <param name="dr">Directory record</param>
        /// <param name="encoding">Encoding of the names of the directory hierarchy</param>
        /// <returns>Rock Ridge name, null if the directory record has none</returns>
        /// <remarks>Like Linux, an entry with any flag but CONTINUE is skipped, like one for the current or parent directory</remarks>
        private static string? GetRockRidgeName(DirectoryRecord dr, Encoding encoding)
        {
            List<byte> name = [];
            foreach (var entry in dr.SystemUseEntries)
            {
                if (entry is AlternateName alternateName && (alternateName.Flags & ~AlternateNameFlags.CONTINUE) == 0)
                    name.AddRange(alternateName.NameContent);
            }

            return name.Count == 0 ? null : encoding.GetString(name.ToArray());
        }

        /// <summary>
        /// Get the path a Rock Ridge symbolic link points to, from the component records of its "SL" entries
        /// </summary>
        /// <param name="dr">Directory record</param>
        /// <returns>Path in the bytes it is recorded in, null if the directory record is not a symbolic link</returns>
        /// <remarks>
        /// Like Linux, a component is followed by "/" unless it is continued or it is the root directory, and the last
        /// component of an entry only if the path is continued in the next entry. Components with other flags are left out.
        /// </remarks>
        private static byte[]? GetSymbolicLinkTarget(DirectoryRecord dr)
        {
            List<byte>? target = null;
            foreach (var entry in dr.SystemUseEntries)
            {
                if (entry is not SymbolicLink symbolicLink)
                    continue;

                target ??= [];
                for (int i = 0; i < symbolicLink.Components.Length; i++)
                {
                    var component = symbolicLink.Components[i];
                    bool root = false;
                    switch (component.Flags & ~ComponentFlags.CONTINUE)
                    {
                        case 0:
                            target.AddRange(component.Content);
                            break;
                        case ComponentFlags.CURRENT:
                            target.Add((byte)'.');
                            break;
                        case ComponentFlags.PARENT:
                            target.Add((byte)'.');
                            target.Add((byte)'.');
                            break;
                        case ComponentFlags.ROOT:
                            target.Add((byte)'/');
                            root = true;
                            break;

                        // Like Linux, other components only write the separator after them
                        case ComponentFlags.CONTINUE:
                        case ComponentFlags.VOLROOT:
                        case ComponentFlags.HOST:
                        default:
                            break;
                    }

                    bool continued = (component.Flags & ComponentFlags.CONTINUE) != 0;
                    bool last = i == symbolicLink.Components.Length - 1;
                    if (!root && !continued && (!last || (symbolicLink.Flags & SymbolicLinkFlags.CONTINUE) != 0))
                        target.Add((byte)'/');
                }
            }

            return target?.ToArray();
        }

        /// <summary>
        /// Read the Finder type, creator and flags from the Apple extension in the System Use entries of a directory record
        /// </summary>
        /// <param name="dr">Directory record</param>
        /// <param name="finderInfo">Finder info to fill</param>
        /// <returns>True if the System Use entries hold the Apple extension for HFS, false otherwise</returns>
        private static bool ReadFinderInfo(DirectoryRecord dr, byte[] finderInfo)
        {
            foreach (var entry in dr.SystemUseEntries)
            {
                // Apple extension for HFS: type, creator and Finder flags
                if (entry is UnknownEntry unknownEntry
                    && unknownEntry.Length == 14
                    && unknownEntry.Version == 2
                    && Encoding.ASCII.GetString(unknownEntry.Signature) == Constants.AppleExtensionSignature)
                {
                    Array.Copy(unknownEntry.Data, 0, finderInfo, 0, 10);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the header of an AppleDouble file in the layout macOS writes, the resource fork follows it
        /// </summary>
        /// <param name="finderInfo">Finder info of the file</param>
        /// <param name="resourceForkLength">Length of the resource fork</param>
        private static byte[] GetAppleDoubleHeader(byte[] finderInfo, uint resourceForkLength)
        {
            const int finderInfoOffset = 0x32;
            const int attributesOffset = 0x54;
            const int resourceForkOffset = 0xEE2;

            byte[] header = new byte[resourceForkOffset];
            int offset = 0;

            // Magic number, version and filler
            header.WriteBigEndian(ref offset, 0x00051607u);
            header.WriteBigEndian(ref offset, 0x00020000u);
            header.Write(ref offset, Encoding.ASCII.GetBytes("Mac OS X        "));

            // Finder info entry, which macOS extends by an empty extended attributes header, and resource fork entry
            header.WriteBigEndian(ref offset, (ushort)2);
            header.WriteBigEndian(ref offset, 9u);
            header.WriteBigEndian(ref offset, (uint)finderInfoOffset);
            header.WriteBigEndian(ref offset, (uint)(resourceForkOffset - finderInfoOffset));
            header.WriteBigEndian(ref offset, 2u);
            header.WriteBigEndian(ref offset, (uint)resourceForkOffset);
            header.WriteBigEndian(ref offset, resourceForkLength);

            // Finder info
            offset = finderInfoOffset;
            header.Write(ref offset, finderInfo);

            // Extended attributes header: magic number, debug tag, total size and start of the attribute data
            offset = attributesOffset;
            header.Write(ref offset, Encoding.ASCII.GetBytes("ATTR"));
            header.WriteBigEndian(ref offset, 0u);
            header.WriteBigEndian(ref offset, (uint)resourceForkOffset);
            header.WriteBigEndian(ref offset, 0x78u);

            return header;
        }
    }
}
