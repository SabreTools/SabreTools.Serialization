using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.SecuROM;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SecuROMMatroschkaPackage : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
            => Print(builder, Model);

        private static void Print(StringBuilder builder, MatroshkaPackage package)
        {
            builder.AppendLine("SecuROM Matroschka Package Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();
            builder.AppendLine(package.Signature, "Signature");
            builder.AppendLine(package.EntryCount, "Entry count");
            builder.AppendLine(package.UnknownRCValue1, "Unknown RC value 1");
            builder.AppendLine(package.UnknownRCValue2, "Unknown RC value 2");
            builder.AppendLine(package.UnknownRCValue3, "Unknown RC value 3");
            builder.AppendLine(package.KeyHexString, "Key hex string");
            builder.AppendLine(package.Padding, "Padding");
            builder.AppendLine();

            Print(builder, package.Entries);
        }

        private static void Print(StringBuilder builder, MatroshkaEntry[]? entries)
        {
            builder.AppendLine("  Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var file = entries[i];

                builder.AppendLine($"  File {i}");
                builder.AppendLine(file.Path, "    Path");
                builder.AppendLine($"Entry type: {file.EntryType} (0x{(uint)file.EntryType:X8})");
                builder.AppendLine(file.Size, "    Size");
                builder.AppendLine(file.Offset, "    Offset");
                builder.AppendLine(file.Unknown, "    Unknown");
                builder.AppendLine(file.ModifiedTime, "    Modified time");
                builder.AppendLine(file.CreatedTime, "    Created time");
                builder.AppendLine(file.AccessedTime, "    Accessed time");
                builder.AppendLine(file.MD5, "    MD5");
                // builder.AppendLine(file.FileData, "    File data");
            }

            builder.AppendLine();
        }
    }
}
