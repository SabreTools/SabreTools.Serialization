using System;
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

            long initialOffset = _dataSource.Position;

            var videoWrapper = new SabreTools.Wrappers.ISO9660(VideoPartition, _dataSource, initialOffset, _dataSource.Length);
            if (videoWrapper is not null)
                videoWrapper.PrintInformation(builder);

            var gameWrapper = new SabreTools.Wrappers.XDVDFS(GamePartition, _dataSource, initialOffset + Constants.XisoOffsets[XGDType], Constants.XisoLengths[XGDType]);
            if (gameWrapper is not null)
                gameWrapper.PrintInformation(builder);
        }
    }
}
