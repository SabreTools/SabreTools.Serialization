using System.IO;
using System.Text;
using SabreTools.Data.Models.BFPK;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.BFPK.Constants;

namespace SabreTools.Serialization.Readers
{
    public class BFPK : BaseBinaryReader<Archive>
    {
        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

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
                    files[i] = ParseFileEntry(data, initialOffset);
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
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <returns>Filled FileEntry on success, null on error</returns>
        public static FileEntry ParseFileEntry(Stream data, long initialOffset)
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
                data.SeekIfPossible(initialOffset + fileEntry.Offset, SeekOrigin.Begin);
                fileEntry.CompressedSize = data.ReadInt32LittleEndian();
                data.SeekIfPossible(currentOffset, SeekOrigin.Begin);
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
