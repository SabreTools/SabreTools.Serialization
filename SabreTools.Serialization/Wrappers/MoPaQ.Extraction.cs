using System;
using SabreTools.Serialization.Interfaces;
#if (NET452_OR_GREATER || NETCOREAPP) && (WINX86 || WINX64)
using StormLibSharp;
#endif

namespace SabreTools.Serialization.Wrappers
{
    public partial class MoPaQ : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
#if NET20 || NET35 || !(WINX86 || WINX64)
            Console.WriteLine("Extraction is not supported for this framework!");
            Console.WriteLine();
            return false;
#else
            try
            {
                if (Filename == null || !File.Exists(Filename))
                    return false;

                // Try to open the archive and listfile
                var mpqArchive = new MpqArchive(Filename, FileAccess.Read);
                string? listfile = null;
                MpqFileStream listStream = mpqArchive.OpenFile("(listfile)");

                // If we can't read the listfile, we just return
                if (!listStream.CanRead)
                    return false;

                // Read the listfile in for processing
                using (var sr = new StreamReader(listStream))
                {
                    listfile = sr.ReadToEnd();
                }

                // Split the listfile by newlines
                string[] listfileLines = listfile.Replace("\r\n", "\n").Split('\n');

                // Loop over each entry
                foreach (string sub in listfileLines)
                {
                    // Ensure directory separators are consistent
                    string filename = sub;
                    if (Path.DirectorySeparatorChar == '\\')
                        filename = filename.Replace('/', '\\');
                    else if (Path.DirectorySeparatorChar == '/')
                        filename = filename.Replace('\\', '/');

                    // Ensure the full output directory exists
                    filename = Path.Combine(outDir, filename);
                    var directoryName = Path.GetDirectoryName(filename);
                    if (directoryName != null && !Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    // Try to write the data
                    try
                    {
                        mpqArchive.ExtractFile(sub, filename);
                    }
                    catch (System.Exception ex)
                    {
                        if (includeDebug) System.Console.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                if (includeDebug) System.Console.WriteLine(ex);
                return false;
            }
#endif
        }
    }
}
