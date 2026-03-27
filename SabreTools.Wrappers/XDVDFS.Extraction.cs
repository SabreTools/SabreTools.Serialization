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
        private readonly List<uint> extractedFiles = [];

        #endregion

        /// <inheritdoc/>
        public virtual bool Extract(string outputDirectory, bool includeDebug)
        {
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

                    allExtracted |= ExtractDescriptor(outputDirectory, includeDebug, dr.ExtentOffset);
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
                    if (dr.ExtentOffset * Constants.SectorSize + dr.ExtentSize > _dataSource.Length)
                    {
                        if (includeDebug) Console.WriteLine($"Invalid file location for file {dr.Filename} at sector {dr.ExtentOffset}");
                        continue;
                    }
                }
            }

            return allExtracted;
        }
    }
}
