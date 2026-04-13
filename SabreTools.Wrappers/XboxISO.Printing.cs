using System.Text;
using SabreTools.Data.Models.XboxISO;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class XboxISO : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("Xbox / Xbox 360 Disc Image Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Custom XGD type string
            string xgdType = "XGD?";
            if (XGDType == 0)
                xgdType = "XGD1 (Xbox)";
            else if (XGDType == 1)
                xgdType = "XGD2 (Xbox 360)";
            else if (XGDType == 2)
                xgdType = "XGD2 (Xbox 360 / DVD-Video Hybrid)";
            else if (XGDType == 3)
                xgdType = "XGD3 (Xbox 360)";

            builder.AppendLine(xgdType, "XGD Type");
            builder.AppendLine();

            long initialOffset = _dataSource.Position;

            // Print all information of video partition model
            var videoWrapper = new ISO9660(VideoPartition, _dataSource, initialOffset, _dataSource.Length);
            videoWrapper?.PrintInformation(builder);

            // Print all information of game partition model
            var gameWrapper = new XDVDFS(GamePartition, _dataSource, initialOffset + Constants.XisoOffsets[XGDType], Constants.XisoLengths[XGDType]);
            gameWrapper?.PrintInformation(builder);
        }
    }
}
