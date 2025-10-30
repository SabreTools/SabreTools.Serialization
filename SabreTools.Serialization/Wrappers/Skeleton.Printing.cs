using System;
using System.Text;
using SabreTools.Data.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class Skeleton : IPrintable
    {
        /// <inheritdoc/>
        public new void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("Redumper Skeleton Information:");
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

            // TODO: Parse the volume descriptors to print the Path Table Groups and Directory Descriptors with proper encoding
            Encoding encoding = Encoding.UTF8;
            Print(builder, Model.PathTableGroups, encoding);
            Print(builder, Model.DirectoryDescriptors, encoding);
        }
    }
}
