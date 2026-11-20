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

            foreach (var dr in dir.DirectoryRecords)
            {
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

            // TODO: Decode properly (Use VD's separator characters and encoding)
            string filename = encoding.GetString(dr.FileIdentifier);
            int index = filename.LastIndexOf(';');
            if (index > 0)
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
                filename = filename[..index];
#else
                filename = filename.Substring(0, index);
#endif

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
    }
}
