using System.IO;
using System.Text;
using SabreTools.Data.Models.AdvancedInstaller;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.AdvancedInstaller.Constants;

namespace SabreTools.Serialization.Readers
{
    public class AdvancedInstaller : BaseBinaryReader<SFX>
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
                var footer = ParseFooter(data, initialOffset);
                if (footer == null)
                    return null;
                if (footer.Signature != SignatureString)
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
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <returns>Filled Footer on success, null on error</returns>
        public static Footer? ParseFooter(Stream data, long initialOffset)
        {
            // Seek to the end of the end of the data
            data.Seek(0, SeekOrigin.End);

            // Cache the current offset
            long endOffset = data.Position;

            // Set a maximum of 10 iterations to find the signature
            int iterations = 10;

            // Seek backward to the first point it could be
            data.Seek(-10, SeekOrigin.Current);

            // Search backward for the signature
            bool signatureFound = false;
            do
            {
                byte[] tempBytes = data.ReadBytes(10);
                string tempStr = Encoding.ASCII.GetString(tempBytes);
                if (tempStr == SignatureString)
                {
                    signatureFound = true;
                    break;
                }

                iterations--;
                data.Seek(-11, SeekOrigin.Current);

            } while (iterations > 0);

            // If no signature was found
            if (!signatureFound)
                return null;

            // Seek to the first footer offset field
            data.Seek(-70, SeekOrigin.Current);

            // Find the actual offset of the start of the footer
            uint footerStart = data.ReadUInt32LittleEndian();
            if (footerStart > endOffset)
                return null;

            // If the offset is immediately prior, no name exists
            bool shortFooter = footerStart == data.Position - 8;

            // Seek to the start of the footer
            data.Seek(initialOffset + footerStart, SeekOrigin.Begin);

            var obj = new Footer();

            obj.Unknown0 = data.ReadUInt32LittleEndian();

            // TODO: Make this block safer
            if (!shortFooter)
            {
                obj.OriginalFilenameSize = data.ReadUInt32LittleEndian();
                byte[] filenameBytes = data.ReadBytes((int)obj.OriginalFilenameSize * 2);
                obj.OriginalFilename = Encoding.Unicode.GetString(filenameBytes);
                obj.Unknown1 = data.ReadUInt32LittleEndian();
            }

            obj.FooterOffset = data.ReadUInt32LittleEndian();
            obj.EntryCount = data.ReadUInt32LittleEndian();
            obj.Unknown2 = data.ReadUInt32LittleEndian();
            obj.UnknownOffset = data.ReadUInt32LittleEndian();
            obj.TablePointer = data.ReadUInt32LittleEndian();
            obj.FileDataStart = data.ReadUInt32LittleEndian();
            byte[] hexStringBytes = data.ReadBytes(32);
            obj.HexString = Encoding.ASCII.GetString(hexStringBytes);
            obj.Unknown3 = data.ReadUInt32LittleEndian();
            byte[] signatureBytes = data.ReadBytes(10);
            obj.Signature = Encoding.ASCII.GetString(signatureBytes);
            // TODO: Handle padding?

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

            obj.Unknown0 = data.ReadUInt32LittleEndian();
            obj.Unknown1 = data.ReadUInt32LittleEndian();
            obj.Unknown2 = data.ReadUInt32LittleEndian();
            obj.FileSize = data.ReadUInt32LittleEndian();
            obj.FileOffset = data.ReadUInt32LittleEndian();
            obj.NameSize = data.ReadUInt32LittleEndian();
            byte[] nameBytes = data.ReadBytes((int)obj.NameSize * 2);
            obj.Name = Encoding.Unicode.GetString(nameBytes);

            return obj;
        }
    }
}
