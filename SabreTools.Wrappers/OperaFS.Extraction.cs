using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.OperaFS;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class OperaFS : IExtractable
    {
        #region Extraction State

        /// <summary>
        /// List of extracted files by their sector offset
        /// </summary>
        private readonly List<uint> extractedFiles = [];

        /// <summary>
        /// List of extracted directories
        /// </summary>
        private readonly List<DirectoryDescriptor> extractedDirectories = [];

        #endregion

        /// <inheritdoc/>
        public virtual bool Extract(string outputDirectory, bool includeDebug)
        {
            // Clear the extraction state
            extractedFiles.Clear();
            extractedDirectories.Clear();

            bool allExtracted = true;

            for (int i = 0; i <= VolumeDescriptor.RootDirectoryLastAvatarIndex; i++)
            {
                var rootDirectory = Directories[VolumeDescriptor.RootDirectoryAvatarList[i]];
                if (extractedDirectories.Contains(rootDirectory))
                {
                    if (includeDebug) Console.WriteLine($"Root directory duplicate at sector {VolumeDescriptor.RootDirectoryAvatarList[i]}");
                    continue;
                }

                if (includeDebug) Console.WriteLine($"Extracting from root directory at sector {VolumeDescriptor.RootDirectoryAvatarList[i]}");
                allExtracted |= ExtractDirectory(outputDirectory, includeDebug, rootDirectory);
                extractedDirectories.Add(rootDirectory);
            }

            return allExtracted;
        }

        public bool ExtractDirectory(string outputDirectory, bool includeDebug, DirectoryDescriptor dir)
        {
            // Create output directory if it does not exist
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            bool allExtracted = true;
            foreach (var dr in dir.DirectoryRecords)
            {
                var filename = Encoding.UTF8.GetString(dr.Filename).TrimEnd('\0');
                if ((dr.DirectoryRecordFlags & DirectoryRecordFlags.DIRECTORY) == 0)
                {
                    // Skip filesystem only files (e.g. Volume Descriptor "Disc Label")
                    if ((dr.DirectoryRecordFlags & DirectoryRecordFlags.SYSTEM) != 0)
                    {
                        if (includeDebug) Console.WriteLine($"Skipping filesystem object {filename}");
                        continue;
                    }

                    var filePath = Path.Combine(outputDirectory, filename);
                    for (int i = 0; i <= dr.LastAvatarIndex; i++)
                    {
                        uint fileOffset = (uint)Constants.SectorSize * dr.AvatarList[i];

                        // TODO: Check that all avatars are identical?
                        if (extractedFiles.Contains(fileOffset))
                        {
                            if (includeDebug) Console.WriteLine($"File duplicate at sector {dr.AvatarList[i]}");
                            continue;
                        }

                        try
                        {
                            if (File.Exists(filePath))
                                continue;

                            const uint chunkSize = 2048 * 1024;
                            lock (_dataSourceLock)
                            {
                                _dataSource.SeekIfPossible(fileOffset, SeekOrigin.Begin);

                                // Get the length, and make sure it won't EOF
                                uint length = dr.ByteCount;
                                if (length > _dataSource.Length - _dataSource.Position)
                                {
                                    allExtracted = false;
                                    continue;
                                }

                                // Write the output file
                                if (includeDebug) Console.WriteLine($"Extracting file {filename} at sector {dr.AvatarList[i]}");
                                using var fs = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                                while (length > 0)
                                {
                                    int bytesToRead = (int)Math.Min(length, chunkSize);

                                    byte[] buffer = _dataSource.ReadBytes(bytesToRead);
                                    fs.Write(buffer, 0, bytesToRead);
                                    fs.Flush();

                                    length -= (uint)bytesToRead;
                                }
                            }

                            extractedFiles.Add(fileOffset);
                        }
                        catch
                        {
                            allExtracted = false;
                        }
                    }
                }
                else
                {
                    // Iterate over all avatars, in case they're not identical
                    for (int i = 0; i <= dr.LastAvatarIndex; i++)
                    {
                        // Check whether directory is already extracted
                        var childDir = Directories[dr.AvatarList[i]];
                        if (extractedDirectories.Contains(childDir))
                        {
                            if (includeDebug) Console.WriteLine($"Directory duplicate at sector {dr.AvatarList[i]}");
                            continue;
                        }

                        try
                        {
                            if (includeDebug) Console.WriteLine($"Extracting directory {filename} at sector {dr.AvatarList[i]}");
                            var outputPath = Path.Combine(outputDirectory, filename);
                            allExtracted |= ExtractDirectory(outputPath, includeDebug, childDir);
                            extractedDirectories.Add(childDir);
                        }
                        catch
                        {
                            allExtracted = false;
                            break;
                        }
                    }
                }
            }

            return allExtracted;
        }
    }
}
