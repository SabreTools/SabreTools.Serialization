using System;
using System.IO;
using System.Text;
using SabreTools.Models.TAR;
using SabreTools.Serialization.Interfaces;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
#endif

namespace SabreTools.Serialization.Wrappers
{
    public class TapeArchive : WrapperBase<Archive>, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Tape Archive (or Derived Format)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.Entries"/>
        /// TODO: Simplify when Models is updated
        public Entry[]? Entries => Model.Entries != null ? [.. Model.Entries] : null;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public TapeArchive(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public TapeArchive(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a tape archive (or derived format) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A tape archive wrapper on success, null on failure</returns>
        public static TapeArchive? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a tape archive (or derived format) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A tape archive wrapper on success, null on failure</returns>
        public static TapeArchive? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var model = Deserializers.TapeArchive.DeserializeStream(data);
                if (model == null)
                    return null;

                return new TapeArchive(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region JSON Export

#if NETCOREAPP
        /// <inheritdoc/>
        public override string ExportJSON() => throw new System.NotImplementedException();
#endif

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            if (_dataSource == null || !_dataSource.CanRead)
                return false;

#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                var tarFile = TarArchive.Open(_dataSource);

                // Try to read the file path if no entries are found
                if (tarFile.Entries.Count == 0 && !string.IsNullOrEmpty(Filename) && File.Exists(Filename!))
                    tarFile = TarArchive.Open(Filename!);

                foreach (var entry in tarFile.Entries)
                {
                    try
                    {
                        // If the entry is a directory
                        if (entry.IsDirectory)
                            continue;

                        // If the entry has an invalid key
                        if (entry.Key == null)
                            continue;

                        // If we have a partial entry due to an incomplete multi-part archive, skip it
                        if (!entry.IsComplete)
                            continue;

                        // Ensure directory separators are consistent
                        string filename = entry.Key;
                        if (Path.DirectorySeparatorChar == '\\')
                            filename = filename.Replace('/', '\\');
                        else if (Path.DirectorySeparatorChar == '/')
                            filename = filename.Replace('\\', '/');

                        // Ensure the full output directory exists
                        filename = Path.Combine(outputDirectory, filename);
                        var directoryName = Path.GetDirectoryName(filename);
                        if (directoryName != null && !Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        entry.WriteToFile(filename);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.Error.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
#else
            return false;
#endif
        }

        /// <inheritdoc cref="Extract(string, bool)"/>
        public bool ExtractExperimental(string outputDirectory, bool includeDebug)
        {
            // Ensure there are entries to extract
            if (Entries == null || Entries.Length == 0)
                return true;

            try
            {
                // Loop through and extract the data
                for (int i = 0; i < Entries.Length; i++)
                {
                    var entry = Entries[i];
                    if (entry.Header == null)
                        continue;

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
                            continue;

                        // Directories
                        case TypeFlag.DIRTYPE:
                            string? entryDirectory = header.FileName?.TrimEnd('\0');
                            if (entryDirectory == null)
                                continue;

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

                    // Ensure there are blocks to extract
                    if (entry.Blocks == null)
                        continue;

                    // Get the file size
                    string sizeOctalString = Encoding.ASCII.GetString(header.Size!).TrimEnd('\0');
                    if (sizeOctalString.Length == 0)
                        continue;

                    int entrySize = Convert.ToInt32(sizeOctalString, 8);

                    // Setup the temporary buffer
                    byte[] dataBytes = new byte[entrySize];
                    int dataBytesPtr = 0;

                    // Loop through and copy the bytes to the array for writing
                    int blockNumber = 0;
                    while (entrySize > 0)
                    {
                        // Exit early if block number is invalid
                        if (blockNumber >= entry.Blocks.Count)
                            break;

                        // Exit early if the block has no data
                        var block = entry.Blocks[blockNumber++];
                        if (block.Data == null || block.Data.Length != 512)
                            break;

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

        #endregion
    }
}
