using System;
using System.IO;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class InstallShieldExecutable : IExtractable
    {
        #region Extraction

        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            const int chunkSize = 2048 * 1024;
            try
            {
                for (int i = 0; i < Entries.Length; i++)
                {
                    var entry = Entries[i];
                    _dataSource.SeekIfPossible(entry.Offset, SeekOrigin.Begin);

                    // Get the length, and make sure it won't EOF
                    long length = (long)entry.Length;
                    if (length > _dataSource.Length - _dataSource.Position)
                        break;

                    // Ensure directory separators are consistent
                    var filename = entry.Path.TrimEnd('\0');
                    if (Path.DirectorySeparatorChar == '\\')
                        filename = filename.Replace('/', '\\');
                    else if (Path.DirectorySeparatorChar == '/')
                        filename = filename.Replace('\\', '/');

                    // Ensure the full output directory exists
                    filename = Path.Combine(outputDirectory, filename);
                    var directoryName = Path.GetDirectoryName(filename);
                    if (directoryName is not null && !Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    // Write the output file
                    using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                    while (length > 0)
                    {
                        int bytesToRead = (int)Math.Min(length, chunkSize);

                        byte[] buffer = _dataSource.ReadBytes(bytesToRead);
                        fs.Write(buffer, 0, bytesToRead);
                        fs.Flush();

                        length -= bytesToRead;
                    }
                }

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
