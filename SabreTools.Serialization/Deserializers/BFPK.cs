using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.BFPK;
using static SabreTools.Models.BFPK.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class BFPK : BaseBinaryDeserializer<Archive>
    {
        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Create a new archive to fill
            var archive = new Archive();

            #region Header

            // Try to parse the header
            var header = data.ReadType<Header>();
            if (header?.Magic != SignatureString)
                return null;

            // Set the archive header
            archive.Header = header;

            #endregion

            #region Files

            // If we have any files
            var files = new FileEntry[header.Files];

            // Read all entries in turn
            for (int i = 0; i < header.Files; i++)
            {
                files[i] = ParseFileEntry(data);
            }

            // Set the files
            archive.Files = files;

            #endregion

            return archive;
        }

        /// <summary>
        /// Parse a Stream into a file entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled file entry on success, null on error</returns>
        private static FileEntry ParseFileEntry(Stream data)
        {
            var fileEntry = new FileEntry();

            fileEntry.NameSize = data.ReadInt32();
            if (fileEntry.NameSize > 0)
            {
                byte[] name = data.ReadBytes(fileEntry.NameSize);
                fileEntry.Name = Encoding.ASCII.GetString(name);
            }

            fileEntry.UncompressedSize = data.ReadInt32();
            fileEntry.Offset = data.ReadInt32();
            if (fileEntry.Offset > 0)
            {
                long currentOffset = data.Position;
                data.Seek(fileEntry.Offset, SeekOrigin.Begin);
                fileEntry.CompressedSize = data.ReadInt32();
                data.Seek(currentOffset, SeekOrigin.Begin);
            }

            return fileEntry;
        }
    }
}