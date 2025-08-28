using System.Text;
using SabreTools.Models.GZIP;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class GZip : IPrinter<Archive>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Archive model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Archive file)
        {
            builder.AppendLine("gzip Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // TODO: Implement
        }
    }
}
