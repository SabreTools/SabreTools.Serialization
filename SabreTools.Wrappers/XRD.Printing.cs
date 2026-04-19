using System.Text;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class XRD : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("XRD Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(Model.Magic, "Magic");
            builder.AppendLine(Model.Version, "Version");
            builder.AppendLine(Model.XGDType, "XGD Type");
            builder.AppendLine(Model.XGDSubtype, "XGD Subtype");
            builder.AppendLine(Model.Ringcode, "Ringcode", Encoding.ASCII);
            builder.AppendLine(Model.RedumpSize, "Redump Size");
            builder.AppendLine(Model.RedumpCRC, "Redump CRC-32");
            builder.AppendLine(Model.RedumpMD5, "Redump MD5");
            builder.AppendLine(Model.RedumpSHA1, "Redump SHA-1");
            builder.AppendLine(Model.RawXISOSize, "Raw XISO Size");
            builder.AppendLine(Model.RawXISOCRC, "Raw XISO CRC-32");
            builder.AppendLine(Model.RawXISOMD5, "Raw XISO MD5");
            builder.AppendLine(Model.RawXISOSHA1, "Raw XISO SHA-1");
            builder.AppendLine(Model.CookedXISOSize, "Cooked XISO Size");
            builder.AppendLine(Model.CookedXISOCRC, "Cooked XISO CRC-32");
            builder.AppendLine(Model.CookedXISOMD5, "Cooked XISO MD5");
            builder.AppendLine(Model.CookedXISOSHA1, "Cooked XISO SHA-1");
            builder.AppendLine(Model.VideoISOSize, "Video ISO Size");
            builder.AppendLine(Model.VideoISOCRC, "Video ISO CRC-32");
            builder.AppendLine(Model.VideoISOMD5, "Video ISO MD5");
            builder.AppendLine(Model.VideoISOSHA1, "Video ISO SHA-1");

            if (Model.WipedVideoISOSize is not null)
                builder.AppendLine(Model.WipedVideoISOSize, "Wiped Video ISO Size");

            if (Model.WipedVideoISOCRC is not null)
                builder.AppendLine(Model.WipedVideoISOCRC, "Wiped Video ISO CRC-32");

            if (Model.WipedVideoISOMD5 is not null)
                builder.AppendLine(Model.WipedVideoISOMD5, "Wiped Video ISO MD5");

            if (Model.WipedVideoISOSHA1 is not null)
                builder.AppendLine(Model.WipedVideoISOSHA1, "Wiped Video ISO SHA-1");

            builder.AppendLine(Model.FillerSize, "First Sector Filler Size");
            builder.AppendLine(Model.FillerCRC, "First Sector Filler CRC-32");
            builder.AppendLine(Model.FillerMD5, "First Sector Filler MD5");
            builder.AppendLine(Model.FillerSHA1, "First Sector Filler SHA-1");

            if (Model.SecuritySectors is not null)
            {
                for (int i = 0; i < Model.SecuritySectors.Length; i++)
                {
                    builder.AppendLine(Model.SecuritySectors[i], $"Security Sector {i}");
                }
            }

            if (Model.XboxCertificate is not null)
                XboxExecutable.Print(builder, Model.XboxCertificate);

            if (Model.Xbox360Certificate is not null)
                XenonExecutable.Print(builder, Model.Xbox360Certificate);

            builder.AppendLine(Model.FileCount, "XISO File Count");

            for (int i = 0; i < Model.FileInfo.Length; i++)
            {
                builder.AppendLine(Model.FileInfo[i].Offset, $"XISO File {i} Offset");
                builder.AppendLine(Model.FileInfo[i].Size, $"XISO File {i} Size");
                builder.AppendLine(Model.FileInfo[i].SHA1, $"XISO File {i} SHA-1");
            }

            XDVDFS.Print(builder, Model.VolumeDescriptor);

            if (Model.LayoutDescriptor is not null)
                XDVDFS.Print(builder, Model.LayoutDescriptor);

            builder.AppendLine(Model.DirectoryCount, "XISO Directory Count");

            for (int i = 0; i < Model.DirectoryInfo.Length; i++)
            {
                builder.AppendLine(Model.DirectoryInfo[i].Offset, $"XISO Directory {i} Offset");
                builder.AppendLine(Model.DirectoryInfo[i].Size, $"XISO Directory {i} Size");
                XDVDFS.Print(builder, Model.DirectoryInfo[i].DirectoryDescriptor, Model.DirectoryInfo[i].Offset);
            }

            if (Model.VideoISOFileCount is not null)
            {
                builder.AppendLine(Model.VideoISOFileCount, "Video ISO File Count");
            }

            if (Model.VideoISOFileInfo is not null)
            {
                for (int i = 0; i < Model.VideoISOFileInfo.Length; i++)
                {
                    builder.AppendLine(Model.VideoISOFileInfo[i].Offset, $"Video ISO File {i} Offset");
                    builder.AppendLine(Model.VideoISOFileInfo[i].Size, $"Video ISO File {i} Size");
                    builder.AppendLine(Model.VideoISOFileInfo[i].SHA1, $"Video ISO File {i} SHA-1");
                }
            }

            builder.AppendLine(Model.XRDSize, "XRD Size");
            builder.AppendLine(Model.XRDSHA1, "XRD SHA-1");
            builder.AppendLine();
        }
    }
}
