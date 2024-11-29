using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Models.XZP;
using static SabreTools.Models.XZP.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class XZP : BaseBinaryDeserializer<Models.XZP.File>
    {
        /// <inheritdoc/>
        public override Models.XZP.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Create a new XBox Package File to fill
            var file = new Models.XZP.File();

            #region Header

            // Try to parse the header
            var header = data.ReadType<Header>();
            if (header?.Signature != HeaderSignatureString)
                return null;
            if (header.Version != 6)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            #region Directory Entries

            // Create the directory entry array
            file.DirectoryEntries = new DirectoryEntry[header.DirectoryEntryCount];

            // Try to parse the directory entries
            for (int i = 0; i < file.DirectoryEntries.Length; i++)
            {
                var directoryEntry = data.ReadType<DirectoryEntry>();
                if (directoryEntry == null)
                    continue;

                file.DirectoryEntries[i] = directoryEntry;
            }

            #endregion

            #region Preload Directory Entries

            if (header.PreloadBytes > 0)
            {
                // Create the preload directory entry array
                file.PreloadDirectoryEntries = new DirectoryEntry[header.PreloadDirectoryEntryCount];

                // Try to parse the preload directory entries
                for (int i = 0; i < file.PreloadDirectoryEntries.Length; i++)
                {
                    var directoryEntry = data.ReadType<DirectoryEntry>();
                    if (directoryEntry == null)
                        continue;

                    file.PreloadDirectoryEntries[i] = directoryEntry;
                }
            }

            #endregion

            #region Preload Directory Mappings

            if (header.PreloadBytes > 0)
            {
                // Create the preload directory mapping array
                file.PreloadDirectoryMappings = new DirectoryMapping[header.PreloadDirectoryEntryCount];

                // Try to parse the preload directory mappings
                for (int i = 0; i < file.PreloadDirectoryMappings.Length; i++)
                {
                    var directoryMapping = data.ReadType<DirectoryMapping>();
                    if (directoryMapping == null)
                        continue;

                    file.PreloadDirectoryMappings[i] = directoryMapping;
                }
            }

            #endregion

            #region Directory Items

            if (header.DirectoryItemCount > 0)
            {
                // Get the directory item offset
                uint directoryItemOffset = header.DirectoryItemOffset;
                if (directoryItemOffset < 0 || directoryItemOffset >= data.Length)
                    return null;

                // Seek to the directory items
                data.Seek(directoryItemOffset, SeekOrigin.Begin);

                // Create the directory item array
                file.DirectoryItems = new DirectoryItem[header.DirectoryItemCount];

                // Try to parse the directory items
                for (int i = 0; i < file.DirectoryItems.Length; i++)
                {
                    var directoryItem = ParseDirectoryItem(data);
                    file.DirectoryItems[i] = directoryItem;
                }
            }

            #endregion

            #region Footer

            // Seek to the footer
            data.Seek(-8, SeekOrigin.End);

            // Try to parse the footer
            var footer = data.ReadType<Footer>();
            if (footer?.Signature != FooterSignatureString)
                return null;

            // Set the package footer
            file.Footer = footer;

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a XBox Package File directory item
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XBox Package File directory item on success, null on error</returns>
        private static DirectoryItem ParseDirectoryItem(Stream data)
        {
            var directoryItem = new DirectoryItem();

            directoryItem.FileNameCRC = data.ReadUInt32();
            directoryItem.NameOffset = data.ReadUInt32();
            directoryItem.TimeCreated = data.ReadUInt32();

            // Cache the current offset
            long currentPosition = data.Position;

            // Seek to the name offset
            data.Seek(directoryItem.NameOffset, SeekOrigin.Begin);

            // Read the name
            directoryItem.Name = data.ReadNullTerminatedAnsiString();

            // Seek back to the right position
            data.Seek(currentPosition, SeekOrigin.Begin);

            return directoryItem;
        }
    }
}