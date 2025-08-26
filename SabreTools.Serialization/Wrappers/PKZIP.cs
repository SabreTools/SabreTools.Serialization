using System.IO;
using SabreTools.Models.PKZIP;
using SabreTools.Serialization.Interfaces;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers;
#endif

namespace SabreTools.Serialization.Wrappers
{
    public class PKZIP : WrapperBase<Archive>, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PKZIP Archive (or Derived Format)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PKZIP(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public PKZIP(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a PKZIP archive (or derived format) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PKZIP wrapper on success, null on failure</returns>
        public static PKZIP? Create(byte[]? data, int offset)
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
        /// Create a PKZIP archive (or derived format) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A PKZIP wrapper on success, null on failure</returns>
        public static PKZIP? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var model = Deserializers.PKZIP.DeserializeStream(data);
                if (model == null)
                    return null;

                return new PKZIP(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
            => Extract(outputDirectory, lookForHeader: false, includeDebug);

        /// <inheritdoc cref="Extract(string, bool)"/>
        public bool Extract(string outputDirectory, bool lookForHeader, bool includeDebug)
        {
            if (_dataSource == null || !_dataSource.CanRead)
                return false;

#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                var readerOptions = new ReaderOptions() { LookForHeader = lookForHeader };
                var zipFile = ZipArchive.Open(_dataSource, readerOptions);

                // If the file exists
                if (!string.IsNullOrEmpty(Filename) && File.Exists(Filename!))
                {
                    // Find all file parts
                    FileInfo[] parts = [.. ArchiveFactory.GetFileParts(new FileInfo(Filename))];
                    
                    // If there are multiple parts
                    if (parts.Length > 1)
                        zipFile = ZipArchive.Open(parts, readerOptions);

                    // Try to read the file path if no entries are found
                    else if (zipFile.Entries.Count == 0)
                        zipFile = ZipArchive.Open(parts, readerOptions);
                }

                foreach (var entry in zipFile.Entries)
                {
                    try
                    {
                        // If the entry is a directory
                        if (entry.IsDirectory)
                            continue;

                        // If the entry has an invalid key
                        if (entry.Key == null)
                            continue;

                        // If the entry is partial due to an incomplete multi-part archive, skip it
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
