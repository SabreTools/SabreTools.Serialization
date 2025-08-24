using System.IO;
using SabreTools.Serialization.Interfaces;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Compressors.Xz;
#endif

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public class XZ : WrapperBase, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "xz Archive";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XZ(byte[]? data, int offset)
            : base(data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public XZ(Stream? data)
            : base(data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a XZ archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A XZ wrapper on success, null on failure</returns>
        public static XZ? Create(byte[]? data, int offset)
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
        /// Create a XZ archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A XZ wrapper on success, null on failure</returns>
        public static XZ? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            return new XZ(data);
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
        public bool Extract(string outDir, bool includeDebug)
        {
#if NET462_OR_GREATER || NETCOREAPP
            if (_dataSource == null || !_dataSource.CanRead)
                return false;

            try
            {
                // Try opening the stream
                using var xzFile = new XZStream(_dataSource);

                // Ensure directory separators are consistent
                string filename = System.Guid.NewGuid().ToString();
                if (Path.DirectorySeparatorChar == '\\')
                    filename = filename.Replace('/', '\\');
                else if (Path.DirectorySeparatorChar == '/')
                    filename = filename.Replace('\\', '/');

                // Ensure the full output directory exists
                filename = Path.Combine(outDir, filename);
                var directoryName = Path.GetDirectoryName(filename);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Extract the file
                using FileStream fs = File.OpenWrite(filename);
                xzFile.CopyTo(fs);
                fs.Flush();

                return true;
            }
            catch (System.Exception ex)
            {
                if (includeDebug) System.Console.Error.WriteLine(ex);
                return false;
            }
#else
            return false;
#endif
        }

        #endregion
    }
}
