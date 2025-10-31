using System;
using System.IO;
using SabreTools.Data.Models.TAR;

namespace SabreTools.Serialization.Wrappers
{
    public partial class TapeArchive : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Ensure there are entries to extract
            if (Entries.Length == 0)
                return false;

            try
            {
                // Loop through and extract the data
                for (int i = 0; i < Entries.Length; i++)
                {
                    var entry = Entries[i];

                    // Handle special entries
                    var header = entry.Header;
                    switch (header.TypeFlag)
                    {
                        // Skipped types
                        case TypeFlag.LNKTYPE:
                        case TypeFlag.SYMTYPE:
                        case TypeFlag.CHRTYPE:
                        case TypeFlag.BLKTYPE:
                        case TypeFlag.FIFOTYPE:
                        case TypeFlag.XHDTYPE:
                        case TypeFlag.XGLTYPE:
                            if (includeDebug) Console.WriteLine($"Unsupported entry type: {header.TypeFlag}");
                            continue;

                        // Skipped vendor types
                        case TypeFlag.VendorSpecificA:
                        case TypeFlag.VendorSpecificB:
                        case TypeFlag.VendorSpecificC:
                        case TypeFlag.VendorSpecificD:
                        case TypeFlag.VendorSpecificE:
                        case TypeFlag.VendorSpecificF:
                        case TypeFlag.VendorSpecificG:
                        case TypeFlag.VendorSpecificH:
                        case TypeFlag.VendorSpecificI:
                        case TypeFlag.VendorSpecificJ:
                        case TypeFlag.VendorSpecificK:
                        case TypeFlag.VendorSpecificL:
                        case TypeFlag.VendorSpecificM:
                        case TypeFlag.VendorSpecificN:
                        case TypeFlag.VendorSpecificO:
                        case TypeFlag.VendorSpecificP:
                        case TypeFlag.VendorSpecificQ:
                        case TypeFlag.VendorSpecificR:
                        case TypeFlag.VendorSpecificS:
                        case TypeFlag.VendorSpecificT:
                        case TypeFlag.VendorSpecificU:
                        case TypeFlag.VendorSpecificV:
                        case TypeFlag.VendorSpecificW:
                        case TypeFlag.VendorSpecificX:
                        case TypeFlag.VendorSpecificY:
                        case TypeFlag.VendorSpecificZ:
                            if (includeDebug) Console.WriteLine($"Unsupported vendor entry type: {header.TypeFlag}");
                            continue;

                        // Directories
                        case TypeFlag.DIRTYPE:
                            string? entryDirectory = header.FileName?.TrimEnd('\0');
                            if (entryDirectory == null)
                            {
                                if (includeDebug) Console.Error.WriteLine($"Entry {i} reported as directory, but no path found! Skipping...");
                                continue;
                            }

                            // Ensure directory separators are consistent
                            entryDirectory = Path.Combine(outputDirectory, entryDirectory);
                            if (Path.DirectorySeparatorChar == '\\')
                                entryDirectory = entryDirectory.Replace('/', '\\');
                            else if (Path.DirectorySeparatorChar == '/')
                                entryDirectory = entryDirectory.Replace('\\', '/');

                            // Create the director
                            Directory.CreateDirectory(entryDirectory);
                            continue;
                    }

                    // Get the file size
                    string sizeOctalString = header.Size!.TrimEnd('\0');
                    if (sizeOctalString.Length == 0)
                    {
                        if (includeDebug) Console.WriteLine($"Entry {i} has an invalid size, skipping...");
                        continue;
                    }

                    int entrySize = Convert.ToInt32(sizeOctalString, 8);

                    // Setup the temporary buffer
                    byte[] dataBytes = new byte[entrySize];
                    int dataBytesPtr = 0;

                    // Loop through and copy the bytes to the array for writing
                    int blockNumber = 0;
                    while (entrySize > 0)
                    {
                        // Exit early if block number is invalid
                        if (blockNumber >= entry.Blocks.Length)
                        {
                            if (includeDebug) Console.Error.WriteLine($"Invalid block number {i + 1} of {entry.Blocks.Length}, file may be incomplete!");
                            break;
                        }

                        // Exit early if the block has no data
                        var block = entry.Blocks[blockNumber++];
                        if (block.Data == null || block.Data.Length != 512)
                        {
                            if (includeDebug) Console.Error.WriteLine($"Invalid data for block number {i + 1}, file may be incomplete!");
                            break;
                        }

                        int nextBytes = Math.Min(512, entrySize);
                        entrySize -= nextBytes;

                        Array.Copy(block.Data, 0, dataBytes, dataBytesPtr, nextBytes);
                        dataBytesPtr += nextBytes;
                    }

                    // Ensure directory separators are consistent
                    string filename = header.FileName?.TrimEnd('\0') ?? $"entry_{i}";
                    if (Path.DirectorySeparatorChar == '\\')
                        filename = filename.Replace('/', '\\');
                    else if (Path.DirectorySeparatorChar == '/')
                        filename = filename.Replace('\\', '/');

                    // Ensure the full output directory exists
                    filename = Path.Combine(outputDirectory, filename);
                    var directoryName = Path.GetDirectoryName(filename);
                    if (directoryName != null && !Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    // Write the file
                    using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                    fs.Write(dataBytes, 0, dataBytes.Length);
                    fs.Flush();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }
    }
}
