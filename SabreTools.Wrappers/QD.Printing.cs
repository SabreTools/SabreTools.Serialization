using System.Text;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class QD : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#else
        /// <inheritdoc/>
        public string ExportJSON() => Newtonsoft.Json.JsonConvert.SerializeObject(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("Quick Disk Image Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            //builder.AppendLine(Model.Data, "Disk Data");
            builder.AppendLine(Model.Data.Length, "Disk Data Length");
        }
    }
}
