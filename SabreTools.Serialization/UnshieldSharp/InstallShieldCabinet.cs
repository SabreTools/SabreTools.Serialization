using System;
using System.IO;
using SabreTools.Hashing;
using SabreTools.IO.Compression.zlib;
using SabreTools.Models.InstallShieldCabinet;
using Header = SabreTools.Serialization.Wrappers.InstallShieldCabinet;

namespace UnshieldSharpInternal
{
    // TODO: Figure out if individual parts of a split cab can be extracted separately
    internal class InstallShieldCabinet
    {
        /// <summary>
        /// Linked CAB headers
        /// </summary>
        public Header? HeaderList { get; private set; }

        /// <summary>
        /// Base filename path for related CAB files
        /// </summary>
        public string? FilenamePattern { get; private set; }

        /// <summary>
        /// Default buffer size
        /// </summary>
        private const int BUFFER_SIZE = 64 * 1024;

        /// <summary>
        /// Maximum size of the window in bits
        /// </summary>
        /// TODO: Remove when Serialization is updated
        private const int MAX_WBITS = 15;

        #region Open Cabinet

        /// <summary>
        /// Open a file as an InstallShield CAB
        /// </summary>
        public static InstallShieldCabinet? Open(string filename)
        {
            var cabinet = new InstallShieldCabinet();

            cabinet.FilenamePattern = Header.CreateFilenamePattern(filename);
            if (cabinet.FilenamePattern == null)
            {
                Console.Error.WriteLine("Failed to create filename pattern");
                return null;
            }

            cabinet.HeaderList = Header.OpenSet(cabinet.FilenamePattern);
            if (cabinet.HeaderList == null)
            {
                Console.Error.WriteLine("Failed to read header files");
                return null;
            }

            return cabinet;
        }

        #endregion

        #region File

