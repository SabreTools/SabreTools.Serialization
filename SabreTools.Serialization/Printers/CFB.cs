using System;
using System.Text;
using SabreTools.Models.CFB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class CFB : IPrinter<Binary>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Binary model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Binary binary)
        {
            builder.AppendLine("Compound File Binary Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, binary.Header);
            Print(builder, binary.FATSectorNumbers, "FAT");
            Print(builder, binary.MiniFATSectorNumbers, "Mini FAT");
            Print(builder, binary.DIFATSectorNumbers, "DIFAT");
            Print(builder, binary.DirectoryEntries);
        }

        private static void Print(StringBuilder builder, FileHeader? header)
        {
            builder.AppendLine("  File Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No file header");
                return;
            }

            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine(header.CLSID, "  CLSID");
            builder.AppendLine(header.MinorVersion, "  Minor version");
            builder.AppendLine(header.MajorVersion, "  Major version");
            builder.AppendLine(header.ByteOrder, "  Byte order");
            builder.AppendLine($"  Sector shift: {header.SectorShift} (0x{header.SectorShift:X}) => {Math.Pow(2, header.SectorShift)}");
            builder.AppendLine($"  Mini sector shift: {header.MiniSectorShift} (0x{header.MiniSectorShift:X}) => {Math.Pow(2, header.MiniSectorShift)}");
            builder.AppendLine(header.Reserved, "  Reserved");
            builder.AppendLine(header.NumberOfDirectorySectors, "  Number of directory sectors");
            builder.AppendLine(header.NumberOfFATSectors, "  Number of FAT sectors");
            builder.AppendLine(header.FirstDirectorySectorLocation, "  First directory sector location");
            builder.AppendLine(header.TransactionSignatureNumber, "  Transaction signature number");
            builder.AppendLine(header.MiniStreamCutoffSize, "  Mini stream cutoff size");
            builder.AppendLine(header.FirstMiniFATSectorLocation, "  First mini FAT sector location");
            builder.AppendLine(header.NumberOfMiniFATSectors, "  Number of mini FAT sectors");
            builder.AppendLine(header.FirstDIFATSectorLocation, "  First DIFAT sector location");
            builder.AppendLine(header.NumberOfDIFATSectors, "  Number of DIFAT sectors");
            builder.AppendLine("  DIFAT:");
            if (header.DIFAT == null || header.DIFAT.Length == 0)
            {
                builder.AppendLine("  No DIFAT entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < header.DIFAT.Length; i++)
            {
                builder.AppendLine($"    DIFAT Entry {i}: {header.DIFAT[i]} (0x{header.DIFAT[i]:X})");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, SectorNumber[]? entries, string name)
        {
            builder.AppendLine($"  {name} Sectors Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine($"  No {name} sectors");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                builder.AppendLine($"  {name} Sector Entry {i}: {entries[i]} (0x{entries[i]:X})");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryEntry[]? entries)
        {
            builder.AppendLine("  Directory Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Directory Entry {i}");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.NameLength, "    Name length");
                builder.AppendLine($"    Object type: {entry.ObjectType} (0x{entry.ObjectType:X})");
                builder.AppendLine($"    Color flag: {entry.ColorFlag} (0x{entry.ColorFlag:X})");
                builder.AppendLine($"    Left sibling ID: {entry.LeftSiblingID} (0x{entry.LeftSiblingID:X})");
                builder.AppendLine($"    Right sibling ID: {entry.RightSiblingID} (0x{entry.RightSiblingID:X})");
                builder.AppendLine($"    Child ID: {entry.ChildID} (0x{entry.ChildID:X})");
                builder.AppendLine(entry.CLSID, "    CLSID");
                builder.AppendLine(entry.StateBits, "    State bits");
                builder.AppendLine(entry.CreationTime, "    Creation time");
                builder.AppendLine(entry.ModifiedTime, "    Modification time");
                builder.AppendLine(entry.StartingSectorLocation, "    Staring sector location");
                builder.AppendLine(entry.StreamSize, "    Stream size");
            }

            builder.AppendLine();
        }
    }
}
