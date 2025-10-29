using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.BDPlus;

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
            => Print(builder, Model);

        private static void Print(StringBuilder builder, SVM svm)
        {
            builder.AppendLine("BD+ SVM Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(svm.Signature, "Signature");
            builder.AppendLine(svm.Unknown1, "Unknown 1");
            builder.AppendLine(svm.Year, "Year");
            builder.AppendLine(svm.Month, "Month");
            builder.AppendLine(svm.Day, "Day");
            builder.AppendLine(svm.Unknown2, "Unknown 2");
            builder.AppendLine(svm.Length, "Length");
            builder.AppendLine(svm.Length, "Data skipped...");
            builder.AppendLine();
        }
    }
}
