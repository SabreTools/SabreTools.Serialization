using System;
using System.Text;
using SabreTools.Data.Models.XDVDFS;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class XDVDFS : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#else
        /// <inheritdoc/>
        public string ExportJSON() => Newtonsoft.Json.JsonConvert.SerializeObject(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("Xbox DVD Filesystem Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.ReservedArea);
            Print(builder, Model.VolumeDescriptor);

            if (Model.LayoutDescriptor is not null)
                Print(builder, Model.LayoutDescriptor);

            foreach (var kvp in Model.DirectoryDescriptors)
            {
                Print(builder, kvp.Value, kvp.Key);
            }
        }

        private static void Print(StringBuilder builder, byte[] reservedArea)
        {
            if (reservedArea.Length == 0)
                builder.AppendLine(reservedArea, "  Reserved Area");
            else if (Array.TrueForAll(reservedArea, b => b == 0))
                builder.AppendLine("Zeroed", "  Reserved Area");
            else
                builder.AppendLine("Not Zeroed", "  Reserved Area");
            builder.AppendLine();
        }

        internal static void Print(StringBuilder builder, VolumeDescriptor vd)
        {
            builder.AppendLine("  Volume Descriptor:");
            builder.AppendLine("  -------------------------");

            builder.AppendLine(Encoding.ASCII.GetString(vd.StartSignature), "    Start Signature");
            builder.AppendLine(vd.RootOffset, "    Root Offset");
            builder.AppendLine(vd.RootSize, "    Root Size");
            DateTime datetime = DateTime.FromFileTime(vd.MasteringTimestamp);
            builder.AppendLine(datetime.ToString("yyyy-MM-dd HH:mm:ss"), "    Mastering Timestamp");
            builder.AppendLine(vd.UnknownByte, "    Unknown Byte");
            if (Array.TrueForAll(vd.Reserved, b => b == 0))
                builder.AppendLine("Zeroed", "    Reserved Bytes");
            else
                builder.AppendLine("Not Zeroed", "    Reserved Bytes");
            builder.AppendLine(Encoding.ASCII.GetString(vd.EndSignature), "    End Signature");

            builder.AppendLine();
        }

        internal static void Print(StringBuilder builder, LayoutDescriptor ld)
        {
            builder.AppendLine("  Xbox DVD Layout Descriptor:");
            builder.AppendLine("  -------------------------");

            builder.AppendLine(Encoding.ASCII.GetString(ld.Signature), "    Signature");
            builder.AppendLine(ld.Unused8Bytes, "    Unusued 8 Bytes");
            builder.AppendLine(GetVersionString(ld.XBLayoutVersion), "    xblayout Version");
            builder.AppendLine(GetVersionString(ld.XBPremasterVersion), "    xbpremaster Version");
            builder.AppendLine(GetVersionString(ld.XBGameDiscVersion), "    xbgamedisc Version");
            builder.AppendLine(GetVersionString(ld.XBOther1Version), "    Unknown Tool 1 Version");
            builder.AppendLine(GetVersionString(ld.XBOther2Version), "    Unknown Tool 2 Version");
            builder.AppendLine(GetVersionString(ld.XBOther3Version), "    Unknown Tool 2 Version");
            if (Array.TrueForAll(ld.Reserved, b => b == 0))
                builder.AppendLine("Zeroed", "    Reserved Bytes");
            else
                builder.AppendLine("Not Zeroed", "    Reserved Bytes");

            builder.AppendLine();
        }

        private static string GetVersionString(FourPartVersionType ver)
        {
            return $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
        }

        internal static void Print(StringBuilder builder, DirectoryDescriptor dd, uint sectorNumber)
        {
            builder.AppendLine($"  Directory Descriptor (Sector {sectorNumber}):");
            builder.AppendLine("  -------------------------");

            foreach (DirectoryRecord dr in dd.DirectoryRecords)
                Print(builder, dr);

            if (dd.Padding is null)
                builder.AppendLine("None", "    Padding");
            else if (Array.TrueForAll(dd.Padding, b => b == 0xFF))
                builder.AppendLine("All 0xFF", "    Padding");
            else
                builder.AppendLine("Not all 0xFF", "    Padding");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryRecord dr)
        {
            builder.AppendLine($"      Directory Record:");
            builder.AppendLine("      -------------------------");

            builder.AppendLine(dr.LeftChildOffset, "      Left Child Offset");
            builder.AppendLine(dr.RightChildOffset, "      Right Child Offset");
            builder.AppendLine(dr.ExtentOffset, "      Extent Offset");
            builder.AppendLine(dr.ExtentSize, "      Extent Size");

            builder.AppendLine("      File Flags:");
            builder.AppendLine((dr.FileFlags & FileFlags.READ_ONLY) == FileFlags.READ_ONLY, "        Read-only");
            builder.AppendLine((dr.FileFlags & FileFlags.HIDDEN) == FileFlags.HIDDEN, "        Hidden");
            builder.AppendLine((dr.FileFlags & FileFlags.SYSTEM) == FileFlags.SYSTEM, "        System");
            builder.AppendLine((dr.FileFlags & FileFlags.VOLUME_ID) == FileFlags.VOLUME_ID, "        Volume ID");
            builder.AppendLine((dr.FileFlags & FileFlags.DIRECTORY) == FileFlags.DIRECTORY, "        Directory");
            builder.AppendLine((dr.FileFlags & FileFlags.ARCHIVE) == FileFlags.ARCHIVE, "        Archive");
            builder.AppendLine((dr.FileFlags & FileFlags.DEVICE) == FileFlags.DEVICE, "        Device");
            builder.AppendLine((dr.FileFlags & FileFlags.NORMAL) == FileFlags.NORMAL, "        Normal");

            builder.AppendLine(dr.FilenameLength, "      Filename Length");
            builder.AppendLine(Encoding.UTF8.GetString(dr.Filename), "      Filename");

            if (dr.Padding is null)
                builder.AppendLine("None", "      Padding");
            else if (Array.TrueForAll(dr.Padding, b => b == 0xFF))
                builder.AppendLine("All 0xFF", "      Padding");
            else
                builder.AppendLine("Not all 0xFF", "      Padding");

            builder.AppendLine();
        }
    }
}