        /// <summary>
        /// Save the file at the given index to the filename specified
        /// </summary>
        public bool FileSave(int index, string filename, bool useOld = false)
        {
            if (HeaderList == null)
            {
                Console.Error.WriteLine("Header list is not built");
                return false;
            }

            // Get the file descriptor
            if (!HeaderList.TryGetFileDescriptor(index, out var fileDescriptor) || fileDescriptor == null)
                return false;

            // If the file is split
            if (fileDescriptor.LinkFlags == LinkFlags.LINK_PREV)
                return FileSave((int)fileDescriptor.LinkPrevious, filename, useOld);

            // Get the reader at the index
            var reader = Reader.Create(this, index, fileDescriptor);
            if (reader == null)
                return false;

            // Create the output file and hasher
            FileStream output = File.OpenWrite(filename);
            var md5 = new HashWrapper(HashType.MD5);

            ulong bytesLeft = Header.GetReadableBytes(fileDescriptor);
            byte[] inputBuffer;
            byte[] outputBuffer = new byte[BUFFER_SIZE];
            ulong totalWritten = 0;

            // Read while there are bytes remaining
            while (bytesLeft > 0)
            {
                ulong bytesToWrite = BUFFER_SIZE;
                int result;

                // Handle compressed files
#if NET20 || NET35
                if ((fileDescriptor.Flags & FileFlags.FILE_COMPRESSED) != 0)
#else
                if (fileDescriptor.Flags.HasFlag(FileFlags.FILE_COMPRESSED))
#endif
                {
                    // Attempt to read the length value
                    byte[] lengthArr = new byte[sizeof(ushort)];
                    if (!reader.Read(lengthArr, 0, lengthArr.Length))
                    {
                        Console.Error.WriteLine($"Failed to read {lengthArr.Length} bytes of file {index} ({HeaderList.GetFileName(index)}) from input cabinet file {fileDescriptor.Volume}");
                        reader.Dispose();
                        output?.Close();
                        return false;
                    }

                    // Validate the number of bytes to read
                    ushort bytesToRead = BitConverter.ToUInt16(lengthArr, 0);
                    if (bytesToRead == 0)
                    {
                        Console.Error.WriteLine("bytesToRead can't be zero");
                        reader.Dispose();
                        output?.Close();
                        return false;
                    }

                    // Attempt to read the specified number of bytes
                    inputBuffer = new byte[BUFFER_SIZE + 1];
                    if (!reader.Read(inputBuffer, 0, bytesToRead))
                    {
                        Console.Error.WriteLine($"Failed to read {lengthArr.Length} bytes of file {index} ({HeaderList.GetFileName(index)}) from input cabinet file {fileDescriptor.Volume}");
                        reader.Dispose();
                        output?.Close();
                        return false;
                    }

                    // Add a null byte to make inflate happy
                    inputBuffer[bytesToRead] = 0;
                    ulong readBytes = (ulong)(bytesToRead + 1);

                    // Uncompress into a buffer
                    if (useOld)
                        result = Header.UncompressOld(outputBuffer, ref bytesToWrite, inputBuffer, ref readBytes);
                    else
                        result = Header.Uncompress(outputBuffer, ref bytesToWrite, inputBuffer, ref readBytes);

                    // If we didn't get a positive result that's not a data error (false positives)
                    if (result != zlibConst.Z_OK && result != zlibConst.Z_DATA_ERROR)
                    {
                        Console.Error.WriteLine($"Decompression failed with code {result.ToZlibConstName()}. bytes_to_read={bytesToRead}, volume={fileDescriptor.Volume}, read_bytes={readBytes}");
                        reader.Dispose();
                        output?.Close();
                        return false;
                    }

                    // Set remaining bytes
                    bytesLeft -= 2;
                    bytesLeft -= bytesToRead;
                }

                // Handle uncompressed files
                else
                {
                    bytesToWrite = Math.Min(bytesLeft, BUFFER_SIZE);
                    if (!reader.Read(outputBuffer, 0, (int)bytesToWrite))
                    {
                        Console.Error.WriteLine($"Failed to write {bytesToWrite} bytes from input cabinet file {fileDescriptor.Volume}");
                        reader.Dispose();
                        output?.Close();
                        return false;
                    }

                    // Set remaining bytes
                    bytesLeft -= (uint)bytesToWrite;
                }

                // Hash and write the next block
                md5.Process(outputBuffer, 0, (int)bytesToWrite);
                output?.Write(outputBuffer, 0, (int)bytesToWrite);
                totalWritten += bytesToWrite;
            }

            // Validate the number of bytes written
            if (fileDescriptor.ExpandedSize != totalWritten)
            {
                Console.Error.WriteLine($"Expanded size expected to be {fileDescriptor.ExpandedSize}, but was {totalWritten}");
                reader.Dispose();
                output?.Close();
                return false;
            }

            // Finalize output values
            md5.Terminate();
            reader?.Dispose();
            output?.Close();

            // Failing the file has been disabled because for a subset of CABs the values don't seem to match
            // TODO: Investigate what is causing this to fail and what data needs to be hashed

            // // Validate the data written, if required
            // if (HeaderList!.MajorVersion >= 6)
            // {
            //     string? md5result = md5.CurrentHashString;
            //     if (md5result == null || md5result != BitConverter.ToString(fileDescriptor.MD5!))
            //     {
            //         Console.Error.WriteLine($"MD5 checksum failure for file {index} ({HeaderList.GetFileName(index)})");
            //         return false;
            //     }
            // }

            return true;
        }

        /// <summary>
        /// Save the file at the given index to the filename specified as raw
        /// </summary>
        public bool FileSaveRaw(int index, string filename)
        {
            if (HeaderList == null)
            {
                Console.Error.WriteLine("Header list is not built");
                return false;
            }

            // Get the file descriptor
            if (!HeaderList.TryGetFileDescriptor(index, out var fileDescriptor) || fileDescriptor == null)
                return false;

            // If the file is split
            if (fileDescriptor.LinkFlags == LinkFlags.LINK_PREV)
                return FileSaveRaw((int)fileDescriptor.LinkPrevious, filename);

            // Get the reader at the index
            var reader = Reader.Create(this, index, fileDescriptor);
            if (reader == null)
                return false;

            // Create the output file
            FileStream output = File.OpenWrite(filename);

            ulong bytesLeft = Header.GetReadableBytes(fileDescriptor);
            byte[] outputBuffer = new byte[BUFFER_SIZE];

            // Read while there are bytes remaining
            while (bytesLeft > 0)
            {
                ulong bytesToWrite = Math.Min(bytesLeft, BUFFER_SIZE);
                if (!reader.Read(outputBuffer, 0, (int)bytesToWrite))
                {
                    Console.Error.WriteLine($"Failed to read {bytesToWrite} bytes from input cabinet file {fileDescriptor.Volume}");
                    reader.Dispose();
                    output?.Close();
                    return false;
                }

                // Set remaining bytes
                bytesLeft -= (uint)bytesToWrite;

                // Write the next block
                output.Write(outputBuffer, 0, (int)bytesToWrite);
            }

            // Finalize output values
            reader.Dispose();
            output?.Close();
            return true;
        }

        #endregion
    }
}
