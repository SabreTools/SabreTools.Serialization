using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.IO.Compression.zlib;
using SabreTools.Models.SGA;

namespace SabreTools.Serialization.Wrappers
{
    public class SGA : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "SGA";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Directory data
        /// </summary>
        public Models.SGA.Directory? Directory => Model.Directory;

        /// <summary>
        /// Number of files in the directory
        /// </summary>
        public int FileCount
        {
            get
            {
                return Directory switch
                {
                    Directory4 d4 => d4.Files?.Length ?? 0,
                    Directory5 d5 => d5.Files?.Length ?? 0,
                    Directory6 d6 => d6.Files?.Length ?? 0,
                    Directory7 d7 => d7.Files?.Length ?? 0,
                    _ => 0,
                };
            }
        }

        /// <summary>
        /// Offset to the file data
        /// </summary>
        public long FileDataOffset
        {
            get
            {
                return Model.Header switch
                {
                    Header4 h4 => h4.FileDataOffset,
                    Header6 h6 => h6.FileDataOffset,
                    _ => -1,
                };
            }
        }

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public SGA(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public SGA(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an SGA from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the SGA</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An SGA wrapper on success, null on failure</returns>
        public static SGA? Create(byte[]? data, int offset)
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
        /// Create a SGA from a Stream
        /// </summary>
        /// <param name="data">Stream representing the SGA</param>
        /// <returns>An SGA wrapper on success, null on failure</returns>
        public static SGA? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var file = Deserializers.SGA.DeserializeStream(data);
                if (file == null)
                    return null;

                return new SGA(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the SGA to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory, bool includeDebug)
        {
            // Get the file count
            int fileCount = FileCount;
            if (fileCount == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < fileCount; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the SGA to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // Get the file count
            int fileCount = FileCount;
            if (fileCount == 0)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= fileCount)
                return false;

            // Create the filename
            var filename = GetFileName(index);
            if (filename == null)
                return false;

            // Loop through and get all parent directories
            var parentNames = new List<string> { filename };

            // Get the parent directory
            string? folderName = GetParentName(index);
            if (folderName != null)
                parentNames.Add(folderName);

            // TODO: Should the section name/alias be used in the path as well?

            // Reverse and assemble the filename
            parentNames.Reverse();
#if NET20 || NET35
            filename = parentNames[0];
            for (int i = 1; i < parentNames.Count; i++)
            {
                filename = Path.Combine(filename, parentNames[i]);
            }
#else
            filename = Path.Combine([.. parentNames]);
#endif

            // Get and adjust the file offset
            long fileOffset = GetFileOffset(index);
            fileOffset += FileDataOffset;
            if (fileOffset < 0)
                return false;

            // Get the file sizes
            long fileSize = GetCompressedSize(index);
            long outputFileSize = GetUncompressedSize(index);

            // Read the compressed data directly
            var compressedData = ReadFromDataSource((int)fileOffset, (int)fileSize);
            if (compressedData == null)
                return false;

            // If the compressed and uncompressed sizes match
            byte[] data;
            if (fileSize == outputFileSize)
            {
                data = compressedData;
            }
            else
            {
                // Inflate the data into the buffer
                var zstream = new ZLib.z_stream_s();
                data = new byte[outputFileSize];
                unsafe
                {
                    fixed (byte* payloadPtr = compressedData)
                    fixed (byte* dataPtr = data)
                    {
                        zstream.next_in = payloadPtr;
                        zstream.avail_in = (uint)compressedData.Length;
                        zstream.total_in = (uint)compressedData.Length;
                        zstream.next_out = dataPtr;
                        zstream.avail_out = (uint)data.Length;
                        zstream.total_out = 0;

                        ZLib.inflateInit_(zstream, ZLib.zlibVersion(), compressedData.Length);
                        int zret = ZLib.inflate(zstream, 1);
                        ZLib.inflateEnd(zstream);
                    }
                }
            }

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !System.IO.Directory.Exists(directoryName))
                System.IO.Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using Stream fs = System.IO.File.OpenWrite(filename);
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }

            return false;
        }

        #endregion

        #region File

        /// <summary>
        /// Get the compressed size of a file
        /// </summary>
        public long GetCompressedSize(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return -1;

            // Get the file and return the name
            var file = GetFile(index);
            return file?.Size ?? -1L;
        }

        /// <summary>
        /// Get the uncompressed size of a file
        /// </summary>
        public long GetUncompressedSize(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return -1;

            // Get the file and return the name
            var file = GetFile(index);
            return file?.SizeOnDisk ?? -1L;
        }

        /// <summary>
        /// Get a file header from the archive
        /// </summary>
        public Models.SGA.File? GetFile(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return null;

            return Directory switch
            {
                Directory4 d4 => d4.Files![index],
                Directory5 d5 => d5.Files![index],
                Directory6 d6 => d6.Files![index],
                Directory7 d7 => d7.Files![index],
                _ => null,
            };
        }

        /// <summary>
        /// Get a file name from the archive
        /// </summary>
        public string? GetFileName(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return null;

            // Get the file and return the name
            var file = GetFile(index);
            return file?.Name;
        }

        /// <summary>
        /// Get a file offset from the archive
        /// </summary>
        public long GetFileOffset(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return -1;

            // Get the file and return the name
            var file = GetFile(index);
            return file?.Offset ?? -1L;
        }

        /// <summary>
        /// Get the parent name for a file
        /// </summary>
        public string? GetParentName(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return null;

            // Get the folder
            Folder? folder = Directory switch
            {
                Directory4 d4 => Array.Find(d4.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                Directory5 d5 => Array.Find(d5.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                Directory6 d6 => Array.Find(d6.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                Directory7 d7 => Array.Find(d7.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                _ => default,
            };

            // Get the folder name
            return folder switch
            {
                Folder4 f4 => f4.Name,
                Folder5 f5 => f5.Name,
                _ => null,
            };
        }

        #endregion
    }
}
