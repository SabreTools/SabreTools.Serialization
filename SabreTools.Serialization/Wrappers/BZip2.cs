using System;
using System.IO;
using SabreTools.IO.Compression.BZip2;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public class BZip2 : WrapperBase, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "bzip2 Archive";

        #endregion

        #region Instance Variables

        /// <summary>
        /// Source filename for the wrapper
        /// </summary>
        private readonly string? _filename;

        /// <summary>
        /// Source stream for the wrapper
        /// </summary>
        private readonly Stream _stream;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new instance of the wrapper from a file path
        /// </summary>
        public BZip2(string filename)
        {
            _filename = filename;
            _stream = File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        public BZip2(Stream stream)
        {
            _filename = null;
            _stream = stream;

            if (stream is FileStream fs)
                _filename = fs.Name;
        }

        /// <summary>
        /// Create a BZip2 archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BZip2 wrapper on success, null on failure</returns>
        public static BZip2? Create(byte[]? data, int offset)
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
        /// Create a BZip2 archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A BZip2 wrapper on success, null on failure</returns>
        public static BZip2? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            return new BZip2(data);
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
            if (_stream == null || !_stream.CanRead)
                return false;

            try
            {
                // Try opening the stream
                using var bz2File = new BZip2InputStream(_stream, true);

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
                bz2File.CopyTo(fs);
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
