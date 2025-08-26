using System.IO;
using SabreTools.Serialization.Interfaces;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Readers;
#endif

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public class RAR : WrapperBase, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "RAR Archive (or Derived Format)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public RAR(byte[]? data, int offset)
            : base(data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public RAR(Stream? data)
            : base(data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a RAR archive (or derived format) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A RAR wrapper on success, null on failure</returns>
        public static RAR? Create(byte[]? data, int offset)
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
        /// Create a RAR archive (or derived format) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A RAR wrapper on success, null on failure</returns>
        public static RAR? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            return new RAR(data);
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
                RarArchive rarFile = RarArchive.Open(_dataSource, readerOptions);

                // If the file exists
                if (!string.IsNullOrEmpty(Filename) && File.Exists(Filename!))
                {
                    // Find all file parts
                    FileInfo[] parts = [.. ArchiveFactory.GetFileParts(new FileInfo(Filename))];

                    // Try to read the file path if no entries are found
                    if (rarFile.Entries.Count == 0)
                        rarFile = RarArchive.Open(parts, readerOptions);
                    
                    // If there's any multipart items, try reading the file as well
                    else if (parts.Length > 1)
                        rarFile = RarArchive.Open(parts, readerOptions);
                }

                if (rarFile.IsSolid)
                    return ExtractSolid(rarFile, outputDirectory, includeDebug);
                else
                    return ExtractNonSolid(rarFile, outputDirectory, includeDebug);

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

#if NET462_OR_GREATER || NETCOREAPP

        /// <summary>
        /// Extraction method for non-solid archives. This iterates over each entry in the archive to extract every 
        /// file individually, in order to extract all valid files from the archive.
        /// </summary>
        private static bool ExtractNonSolid(RarArchive rarFile, string outDir, bool includeDebug)
        {
            foreach (var entry in rarFile.Entries)
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
                    filename = Path.Combine(outDir, filename);
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

        /// <summary>
        /// Extraction method for solid archives. Uses ExtractAllEntries because extraction for solid archives must be
        /// done sequentially, and files beyond a corrupted point in a solid archive will be unreadable anyways.
        /// </summary>
        private static bool ExtractSolid(RarArchive rarFile, string outDir, bool includeDebug)
        {
            try
            {
                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);

                rarFile.WriteToDirectory(outDir, new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = true,
                });

            }
            catch (System.Exception ex)
            {
                if (includeDebug) System.Console.Error.WriteLine(ex);
            }

            return true;
        }
#endif

        #endregion
    }
}
