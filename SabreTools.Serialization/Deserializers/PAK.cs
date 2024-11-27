using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Models.PAK;
using static SabreTools.Models.PAK.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class PAK : BaseBinaryDeserializer<Models.PAK.File>
    {
        /// <inheritdoc/>
        public override Models.PAK.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Create a new Half-Life Package to fill
            var file = new Models.PAK.File();

            #region Header

            // Try to parse the header
            var header = data.ReadType<Header>();
            if (header?.Signature != SignatureString)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            #region Directory Items

            // Get the directory items offset
            uint directoryItemsOffset = header.DirectoryOffset;
            if (directoryItemsOffset < 0 || directoryItemsOffset >= data.Length)
                return null;

            // Seek to the directory items
            data.Seek(directoryItemsOffset, SeekOrigin.Begin);

            // Create the directory item array
            file.DirectoryItems = new DirectoryItem[header.DirectoryLength / 64];

            // Try to parse the directory items
            for (int i = 0; i < file.DirectoryItems.Length; i++)
            {
                var directoryItem = data.ReadType<DirectoryItem>();
                if (directoryItem == null)
                    return null;

                file.DirectoryItems[i] = directoryItem;
            }

            #endregion

            return file;
        }
    }
}