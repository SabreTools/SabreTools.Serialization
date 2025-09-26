using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Models.PAK;
using static SabreTools.Serialization.Models.PAK.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class PAK : BaseBinaryDeserializer<SabreTools.Serialization.Models.PAK.File>
    {
        /// <inheritdoc/>
        public override SabreTools.Serialization.Models.PAK.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new Half-Life Package to fill
                var file = new SabreTools.Serialization.Models.PAK.File();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header.Signature != SignatureString)
                    return null;

                // Set the package header
                file.Header = header;

                #endregion

                #region Directory Items

                // Get the directory items offset
                long directoryItemsOffset = initialOffset + header.DirectoryOffset;
                if (directoryItemsOffset < initialOffset || directoryItemsOffset >= data.Length)
                    return null;

                // Seek to the directory items
                data.Seek(directoryItemsOffset, SeekOrigin.Begin);

                // Create the directory item array
                file.DirectoryItems = new DirectoryItem[header.DirectoryLength / 64];

                // Try to parse the directory items
                for (int i = 0; i < file.DirectoryItems.Length; i++)
                {
                    file.DirectoryItems[i] = ParseDirectoryItem(data);
                }

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
        /// Parse a Stream into a DirectoryItem
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryItem on success, null on error</returns>
        public static DirectoryItem ParseDirectoryItem(Stream data)
        {
            var obj = new DirectoryItem();

            byte[] itemName = data.ReadBytes(56);
            obj.ItemName = Encoding.ASCII.GetString(itemName).TrimEnd('\0');
            obj.ItemOffset = data.ReadUInt32LittleEndian();
            obj.ItemLength = data.ReadUInt32LittleEndian();

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
            obj.DirectoryOffset = data.ReadUInt32LittleEndian();
            obj.DirectoryLength = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
