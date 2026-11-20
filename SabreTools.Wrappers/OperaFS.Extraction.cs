using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.OperaFS;
using SabreTools.IO.Extensions;
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
                allExtracted |= ExtractDirectory(outputDirectory, includeDebug, VolumeDescriptor.RootDirectoryAvatarList[i], VolumeDescriptor.RootDirectoryBlockCount);
                extractedDirectories.Add(rootDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a directory from its first block and the next blocks
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <param name="sector">Sector of the first block</param>
        /// <param name="blockCount">Number of blocks of the directory</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractDirectory(string outputDirectory, bool includeDebug, uint sector, uint blockCount)
        {
            var dir = Directories[sector];
            bool allExtracted = ExtractDirectory(outputDirectory, includeDebug, dir);

            // The next block is relative to the first block, and -1 for the last block
            var blocks = new HashSet<uint> { sector };
            while (dir.NextBlock >= 0 && dir.NextBlock < blockCount)
            {
                uint blockSector = sector + (uint)dir.NextBlock;
                if (!blocks.Add(blockSector) || !Directories.TryGetValue(blockSector, out dir))
                    break;

                allExtracted &= ExtractDirectory(outputDirectory, includeDebug, dir);
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
                        // Skip directories that could not be read
                        if (!Directories.TryGetValue(dr.AvatarList[i], out var childDir))
                        {
                            if (includeDebug) Console.WriteLine($"Directory missing at sector {dr.AvatarList[i]}");
                            continue;
                        }

                        // Check whether directory is already extracted
                        if (extractedDirectories.Contains(childDir))
                        {
                            if (includeDebug) Console.WriteLine($"Directory duplicate at sector {dr.AvatarList[i]}");
                            continue;
                        }

                        try
                        {
                            if (includeDebug) Console.WriteLine($"Extracting directory {filename} at sector {dr.AvatarList[i]}");
                            var outputPath = Path.Combine(outputDirectory, filename);

                            // Mark the directory first, so that a directory that records itself is extracted once
                            extractedDirectories.Add(childDir);
                            allExtracted |= ExtractDirectory(outputPath, includeDebug, dr.AvatarList[i], dr.BlockCount);
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
