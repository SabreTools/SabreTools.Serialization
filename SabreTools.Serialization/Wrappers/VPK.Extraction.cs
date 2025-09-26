using System;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Interfaces;
using static SabreTools.Data.Models.VPK.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public partial class VPK : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no directory items
            if (DirectoryItems == null || DirectoryItems.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < DirectoryItems.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the VPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no directory items
            if (DirectoryItems == null || DirectoryItems.Length == 0)
                return false;

            // If the directory item index is invalid
            if (index < 0 || index >= DirectoryItems.Length)
                return false;

            // Get the directory item
            var directoryItem = DirectoryItems[index];
            if (directoryItem.DirectoryEntry == null)
                return false;

            // If we have an item with no archive
            byte[] data = [];
            if (directoryItem.DirectoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE)
            {
                if (directoryItem.PreloadData == null)
                    return false;

                data = directoryItem.PreloadData;
            }
            else
            {
                // If we have invalid archives
                if (ArchiveFilenames == null || ArchiveFilenames.Length == 0)
                    return false;

                // If we have an invalid index
                if (directoryItem.DirectoryEntry.ArchiveIndex < 0 || directoryItem.DirectoryEntry.ArchiveIndex >= ArchiveFilenames.Length)
                    return false;

                // Get the archive filename
                string archiveFileName = ArchiveFilenames[directoryItem.DirectoryEntry.ArchiveIndex];
                if (string.IsNullOrEmpty(archiveFileName))
                    return false;

                // If the archive doesn't exist
                if (!File.Exists(archiveFileName))
                    return false;

                // Try to open the archive
                var archiveStream = default(Stream);
                try
                {
                    // Open the archive
                    archiveStream = File.Open(archiveFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    // Seek to the data
                    archiveStream.Seek(directoryItem.DirectoryEntry.EntryOffset, SeekOrigin.Begin);

                    // Read the directory item bytes
                    data = archiveStream.ReadBytes((int)directoryItem.DirectoryEntry.EntryLength);
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                    return false;
                }
                finally
                {
                    archiveStream?.Close();
                }

                // If we have preload data, prepend it
                if (data != null && directoryItem.PreloadData != null)
                    data = [.. directoryItem.PreloadData, .. data];
            }

            // If there is nothing to write out
            if (data == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            string filename = $"{directoryItem.Name}.{directoryItem.Extension}";
            if (!string.IsNullOrEmpty(directoryItem.Path))
                filename = Path.Combine(directoryItem.Path, filename);
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
                // Open the output file for writing
                using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }

            return true;
        }
    }
}
