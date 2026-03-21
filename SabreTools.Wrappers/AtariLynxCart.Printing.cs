using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.AtariLynx;

namespace SabreTools.Wrappers
{
    public partial class AtariLynxCart : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("Atari Lynx Cart Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);

            //builder.AppendLine(Model.Data, "ROM Data");
            builder.AppendLine(Model.Data.Length, "ROM Data Length");
        }

        private static void Print(StringBuilder builder, Header? header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header is null)
            {
                builder.AppendLine("  No header present");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Magic, "  Magic");
            builder.AppendLine(Encoding.ASCII.GetString(header.Magic).TrimEnd('\0'), "  Magic (ASCII)");
            builder.AppendLine(header.Bank0PageSize, "  Bank 0 page size");
            builder.AppendLine(header.Bank1PageSize, "  Bank 1 page size");
            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine(header.CartName, "  Cart name");
            builder.AppendLine(Encoding.ASCII.GetString(header.CartName).TrimEnd('\0'), "  Cart name (ASCII)");
            builder.AppendLine(header.Manufacturer, "  Manufacturer");
            builder.AppendLine(Encoding.ASCII.GetString(header.Manufacturer).TrimEnd('\0'), "  Manufacturer (ASCII)");

            string rotation = header.Rotation.FromRotation();
            builder.AppendLine(rotation, "  Rotation");

            builder.AppendLine(header.Spare, "  Padding");
            builder.AppendLine();
        }
    }
}
