using System;
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using System.IO;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.ZArchive;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;
using SharpCompress.Compressors.ZStandard;
#endif

namespace SabreTools.Wrappers
{
    public partial class ZArchive : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            if (_dataSource is null || !_dataSource.CanRead)
                return false;

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            try
            {
                // Extract all files and directories from root (index 0)
                return ExtractDirectory(outputDirectory, includeDebug, 0);
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
#else
            Console.WriteLine("Extraction is not supported for this framework!");
            Console.WriteLine();
            return false;
#endif
        }

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
        /// <inheritdoc/>
        public bool ExtractDirectory(string outputDirectory, bool includeDebug, uint index)
        {
            bool success = true;

            // Create directory
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            // Extract all children of current node
            FileDirectoryEntry node = FileTree[index];
            if (node is DirectoryEntry dir)
            {
                for (uint i = 0; i < dir.Count; i++)
                {
                    uint childIndex = dir.NodeStartIndex + i;
                    var child = FileTree[childIndex];
                    string? name = child.GetName(NameTable);
                    if (string.IsNullOrEmpty(name))
                    {
                        if (includeDebug) Console.WriteLine("Invalid node name");
                        return false;
                    }

                    string outputPath = Path.Combine(outputDirectory, name);
                    if ((child.NameOffsetAndTypeFlag & Constants.FileFlag) == 0)
                        success |= ExtractDirectory(outputPath, includeDebug, childIndex);
                    else
                        success |= ExtractFile(outputPath, includeDebug, childIndex);
                }

                return success;
            }
            else
            {
                if (includeDebug) Console.WriteLine("Invalid directory node");
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ExtractFile(string outputPath, bool includeDebug, uint index)
        {
            // Decompress each chunk to output
            var node = FileTree[index];
            var dataOffset = Footer.SectionCompressedData.Offset;
            var dataLength = Footer.SectionCompressedData.Size;
            if (node is FileEntry file)
            {
                ulong fileOffset = ((ulong)file.FileOffsetHigh << 32) | file.FileOffsetLow;
                ulong fileSize = ((ulong)file.FileSizeHigh << 32) | file.FileSizeLow;

                // Write the output file
                if (includeDebug) Console.WriteLine($"Extracting: {outputPath}");
                using var fs = File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                ulong fileProgress = 0;

                lock (_dataSourceLock)
                {
                    while (fileProgress < fileSize)
                    {
                        // Determine offset and size of next read
                        ulong absoluteOffset = fileOffset + fileProgress;
                        ulong blockIndex = absoluteOffset / Constants.BlockSize;
                        int recordIndex = (int)(blockIndex / Constants.BlocksPerOffsetRecord);
                        if (recordIndex >= OffsetRecords.Length)
                        {
                            if (includeDebug) Console.WriteLine($"File offset out of range: {outputPath}");
                            return false;
                        }

                        var offsetRecord = OffsetRecords[recordIndex];
                        int withinRecordIndex = (int)(blockIndex % Constants.BlocksPerOffsetRecord);
                        if (withinRecordIndex >= offsetRecord.Size.Length)
                        {
                            if (includeDebug) Console.WriteLine($"Blocks per record mismatch: {outputPath}");
                            return false;
                        }

                        int intraBlockOffset = (int)(absoluteOffset % Constants.BlockSize);
                        int bytesToRead = offsetRecord.Size[withinRecordIndex] + 1;
                        int bytesToWrite = (int)Math.Min(fileSize - fileProgress, Constants.BlockSize - (ulong)intraBlockOffset);

                        ulong readOffset = dataOffset + offsetRecord.Offset;
                        for (int i = 0; i < withinRecordIndex; i++)
                        {
                            readOffset += (ulong)offsetRecord.Size[i] + 1;
                        }

                        // Seek to location of block
                        _dataSource.SeekIfPossible((long)readOffset, SeekOrigin.Begin);

                        // Ensure block doesn't exceed compressed section
                        if (offsetRecord.Offset + (ulong)bytesToRead > dataLength)
                        {
                            if (includeDebug) Console.WriteLine("Block exceeds compressed data section");
                            return false;
                        }

                        // Ensure reader won't EOF
                        if (bytesToRead > _dataSource.Length - _dataSource.Position)
                        {
                            if (includeDebug) Console.WriteLine($"File out of bounds: {outputPath}");
                            return false;
                        }

                        // Read block
                        var buffer = _dataSource.ReadBytes(bytesToRead);

                        // Write entire block if it is uncompressed
                        if (bytesToRead == Constants.BlockSize)
                        {
                            // Block is stored uncompressed
                            fs.Write(buffer, intraBlockOffset, bytesToWrite);
                            fileProgress += (ulong)bytesToWrite;
                            continue;
                        }

                        // Decompress block
                        byte[] decompressedBuffer;
                        using var inputStream = new MemoryStream(buffer);
                        using var zstdStream = new ZStandardStream(inputStream);
                        using var outputStream = new MemoryStream();
                        zstdStream.CopyTo(outputStream);
                        decompressedBuffer = outputStream.ToArray();
                        if (decompressedBuffer.Length != Constants.BlockSize)
                        {
                            if (includeDebug) Console.WriteLine($"Invalid decompressed block size {decompressedBuffer.Length}");
                            return false;
                        }

                        // Write decompressed block to output file
                        fs.Write(decompressedBuffer, intraBlockOffset, bytesToWrite);
                        fileProgress += (ulong)bytesToWrite;
                    }
                }

                return true;
            }
            else
            {
                if (includeDebug) Console.WriteLine("Invalid file node");
                return false;
            }
        }
#endif
    }
}
