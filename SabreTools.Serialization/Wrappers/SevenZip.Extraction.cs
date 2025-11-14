using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Readers;
#endif

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public partial class SevenZip : IExtractable
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
                var sevenZip = SevenZipArchive.Open(_dataSource, readerOptions);

                // If the file exists
                if (!string.IsNullOrEmpty(Filename) && File.Exists(Filename!))
                {
                    // Find all file parts
                    FileInfo[] parts = [.. ArchiveFactory.GetFileParts(new FileInfo(Filename))];

                    // If there are multiple parts
                    if (parts.Length > 1)
                        sevenZip = SevenZipArchive.Open(parts, readerOptions);

                    // Try to read the file path if no entries are found
                    else if (sevenZip.Entries.Count == 0)
                        sevenZip = SevenZipArchive.Open(parts, readerOptions);
                }

                // Explained in https://github.com/adamhathcock/sharpcompress/pull/661. in order to determine whether
                // a RAR or 7Z archive is solid or not, you must check the second file in the archive, as the first
                // file is always marked non-solid even for solid archives. This iteration is necessary since things
                // like directories aren't marked solid either.
                // This is only temporary, as solid detection has been fixed in upstream SolidCompress, but they likely
                // won't make a new release for a while, and this is too big an issue to leave unfixed.
                bool firstFile = true;
                bool isSolid = false;
                foreach (var entry in sevenZip.Entries)
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

                        if (firstFile)
                            firstFile = false;
                        else
                        {
                            isSolid = entry.IsSolid;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.WriteLine(ex);
                    }
                }

                // Still check SharpCompress's archive-level isSolid flag. Based on the way the check works on their
                // end, it's possible that a multi-block solid archive might have the second (or more files) file not
                // solid? It doesn't hurt to check either way.
                if (isSolid || sevenZip.IsSolid)
                    return ExtractSolid(sevenZip, outputDirectory, includeDebug);
                else
                    return ExtractNonSolid(sevenZip, outputDirectory, includeDebug);

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
            if (Regex.IsMatch(filename, genericPattern, RegexOptions.IgnoreCase))
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

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
        /// <summary>
        /// Extraction method for non-solid archives. This iterates over each entry in the archive to extract every
        /// file individually, in order to extract all valid files from the archive.
        /// </summary>
        private static bool ExtractNonSolid(SevenZipArchive sevenZip, string outputDirectory, bool includeDebug)
        {
            foreach (var entry in sevenZip.Entries)
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
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }
            return true;
        }

        /// <summary>
        /// Extraction method for solid archives. Uses ExtractAllEntries because extraction for solid archives must be
        /// done sequentially, and files beyond a corrupted point in a solid archive will be unreadable anyways.
        /// </summary>
        private static bool ExtractSolid(SevenZipArchive sevenZip, string outputDirectory, bool includeDebug)
        {
            try
            {
                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);

                int index = 0;
                var entries = sevenZip.ExtractAllEntries();
                while (entries.MoveToNextEntry())
                {
                    var entry = entries.Entry;
                    if (entry.IsDirectory)
                        continue;

                    // Ensure directory separators are consistent
                    string filename = entry.Key ?? $"extracted_file_{index}";
                    if (Path.DirectorySeparatorChar == '\\')
                        filename = filename.Replace('/', '\\');
                    else if (Path.DirectorySeparatorChar == '/')
                        filename = filename.Replace('\\', '/');

                    // Ensure the full output directory exists
                    filename = Path.Combine(outputDirectory, filename);
                    var directoryName = Path.GetDirectoryName(filename);
                    if (directoryName != null && !Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    // Write to file
                    using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                    entries.WriteEntryTo(fs);
                    fs.Flush();

                    // Increment the index
                    index++;
                }

            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
            }

            return true;
        }
#endif
    }
}
