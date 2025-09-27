using System.IO;
using System.Text;
using SabreTools.Data.Models.SpoonInstaller;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class SpoonInstaller : BaseBinaryReader<SFX>
    {
        /// <inheritdoc/>
        public override SFX? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new SFX to populate
                var sfx = new SFX();

                // Try to parse the footer from the data end
                var footer = ParseFooter(data);
                if (footer.TablePointer == 0 || initialOffset + footer.TablePointer > data.Length)
                    return null;

                // Set the footer
                sfx.Footer = footer;

                // Get to the entry table offset
                long tableOffset = initialOffset + footer.TablePointer;
                if (tableOffset < initialOffset || tableOffset >= data.Length)
                    return null;

                // Seek to the entry table
                data.Seek(tableOffset, SeekOrigin.Begin);

                // Try to parse the entry table
                var table = ParseTable(data, footer.EntryCount);
                if (table.Length != footer.EntryCount)
                    return null;

                // Set the entry table
                sfx.Entries = table;

                return sfx;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a Footer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Footer on success, null on error</returns>
        public static Footer ParseFooter(Stream data)
        {
            // Seek from the end (24 bytes has to use -23)
            data.Seek(-23, SeekOrigin.End);

            var obj = new Footer();

            obj.Unknown0 = data.ReadUInt32LittleEndian();
            obj.Unknown1 = data.ReadUInt32LittleEndian();
            obj.TablePointer = data.ReadUInt32LittleEndian();
            obj.EntryCount = data.ReadUInt32LittleEndian();
            obj.Unknown2 = data.ReadUInt32LittleEndian();
            obj.Unknown3 = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an entry table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of entries to parse</param>
        /// <returns>Filled entry table on success, null on error</returns>
        public static FileEntry[] ParseTable(Stream data, uint count)
        {
            if (count == 0)
                return [];

            var obj = new FileEntry[count];
            for (uint i = 0; i < count; i++)
            {
                obj[i] = ParseFileEntry(data);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FileEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FileEntry on success, null on error</returns>
        public static FileEntry ParseFileEntry(Stream data)
        {
            var obj = new FileEntry();

            obj.FileOffset = data.ReadUInt32LittleEndian();
            obj.CompressedSize = data.ReadUInt32LittleEndian();
            obj.UncompressedSize = data.ReadUInt32LittleEndian();
            obj.Crc32 = data.ReadUInt32LittleEndian();
            obj.FilenameLength = data.ReadByteValue();
            byte[] nameBytes = data.ReadBytes(obj.FilenameLength);
            obj.Filename = Encoding.ASCII.GetString(nameBytes);

            return obj;
        }
    }
}
