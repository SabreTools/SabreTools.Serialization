using System.Text;
using SabreTools.Data.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class WiseSectionHeader : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("Wise Section Header Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(Model.UnknownDataSize, "Unknown data size");
            builder.AppendLine(Model.SecondExecutableFileEntryLength, "Second executable file entry length");
            builder.AppendLine(Model.UnknownValue2, "Unknown value 2");
            builder.AppendLine(Model.UnknownValue3, "Unknown value 3");
            builder.AppendLine(Model.UnknownValue4, "Unknown value 4");
            builder.AppendLine(Model.FirstExecutableFileEntryLength, "First executable file entry length");
            builder.AppendLine(Model.MsiFileEntryLength, "MSI file entry length");
            builder.AppendLine(Model.UnknownValue7, "Unknown value 7");
            builder.AppendLine(Model.UnknownValue8, "Unknown value 8");
            builder.AppendLine(Model.ThirdExecutableFileEntryLength, "Third executable file entry length");
            builder.AppendLine(Model.UnknownValue10, "Unknown value 10");
            builder.AppendLine(Model.UnknownValue11, "Unknown value 11");
            builder.AppendLine(Model.UnknownValue12, "Unknown value 12");
            builder.AppendLine(Model.UnknownValue13, "Unknown value 13");
            builder.AppendLine(Model.UnknownValue14, "Unknown value 14");
            builder.AppendLine(Model.UnknownValue15, "Unknown value 15");
            builder.AppendLine(Model.UnknownValue16, "Unknown value 16");
            builder.AppendLine(Model.UnknownValue17, "Unknown value 17");
            builder.AppendLine(Model.UnknownValue18, "Unknown value 18");
            builder.AppendLine(Model.Version, "Version");
            builder.AppendLine(Model.TmpString, "TMP string");
            builder.AppendLine(Model.GuidString, "GUID string");
            builder.AppendLine(Model.NonWiseVersion, "Non-Wise version");
            builder.AppendLine(Model.PreFontValue, "Pre-font value");
            builder.AppendLine(Model.FontSize, "Font size");
            builder.AppendLine(Model.PreStringValues, "Pre-string values");
            builder.AppendLine();
            builder.AppendLine("Strings:");
            if (Model.Strings == null || Model.Strings.Length == 0)
            {
                builder.AppendLine("  No strings");
            }
            else
            {
                for (int i = 0; i < Model.Strings.Length; i++)
                {
                    var entry = Model.Strings[i];
                    builder.AppendLine($"  String {i}: {entry}");
                }
            }
            builder.AppendLine();
        }
    }
}
