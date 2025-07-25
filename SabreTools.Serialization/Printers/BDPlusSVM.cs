using System.Text;
using SabreTools.Models.BDPlus;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class BDPlusSVM : IPrinter<SVM>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, SVM model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, SVM svm)
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
