using System;
using System.Text;
using SabreTools.Data.Models.STFS;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class STFS : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("Secure Transacted File System Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);

            // Do not print info about Hash Table Blocks or Data Blocks
        }

        protected static void Print(StringBuilder builder, Header header)
        {
            builder.AppendLine(header.MagicBytes, "  Magic Bytes");

            builder.AppendLine();

            Print(builder, header.Signature);

            Print(builder, header.LicensingData);

            builder.AppendLine(header.HeaderHash, "  Header Hash");
            builder.AppendLine(header.HeaderSize, "  Header Size");
            builder.AppendLine(header.ContentType, "  Content Type"); // See Enums.ContentType
            builder.AppendLine(header.MetadataVersion, "  Metadata Version");
            builder.AppendLine(header.ContentSize, "  Content Size");
            builder.AppendLine(header.MediaID, "  Media ID");
            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine(header.BaseVersion, "  Base Version");
            builder.AppendLine(header.TitleID, "  Title ID");
            builder.AppendLine(header.Platform, "  Platform");
            builder.AppendLine(header.ExecutableType, "  Executable Type");
            builder.AppendLine(header.DiscNumber, "  Disc Number");
            builder.AppendLine(header.DiscInSet, "  Disc in Set");
            builder.AppendLine(header.SaveGameID, "  Save Game ID");
            builder.AppendLine(header.ConsoleID, "  Console ID");
            builder.AppendLine(header.ProfileID, "  Profile ID");

            builder.AppendLine();

            Print(builder, header.VolumeDescriptor);

            builder.AppendLine(header.DataFileCount, "  Data File Count");
            builder.AppendLine(header.DataFileCombinedSize, "  Data File Combined Size");
            builder.AppendLine(header.DescriptorType, "  Descriptor Type");
            builder.AppendLine(header.Reserved, "  Reserved");

            if (header.MetadataVersion == 2)
            {
                builder.AppendLine(header.SeriesID, "  Series ID");
                builder.AppendLine(header.SeasonID, "  Season ID");
                builder.AppendLine(header.SeasonNumber, "  Season Number");
                builder.AppendLine(header.EpisodeNumber, "  Episode Number");
            }

            if (Array.TrueForAll(header.Padding, b => b == 0))
                builder.AppendLine("Zeroed", "  Padding");
            else
                builder.AppendLine(header.Padding, "  Padding");

            builder.AppendLine(header.DeviceID, "  Device ID");

            if (Array.TrueForAll(header.DisplayName, b => b == 0))
            {
                builder.AppendLine("Zeroed", "  Display Name");
            }
            else
            {
                for (int i = 0; i < 18; i++)
                {
                    byte[] localeString = new byte[128];
                    Array.Copy(header.DisplayName, i * 128, localeString, 0, 128);
                    if (!Array.TrueForAll(localeString, b => b == 0))
                    {
                        builder.AppendLine(localeString, $"  Display Name {i}");
                        builder.AppendLine(Encoding.BigEndianUnicode.GetString(localeString), $"  Display Name {i} (Parsed)");
                    }
                }
            }

            if (Array.TrueForAll(header.DisplayDescription, b => b == 0))
            {
                builder.AppendLine("Zeroed", "  Display Description");
            }
            else
            {
                for (int i = 0; i < 18; i++)
                {
                    byte[] localeString = new byte[128];
                    Array.Copy(header.DisplayDescription, i * 128, localeString, 0, 128);
                    if (!Array.TrueForAll(localeString, b => b == 0))
                    {
                        builder.AppendLine(localeString, $"  Display Description {i}");
                        builder.AppendLine(Encoding.BigEndianUnicode.GetString(localeString), $"  Display Description {i} (Parsed)");
                    }
                }
            }

            if (Array.TrueForAll(header.PublisherName, b => b == 0))
            {
                builder.AppendLine("Zeroed", "  Publisher Name");
            }
            else
            {
                builder.AppendLine(header.PublisherName, "  Publisher Name");
                builder.AppendLine(Encoding.BigEndianUnicode.GetString(header.PublisherName), "  Publisher Name (Parsed)");
            }

            if (Array.TrueForAll(header.TitleName, b => b == 0))
            {
                builder.AppendLine("Zeroed", "  Title Name");
            }
            else
            {
                builder.AppendLine(header.TitleName, "  Title Name");
                builder.AppendLine(Encoding.BigEndianUnicode.GetString(header.TitleName), "  Title Name (Parsed)");
            }

            builder.AppendLine(header.TransferFlags, "  Transfer Flags"); // See Enums.TransferFlags
            builder.AppendLine(header.ThumbnailImageSize, "  Thumbnail Image Size");
            builder.AppendLine(header.TitleThumbnailImageSize, "  Title Thumbnail Image Size");

            if (header.AdditionalDisplayNames is not null)
            {
                for (int i = 0; i < 6; i++)
                {
                    byte[] localeString = new byte[128];
                    Array.Copy(header.AdditionalDisplayNames, i * 128, localeString, 0, 128);
                    if (!Array.TrueForAll(localeString, b => b == 0))
                    {
                        builder.AppendLine(localeString, $"  Additional Display Name {i}");
                        builder.AppendLine(Encoding.BigEndianUnicode.GetString(localeString), $"  Additional Display Name {i} (Parsed)");
                    }
                }
            }

            if (header.AdditionalDisplayDescriptions is not null)
            {
                for (int i = 0; i < 6; i++)
                {
                    byte[] localeString = new byte[128];
                    Array.Copy(header.AdditionalDisplayDescriptions, i * 128, localeString, 0, 128);
                    if (!Array.TrueForAll(localeString, b => b == 0))
                    {
                        builder.AppendLine(localeString, $"  Additional Display Description {i}");
                        builder.AppendLine(Encoding.BigEndianUnicode.GetString(localeString), $"  Additional Display Description {i} (Parsed)");
                    }
                }
            }

            if (header.InstallerHeader is not null)
                Print(builder, header.InstallerHeader);

            builder.AppendLine();
        }

        protected static void Print(StringBuilder builder, Signature? signature)
        {
            builder.AppendLine("  Signature Information");
            builder.AppendLine("  -------------------------");

            if (signature is MicrosoftSignature ms)
            {
                builder.AppendLine(ms.PackageSignature, "    Package Signature");
                if (Array.TrueForAll(ms.Padding, b => b == 0))
                    builder.AppendLine("Zeroed", "    Padding");
                else
                    builder.AppendLine(ms.Padding, "    Padding");
            }
            else if (signature is ConsoleSignature cs)
            {
                builder.AppendLine(cs.CertificateSize, "    Certificate Size");
                builder.AppendLine(cs.ConsoleID, "    Console ID");
                builder.AppendLine(cs.PartNumber, "    Part Number");
                builder.AppendLine(cs.ConsoleType, "    Console Type");
                builder.AppendLine(cs.CertificateDate, "    Certificate Date");
                builder.AppendLine(cs.PublicExponent, "    Public Exponent");
                builder.AppendLine(cs.PublicModulus, "    Public Modulus");
                builder.AppendLine(cs.CertificateSignature, "    Certificate Signature");
                builder.AppendLine(cs.Signature, "    Signature");
            }
            else
            {
                builder.AppendLine("    Unknown Signature Type");
            }

            builder.AppendLine();
        }

        protected static void Print(StringBuilder builder, LicenseEntry[] ld)
        {
            builder.AppendLine("  Licensing Data Information");
            builder.AppendLine("  -------------------------");

            int lastLicenseData = 0;
            for (int i = ld.Length - 1; i >= 0; i--)
            {
                if (ld[i].LicenseID != 0 || ld[i].LicenseBits != 0 || ld[i].LicenseFlags != 0)
                {
                    lastLicenseData = i + 1;
                    break;
                }

                if (i == 0)
                    builder.AppendLine("Zeroed", "    Licensing Data");
            }

            for (int i = 0; i < lastLicenseData; i++)
            {
                if (ld[i].LicenseID == 0 && ld[i].LicenseBits == 0 && ld[i].LicenseFlags == 0)
                {
                    builder.AppendLine("Zeroed", $"    License Entry {i}");
                }
                else
                {
                    builder.AppendLine(ld[i].LicenseID, $"    License Entry {i} ID");
                    builder.AppendLine(ld[i].LicenseBits, $"    License Entry {i} Bits");
                    builder.AppendLine(ld[i].LicenseFlags, $"    License Entry {i} Flags");
                }
            }

            builder.AppendLine();
        }

        protected static void Print(StringBuilder builder, VolumeDescriptor? vd)
        {
            builder.AppendLine("  Volume Descriptor Information");
            builder.AppendLine("  -------------------------");
            if (vd is STFSDescriptor stfs)
            {
                builder.AppendLine(stfs.VolumeDescriptorSize, "    Volume Descriptor Size");
                builder.AppendLine(stfs.Reserved, "    Reserved");
                builder.AppendLine(stfs.BlockSeparation, "    Block Separation");
                builder.AppendLine(stfs.FileTableBlockCount, "    File Table Block Count");
                builder.AppendLine((uint)stfs.FileTableBlockNumber, "    File Table Block Number");
                builder.AppendLine(stfs.TopHashTableHash, "    Top Hash Table Hash");
                builder.AppendLine(stfs.TotalAllocatedBlockCount, "    Total Allocated Block Count");
                builder.AppendLine(stfs.TotalUnallocatedBlockCount, "    Total Unallocated Block Count");
            }
            else if (vd is SVODDescriptor svod)
            {
                builder.AppendLine(svod.VolumeDescriptorSize, "    Volume Descriptor Size");
                builder.AppendLine(svod.BlockCacheElementCount, "    Block Cache Element Count");
                builder.AppendLine(svod.WorkerThreadProcessor, "    Worker Thread Processor");
                builder.AppendLine(svod.WorkerThreadPriority, "    Worker Thread Priority");
                builder.AppendLine(svod.Hash, "    Hash");
                builder.AppendLine((uint)svod.DataBlockCount, "    Data Block Count");
                builder.AppendLine((uint)svod.DataBlockOffset, "    Data Block Offset");
                builder.AppendLine(svod.Padding, "    Padding");
            }
            else
            {
                builder.AppendLine("    Unknown Volume Descriptor Type");
            }

            builder.AppendLine();
        }

        protected static void Print(StringBuilder builder, InstallerHeader installerHeader)
        {
            builder.AppendLine("  Installer Information");
            builder.AppendLine("  -------------------------");

            builder.AppendLine(installerHeader.InstallerType, "    Installer Type");
            builder.AppendLine(Encoding.UTF8.GetString(installerHeader.InstallerType), "    Installer Type (Parsed)");

            if (installerHeader is InstallerUpdateHeader updateHeader)
            {
                builder.AppendLine(updateHeader.InstallerBaseVersion, "    Installer Base Version");

                uint bvMajor = updateHeader.InstallerBaseVersion >> 28; // Top 4 bits
                uint bvMinor = (updateHeader.InstallerBaseVersion >> 24) & 0xF; // Next top 4 bits
                uint bvBuild = (updateHeader.InstallerBaseVersion >> 8) & 0xFFFF; // Next 16 bits
                uint bvRevision = updateHeader.InstallerBaseVersion & 0xFF; // Lowest 8 bits
                builder.AppendLine($"{bvMajor}.{bvMinor}.{bvBuild}.{bvRevision}", "    Installer Base Version (Parsed)");

                builder.AppendLine(updateHeader.InstallerVersion, "    Installer Version");

                uint vMajor = updateHeader.InstallerVersion >> 28; // Top 4 bits
                uint vMinor = (updateHeader.InstallerVersion >> 24) & 0xF; // Next top 4 bits
                uint vBuild = (updateHeader.InstallerVersion >> 8) & 0xFFFF; // Next 8 bits
                uint vRevision = updateHeader.InstallerVersion & 0xFF; // Lowest 8 bits
                builder.AppendLine($"{vMajor}.{vMinor}.{vBuild}.{vRevision}", "    Installer Version (Parsed)");
            }
            else if (installerHeader is InstallerCacheHeader cacheHeader)
            {
                builder.AppendLine(cacheHeader.ResumeState, "    Resume State"); // See Enums.ResumeState
                builder.AppendLine(cacheHeader.CurrentFileIndex, "    Current File Index");
                builder.AppendLine(cacheHeader.BytesProcessed, "    Bytes Processed");
                builder.AppendLine(cacheHeader.LastModifiedDateTime, "    Last Modified Date Time");

                DateTime datetime = DateTime.FromFileTime(cacheHeader.LastModifiedDateTime);
                builder.AppendLine(datetime.ToString("yyyy-MM-dd HH:mm:ss"), "    Last Modified Date Time (Parsed)");

                builder.AppendLine(cacheHeader.ResumeData, "    Resume Data");
            }

            builder.AppendLine();
        }
    }
}
