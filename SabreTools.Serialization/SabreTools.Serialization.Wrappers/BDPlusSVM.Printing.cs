using System.Text;
using SabreTools.Data.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class BDPlusSVM : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("BD+ SVM Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(Model.Signature, "Signature");
            builder.AppendLine(Model.Unknown1, "Unknown 1");
            builder.AppendLine(Model.Year, "Year");
            builder.AppendLine(Model.Month, "Month");
            builder.AppendLine(Model.Day, "Day");
            builder.AppendLine(Model.Unknown2, "Unknown 2");
            builder.AppendLine(Model.Length, "Length");
            builder.AppendLine(Model.Length, "Data skipped...");
            builder.AppendLine();
        }
    }
}
