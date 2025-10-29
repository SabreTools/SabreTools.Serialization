using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.WiseInstaller;

namespace SabreTools.Serialization.Wrappers
{
    public partial class WiseOverlayHeader : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
#if NET20 || NET35
            bool pkzip = (Model.Flags & OverlayHeaderFlags.WISE_FLAG_PK_ZIP) != 0;
#else
            bool pkzip = Model.Flags.HasFlag(OverlayHeaderFlags.WISE_FLAG_PK_ZIP);
#endif

            builder.AppendLine("Wise Installer Overlay Header Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(Model.DllNameLen, "DLL name length");
            builder.AppendLine(Model.DllName, "DLL name");
            builder.AppendLine(Model.DllSize, "DLL size");
            builder.AppendLine($"Flags: {Model.Flags} (0x{(uint)Model.Flags:X8})");
            builder.AppendLine(pkzip, "  Uses PKZIP containers");
            builder.AppendLine(Model.GraphicsData, "Graphics data");
            builder.AppendLine(Model.WiseScriptExitEventOffset, "Wise script exit event offset");
            builder.AppendLine(Model.WiseScriptCancelEventOffset, "Wise script cancel event offset");
            builder.AppendLine(Model.WiseScriptInflatedSize, "Wise script inflated size");
            builder.AppendLine(Model.WiseScriptDeflatedSize, "Wise script deflated size");
            builder.AppendLine(Model.WiseDllDeflatedSize, "Wise DLL deflated size");
            builder.AppendLine(Model.Ctl3d32DeflatedSize, "CTL3D32.DLL deflated size");
            builder.AppendLine(Model.SomeData4DeflatedSize, "FILE0004 deflated size");
            builder.AppendLine(Model.RegToolDeflatedSize, "Ocxreg32.EXE deflated size");
            builder.AppendLine(Model.ProgressDllDeflatedSize, "PROGRESS.DLL deflated size");
            builder.AppendLine(Model.SomeData7DeflatedSize, "FILE0007 deflated size");
            builder.AppendLine(Model.SomeData8DeflatedSize, "FILE0008 deflated size");
            builder.AppendLine(Model.SomeData9DeflatedSize, "FILE0009 deflated size");
            builder.AppendLine(Model.SomeData10DeflatedSize, "FILE000A deflated size");
            builder.AppendLine(Model.FinalFileDeflatedSize, "FILE000{n}.DAT deflated size");
            builder.AppendLine(Model.FinalFileInflatedSize, "FILE000{n}.DAT inflated size");
            builder.AppendLine(Model.EOF, "EOF");
            builder.AppendLine(Model.DibDeflatedSize, "DIB deflated size");
            builder.AppendLine(Model.DibInflatedSize, "DIB inflated size");
            builder.AppendLine(Model.InstallScriptDeflatedSize, "Install script deflated size");
            if (Model.CharacterSet != null)
                builder.AppendLine($"Character set: {Model.CharacterSet} (0x{(uint)Model.CharacterSet:X8})");
            else
                builder.AppendLine((uint?)null, $"Character set");
            builder.AppendLine($"Endianness: {Model.Endianness} (0x{(ushort)Model.Endianness:X4})");
            builder.AppendLine(Model.InitTextLen, "Init text length");
            builder.AppendLine(Model.InitText, "Init text");
            builder.AppendLine();
        }
    }
}
