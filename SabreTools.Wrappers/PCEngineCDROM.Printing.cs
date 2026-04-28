using System;
using System.Collections.Generic;
using System.Text;
using SabreTools.Data.Models.PCEngineCDROM;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class PCEngineCDROM : IPrintable
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
            builder.AppendLine("PC Engine CD-ROM Header Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.BootSector);
            Print(builder, Model.IPL);
        }

        internal static void Print(StringBuilder builder, BootSector bootSector)
        {
            builder.AppendLine("  Boot Sector:");
            builder.AppendLine("  -------------------------");

#if NET462_OR_GREATER || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            var message = Environment.NewLine + "    " + Encoding.GetEncoding(932).GetString(bootSector.CopyrightString).Replace("\0", Environment.NewLine + "    ");

            builder.AppendLine(message, "  Copyright String");
            builder.AppendLine(bootSector.BootROM, "  HuC6280 Machine Code");
            if (bootSector.Padding is null)
                builder.AppendLine(bootSector.Padding, "  Padding Bytes");
            else if (Array.TrueForAll(bootSector.Padding, b => b == 0))
                builder.AppendLine("Zeroed", "  Padding Bytes");
            else
                builder.AppendLine("Not Zeroed", "  Padding Bytes");

            builder.AppendLine();
        }

        internal static void Print(StringBuilder builder, IPL ipl)
        {
            builder.AppendLine("  Initial Program Loader:");
            builder.AppendLine("  -------------------------");

            builder.AppendLine((uint)ipl.IPLBLK, "  Load Start Record Number of CD");
            builder.AppendLine(ipl.IPLBLN, "  Load Block Length of CD");
            builder.AppendLine(ipl.IPLSTA, "  Program Load Address");
            builder.AppendLine(ipl.IPLJMP, "  Program Execute Address");
            builder.AppendLine(ipl.IPLMPR, "  Memory Page Register (2-6)");
            builder.AppendLine((byte)ipl.OpenMode, "  OpenMode");
            builder.AppendLine((uint)ipl.GRPBLK, "  Opening Graphic Data Record Number");
            builder.AppendLine(ipl.GRPBLN, "  Opening Graphic Data Length");
            builder.AppendLine(ipl.GRPADR, "  Opening Graphic Data Read Address");
            builder.AppendLine((uint)ipl.ADPBLK, "  Opening ADPCM Data Record Number");
            builder.AppendLine(ipl.ADPBLN, "  Opening ADPCM Data Length");
            builder.AppendLine(ipl.ADPRATE, "  Opening ADPCM Sampling Rate");
            if (ipl.Reserved is not null && Array.TrueForAll(ipl.Reserved, b => b == 0))
                builder.AppendLine($"Zeroed", "  Reserved Bytes");
            else
                builder.AppendLine(ipl.Reserved, "  Reserved Bytes");

            builder.AppendLine(Encoding.GetEncoding(932).GetString(ipl.SystemString), "  ID String");
            builder.AppendLine(Encoding.GetEncoding(932).GetString(ipl.CopyrightString), "  Copyright String");
            builder.AppendLine(Encoding.GetEncoding(932).GetString(ipl.ProgramName), "  Program Name");
            builder.AppendLine(Encoding.GetEncoding(932).GetString(ipl.AdditionalString), "  Additional String");

            builder.AppendLine();
        }
    }
}
