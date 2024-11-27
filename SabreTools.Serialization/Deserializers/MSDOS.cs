using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.MSDOS;
using static SabreTools.Models.MSDOS.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class MSDOS : BaseBinaryDeserializer<Executable>
    {
        /// <inheritdoc/>
        public override Executable? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new executable to fill
            var executable = new Executable();

            #region Executable Header

            // Try to parse the executable header
            var executableHeader = ParseExecutableHeader(data);
            if (executableHeader == null)
                return null;

            // Set the executable header
            executable.Header = executableHeader;

            #endregion

            #region Relocation Table

            // If the offset for the relocation table doesn't exist
            int tableAddress = initialOffset + executableHeader.RelocationTableAddr;
            if (tableAddress >= data.Length)
                return executable;

            // Try to parse the relocation table
            data.Seek(tableAddress, SeekOrigin.Begin);
            var relocationTable = ParseRelocationTable(data, executableHeader.RelocationItems);
            if (relocationTable == null)
                return null;

            // Set the relocation table
            executable.RelocationTable = relocationTable;

            #endregion

            // Return the executable
            return executable;
        }

        /// <summary>
        /// Parse a Stream into an MS-DOS executable header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled executable header on success, null on error</returns>
        private static ExecutableHeader? ParseExecutableHeader(Stream data)
        {
            var header = new ExecutableHeader();

            #region Standard Fields

            byte[] magic = data.ReadBytes(2);
            header.Magic = Encoding.ASCII.GetString(magic);
            if (header.Magic != SignatureString)
                return null;

            header.LastPageBytes = data.ReadUInt16();
            header.Pages = data.ReadUInt16();
            header.RelocationItems = data.ReadUInt16();
            header.HeaderParagraphSize = data.ReadUInt16();
            header.MinimumExtraParagraphs = data.ReadUInt16();
            header.MaximumExtraParagraphs = data.ReadUInt16();
            header.InitialSSValue = data.ReadUInt16();
            header.InitialSPValue = data.ReadUInt16();
            header.Checksum = data.ReadUInt16();
            header.InitialIPValue = data.ReadUInt16();
            header.InitialCSValue = data.ReadUInt16();
            header.RelocationTableAddr = data.ReadUInt16();
            header.OverlayNumber = data.ReadUInt16();

            #endregion

            // If we don't have enough data for PE extensions
            if (data.Position >= data.Length || data.Length - data.Position < 36)
                return header;

            #region PE Extensions

            header.Reserved1 = new ushort[4];
            for (int i = 0; i < header.Reserved1.Length; i++)
            {
                header.Reserved1[i] = data.ReadUInt16();
            }
            header.OEMIdentifier = data.ReadUInt16();
            header.OEMInformation = data.ReadUInt16();
            header.Reserved2 = new ushort[10];
            for (int i = 0; i < header.Reserved2.Length; i++)
            {
                header.Reserved2[i] = data.ReadUInt16();
            }
            header.NewExeHeaderAddr = data.ReadUInt32();

            #endregion

            return header;
        }

        /// <summary>
        /// Parse a Stream into a relocation table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of relocation table entries to read</param>
        /// <returns>Filled relocation table on success, null on error</returns>
        private static RelocationEntry[]? ParseRelocationTable(Stream data, int count)
        {
            var relocationTable = new RelocationEntry[count];

            for (int i = 0; i < count; i++)
            {
                var entry = data.ReadType<RelocationEntry>();
                if (entry == null)
                    return null;

                relocationTable[i] = entry;
            }

            return relocationTable;
        }
    }
}