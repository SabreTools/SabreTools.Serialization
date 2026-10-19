using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.UDF;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class UDF : IExtractable
    {
        /// <inheritdoc/>
        public virtual bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no file set or directories, there is nothing to extract
            if (FileSetDescriptor is null || Directories.Count == 0)
                return true;

            long root = FileSetDescriptor.RootDirectoryICB.ExtentLocation.GetKey();
            var visited = new HashSet<long> { root };
            return ExtractDirectory(root, Path.GetFullPath(outputDirectory), visited, includeDebug);
        }

        /// <summary>
        /// Extract all files and directories within a directory
        /// </summary>
        private bool ExtractDirectory(long key, string outputPath, HashSet<long> visited, bool includeDebug)
        {
            if (!Directories.TryGetValue(key, out var fids))
                return false;

            bool succeeded = true;
            foreach (var fid in fids)
            {
                // Skip deleted entries and the parent directory
                if ((fid.FileCharacteristics & (FileCharacteristics.DELETED | FileCharacteristics.PARENT)) != 0)
                    continue;

                string? name = fid.FileIdentifier.DecodeDCharacters();
                if (!IsSafeName(name))
                {
                    if (includeDebug) Console.WriteLine($"Skipping entry with unsafe name in {outputPath}");

                    succeeded = false;
                    continue;
                }

                long childKey = fid.ICB.ExtentLocation.GetKey();
                if (!FileEntries.TryGetValue(childKey, out var entry))
                {
                    if (includeDebug) Console.WriteLine($"File Entry missing, cannot extract: {name}");

                    succeeded = false;
                    continue;
                }

                string childPath = Path.Combine(outputPath, name!);
                if (entry.ICBTag.FileType == FileType.DIRECTORY)
                {
                    if (includeDebug) Console.WriteLine($"Extracting to directory: {childPath}");
                    if (!Directory.Exists(childPath))
                        Directory.CreateDirectory(childPath);

                    // Directories reachable twice (hard links, loops) are extracted once
                    if (visited.Add(childKey))
                        succeeded &= ExtractDirectory(childKey, childPath, visited, includeDebug);
                }
                else
                {
                    succeeded &= ExtractFile(childKey, entry, childPath, includeDebug);
                }
            }

            return succeeded;
        }

        /// <summary>
        /// Extract a file's data
        /// </summary>
        private bool ExtractFile(long key, FileEntry entry, string filePath, bool includeDebug)
        {
            if (File.Exists(filePath) || Directory.Exists(filePath))
            {
                if (includeDebug) Console.WriteLine($"File/Folder already exists, cannot extract: {filePath}");
                return false;
            }

            if (includeDebug) Console.WriteLine($"Extracting: {filePath}");

            long size = (long)entry.InformationLength;
            if (entry.ICBTag.GetAllocationDescriptorType() == AllocationDescriptorType.EMBEDDED)
            {
                using var embedded = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                embedded.Write(entry.AllocationDescriptors, 0, (int)size);
                return true;
            }

            var descriptors = Model.GetAllocationDescriptors(key, entry);
            if (descriptors is null)
            {
                if (includeDebug) Console.WriteLine($"Allocation descriptors not supported or incomplete: {filePath}");
                return false;
            }

            const int chunkSize = 2048 * 1024;
            using var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            long written = 0;
            foreach (var ad in descriptors)
            {
                if (written >= size)
                    break;

                long length = Math.Min(UDFExtensions.GetExtentLength(ad.ExtentLength), size - written);
                bool recorded = UDFExtensions.GetExtentType(ad.ExtentLength) == ExtentType.RECORDED_AND_ALLOCATED;

                long sector = 0;
                if (recorded && !Model.TryGetSector(ad.ExtentLocation.GetKey(), out sector))
                    return false;

                // Make sure the extent won't EOF
                if (recorded && (sector * Constants.SectorSize) + length > _dataSource.Length)
                    return false;

                long done = 0;
                while (done < length)
                {
                    int count = (int)Math.Min(chunkSize, length - done);
                    byte[] buffer;
                    if (recorded)
                    {
                        lock (_dataSourceLock)
                        {
                            _dataSource.SeekIfPossible((sector * Constants.SectorSize) + done, SeekOrigin.Begin);
                            buffer = _dataSource.ReadBytes(count);
                        }
                    }
                    else
                    {
                        // Extents that are not recorded read as zeros
                        buffer = new byte[count];
                    }

                    fs.Write(buffer, 0, count);
                    done += count;
                }

                written += length;
            }

            return written == size;
        }

        /// <summary>
        /// Check that a file identifier can be used as a single path component
        /// </summary>
        private static bool IsSafeName(string? name)
        {
            if (string.IsNullOrEmpty(name) || name == "." || name == "..")
                return false;

            char[] invalid = Path.GetInvalidFileNameChars();
            foreach (char c in name!)
            {
                if (c == '/' || c == '\\' || c == '\0' || Array.IndexOf(invalid, c) >= 0)
                    return false;
            }

            return true;
        }
    }
}
