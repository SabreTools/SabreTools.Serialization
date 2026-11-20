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
        /// List of files extracted from the directory hierarchies before the current one, by their sector offset
        /// </summary>
        private readonly HashSet<int> earlierExtractedFiles = [];

        /// <summary>
        /// List of written files by their sector offset
        /// </summary>
        /// <remarks>Files of one directory hierarchy can share a sector offset, like an empty file and the file after it, so they are marked as extracted when the next directory hierarchy starts</remarks>
        private readonly Dictionary<int, uint> writtenFiles = [];

        /// <summary>
        /// List of multi-extent files written, by their FileIdentifier
        /// </summary>
        /// <remarks>Used as last-extent has mutli-extent flag=0</remarks>
        private readonly List<byte[]> multiExtentFiles = [];

        #endregion

        /// <inheritdoc/>
        public virtual bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no volume or directory descriptors, there is nothing to extract
            if (VolumeDescriptorSet.Length == 0 || DirectoryDescriptors.Count == 0)
                return true;

            // Clear the extraction state
            extractedFiles.Clear();
            writtenFiles.Clear();
            multiExtentFiles.Clear();

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

                // Note the files extracted from the directory hierarchies before this one
                foreach (var writtenFile in writtenFiles)
                {
                    extractedFiles[writtenFile.Key] = writtenFile.Value;
                }

                earlierExtractedFiles.Clear();
                foreach (int extractedLocation in extractedFiles.Keys)
                {
                    earlierExtractedFiles.Add(extractedLocation);
                }

                // Extract all files within root directory hierarchy
                allExtracted &= ExtractExtent(rootDirectoryRecord.ExtentLocation.LittleEndian, encoding, blockLength, outputDirectory, includeDebug);

                // If Big Endian extent location differs from Little Endian extent location, also extract that directory hierarchy
                if (!rootDirectoryRecord.ExtentLocation.IsValid)
                {
                    if (includeDebug) Console.WriteLine($"Extracting from volume descriptor (big endian root dir location)");
                    allExtracted &= ExtractExtent(rootDirectoryRecord.ExtentLocation.BigEndian, encoding, blockLength, outputDirectory, includeDebug);
                }

                // Also extract the directories that are only recorded in a path table
                foreach (var directory in GetPathTableDirectories(rootDirectoryRecord, encoding))
                {
                    string outputPath = Path.Combine(outputDirectory, directory.Value);
                    if (includeDebug) Console.WriteLine($"Extracting to directory: {outputPath}");

                    // Create directory (even if empty)
                    // Note: A directory whose files were all extracted from an earlier directory hierarchy is skipped
                    if (!Directory.Exists(outputPath) && !IsExtractedDirectory(directory.Key))
                        Directory.CreateDirectory(outputPath);

                    ExtractExtent(directory.Key, encoding, blockLength, outputPath, includeDebug);
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
                    // Note: A directory whose files were all extracted from an earlier directory hierarchy is skipped
                    // Note: A directory with an ambiguous extent location is always created, as both extents are extracted
                    if (!string.IsNullOrEmpty(outputPath) && !Directory.Exists(outputPath) && (!dr.ExtentLocation.IsValid || !IsExtractedDirectory(dr.ExtentLocation.LittleEndian)))
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
        /// Get the directories that are recorded in the path tables of a directory hierarchy but not reached from its root directory
        /// </summary>
        /// <param name="rootDirectoryRecord">Root directory record of the directory hierarchy</param>
        /// <param name="encoding">Encoding of the directory identifiers</param>
        /// <returns>Extent location and path of each directory, in path table order</returns>
        private List<KeyValuePair<int, string>> GetPathTableDirectories(DirectoryRecord rootDirectoryRecord, Encoding encoding)
        {
            List<KeyValuePair<int, string>> directories = [];
            Dictionary<int, bool> reached = [];
            AddReachedDirectories(rootDirectoryRecord.ExtentLocation.LittleEndian, reached);
            AddReachedDirectories(rootDirectoryRecord.ExtentLocation.BigEndian, reached);

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

                        string path = Path.Combine(parentPath, encoding.GetString(pathTable[i].DirectoryIdentifier));
                        paths[i] = path;

                        // Each directory hierarchy is extracted once
                        if (AddReachedDirectories(pathTable[i].ExtentLocation, reached))
                            directories.Add(new KeyValuePair<int, string>(pathTable[i].ExtentLocation, path));
                    }
                }
            }

            return directories;
        }

        /// <summary>
        /// Add a directory and the directories its directory records point to, if it was not reached yet
        /// </summary>
        /// <param name="extentLocation">Extent location of the directory</param>
        /// <param name="reached">Extent locations of the reached directories</param>
        /// <returns>True if a directory was reached, false otherwise</returns>
        private bool AddReachedDirectories(int extentLocation, Dictionary<int, bool> reached)
        {
            if (reached.ContainsKey(extentLocation) || !DirectoryDescriptors.TryGetValue(extentLocation, out FileExtent? value) || value is not DirectoryExtent dir)
                return false;

            reached[extentLocation] = true;
            foreach (var dr in dir.DirectoryRecords)
            {
                AddReachedDirectories(dr.ExtentLocation.LittleEndian, reached);
                AddReachedDirectories(dr.ExtentLocation.BigEndian, reached);
            }

            return true;
        }

        /// <summary>
        /// Check if all files of a directory were extracted from an earlier directory hierarchy
        /// </summary>
        /// <param name="extentLocation">Extent location of the directory</param>
        /// <returns>True if the directory and its subdirectories have files and all of them were extracted from an earlier directory hierarchy, false otherwise</returns>
        /// <remarks>Such a directory is the record of an extracted directory in another directory hierarchy, like the primary volume's record of a Joliet directory</remarks>
        private bool IsExtractedDirectory(int extentLocation)
        {
            bool hasFiles = false;
            return HasOnlyEarlierExtractedFiles(extentLocation, ref hasFiles) && hasFiles;
        }

        /// <summary>
        /// Check if all files of a directory and its subdirectories were extracted from an earlier directory hierarchy
        /// </summary>
        /// <param name="extentLocation">Extent location of the directory</param>
        /// <param name="hasFiles">Set to true if the directory or a subdirectory has a file</param>
        /// <returns>False if a file was not extracted from an earlier directory hierarchy, true otherwise</returns>
        private bool HasOnlyEarlierExtractedFiles(int extentLocation, ref bool hasFiles)
        {
            if (!DirectoryDescriptors.TryGetValue(extentLocation, out FileExtent? value) || value is not DirectoryExtent dir)
                return true;

            foreach (var dr in dir.DirectoryRecords)
            {
#if NET20 || NET35
                if ((dr.FileFlags & FileFlags.DIRECTORY) != 0)
#else
                if (dr.FileFlags.HasFlag(FileFlags.DIRECTORY))
#endif
                {
                    // Don't check up or self
                    if (dr.FileIdentifier.EqualsExactly(Constants.CurrentDirectory) || dr.FileIdentifier.EqualsExactly(Constants.ParentDirectory))
                        continue;

                    if (!HasOnlyEarlierExtractedFiles(dr.ExtentLocation.LittleEndian, ref hasFiles))
                        return false;
                }
#if NET20 || NET35
                else if ((dr.FileFlags & FileFlags.ASSOCIATED_FILE) != 0)
#else
                else if (dr.FileFlags.HasFlag(FileFlags.ASSOCIATED_FILE))
#endif
                {
                    // Don't count associated files as extracted, so their directory is always created
                    return false;
                }
                else if (earlierExtractedFiles.Contains(dr.ExtentLocation.LittleEndian))
                {
                    hasFiles = true;
                }
                else
                {
                    return false;
                }
            }

            return true;
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
                    // A file sharing the location of a written file is only extracted if its name is free
                    if (writtenFiles.ContainsKey(extentLocation))
                        return true;

                    if (includeDebug) Console.WriteLine($"File/Folder already exists, cannot extract: {filename}");
                    return false;
                }
            }

            const uint chunkSize = 2048 * 1024;
            lock (_dataSourceLock)
            {
                long fileOffset = ((long)extentLocation + dr.ExtendedAttributeRecordLength) * blockLength;
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
                writtenFiles[extentLocation] = dr.ExtentLength;
            }

            return true;
        }
    }
}
