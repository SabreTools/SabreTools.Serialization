using System;
using System.Text;
using SabreTools.Data.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class CDROM : IPrintable
    {
        /// <inheritdoc/>
        public new void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("CD-ROM Data Track Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            if (Model.SystemArea == null || Model.SystemArea.Length == 0)
                builder.AppendLine(Model.SystemArea, "System Area");
            else if (Array.TrueForAll(Model.SystemArea, b => b == 0))
                builder.AppendLine("Zeroed", "System Area");
            else
                builder.AppendLine("Not Zeroed", "System Area");
            builder.AppendLine();

            Print(builder, Model.VolumeDescriptorSet);
        }
    }
}
