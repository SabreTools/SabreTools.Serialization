using System;
using System.Collections.Generic;
using System.Text;
using SabreTools.Models.CHD;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class CHD : IPrinter<Header>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Header model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Header header)
        {
            builder.AppendLine("CHD Header Information:");
            builder.AppendLine("-------------------------");

            if (header == null)
            {
                builder.AppendLine("No header");
                builder.AppendLine();
                return;
            }

            switch (header)
            {
                case HeaderV1 v1:
                    Print(builder, v1);
                    break;
                case HeaderV2 v2:
                    Print(builder, v2);
                    break;
                case HeaderV3 v3:
                    Print(builder, v3);
                    break;
                case HeaderV4 v4:
                    Print(builder, v4);
                    break;
                case HeaderV5 v5:
                    Print(builder, v5);
                    break;
                default:
                    builder.AppendLine("Unrecognized header type");
                    builder.AppendLine();
                    break;
            }
        }

        private static void Print(StringBuilder builder, HeaderV1 header)
        {
            builder.AppendLine(header.Tag, $"Tag");
            builder.AppendLine(header.Length, $"Length");
            builder.AppendLine(header.Version, $"Version");
            builder.AppendLine($"  Flags: {header.Flags} (0x{header.Flags:X})");
            builder.AppendLine($"  Compression: {header.Compression} (0x{header.Compression:X})");
            builder.AppendLine(header.HunkSize, $"Hunk size");
            builder.AppendLine(header.TotalHunks, $"Total hunks");
            builder.AppendLine(header.Cylinders, $"Cylinders");
            builder.AppendLine(header.Heads, $"Heads");
            builder.AppendLine(header.Sectors, $"Sectors");
            builder.AppendLine(header.MD5, $"MD5");
            builder.AppendLine(header.ParentMD5, $"Parent MD5");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, HeaderV2 header)
        {
            builder.AppendLine(header.Tag, $"Tag");
            builder.AppendLine(header.Length, $"Length");
            builder.AppendLine(header.Version, $"Version");
            builder.AppendLine($"  Flags: {header.Flags} (0x{header.Flags:X})");
            builder.AppendLine($"  Compression: {header.Compression} (0x{header.Compression:X})");
            builder.AppendLine(header.HunkSize, $"Hunk size");
            builder.AppendLine(header.TotalHunks, $"Total hunks");
            builder.AppendLine(header.Cylinders, $"Cylinders");
            builder.AppendLine(header.Heads, $"Heads");
            builder.AppendLine(header.Sectors, $"Sectors");
            builder.AppendLine(header.MD5, $"MD5");
            builder.AppendLine(header.ParentMD5, $"Parent MD5");
            builder.AppendLine(header.BytesPerSector, $"Bytes per sector");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, HeaderV3 header)
        {
            builder.AppendLine(header.Tag, $"Tag");
            builder.AppendLine(header.Length, $"Length");
            builder.AppendLine(header.Version, $"Version");
            builder.AppendLine($"  Flags: {header.Flags} (0x{header.Flags:X})");
            builder.AppendLine($"  Compression: {header.Compression} (0x{header.Compression:X})");
            builder.AppendLine(header.TotalHunks, $"Total hunks");
            builder.AppendLine(header.LogicalBytes, $"Logical bytes");
            builder.AppendLine(header.MetaOffset, $"Meta offset");
            builder.AppendLine(header.MD5, $"MD5");
            builder.AppendLine(header.ParentMD5, $"Parent MD5");
            builder.AppendLine(header.HunkBytes, $"Hunk bytes");
            builder.AppendLine(header.SHA1, $"SHA-1");
            builder.AppendLine(header.ParentSHA1, $"Parent SHA-1");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, HeaderV4 header)
        {
            builder.AppendLine(header.Tag, $"Tag");
            builder.AppendLine(header.Length, $"Length");
            builder.AppendLine(header.Version, $"Version");
            builder.AppendLine($"  Flags: {header.Flags} (0x{header.Flags:X})");
            builder.AppendLine($"  Compression: {header.Compression} (0x{header.Compression:X})");
            builder.AppendLine(header.TotalHunks, $"Total hunks");
            builder.AppendLine(header.LogicalBytes, $"Logical bytes");
            builder.AppendLine(header.MetaOffset, $"Meta offset");
            builder.AppendLine(header.HunkBytes, $"Hunk bytes");
            builder.AppendLine(header.SHA1, $"SHA-1");
            builder.AppendLine(header.ParentSHA1, $"Parent SHA-1");
            builder.AppendLine(header.RawSHA1, $"Raw SHA-1");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, HeaderV5 header)
        {
            builder.AppendLine(header.Tag, $"Tag");
            builder.AppendLine(header.Length, $"Length");
            builder.AppendLine(header.Version, $"Version");

            string compressorsLine = "Compressors: ";
            if (header.Compressors == null)
            {
                compressorsLine += "[NULL]";
            }
            else
            {
                var compressors = new List<string>();
                for (int i = 0; i < header.Compressors.Length; i++)
                {
                    uint compressor = (uint)header.Compressors[i];
                    byte[] compressorBytes = BitConverter.GetBytes(compressor);
                    Array.Reverse(compressorBytes);
                    string compressorString = Encoding.ASCII.GetString(compressorBytes);
                    compressors.Add(compressorString);
                }

                compressorsLine += string.Join(", ", [.. compressors]);
            }
            builder.AppendLine(compressorsLine);

            builder.AppendLine(header.LogicalBytes, $"Logical bytes");
            builder.AppendLine(header.MapOffset, $"Map offset");
            builder.AppendLine(header.MetaOffset, $"Meta offset");
            builder.AppendLine(header.HunkBytes, $"Hunk bytes");
            builder.AppendLine(header.UnitBytes, $"Unit bytes");
            builder.AppendLine(header.RawSHA1, $"Raw SHA-1");
            builder.AppendLine(header.SHA1, $"SHA-1");
            builder.AppendLine(header.ParentSHA1, $"Parent SHA-1");
            builder.AppendLine();
        }
    }
}
