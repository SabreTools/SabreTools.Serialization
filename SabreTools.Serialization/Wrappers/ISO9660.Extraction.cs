using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class ISO9660 : IExtractable
    {
        /// <inheritdoc/>
        public virtual bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no volume or directory descriptors, there is nothing to extract
            if (VolumeDescriptorSet.Length == 0 || DirectoryDescriptors.Count == 0)
                return true;

            bool allExtracted = true;

            // Determine and validate sector length, default to 2048
            short sectorLength = (short)(SystemArea.Length / 16);
            if (sectorLength < 2048 || (sectorLength & (sectorLength - 1)) != 0)
                sectorLength = 2048;

            // Loop through all Base Volume Descriptors to extract files from each directory hierarchy
            // Note: This will prioritize the last volume descriptor directory hierarchies first (prioritises those filenames)
            for (int i = VolumeDescriptorSet.Length - 1; i >= 0; i--)
            {
                if (VolumeDescriptorSet[i] is BaseVolumeDescriptor bvd)
                {
                    var rootDir = bvd.RootDirectoryRecord;

                    var blockLength = bvd.GetLogicalBlockSize(sectorLength);

                    // TODO: Better encoding detection (EscapeSequences)
                    var encoding = Encoding.UTF8;
                    if (bvd is SupplementaryVolumeDescriptor svd)
                        encoding = Encoding.BigEndianUnicode;

                    // Extract all files within root directory hierarchy
                    allExtracted &= ExtractExtent(rootDir.ExtentLocation.LittleEndian, encoding, blockLength, outputDirectory, includeDebug);
                    // If Big Endian extent location differs from Little Endian extent location, also extract that directory hierarchy
                    if (!rootDir.ExtentLocation.IsValid)
                    {
                        if (includeDebug) Console.WriteLine($"Extracting from volume descriptor (big endian root dir location)");
                        allExtracted &= ExtractExtent(rootDir.ExtentLocation.BigEndian, encoding, blockLength, outputDirectory, includeDebug);
                    }
                }
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract all files from within a directory/file extent
        /// </summary>
        private bool ExtractExtent(int extentLocation, Encoding encoding, int blockLength, string outputDirectory, bool includeDebug)
        {
            // Check that directory exists in model
            if (!DirectoryDescriptors.ContainsKey(extentLocation))
                return false;

            bool succeeded = true;
            if (DirectoryDescriptors[extentLocation] is DirectoryExtent dir)
            {
                foreach (var dr in dir.DirectoryRecords)
                {
                    // Recurse if record is directory
                    if ((dr.FileFlags & FileFlags.DIRECTORY) == FileFlags.DIRECTORY)
                    {
                        // Don't recurse up or self
                        if (dr.FileIdentifier.EqualsExactly(Constants.CurrentDirectory) || dr.FileIdentifier.EqualsExactly(Constants.ParentDirectory))
                            continue;

                        // Append directory name
                        string outDirTemp = Path.Combine(outputDirectory, encoding.GetString(dr.FileIdentifier));
                        if (includeDebug) Console.WriteLine($"Extracting to directory: {outDirTemp}");

                        // Recursively extract from LittleEndian extent location
                        ExtractExtent(dr.ExtentLocation.LittleEndian, encoding, blockLength, outDirTemp, includeDebug);

                        // Also extract from BigEndian values if ambiguous
                        if (!dr.ExtentLocation.IsValid!)
                            ExtractExtent(dr.ExtentLocation.BigEndian, encoding, blockLength, outDirTemp, includeDebug);
                    }
                    else
                    {
                        // Record is a simple file extent, extract file
                        succeeded &= ExtractFile(dr, encoding, blockLength, false, outputDirectory, includeDebug);
                        // Also extract from BigEndian values if ambiguous
                        if (!dr.ExtentLocation.IsValid!)
                            succeeded &= ExtractFile(dr, encoding, blockLength, true, outputDirectory, includeDebug);
                    }
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
            if ((dr.FileFlags & FileFlags.DIRECTORY) == FileFlags.DIRECTORY)
                return false;

            int extentLocation = bigEndian ? dr.ExtentLocation.BigEndian : dr.ExtentLocation.LittleEndian;

            // Check that the file hasn't been extracted already
            if (extractedFiles.ContainsKey(dr.ExtentLocation))
                return true;

            // TODO: Decode properly (Use VD's separator characters and encoding)
            string filename = encoding.GetString(dr.FileIdentifier);
            int index = filename.LastIndexOf(';');
            if (index > 0)
                filename = filename.Substring(0, index);

            // Ensure the full output directory exists
            var filepath = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filepath);
            if (directoryName != null && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            bool multiExtent = false;
            
            // Currently cannot extract multi-extent or interleaved files
            if ((dr.FileFlags & FileFlags.MULTI_EXTENT) != 0)
            {
                Console.WriteLine($"Extraction of multi-extent files is currently WIP: {filename}");
                multiExtentFiles.Add(dr.FileIdentifier);
                multiExtent = true;
            }
            else if (dr.FileUnitSize != 0 || dr.InterleaveGapSize != 0)
            {
                Console.WriteLine($"Extraction of interleaved files is currently not supported: {filename}");
                extractedFiles.Add(dr.ExtentLocation, dr.ExtentLength);
                return false;
            }

            // Check that the output file doesn't already exist
            if (!multiExtent && (File.Exists(filepath) || Directory.Exists(filepath)))
            {
                // If it's the last extent of a multi-extent file, continue to append
                if (multiExtentFiles.Exists(item => item.EqualsExactly(dr.FileIdentifier)))
                {
                    if (includeDebug) Console.WriteLine($"File/Folder already exists, cannot extract: {filename}");
                    return false;
                }
            }

            const int chunkSize = 2048 * 1024;
            lock (_dataSourceLock)
            {
                long fileOffset = ((long)dr.ExtentLocation + (long)dr.ExtendedAttributeRecordLength) * (long)blockLength;
                _dataSource.SeekIfPossible(fileOffset, SeekOrigin.Begin);

                // Get the length, and make sure it won't EOF
                int length = dr.ExtentLength;
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

                    length -= bytesToRead;
                }

                // Mark the file as extracted
                extractedFiles.Add(dr.ExtentLocation, dr.ExtentLength);
            }

            return true;
        }
    }
}
