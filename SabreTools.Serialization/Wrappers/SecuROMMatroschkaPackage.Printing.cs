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
        {
            builder.AppendLine("SecuROM Matroschka Package Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();
            builder.AppendLine(Model.Signature, "Signature");
            builder.AppendLine(Model.EntryCount, "Entry count");
            builder.AppendLine(Model.UnknownRCValue1, "Unknown RC value 1");
            builder.AppendLine(Model.UnknownRCValue2, "Unknown RC value 2");
            builder.AppendLine(Model.UnknownRCValue3, "Unknown RC value 3");
            builder.AppendLine(Model.KeyHexString, "Key hex string");
            builder.AppendLine(Model.Padding, "Padding");
            builder.AppendLine();

            Print(builder, Model.Entries);
        }

        private static void Print(StringBuilder builder, MatroshkaEntry[] entries)
        {
            builder.AppendLine("  Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries.Length == 0)
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
