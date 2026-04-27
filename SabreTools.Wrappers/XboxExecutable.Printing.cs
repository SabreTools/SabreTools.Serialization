using System;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.XboxExecutable;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class XboxExecutable : IPrintable
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
            builder.AppendLine("XBox Executable Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);
            Print(builder, Model.Certificate);
            Print(builder, Model.SectionHeaders);
            Print(builder, Model.ThreadLocalStorage);
            Print(builder, Model.LibraryVersions);
            Print(builder, Model.KernelLibraryVersion, "Kernel ", string.Empty, 2);
            Print(builder, Model.XAPILibraryVersion, "XAPI ", string.Empty, 2);
        }

        internal static void Print(StringBuilder builder, Certificate? certificate)
        {
            builder.AppendLine("  Certificate Information:");
            builder.AppendLine("  -------------------------");
            if (certificate is null)
            {
                builder.AppendLine("  No certificate");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(certificate.SizeOfCertificate, "  Size of certificate");
            builder.AppendLine(certificate.TimeDate, "  Time/Date stamp");
            builder.AppendLine(certificate.TitleID, "  Title ID");
            builder.AppendLine(certificate.TitleID.ToFormattedXBETitleID(), "  Title ID (Formatted)");
            builder.AppendLine(certificate.TitleName, "  Title name");
            builder.AppendLine(Encoding.Unicode.GetString(certificate.TitleName).TrimEnd('\0'), "  Title name (Unicode)");
            builder.AppendLine(certificate.AlternativeTitleIDs, "  Alternative title IDs");
            builder.AppendLine(Array.ConvertAll(certificate.AlternativeTitleIDs, b => b.ToFormattedXBETitleID()), "  Alternative title IDs (Formatted)");
            builder.AppendLine($"  Allowed media types: {certificate.AllowedMediaTypes} (0x{certificate.AllowedMediaTypes:X})");
            builder.AppendLine($"  Game region: {certificate.GameRegion} (0x{certificate.GameRegion:X})");
            builder.AppendLine(certificate.GameRatings, "  Game ratings");
            builder.AppendLine(certificate.DiskNumber, "  Disk number");
            builder.AppendLine(certificate.Version, "  Version");
            builder.AppendLine(certificate.LANKey, "  LAN key");
            builder.AppendLine(certificate.SignatureKey, "  Signature key");
            builder.AppendLine(certificate.AlternateSignatureKeys, "  Alternate signature keys");
            builder.AppendLine(certificate.OriginalCertificateSize, "  Original certificate size");
            builder.AppendLine(certificate.OnlineService, "  Online service ID");
            builder.AppendLine(certificate.SecurityFlags, "  Extra security flags");
            builder.AppendLine(certificate.CodeEncKey, "  Code encryption key");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Header? header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header is null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.MagicNumber, "  Magic number");
            builder.AppendLine(header.DigitalSignature, "  Digital signature");
            builder.AppendLine(header.BaseAddress, "  Base address");
            builder.AppendLine(header.SizeOfHeaders, "  Size of headers");
            builder.AppendLine(header.SizeOfImage, "  Size of image");
            builder.AppendLine(header.SizeOfImageHeader, "  Size of image header");
            builder.AppendLine(header.TimeDate, "  Time/Date stamp");
            builder.AppendLine(header.CertificateAddress, "  Certificate address");
            builder.AppendLine(header.NumberOfSections, "  Number of sections");
            builder.AppendLine(header.SectionHeadersAddress, "  Section headers address");
            builder.AppendLine($"  Initialization flags: {header.InitializationFlags} (0x{header.InitializationFlags:X})");
            builder.AppendLine(header.EntryPoint, "  Entry point");
            builder.AppendLine(header.TLSAddress, "  TLS address");
            builder.AppendLine(header.PEStackCommit, "  PE stack commit");
            builder.AppendLine(header.PEHeapReserve, "  PE heap reserve");
            builder.AppendLine(header.PEHeapCommit, "  PE heap commit");
            builder.AppendLine(header.PEBaseAddress, "  PE base address");
            builder.AppendLine(header.PESizeOfImage, "  PE size of image");
            builder.AppendLine(header.PEChecksum, "  PE checksum");
            builder.AppendLine(header.PETimeDate, "  PE time/date stamp");
            builder.AppendLine(header.DebugPathNameAddress, "  Debug path name address");
            builder.AppendLine(header.DebugFileNameAddress, "  Debug file name address");
            builder.AppendLine(header.DebugUnicodeFileNameAddress, "  Debug Unicode file name address");
            builder.AppendLine(header.KernelImageThunkAddress, "  Kernel image thunk address");
            builder.AppendLine(header.NonKernelImportDirectoryAddress, "  Non-kernel import directory address");
            builder.AppendLine(header.NumberOfLibraryVersions, "  Number of library versions");
            builder.AppendLine(header.LibraryVersionsAddress, "  Library versions address");
            builder.AppendLine(header.KernelLibraryVersionAddress, "  Kernel library version address");
            builder.AppendLine(header.XAPILibraryVersionAddress, "  XAPI library version address");
            builder.AppendLine(header.LogoBitmapAddress, "  Logo bitmap address");
            builder.AppendLine(header.LogoBitmapSize, "  Logo bitmap size");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, LibraryVersion? libraryVersion, string prefix, string postfix, int padding)
        {
            builder.AppendLine($"{"".PadLeft(padding)}{prefix}Library Version Information{postfix}:");
            builder.AppendLine($"{"".PadLeft(padding)}-------------------------");
            if (libraryVersion is null)
            {
                builder.AppendLine($"{"".PadLeft(padding)}No library version");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(libraryVersion.LibraryName, $"{"".PadLeft(padding)}Library name");
            builder.AppendLine(Encoding.ASCII.GetString(libraryVersion.LibraryName).TrimEnd('\0'), $"{"".PadLeft(padding)}Library name (ASCII)");
            builder.AppendLine(libraryVersion.MajorVersion, $"{"".PadLeft(padding)}Major version");
            builder.AppendLine(libraryVersion.MinorVersion, $"{"".PadLeft(padding)}Minor version");
            builder.AppendLine(libraryVersion.BuildVersion, $"{"".PadLeft(padding)}Build version");
            builder.AppendLine($"{"".PadLeft(padding)}Library flags: {libraryVersion.LibraryFlags} (0x{libraryVersion.LibraryFlags:X})");
            builder.AppendLine();
        }

        private void Print(StringBuilder builder, LibraryVersion[] entries)
        {
            builder.AppendLine("  Library Version Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries is null || entries.Length == 0)
            {
                builder.AppendLine("  No library version table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries!.Length; i++)
            {
                var entry = entries[i];
                Print(builder, entry, string.Empty, $" {i}", 4);
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, SectionHeader[] entries)
        {
            builder.AppendLine("  Section Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries is null || entries.Length == 0)
            {
                builder.AppendLine("  No section table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries!.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Section Table Entry {i}");
                builder.AppendLine($"    Allowed media types: {entry.SectionFlags} (0x{entry.SectionFlags:X})");
                builder.AppendLine(entry.VirtualAddress, "    Virtual address");
                builder.AppendLine(entry.VirtualSize, "    Virtual size");
                builder.AppendLine(entry.RawAddress, "    Raw address");
                builder.AppendLine(entry.RawSize, "    Raw size");
                builder.AppendLine(entry.SectionNameAddress, "    Section name address");
                builder.AppendLine(entry.SectionNameReferenceCount, "    Section name reference count");
                builder.AppendLine(entry.HeadSharedPageReferenceCountAddress, "    Head shared page reference count address");
                builder.AppendLine(entry.TailSharedPageReferenceCountAddress, "    Tail shared page reference count address");
                builder.AppendLine(entry.SectionDigest, "    Section digest");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ThreadLocalStorage? tls)
        {
            builder.AppendLine("  Thread-Local Storage (TLS) Information:");
            builder.AppendLine("  -------------------------");
            if (tls is null)
            {
                builder.AppendLine("  No thread-local storage");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(tls.DataStartAddress, "  Data start address");
            builder.AppendLine(tls.DataEndAddress, "  Data end address");
            builder.AppendLine(tls.TLSIndexAddress, "  TLS index address");
            builder.AppendLine(tls.TLSCallbackAddress, "  TLS callback address");
            builder.AppendLine(tls.SizeOfZeroFill, "  Size of zero fill");
            builder.AppendLine(tls.Characteristics, "  Characteristics");
            builder.AppendLine();
        }
    }
}
