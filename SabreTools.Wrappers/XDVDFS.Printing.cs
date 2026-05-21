using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.XDVDFS;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class XDVDFS : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON(bool recursive) => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#else
        /// <inheritdoc/>
        public string ExportJSON(bool recursive) => Newtonsoft.Json.JsonConvert.SerializeObject(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, bool recursive)
        {
            builder.AppendLine("Xbox DVD Filesystem Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.ReservedArea);
            Print(builder, Model.VolumeDescriptor);

            if (Model.LayoutDescriptor is not null)
                Print(builder, Model.LayoutDescriptor);

            List<DirectoryRecord> subFiles = [];
            foreach (var kvp in Model.DirectoryDescriptors)
            {
                var files = Print(builder, kvp.Value, kvp.Key);
                if (recursive)
                    subFiles.AddRange(files);
            }

            if (recursive)
            {
                long initialOffset = _dataSource.Position;
                foreach (DirectoryRecord dr in subFiles)
                {
                    // Parse embedded file
                    _dataSource.Seek(initialOffset + Constants.SectorSize * dr.ExtentOffset, SeekOrigin.Begin);
                    byte[] magic = _dataSource.PeekBytes(16);
                    string filename = Encoding.UTF8.GetString(dr.Filename);
                    string extension = Path.GetExtension(filename).TrimStart('.');
                    WrapperType ft = WrapperFactory.GetFileType(magic, extension);
                    var wrapper = WrapperFactory.CreateWrapper(ft, _dataSource);
                    if (wrapper is null || wrapper is not IPrintable printable)
                        continue;
                    
                    // Print info for embedded file
                    builder.AppendLine($"Information for {filename}");
                    builder.AppendLine("-------------------------");
                    printable.PrintInformation(builder, recursive);
                }
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

        internal static List<DirectoryRecord> Print(StringBuilder builder, DirectoryDescriptor dd, uint sectorNumber)
        {
            builder.AppendLine($"  Directory Descriptor (Sector {sectorNumber}):");
            builder.AppendLine("  -------------------------");

            List<DirectoryRecord> files = [];
            foreach (DirectoryRecord dr in dd.DirectoryRecords)
            {
                Print(builder, dr);

                // Append files to queue for recursive printing
                if ((dr.FileFlags & FileFlags.DIRECTORY) != FileFlags.DIRECTORY)
                    files.Add(dr);
            }

            if (dd.Padding is null)
                builder.AppendLine("None", "    Padding");
            else if (Array.TrueForAll(dd.Padding, b => b == 0xFF))
                builder.AppendLine("All 0xFF", "    Padding");
            else
                builder.AppendLine("Not all 0xFF", "    Padding");

            builder.AppendLine();

            return files;
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
