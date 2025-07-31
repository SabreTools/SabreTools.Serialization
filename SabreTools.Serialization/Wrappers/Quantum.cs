using System;
using System.IO;
using SabreTools.Models.Quantum;

namespace SabreTools.Serialization.Wrappers
{
    public class Quantum : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Quantum Archive";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.CompressedDataOffset"/>
        public long CompressedDataOffset => Model.CompressedDataOffset;

        /// <inheritdoc cref="Header.FileCount"/>
        public ushort FileCount => Header?.FileCount ?? 0;

        /// <inheritdoc cref="Archive.FileList"/>
        public FileDescriptor[] FileList => Model.FileList ?? [];

        /// <inheritdoc cref="Archive.Header"/>
        public Header? Header => Model.Header;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public Quantum(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public Quantum(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a Quantum archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
        public static Quantum? Create(byte[]? data, int offset)
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
        /// Create a Quantum archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
        public static Quantum? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var archive = Deserializers.Quantum.DeserializeStream(data);
                if (archive == null)
                    return null;

                return new Quantum(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the Quantum archive to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (FileList == null || FileList.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < FileList.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the Quantum archive to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (Header == null || FileCount == 0 || FileList == null || FileList.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= FileList.Length)
                return false;

            // Get the file information
            var fileDescriptor = FileList[index];

            // Read the entire compressed data
            int compressedDataOffset = (int)CompressedDataOffset;
            int compressedDataLength = GetEndOfFile() - compressedDataOffset;
            var compressedData = ReadFromDataSource(compressedDataOffset, compressedDataLength);

            // Print a debug reminder
            if (includeDebug) Console.WriteLine("Quantum archive extraction is unsupported");

            // TODO: Figure out decompression
            // - Single-file archives seem to work
            // - Single-file archives with files that span a window boundary seem to work
            // - The first files in each archive seem to work
            return false;

            // // Setup the decompression state
            // State state = new State();
            // Decompressor.InitState(state, TableSize, CompressionFlags);

            // // Decompress the entire array
            // int decompressedDataLength = (int)FileList.Sum(fd => fd.ExpandedFileSize);
            // byte[] decompressedData = new byte[decompressedDataLength];
            // Decompressor.Decompress(state, compressedData.Length, compressedData, decompressedData.Length, decompressedData);

            // // Read the data
            // int offset = (int)FileList.Take(index).Sum(fd => fd.ExpandedFileSize);
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // Array.Copy(decompressedData, offset, data, 0, data.Length);

            // // Loop through all files before the current
            // for (int i = 0; i < index; i++)
            // {
            //     // Decompress the next block of data
            //     byte[] tempData = new byte[FileList[i].ExpandedFileSize];
            //     int lastRead = Decompressor.Decompress(state, compressedData.Length, compressedData, tempData.Length, tempData);
            //     compressedData = new ReadOnlySpan<byte>(compressedData, (lastRead), compressedData.Length - (lastRead)).ToArray();
            // }

            // // Read the data
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // _ = Decompressor.Decompress(state, compressedData.Length, compressedData, data.Length, data);

            // // Create the filename
            // string filename = fileDescriptor.FileName;

            // // If we have an invalid output directory
            // if (string.IsNullOrEmpty(outputDirectory))
            //     return false;

            // // Create the full output path
            // filename = Path.Combine(outputDirectory, filename);

            // // Ensure the output directory is created
            // Directory.CreateDirectory(Path.GetDirectoryName(filename));

            // // Try to write the data
            // try
            // {
            //     // Open the output file for writing
            //     using (Stream fs = File.OpenWrite(filename))
            //     {
            //         fs.Write(data, 0, data.Length);
            //     }
            // }
            // catch
            // {
            //     return false;
            // }

            // return true;
        }

        #endregion
    }
}
