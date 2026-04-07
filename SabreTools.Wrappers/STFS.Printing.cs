using System;
using System.Text;
using SabreTools.Data.Models.STFS;
using SabreTools.IO.Numerics;
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

            Print(builder, header.Signature)

            for (var le in header.LicensingData)
            {
                Print(builder, le)
            }

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

            Print(builder, header.VolumeDescriptor)

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

            // TODO: Print "Zeroed" if padding is all zeroes
            builder.AppendLine(header.Padding, "  Padding");
            builder.AppendLine(header.DeviceID, "  Device ID");
            
            // TODO: print as 18 different strings, 128 bytes each
            builder.AppendLine(header.DisplayName, "  Display Name");
            
            // TODO: print as 18 different strings, 128 bytes each
            builder.AppendLine(header.DisplayDescription, "  Display Description");

            builder.AppendLine(header.PublisherName, "  Publisher Name");
            builder.AppendLine(header.TitleName, "  Title Name");
            builder.AppendLine(header.TransferFlags, "  Transfer Flags"); // See Enums.TransferFlags
            builder.AppendLine(header.ThumbnailImageSize, "  Thumbnail Image Size");
            builder.AppendLine(header.TitleThumbnailImageSize, "  Title Thumbnail Image Size");
            
            if (header.AdditionalDisplayNames is not null)
            {
                // TODO: print as 18 different strings, 128 bytes each
                builder.AppendLine(header.AdditionalDisplayNames, "  Additional Display Names");
            }
            if (header.AdditionalDisplayDescriptions is not null)
            {
                // TODO: print as 18 different strings, 128 bytes each
                builder.AppendLine(header.AdditionalDisplayDescriptions, "  Additional Display Descriptions");                
            }

            builder.AppendLine();
        }

        protected static void Print(StringBuilder builder, Signature signature)
        {
            if (signature is MicrosoftSignature ms)
            {
                builder.AppendLine(header.PackageSignature, "  Package Signature");
                // TODO: Print "Zeroed" if padding is all zeroes
                builder.AppendLine(header.Padding, "  Padding");
            }
            else if (signature is ConsoleSignature cs)
            {
                builder.AppendLine(header.CertificateSize, "  Certificate Size");
                builder.AppendLine(header.ConsoleID, "  Console ID");
                builder.AppendLine(header.PartNumber, "  Part Number");
                builder.AppendLine(header.ConsoleType, "  Console Type");
                builder.AppendLine(header.CertificateDate, "  Certificate Date");
                builder.AppendLine(header.PublicExponent, "  Public Exponent");
                builder.AppendLine(header.PublicModulus, "  Public Modulus");
                builder.AppendLine(header.CertificateSignature, "  Certificate Signature");
                builder.AppendLine(header.Signature, "  Signature");
            }
            else
            {
                builder.AppendLine("  Unknown Signature Type");
            }

            builder.AppendLine();
        }

        protected static void Print(StringBuilder builder, LicenseEntry le)
        {
            builder.AppendLine(le.LicenseID, "  License ID");
            builder.AppendLine(le.LicenseBits, "  License Bits");
            builder.AppendLine(le.LicenseFlags, "  License Flags");

            builder.AppendLine();
        }

        protected static void Print(StringBuilder builder, VolumeDescriptor vd)
        {
            if (vd is SFTSDescriptor stfs)
            {
                builder.AppendLine(header.VolumeDescriptorSize, "  Volume Descriptor Size");
                builder.AppendLine(header.Reserved, "  Reserved");
                builder.AppendLine(header.BlockSeparation, "  Block Separation");
                builder.AppendLine(header.FileTableBlockCount, "  File Table Block Count");
                builder.AppendLine(header.FileTableBlockNumber, "File Table Block Number");
                builder.AppendLine(header.TopHashTableHash, "  Top Hash Table Hash");
                builder.AppendLine(header.TotalAllocatedBlockCount, "  Total Allocated Block Count");
                builder.AppendLine(header.TotalUnallocatedBlockCount, "  Total Unallocated Block Count");
            }
            else if (vd is SVODDescriptor svod)
            {
                builder.AppendLine(header.VolumeDescriptorSize, "  Volume Descriptor Size");
                builder.AppendLine(header.BlockCacheElementCount, "  Block Cache Element Count");
                builder.AppendLine(header.WorkerThreadProcessor, "  Worker Thread Processor");
                builder.AppendLine(header.WorkerThreadPriority, "  Worker Thread Priority");
                builder.AppendLine(header.Hash, "  Hash");
                builder.AppendLine(header.DataBlockCount, "  Data Block Count");
                builder.AppendLine(header.DataBlockOffset, "  Data Block Offset");
                builder.AppendLine(header.Padding, "  Padding");
            }
            else
            {
                builder.AppendLine("  Unknown Volume Descriptor Type");
            }

            builder.AppendLine();
        }
    }
}
