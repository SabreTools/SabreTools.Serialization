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
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new archive to fill
                var archive = new Archive();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header.Magic != SignatureString)
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
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a FileEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FileEntry on success, null on error</returns>
        public static FileEntry ParseFileEntry(Stream data)
        {
            var fileEntry = new FileEntry();

            fileEntry.NameSize = data.ReadInt32LittleEndian();
            if (fileEntry.NameSize > 0)
            {
                byte[] name = data.ReadBytes(fileEntry.NameSize);
                fileEntry.Name = Encoding.ASCII.GetString(name);
            }

            fileEntry.UncompressedSize = data.ReadInt32LittleEndian();
            fileEntry.Offset = data.ReadInt32LittleEndian();
            if (fileEntry.Offset > 0)
            {
                long currentOffset = data.Position;
                data.Seek(fileEntry.Offset, SeekOrigin.Begin);
                fileEntry.CompressedSize = data.ReadInt32LittleEndian();
                data.Seek(currentOffset, SeekOrigin.Begin);
            }

            return fileEntry;
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header ParseHeader(Stream data)
        {
            var obj = new Header();

            byte[] magic = data.ReadBytes(4);
            obj.Magic = Encoding.ASCII.GetString(magic);
            obj.Version = data.ReadInt32LittleEndian();
            obj.Files = data.ReadInt32LittleEndian();

            return obj;
        }
    }
}
