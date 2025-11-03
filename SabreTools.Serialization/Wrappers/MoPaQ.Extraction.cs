using System;
using System.IO;
using StormLibSharp;
using static SabreTools.Data.Models.MoPaQ.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public partial class MoPaQ : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            try
            {
                // Limit use to Windows only
                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                {
                    Console.WriteLine("Extraction is not supported for this operating system!");
                    return false;
                }

                if (Filename == null || !File.Exists(Filename))
                    return false;

                // Try to open the archive and listfile
                var mpqArchive = new MpqArchive(Filename, FileAccess.Read);
                string? listfile = null;
                MpqFileStream listStream = mpqArchive.OpenFile(LISTFILE_NAME);

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
                    string filename = sub;
                    if (Path.DirectorySeparatorChar == '\\')
                        filename = filename.Replace('/', '\\');
                    else if (Path.DirectorySeparatorChar == '/')
                        filename = filename.Replace('\\', '/');

                    // Ensure the full output directory exists
                    filename = Path.Combine(outputDirectory, filename);
                    var directoryName = Path.GetDirectoryName(filename);
                    if (directoryName != null && !Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    // Try to write the data
                    try
                    {
                        mpqArchive.ExtractFile(sub, filename);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }
    }
}
