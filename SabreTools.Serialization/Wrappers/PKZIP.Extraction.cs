using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers;
#endif

namespace SabreTools.Serialization.Wrappers
{
    public partial class PKZIP : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
            => Extract(outputDirectory, lookForHeader: false, includeDebug);

        /// <inheritdoc cref="Extract(string, bool)"/>
        public bool Extract(string outputDirectory, bool lookForHeader, bool includeDebug)
        {
            if (_dataSource == null || !_dataSource.CanRead)
                return false;

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
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

                bool encrypted = false;
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
                        
                        // If the entry is password-protected, skip it
                        if (entry.IsEncrypted)
                        {
                            if (!encrypted)
                            {
                                if (includeDebug) Console.WriteLine("Some or all files in zip are password-protected!");
                                encrypted = true;
                            }
                            
                            continue;
                        }

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
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.Error.WriteLine(ex);
                    }
                }

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

        /// <summary>
        /// Try to find all parts of the archive, if possible
        /// </summary>
        /// <param name="firstPart">Path of the first archive part</param>
        /// <returns>List of all found parts, if possible</returns>
        public static List<string> FindParts(string firstPart)
        {
            // Define the regex patterns
            const string zipPattern = @"^(.*\.)(zipx?|zx?[0-9]+)$";
            const string genericPattern = @"^(.*\.)([0-9]+)$";

            // Ensure the full path is available
            firstPart = Path.GetFullPath(firstPart);
            string filename = Path.GetFileName(firstPart);
            string? directory = Path.GetDirectoryName(firstPart);

            // Make the output list
            List<string> parts = [];

            // Determine which pattern is being used
            Match match;
            Func<int, string> nextPartFunc;
            if (Regex.IsMatch(filename, zipPattern, RegexOptions.IgnoreCase))
            {
                match = Regex.Match(filename, zipPattern, RegexOptions.IgnoreCase);
                nextPartFunc = (i) =>
                {
                    return string.Concat(
                        match.Groups[1].Value,
                        Regex.Replace(match.Groups[2].Value, @"[^xz]", ""),
                        $"{i:D2}");
                };
            }
            else if (Regex.IsMatch(filename, genericPattern, RegexOptions.IgnoreCase))
            {
                match = Regex.Match(filename, genericPattern, RegexOptions.IgnoreCase);
                nextPartFunc = (i) =>
                {
                    return string.Concat(
                        match.Groups[1].Value,
                        $"{i + 1}".PadLeft(match.Groups[2].Value.Length, '0')
                    );
                };
            }
            else
            {
                return [firstPart];
            }

            // Loop and add the files
            parts.Add(firstPart);
            for (int i = 1; ; i++)
            {
                string nextPart = nextPartFunc(i);
                if (directory != null)
                    nextPart = Path.Combine(directory, nextPart);

                if (!File.Exists(nextPart))
                    break;

                parts.Add(nextPart);
            }

            return parts;
        }
    }
}
