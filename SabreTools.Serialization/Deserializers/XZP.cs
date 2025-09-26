using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Models.XZP;
using static SabreTools.Serialization.Models.XZP.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class XZP : BaseBinaryDeserializer<Models.XZP.File>
    {
        /// <inheritdoc/>
        public override Models.XZP.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new XBox Package File to fill
                var file = new Models.XZP.File();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header.Signature != HeaderSignatureString)
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
                    file.DirectoryEntries[i] = ParseDirectoryEntry(data);
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
                        file.PreloadDirectoryEntries[i] = ParseDirectoryEntry(data);
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
                        file.PreloadDirectoryMappings[i] = ParseDirectoryMapping(data);
                    }
                }

                #endregion

                #region Directory Items

                if (header.DirectoryItemCount > 0)
                {
                    // Get the directory item offset
                    long directoryItemOffset = initialOffset + header.DirectoryItemOffset;
                    if (directoryItemOffset < initialOffset || directoryItemOffset >= data.Length)
                        return null;

                    // Seek to the directory items
                    data.Seek(directoryItemOffset, SeekOrigin.Begin);

                    // Create the directory item array
                    file.DirectoryItems = new DirectoryItem[header.DirectoryItemCount];

                    // Try to parse the directory items
                    for (int i = 0; i < file.DirectoryItems.Length; i++)
                    {
                        file.DirectoryItems[i] = ParseDirectoryItem(data, initialOffset);
                    }
                }

                #endregion

                #region Footer

                // Seek to the footer
                data.Seek(-8, SeekOrigin.End);

                // Try to parse the footer
                var footer = ParseFooter(data);
                if (footer.Signature != FooterSignatureString)
                    return null;

                // Set the package footer
                file.Footer = footer;

                #endregion

                return file;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a DirectoryEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryEntry on success, null on error</returns>
        public static DirectoryEntry ParseDirectoryEntry(Stream data)
        {
            var obj = new DirectoryEntry();

            obj.FileNameCRC = data.ReadUInt32LittleEndian();
            obj.EntryLength = data.ReadUInt32LittleEndian();
            obj.EntryOffset = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryItem
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <returns>Filled DirectoryItem on success, null on error</returns>
        public static DirectoryItem ParseDirectoryItem(Stream data, long initialOffset)
        {
            var obj = new DirectoryItem();

            obj.FileNameCRC = data.ReadUInt32LittleEndian();
            obj.NameOffset = data.ReadUInt32LittleEndian();
            obj.TimeCreated = data.ReadUInt32LittleEndian();

            // Cache the current offset
            long currentPosition = data.Position;

            // Seek to the name offset
            data.Seek(initialOffset + obj.NameOffset, SeekOrigin.Begin);

            // Read the name
            obj.Name = data.ReadNullTerminatedAnsiString();

            // Seek back to the right position
            data.Seek(currentPosition, SeekOrigin.Begin);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryMapping
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryMapping on success, null on error</returns>
        public static DirectoryMapping ParseDirectoryMapping(Stream data)
        {
            var obj = new DirectoryMapping();

            obj.PreloadDirectoryEntryIndex = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Footer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Footer on success, null on error</returns>
        public static Footer ParseFooter(Stream data)
        {
            var obj = new Footer();

            obj.FileLength = data.ReadUInt32LittleEndian();
            byte[] signature = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(signature);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header ParseHeader(Stream data)
        {
            var obj = new Header();

            byte[] signature = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.Version = data.ReadUInt32LittleEndian();
            obj.PreloadDirectoryEntryCount = data.ReadUInt32LittleEndian();
            obj.DirectoryEntryCount = data.ReadUInt32LittleEndian();
            obj.PreloadBytes = data.ReadUInt32LittleEndian();
            obj.HeaderLength = data.ReadUInt32LittleEndian();
            obj.DirectoryItemCount = data.ReadUInt32LittleEndian();
            obj.DirectoryItemOffset = data.ReadUInt32LittleEndian();
            obj.DirectoryItemLength = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
