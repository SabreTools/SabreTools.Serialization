using System;
using System.IO;
using SabreTools.IO.Compression.BZip2;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public partial class BZip2 : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            if (_dataSource == null || !_dataSource.CanRead)
                return false;

            try
            {
                // Try opening the stream
                using var bz2File = new BZip2InputStream(_dataSource, true);

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
                using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
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
    }
}
