using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class ISO9660 : IExtractable
    {
        #region Extraction State

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
            extractedFiles.Clear();
            multiExtentFiles.Clear();
            waveFiles.Clear();
            extractedAssociatedFiles.Clear();

            bool allExtracted = true;

            // Determine and validate sector length, default to 2048
            short sectorLength = (short)(SystemArea.Length / 16);
            if (sectorLength < 2048 || (sectorLength & (sectorLength - 1)) != 0)
                sectorLength = 2048;

            // Loop through all Volume Descriptors to extract files from each directory hierarchy
            // Note: This will prioritize the last volume descriptor directory hierarchies first (prioritises those filenames)
            for (int i = VolumeDescriptorSet.Length - 1; i >= 0; i--)
            {
                var vd = VolumeDescriptorSet[i];

                DirectoryRecord rootDirectoryRecord;
                if (vd is PrimaryVolumeDescriptor pvd)
                    rootDirectoryRecord = pvd.RootDirectoryRecord;
                else if (vd is SupplementaryVolumeDescriptor svd)
                    rootDirectoryRecord = svd.RootDirectoryRecord;
                else
                    continue;

                var blockLength = vd.GetLogicalBlockSize(sectorLength);

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

                // Recurse if record is directory
#if NET20 || NET35
                if ((dr.FileFlags & FileFlags.DIRECTORY) != 0)
#else
                if (dr.FileFlags.HasFlag(FileFlags.DIRECTORY))
#endif
                {
                    // Don't recurse up or self
                    if (dr.FileIdentifier.EqualsExactly(Constants.CurrentDirectory) || dr.FileIdentifier.EqualsExactly(Constants.ParentDirectory))
                        continue;

                    // Append directory name
                    string outputPath = Path.Combine(outputDirectory, encoding.GetString(dr.FileIdentifier));
                    if (includeDebug) Console.WriteLine($"Extracting to directory: {outputPath}");

                    // Create directory (even if empty)
                    if (!string.IsNullOrEmpty(outputPath) && !Directory.Exists(outputPath))
                        Directory.CreateDirectory(outputPath);

                    // Recursively extract from LittleEndian extent location
                    ExtractExtent(dr.ExtentLocation.LittleEndian, encoding, blockLength, outputPath, includeDebug);

                    // Also extract from BigEndian values if ambiguous
                    if (!dr.ExtentLocation.IsValid)
                        ExtractExtent(dr.ExtentLocation.BigEndian, encoding, blockLength, outputPath, includeDebug);
                }
                else
                {
                    // Associated files are written to an AppleDouble file, the file of the same name follows in its own record
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

            return succeeded;
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
            var filepath = Path.Combine(outputDirectory, filename);

            bool multiExtent = false;

            // Currently cannot extract multi-extent or interleaved files
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
            else if (dr.FileUnitSize != 0 || dr.InterleaveGapSize != 0)
            {
                Console.WriteLine($"Extraction of interleaved files is currently not supported: {filename}");
                extractedFiles.Add(extentLocation, dr.ExtentLength);
                return false;
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
                long fileOffset = ((long)extentLocation + dr.ExtendedAttributeRecordLength) * blockLength;

                // CD-DA files are extracted as WAVE files of their raw sectors, if the volume was read from raw sectors
                var rawSectorSource = RawSectorSource;
                if (rawSectorSource is not null && dr.IsCDDA())
                    return ExtractCDDAFile(rawSectorSource, dr, extentLocation, fileOffset, filepath, includeDebug);

                _dataSource.SeekIfPossible(fileOffset, SeekOrigin.Begin);

                // Get the length, and make sure it won't EOF
                uint length = dr.ExtentLength;
                if (length > _dataSource.Length - _dataSource.Position)
                    return false;

                // Write the output file
                if (includeDebug) Console.WriteLine($"Extracting: {filepath}");
                using var fs = File.Open(filepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                while (length > 0)
                {
                    int bytesToRead = (int)Math.Min(length, chunkSize);

                    byte[] buffer = _dataSource.ReadBytes(bytesToRead);
                    fs.Write(buffer, 0, bytesToRead);
                    fs.Flush();

                    length -= (uint)bytesToRead;
                }

                // Mark the file as extracted
                extractedFiles.Add(extentLocation, dr.ExtentLength);
            }

            return true;
        }

        /// <summary>
        /// Extract an associated file to an AppleDouble file next to the file it belongs to
        /// </summary>
        /// <param name="records">Directory records of the directory</param>
        /// <param name="first">Index of the first record of the associated file</param>
        /// <param name="last">Index of the last record of the associated file</param>
        /// <remarks>
        /// With the Apple extensions to ISO 9660, the associated file is the resource fork of the file, and the
        /// System Use field holds its Finder type, creator and flags. The AppleDouble file is written in the layout
        /// macOS uses on volumes without forks, macOS reads both from it as attributes of the file.
        /// </remarks>
        private bool ExtractAssociatedFile(DirectoryRecord[] records, int first, int last, Encoding encoding, int blockLength, string outputDirectory, bool includeDebug)
        {
            var dr = records[first];
            int extentLocation = dr.ExtentLocation.LittleEndian;

            // Get the output file path
            string filename = $"._{GetFileName(dr, encoding)}";
            var filepath = Path.Combine(outputDirectory, filename);

            // Check that the associated file hasn't been extracted already
            if (extractedAssociatedFiles.TryGetValue(filepath, out int extractedLocation) && extractedLocation == extentLocation)
                return true;

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

            // Take the Finder info from the file, which directly follows, or else from the associated file
            byte[] finderInfo = new byte[32];
            bool fromFile = last + 1 < records.Length
                && records[last + 1].FileIdentifier.EqualsExactly(dr.FileIdentifier)
                && ReadFinderInfo(records[last + 1].SystemUse, finderInfo);
            if (!fromFile)
                ReadFinderInfo(dr.SystemUse, finderInfo);

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

                // Write the AppleDouble header, followed by the resource fork
                if (includeDebug) Console.WriteLine($"Extracting: {filepath}");
                using var fs = File.Open(filepath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                byte[] header = GetAppleDoubleHeader(finderInfo, length);
                fs.Write(header, 0, header.Length);
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
        /// Get the name of the file of a directory record, without its version number
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

            return filename;
        }

        /// <summary>
        /// Get the offset of the data of a file
        /// </summary>
        private static long GetFileOffset(DirectoryRecord dr, int blockLength)
            => ((long)dr.ExtentLocation.LittleEndian + dr.ExtendedAttributeRecordLength) * blockLength;

        /// <summary>
        /// Read the Finder type, creator and flags from the Apple extension in a System Use field
        /// </summary>
        /// <param name="systemUse">System Use field of a directory record</param>
        /// <param name="finderInfo">Finder info to fill</param>
        /// <returns>True if the System Use field holds the Apple extension for HFS, false otherwise</returns>
        private static bool ReadFinderInfo(byte[] systemUse, byte[] finderInfo)
        {
            // The System Use entries follow the CD-ROM XA system use information, if there is any
            int offset = systemUse.Length >= 14 && systemUse[6] == 'X' && systemUse[7] == 'A' ? 14 : 0;

            // Every entry starts with a signature of 2 bytes and its length
            while (offset + 4 <= systemUse.Length)
            {
                int entryLength = systemUse[offset + 2];
                if (entryLength < 4 || offset + entryLength > systemUse.Length)
                    return false;

                // Apple extension for HFS: type, creator and Finder flags
                if (systemUse[offset] == 'A' && systemUse[offset + 1] == 'A' && entryLength == 14 && systemUse[offset + 3] == 2)
                {
                    Array.Copy(systemUse, offset + 4, finderInfo, 0, 10);
                    return true;
                }

                offset += entryLength;
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
