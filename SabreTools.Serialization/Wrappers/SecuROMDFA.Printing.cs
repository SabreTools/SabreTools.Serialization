using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.SecuROM;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SecuROMDFA : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("SecuROM DFA File Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(Model.Signature, "Signature");
            builder.AppendLine(Model.BlockOrHeaderSize, "Block or header size");
            builder.AppendLine();


            Print(builder, Model.Entries);
        }

        private static void Print(StringBuilder builder, DFAEntry[] entries)
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
                var entry = entries[i];

                builder.AppendLine($"  Entry {i}");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine(entry.Value, "    Value");
            }

            builder.AppendLine();
        }
    }
}
