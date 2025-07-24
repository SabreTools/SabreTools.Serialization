using System.Text;
using SabreTools.Models.AACS;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class AACSMediaKeyBlock : IPrinter<MediaKeyBlock>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, MediaKeyBlock model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, MediaKeyBlock mediaKeyBlock)
        {
            builder.AppendLine("AACS Media Key Block Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, mediaKeyBlock.Records);
        }

        private static void Print(StringBuilder builder, Record[]? records)
        {
            builder.AppendLine("  Records Information:");
            builder.AppendLine("  -------------------------");
            if (records == null || records.Length == 0)
            {
                builder.AppendLine("  No records");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < records.Length; i++)
            {
                var record = records[i];
                Print(builder, record, i);
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Record? record, int index)
        {
            builder.AppendLine($"  Record Entry {index}");
            if (record == null)
            {
                builder.AppendLine("    [NULL]");
                return;
            }

            builder.AppendLine($"    Record type: {record.RecordType} (0x{record.RecordType:X})");
            builder.AppendLine(record.RecordLength, "    Record length");

            switch (record)
            {
                case EndOfMediaKeyBlockRecord eomkb:
                    Print(builder, eomkb);
                    break;
                case ExplicitSubsetDifferenceRecord esd:
                    Print(builder, esd);
                    break;
                case MediaKeyDataRecord mkd:
                    Print(builder, mkd);
                    break;
                case SubsetDifferenceIndexRecord sdi:
                    Print(builder, sdi);
                    break;
                case TypeAndVersionRecord tav:
                    Print(builder, tav);
                    break;
                case DriveRevocationListRecord drl:
                    Print(builder, drl);
                    break;
                case HostRevocationListRecord hrl:
                    Print(builder, hrl);
                    break;
                case VerifyMediaKeyRecord vmk:
                    Print(builder, vmk);
                    break;
                case CopyrightRecord c:
                    Print(builder, c);
                    break;
            }
        }

        private static void Print(StringBuilder builder, EndOfMediaKeyBlockRecord record)
        {
            builder.AppendLine(record.SignatureData, "    Signature data");
        }

        private static void Print(StringBuilder builder, ExplicitSubsetDifferenceRecord record)
        {
            builder.AppendLine("    Subset Differences:");
            builder.AppendLine("    -------------------------");
            if (record?.SubsetDifferences == null || record.SubsetDifferences.Length == 0)
            {
                builder.AppendLine("    No subset differences");
                return;
            }

            for (int i = 0; i < record.SubsetDifferences.Length; i++)
            {
                var sd = record.SubsetDifferences[i];

                builder.AppendLine($"    Subset Difference {i}");
                builder.AppendLine(sd.Mask, "      Mask");
                builder.AppendLine(sd.Number, "      Number");
            }
        }

        private static void Print(StringBuilder builder, MediaKeyDataRecord record)
        {
            builder.AppendLine("    Media Keys:");
            builder.AppendLine("    -------------------------");
            if (record?.MediaKeyData == null || record.MediaKeyData.Length == 0)
            {
                builder.AppendLine("    No media keys");
                return;
            }

            for (int i = 0; i < record.MediaKeyData.Length; i++)
            {
                var mk = record.MediaKeyData[i];
                builder.AppendLine(mk, $"      Media key {i}");
            }
        }

        private static void Print(StringBuilder builder, SubsetDifferenceIndexRecord record)
        {
            builder.AppendLine($"    Span: {record.Span} (0x{record.Span:X})");
            builder.AppendLine("    Offsets:");
            builder.AppendLine("    -------------------------");
            if (record.Offsets == null || record.Offsets.Length == 0)
            {
                builder.AppendLine("    No offsets");
                return;
            }

            for (int i = 0; i < record.Offsets.Length; i++)
            {
                var offset = record.Offsets[i];
                builder.AppendLine(offset, $"      Offset {i}");
            }
        }

        private static void Print(StringBuilder builder, TypeAndVersionRecord record)
        {
            builder.AppendLine($"    Media key block type: {record.MediaKeyBlockType} (0x{record.MediaKeyBlockType:X})");
            builder.AppendLine(record.VersionNumber, "    Version number");
        }

        private static void Print(StringBuilder builder, DriveRevocationListRecord record)
        {
            builder.AppendLine(record.TotalNumberOfEntries, "    Total number of entries");
            builder.AppendLine("    Signature Blocks:");
            builder.AppendLine("    -------------------------");
            if (record.SignatureBlocks == null || record.SignatureBlocks.Length == 0)
            {
                builder.AppendLine("    No signature blocks");
                return;
            }

            for (int i = 0; i < record.SignatureBlocks.Length; i++)
            {
                var block = record.SignatureBlocks[i];

                builder.AppendLine($"    Signature Block {i}");
                builder.AppendLine(block.NumberOfEntries, "      Number of entries");
                builder.AppendLine("      Entry Fields:");
                builder.AppendLine("      -------------------------");
                if (block.EntryFields == null || block.EntryFields.Length == 0)
                {
                    builder.AppendLine("      No entry fields");
                    continue;
                }

                for (int j = 0; j < block.EntryFields.Length; j++)
                {
                    var ef = block.EntryFields[j];

                    builder.AppendLine($"      Entry {j}");
                    builder.AppendLine(ef.Range, "        Range");
                    builder.AppendLine(ef.DriveID, "        Drive ID");
                }
            }
        }

        private static void Print(StringBuilder builder, HostRevocationListRecord record)
        {
            builder.AppendLine($"    Total number of entries: {record.TotalNumberOfEntries} (0x{record.TotalNumberOfEntries:X})");
            builder.AppendLine("    Signature Blocks:");
            builder.AppendLine("    -------------------------");
            if (record.SignatureBlocks == null || record.SignatureBlocks.Length == 0)
            {
                builder.AppendLine("    No signature blocks");
                return;
            }

            for (int i = 0; i < record.SignatureBlocks.Length; i++)
            {
                var block = record.SignatureBlocks[i];

                builder.AppendLine($"    Signature Block {i}");
                builder.AppendLine(block.NumberOfEntries, "      Number of entries");
                builder.AppendLine("      Entry Fields:");
                builder.AppendLine("      -------------------------");
                if (block.EntryFields == null || block.EntryFields.Length == 0)
                {
                    builder.AppendLine("      No entry fields");
                    continue;
                }

                for (int j = 0; j < block.EntryFields.Length; j++)
                {
                    var ef = block.EntryFields[j];

                    builder.AppendLine($"      Entry {j}");
                    builder.AppendLine(ef.Range, "        Range");
                    builder.AppendLine(ef.HostID, "        Host ID");
                }
            }
        }

        private static void Print(StringBuilder builder, VerifyMediaKeyRecord record)
        {
            builder.AppendLine(record.CiphertextValue, "    Ciphertext value");
        }

        private static void Print(StringBuilder builder, CopyrightRecord record)
        {
            builder.AppendLine(record.Copyright, "    Copyright");
        }
    }
}
