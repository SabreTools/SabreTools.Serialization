using System;
using System.IO;
using SabreTools.IO.Compression.Deflate;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public class GZip : WrapperBase, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "gzip Archive";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public GZip(byte[]? data, int offset)
            : base(data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public GZip(Stream? data)
            : base(data)
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

            return new GZip(data);
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
            if (DataSource == null || !DataSource.CanRead)
                return false;

            try
            {
                // Try opening the stream
                using var gzipFile = new GZipStream(DataSource, CompressionMode.Decompress, true);

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
