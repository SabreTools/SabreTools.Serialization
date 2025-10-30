using System;
using System.IO;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Compressors.ZStandard;
#endif

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public partial class ZSTD : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Ensure there is data to extract
            if (Magic == null)
            {
                if (includeDebug) Console.Error.WriteLine("Invalid archive detected, skipping...");
                return false;
            }

#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                // Ensure directory separators are consistent
                string filename = string.IsNullOrEmpty(Filename) ? "filename" : Path.GetFileNameWithoutExtension(Filename);
                if (Path.DirectorySeparatorChar == '\\')
                    filename = filename.Replace('/', '\\');
                else if (Path.DirectorySeparatorChar == '/')
                    filename = filename.Replace('\\', '/');

                // Ensure the full output directory exists
                filename = Path.Combine(outputDirectory, filename);
                var directoryName = Path.GetDirectoryName(filename);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Open the source as a zStandard stream
                var zstdStream = new ZStandardStream(_dataSource, false);

                // Write the file
                using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                zstdStream.CopyTo(fs);
                fs.Flush();

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
#else
            Console.WriteLine("Extraction is not supported for this framework!");
            Console.WriteLine();
            return false;
#endif
        }
    }
}
