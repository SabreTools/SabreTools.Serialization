using System.IO;
using SabreTools.Serialization.Interfaces;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
#endif

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public class TapeArchive : WrapperBase, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Tape Archive (or Derived Format)";

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
        public TapeArchive(string filename)
        {
            _filename = filename;
            _stream = File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        public TapeArchive(Stream stream)
        {
            _filename = null;
            _stream = stream;

            if (stream is FileStream fs)
                _filename = fs.Name;
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

            return new TapeArchive(data);
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
            if (_stream == null || !_stream.CanRead)
                return false;

#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                var tarFile = TarArchive.Open(_stream);

                // Try to read the file path if no entries are found
                if (tarFile.Entries.Count == 0 && !string.IsNullOrEmpty(_filename) && File.Exists(_filename))
                    tarFile = TarArchive.Open(_filename);

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
                    catch (System.Exception ex)
                    {
                        if (includeDebug) System.Console.Error.WriteLine(ex);
                    }
                }

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
