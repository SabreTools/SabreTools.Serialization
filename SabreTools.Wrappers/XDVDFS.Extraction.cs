using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.XDVDFS;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class XDVDFS : IExtractable
    {
        #region Extraction State

        /// <summary>
        /// List of extracted files by their sector offset
        /// </summary>
        private readonly Dictionary<uint, uint> extractedFiles = [];

        #endregion

        /// <inheritdoc/>
        public virtual bool Extract(string outputDirectory, bool includeDebug)
        {
            // Clear the extraction state
            extractedFiles.Clear();

            // Extract files from all directories from root directory
            return ExtractDescriptor(outputDirectory, includeDebug, VolumeDescriptor.RootOffset);
        }

        /// <summary>
        /// Extracts all directory records recursively from directory descriptor
        /// </summary>
        public bool ExtractDescriptor(string outputDirectory, bool includeDebug, uint sectorNumber)
        {
            // If no descriptor exists at that sector, we cannot extract from it 
            if (!DirectoryDescriptors.ContainsKey(sectorNumber))
                return false;
            
            bool allExtracted = true;

            // Extract directory records within directory descriptor
            foreach (var dr in DirectoryDescriptors[sectorNumber].DirectoryRecords)
            {
                // Skip invalid records
                if (dr.FilenameLength == 0 || dr.Filename is null)
                {
                    if (includeDebug) Console.WriteLine($"Empty filename in directory record at sector {sectorNumber}");
                    continue;
                }

                string outputPath = Path.Combine(outputDirectory, Encoding.UTF8.GetString(dr.Filename));

                // If record is a directory, create it and extract child records
                if ((dr.FileFlags & FileFlags.DIRECTORY) == FileFlags.DIRECTORY)
                {
                    if (!string.IsNullOrEmpty(outputPath) && !Directory.Exists(outputPath))
                        Directory.CreateDirectory(outputPath);

                    allExtracted |= ExtractDescriptor(outputPath, includeDebug, dr.ExtentOffset);
                }
                else
                {
                    // Skip invalid file size
                    if (dr.ExtentSize == 0)
                    {
                        if (includeDebug) Console.WriteLine($"Zero file size for file {dr.Filename} at sector {dr.ExtentOffset}");
                        continue;
                    }

                    // Skip invalid file location
                    if (((long)dr.ExtentOffset) * Constants.SectorSize + dr.ExtentSize > _dataSource.Length)
                    {
                        if (includeDebug) Console.WriteLine($"Invalid file location for file {dr.Filename} at sector {dr.ExtentOffset}");
                        continue;
                    }

                    // Check that the file hasn't been extracted already
                    if (extractedFiles.ContainsKey(dr.ExtentOffset))
                    {
                        if (includeDebug) Console.WriteLine($"File {dr.Filename} at sector {dr.ExtentOffset} already extracted");
                        continue;
                    }

                    // Read and extract the file extent
                    const uint chunkSize = 2048 * 1024;
                    lock (_dataSourceLock)
                    {
                        long fileOffset = ((long)dr.ExtentOffset) * Constants.SectorSize;
                        _dataSource.SeekIfPossible(fileOffset, SeekOrigin.Begin);

                        // Get the length, and make sure it won't EOF
                        uint length = dr.ExtentSize;
                        if (length > _dataSource.Length - _dataSource.Position)
                            return false;

                        // Write the output file
                        if (includeDebug) Console.WriteLine($"Extracting: {outputPath}");
                        using var fs = File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        while (length > 0)
                        {
                            int bytesToRead = (int)Math.Min(length, chunkSize);

                            byte[] buffer = _dataSource.ReadBytes(bytesToRead);
                            fs.Write(buffer, 0, bytesToRead);
                            fs.Flush();

                            length -= (uint)bytesToRead;
                        }
                    }

                    // Mark the file as extracted
                    extractedFiles.Add(dr.ExtentOffset, dr.ExtentSize);

                    // Don't set any file attributes if file is normal
                    if ((dr.FileFlags & FileFlags.NORMAL) == FileFlags.NORMAL))
                        continue;
                    
                    // Copy over hidden flag
                    if ((dr.FileFlags & FileFlags.HIDDEN) == FileFlags.HIDDEN))
                        File.SetAttributes(path, FileAttributes.Hidden);
                    
                    // Copy over read-only flag
                    if ((dr.FileFlags & FileFlags.READ_ONLY) == FileFlags.READ_ONLY))
                        File.SetAttributes(path, FileAttributes.ReadOnly);
                    
                    // Copy over system flag
                    if ((dr.FileFlags & FileFlags.SYSTEM) == FileFlags.SYSTEM))
                        File.SetAttributes(path, FileAttributes.System);
                }
            }

            return allExtracted;
        }
    }
}
