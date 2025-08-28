using System;
using System.IO;
using SabreTools.IO.Compression.Deflate;
using SabreTools.Models.GZIP;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public class GZip : WrapperBase<Archive>, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "gzip Archive";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Offset to the compressed data
        /// </summary>
        /// <remarks>Returns -1 on error</remarks>
        public long DataOffset
        {
            get
            {
                if (_dataOffset != null)
                    return _dataOffset.Value;

                if (Header == null)
                    return -1;

                // Minimum offset is 10 bytes:
                // - ID1 (1)
                // - ID2 (1)
                // - CompressionMethod (1)
                // - Flags (1)
                // - LastModifiedTime (4)
                // - ExtraFlags (1)
                // - OperatingSystem (1)
                _dataOffset = 10;

                // Add extra lengths
                _dataOffset += Header.XLEN;
                if (Header.OriginalFileName != null)
                    _dataOffset += Header.OriginalFileName.Length + 1;
                if (Header.FileComment != null)
                    _dataOffset += Header.FileComment.Length + 1;
                if (Header.CRC16 != null)
                    _dataOffset += 2;

                return _dataOffset.Value;
            }
        }

        /// <inheritdoc cref="Archive.Header"/>
        public Header? Header => Model.Header;

        /// <inheritdoc cref="Archive.Trailer"/>
        public Trailer? Trailer => Model.Trailer;

        #endregion

        #region Instance Variables

        /// <summary>
        /// Offset to the compressed data
        /// </summary>
        private long? _dataOffset = null;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public GZip(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public GZip(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a GZip archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A GZip wrapper on success, null on failure</returns>
        public static GZip? Create(byte[]? data, int offset)
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
        /// Create a GZip archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A GZip wrapper on success, null on failure</returns>
        public static GZip? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var model = Deserializers.GZip.DeserializeStream(data);
                if (model == null)
                    return null;

                return new GZip(model, data);
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
        public override string ExportJSON() => throw new NotImplementedException();
#endif

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            if (_dataSource == null || !_dataSource.CanRead)
                return false;

            try
            {
                // Try opening the stream
                using var gzipFile = new GZipStream(_dataSource, CompressionMode.Decompress, true);

                // Ensure directory separators are consistent
                string filename = Guid.NewGuid().ToString();
                if (Path.DirectorySeparatorChar == '\\')
                    filename = filename.Replace('/', '\\');
                else if (Path.DirectorySeparatorChar == '/')
                    filename = filename.Replace('\\', '/');

                // Ensure the full output directory exists
                filename = Path.Combine(outputDirectory, filename);
                var directoryName = Path.GetDirectoryName(filename);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Extract the file
                using FileStream fs = File.OpenWrite(filename);
                gzipFile.CopyTo(fs);
                fs.Flush();

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
