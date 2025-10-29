using System;
using System.Collections.Generic;
using System.Text;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Numerics;

namespace SabreTools.Data.Printers
{
    public class ISO9660 : IPrinter<Volume>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Volume model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Volume volume)
        {
            builder.AppendLine("ISO 9660 Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            if (volume.SystemArea == null || volume.SystemArea.Length == 0)
                builder.AppendLine(volume.SystemArea, "System Area");
            else if (Array.TrueForAll(volume.SystemArea, b => b == 0))
                builder.AppendLine("Zeroed", "System Area");
            else
                builder.AppendLine("Not Zeroed", "System Area");
            builder.AppendLine();

            Print(builder, volume.VolumeDescriptorSet);
            Print(builder, volume.PathTableGroups);
            Print(builder, volume.DirectoryDescriptors);
        }

        #region Volume Descriptors

        private static void Print(StringBuilder builder, VolumeDescriptor[]? vdSet)
        {
            builder.AppendLine("  Volume Descriptors:");
            builder.AppendLine("  -------------------------");
            if (vdSet == null)
            {
                builder.AppendLine("  No volume descriptor set");
                builder.AppendLine();
                return;
            }

            foreach (var vd in vdSet)
            {
                if (vd is BootRecordVolumeDescriptor brvd)
                    Print(builder, brvd);
                else if (vd is BaseVolumeDescriptor bvd)
                    Print(builder, bvd);
                else if (vd is VolumePartitionDescriptor vpd)
                    Print(builder, vpd);
                else if (vd is VolumeDescriptorSetTerminator vdst)
                    Print(builder, vdst);
                else if (vd is GenericVolumeDescriptor gvd)
                    Print(builder, gvd);
                else
                {
                    builder.AppendLine("  Unknown Volume Descriptor");
                    builder.AppendLine();
                }
            }
        }

        private static void Print(StringBuilder builder, BootRecordVolumeDescriptor vd)
        {
            builder.AppendLine("    Boot Record Volume Descriptor:");
            builder.AppendLine("    -------------------------");

            builder.AppendLine(vd.BootSystemIdentifier, "    Boot System Identifier");
            builder.AppendLine(vd.BootSystemIdentifier, "    Boot Identifier");

            if (vd.BootSystemUse == null || vd.BootSystemUse.Length == 0)
                builder.AppendLine(vd.BootSystemUse, "    Boot System Use");
            else if (Array.TrueForAll(vd.BootSystemUse, b => b == 0))
                builder.AppendLine("Zeroed", "    Boot System Use");
            else
                builder.AppendLine("Not Zeroed", "    Boot System Use");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, BaseVolumeDescriptor vd)
        {            
            // TOOD: Determine encoding based on vd.Type, svd.EscapeSequence (and manual detection?)

            if (vd.Type == VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR)
                builder.AppendLine("    Primary Volume Descriptor:");
            else if (vd.Type == VolumeDescriptorType.SUPPLEMENTARY_VOLUME_DESCRIPTOR)
                builder.AppendLine("    Supplementary Volume Descriptor:");
            else
                builder.AppendLine("    Unidentified Base Volume Descriptor:");
            builder.AppendLine("    -------------------------");

            if (vd is PrimaryVolumeDescriptor pvd)
            {
                builder.AppendLine(pvd.UnusedByte, "    Unused Byte");
            }
            else if (vd is SupplementaryVolumeDescriptor svd)
            {
                builder.AppendLine("    Volume Flags:");
#if NET20 || NET35
                builder.AppendLine((svd.VolumeFlags & VolumeFlags.UNREGISTERED_ESCAPE_SEQUENCES) != 0, "    Unregistered Escape Sequences");
#else
                builder.AppendLine(svd.VolumeFlags.HasFlag(VolumeFlags.UNREGISTERED_ESCAPE_SEQUENCES), "    Unregistered Escape Sequences");
#endif
                if ((byte)svd.VolumeFlags > 1)
                    builder.AppendLine("Not Zeroed", "      Reserved Flags");
                else
                    builder.AppendLine("Zeroed", "      Reserved Flags");
            }

            // TODO: Decode all byte arrays into strings (based on encoding above)

            builder.AppendLine(vd.SystemIdentifier, "    System Identifier");
            builder.AppendLine(vd.VolumeIdentifier, "    Volume Identifier");

            
            if (vd.Unused8Bytes != null && Array.TrueForAll(vd.Unused8Bytes, b => b == 0))
                builder.AppendLine("Zeroed", "    Unused 8 Bytes");
            else
                builder.AppendLine(vd.Unused8Bytes, "    Unused 8 Bytes");

            builder.AppendLineBothEndian(vd.VolumeSpaceSize, "    Volume Space Size");

            if (vd is PrimaryVolumeDescriptor pvd2)
            {
                if (pvd2.Unused32Bytes != null && Array.TrueForAll(pvd2.Unused32Bytes, b => b == 0))
                    builder.AppendLine("Zeroed", "    Unused 32 Bytes");
                else
                    builder.AppendLine(pvd2.Unused32Bytes, "    Unused 32 Bytes");
            }
            if (vd is SupplementaryVolumeDescriptor svd2)
            {
                // TODO: Trim trailing 0x00 and split array into characters (multi-byte encoding detection)
                builder.AppendLine(svd2.EscapeSequences, "    Escape Sequences");
            }

            builder.AppendLineBothEndian(vd.VolumeSetSize, "    Volume Set Size");
            builder.AppendLineBothEndian(vd.VolumeSequenceNumber, "    Volume Sequence Number");
            builder.AppendLineBothEndian(vd.LogicalBlockSize, "    Logical Block Size");
            builder.AppendLineBothEndian(vd.PathTableSize, "    Path Table Size");
            builder.AppendLine(vd.PathTableLocationL, "    Type-L Path Table Location");
            builder.AppendLine(vd.OptionalPathTableLocationL, "    Optional Type-L Path Table Location");
            builder.AppendLine(vd.PathTableLocationM, "    Type-M Path Table Location");
            builder.AppendLine(vd.OptionalPathTableLocationM, "    Optional Type-M Path Table Location");
        
            Print(builder, vd.RootDirectoryRecord);

            builder.AppendLine(vd.VolumeSetIdentifier, "    Volume Set Identifier");
            builder.AppendLine(vd.PublisherIdentifier, "    Publisher Identifier");
            builder.AppendLine(vd.DataPreparerIdentifier, "    Data Preparer Identifier");
            builder.AppendLine(vd.ApplicationIdentifier, "    Application Identifier");
            builder.AppendLine(vd.CopyrightFileIdentifier, "    Copyright Identifier");
            builder.AppendLine(vd.AbstractFileIdentifier, "    Abstract Identifier");
            builder.AppendLine(vd.BibliographicFileIdentifier, "    Bibliographic Identifier");

            builder.AppendLine("    Volume Creation Date Time:");
            Print(builder, vd.VolumeCreationDateTime);
            builder.AppendLine("    Volume Modification Date Time:");
            Print(builder, vd.VolumeModificationDateTime);
            builder.AppendLine("    Volume Expiration Date Time:");
            Print(builder, vd.VolumeExpirationDateTime);
            builder.AppendLine("    Volume Effective Date Time:");
            Print(builder, vd.VolumeEffectiveDateTime);

            builder.AppendLine(vd.FileStructureVersion, "    File Structure Version");

            builder.AppendLine(vd.ReservedByte, "    Reserved Byte");

            if (vd.ApplicationUse == null || vd.ApplicationUse.Length == 0)
                builder.AppendLine(vd.ApplicationUse, "    Application Use");
            else if (Array.TrueForAll(vd.ApplicationUse, b => b == 0))
                builder.AppendLine("Zeroed", "    Application Use");
            else
                builder.AppendLine("Not Zeroed", "    Application Use");

            if (vd.Reserved653Bytes == null || vd.Reserved653Bytes.Length == 0)
                builder.AppendLine(vd.Reserved653Bytes, "    Reserved 653 Bytes");
            else if (Array.TrueForAll(vd.Reserved653Bytes, b => b == 0))
                builder.AppendLine("Zeroed", "    Reserved 653 Bytes");
            else
                builder.AppendLine("Not Zeroed", "    Reserved 653 Bytes");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, VolumePartitionDescriptor vd)
        {
            builder.AppendLine("    Volume Partition Descriptor:");
            builder.AppendLine("    -------------------------");

            builder.AppendLine(vd.SystemIdentifier, "    System Identifier");
            builder.AppendLine(vd.VolumePartitionIdentifier, "    Volume Partition Identifier");
            builder.AppendLineBothEndian(vd.VolumePartitionLocation, "    Volume Partition Location");
            builder.AppendLineBothEndian(vd.VolumePartitionSize, "    Volume Partition Size");

            if (vd.SystemUse == null || vd.SystemUse.Length == 0)
                builder.AppendLine(vd.SystemUse, "    System Use");
            else if (Array.TrueForAll(vd.SystemUse, b => b == 0))
                builder.AppendLine("Zeroed", "    System Use");
            else
                builder.AppendLine("Not Zeroed", "    System Use");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, VolumeDescriptorSetTerminator vd)
        {
            builder.AppendLine("    Volume Descriptor Set Terminator:");
            builder.AppendLine("    -------------------------");

            if (vd.Reserved2041Bytes == null || vd.Reserved2041Bytes.Length == 0)
                builder.AppendLine(vd.Reserved2041Bytes, "    Reserved Bytes");
            else if (Array.TrueForAll(vd.Reserved2041Bytes, b => b == 0))
                builder.AppendLine("Zeroed", "    Reserved Bytes");
            else
                builder.AppendLine("Not Zeroed", "    Reserved Bytes");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, GenericVolumeDescriptor vd)
        {
            builder.AppendLine("    Unidentified Volume Descriptor:");
            builder.AppendLine("    -------------------------");

            if (vd.Data == null || vd.Data.Length == 0)
                builder.AppendLine(vd.Data, "    Data");
            else if (Array.TrueForAll(vd.Data, b => b == 0))
                builder.AppendLine("Zeroed", "    Data");
            else
                builder.AppendLine("Not Zeroed", "    Data");

            builder.AppendLine();
        }

        #endregion

        #region Path Tables

        private static void Print(StringBuilder builder, PathTableGroup[]? ptgs)
        {
            builder.AppendLine("  Path Table Group(s):");
            builder.AppendLine("  -------------------------");
            if (ptgs == null)
            {
                builder.AppendLine("  No path table groups");
                builder.AppendLine();
                return;
            }

            for (int tableNum = 0; tableNum < ptgs.Length; tableNum++)
            {
                if (ptgs[tableNum].PathTableL != null)
                {
                    builder.AppendLine($"    Type-L Path Table {tableNum}:");
                    builder.AppendLine("    -------------------------");
                    Print(builder, ptgs[tableNum].PathTableL);
                }
                else
                {
                    builder.AppendLine($"    No Type-L Path Table {tableNum}:");
                    builder.AppendLine();
                }
                if (ptgs[tableNum].OptionalPathTableL != null)
                {
                    builder.AppendLine($"    Optional Type-L Path Table {tableNum}:");
                    builder.AppendLine("    -------------------------");
                    Print(builder, ptgs[tableNum].OptionalPathTableL);
                }
                else
                {
                    builder.AppendLine($"    No Optional Type-L Path Table {tableNum}:");
                    builder.AppendLine();
                }
                if (ptgs[tableNum].PathTableM != null)
                {
                    builder.AppendLine($"    Type-M Path Table {tableNum}:");
                    builder.AppendLine("    -------------------------");
                    Print(builder, ptgs[tableNum].PathTableM);
                }
                else
                {
                    builder.AppendLine($"    No Type-M Path Table {tableNum}:");
                    builder.AppendLine();
                }
                if (ptgs[tableNum].OptionalPathTableM != null)
                {
                    builder.AppendLine($"    Optional Type-M Path Table {tableNum}:");
                    builder.AppendLine("    -------------------------");
                    Print(builder, ptgs[tableNum].OptionalPathTableM);
                }
                else
                {
                    builder.AppendLine($"    No Optional Type-M Path Table {tableNum}:");
                    builder.AppendLine();
                }
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, PathTableRecord[] records)
        {
            if (records.Length == 0)
            {
                builder.AppendLine("    No records");
                builder.AppendLine();
                return;
            }

            for (int recordNum = 0; recordNum < records.Length; recordNum++)
            {
                builder.AppendLine($"    Path Table Record {recordNum}");
                builder.AppendLine(records[recordNum].DirectoryIdentifierLength, "      Directory Identifier Length");
                builder.AppendLine(records[recordNum].ExtendedAttributeRecordLength, "      Extended Attribute Record Length");
                builder.AppendLine(records[recordNum].ExtentLocation, "      Extent Location");
                builder.AppendLine(records[recordNum].DirectoryIdentifier, "      Directory Identifier");
                if (records[recordNum].PaddingField != null)
                    builder.AppendLine(records[recordNum].PaddingField, "      Padding Field");
            }

            builder.AppendLine();
        }

        #endregion

        #region Directories

        private static void Print(StringBuilder builder, Dictionary<int, DirectoryExtent>? dirs)
        {
            builder.AppendLine("  Directory Descriptors Information:");
            builder.AppendLine("  -------------------------");
            if (dirs == null)
            {
                builder.AppendLine("  No directory descriptors");
                builder.AppendLine();
                return;
            }

            foreach(var kvp in dirs)
            {
                builder.AppendLine($"    Directory at Sector {kvp.Key}");
                builder.AppendLine("    -------------------------");
                Print(builder, kvp.Value);
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryExtent? dir)
        {
            if (dir == null)
            {
                builder.AppendLine("    No directory descriptor");
                builder.AppendLine();
                return;
            }
            if (dir.DirectoryRecords == null)
            {
                builder.AppendLine("    No directory records");
                builder.AppendLine();
                return;
            }

            for (int recordNum = 0; recordNum < records.Length; recordNum++)
            {
                builder.AppendLine($"      Directory Record {recordNum}:");
                builder.AppendLine("      -------------------------");
                Print(builder, dir.DirectoryRecords[recordNum]);
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryRecord? dr)
        {
            if (dr == null)
            {
                builder.AppendLine("      No directory record");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(dr.DirectoryRecordLength, "      Directory Record Length");
            builder.AppendLine(dr.ExtendedAttributeRecordLength, "      Extended Attribute Record Length");

            builder.AppendLineBothEndian(dr.ExtentLocation, "      Extent Location");
            builder.AppendLineBothEndian(dr.ExtentLength, "      Extent Length");

            Print(builder, dr.RecordingDateTime);
            
            builder.AppendLine("      File Flags:");
            builder.AppendLine((dr.FileFlags & FileFlags.EXISTENCE) == FileFlags.EXISTENCE, "        Existence");
            builder.AppendLine((dr.FileFlags & FileFlags.DIRECTORY) == FileFlags.DIRECTORY, "        Directory");
            builder.AppendLine((dr.FileFlags & FileFlags.ASSOCIATED_FILE) == FileFlags.ASSOCIATED_FILE, "        Associated File");
            builder.AppendLine((dr.FileFlags & FileFlags.RECORD) == FileFlags.RECORD, "        Record");
            builder.AppendLine((dr.FileFlags & FileFlags.PROTECTION) == FileFlags.PROTECTION, "        Protection");
            builder.AppendLine((dr.FileFlags & FileFlags.RESERVED_BIT5) == FileFlags.RESERVED_BIT5, "        Reserved Flag (Bit 5)");
            builder.AppendLine((dr.FileFlags & FileFlags.RESERVED_BIT6) == FileFlags.RESERVED_BIT6, "        Reserved Flag (Bit 6)");
            builder.AppendLine((dr.FileFlags & FileFlags.MULTI_EXTENT) == FileFlags.MULTI_EXTENT, "        Multi-Extent");

            builder.AppendLine(dr.FileUnitSize, "      File Unit Size");
            builder.AppendLine(dr.InterleaveGapSize, "      Interleave Gap Size");

            builder.AppendLineBothEndian(dr.VolumeSequenceNumber, "      Volume Sequence Number");

            builder.AppendLine(dr.FileIdentifierLength, "      File Identifier Length");
            builder.AppendLine(dr.FileIdentifier, "      File Identifier");
            builder.AppendLine(dr.PaddingField, "      Padding Field");
            
            if (dr.SystemUse == null || dr.SystemUse.Length == 0)
                builder.AppendLine(dr.SystemUse, "      System Use");
            else if (Array.TrueForAll(dr.SystemUse, b => b == 0))
                builder.AppendLine($"Zeroed ({dr.SystemUse.Length} bytes)", "      System Use");
            else
                builder.AppendLine($"Not Zeroed ({dr.SystemUse.Length} bytes)", "      System Use");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryRecordDateTime? drdt)
        {
            if (drdt == null)
            {
                builder.AppendLine("[NULL]", "      Directory Record Date Time");
                return;
            }
            builder.AppendLine("      Directory Record Date Time:");

            builder.AppendLine(drdt.YearsSince1990, "        Years Since 1900");
            builder.AppendLine(drdt.Month, "        Month");
            builder.AppendLine(drdt.Day, "        Day");
            builder.AppendLine(drdt.Hour, "        Hour");
            builder.AppendLine(drdt.Minute, "        Minute");
            builder.AppendLine(drdt.Second, "        Second");
            string tz = $"{((drdt.TimezoneOffset-48)*15/60):+0;-0}:{((drdt.TimezoneOffset-48)*15%60+60)%60:00} (0x{drdt.TimezoneOffset.ToString("X2")})";
            builder.AppendLine(tz, "        Timezone Offset");
        }

        #endregion

        private static void Print(StringBuilder builder, DecDateTime? dt)
        {
            if (dt == null)
            {
                builder.AppendLine("      [NULL]");
                return;
            }
            
            if (IsDigits(dt.Year))
                builder.AppendLine(Encoding.ASCII.GetString(dt.Year), "      Year");
            else
                builder.AppendLine(dt.Year, "      Year");
            if (IsDigits(dt.Month))
                builder.AppendLine(Encoding.ASCII.GetString(dt.Month), "      Month");
            else
                builder.AppendLine(dt.Month, "      Month");
            if (IsDigits(dt.Day))
                builder.AppendLine(Encoding.ASCII.GetString(dt.Day), "      Day");
            else
                builder.AppendLine(dt.Day, "      Day");
            if (IsDigits(dt.Hour))
                builder.AppendLine(Encoding.ASCII.GetString(dt.Hour), "      Hour");
            else
                builder.AppendLine(dt.Hour, "      Hour");
            if (IsDigits(dt.Minute))
                builder.AppendLine(Encoding.ASCII.GetString(dt.Minute), "      Minute");
            else
                builder.AppendLine(dt.Minute, "      Minute");
            if (IsDigits(dt.Second))
                builder.AppendLine(Encoding.ASCII.GetString(dt.Second), "      Second");
            else
                builder.AppendLine(dt.Second, "      Second");
            if (IsDigits(dt.Centisecond))
                builder.AppendLine(Encoding.ASCII.GetString(dt.Centisecond), "      Centisecond");
            else
                builder.AppendLine(dt.Centisecond, "      Centisecond");
            string tz = $"{((dt.TimezoneOffset-48)*15/60):+0;-0}:{((dt.TimezoneOffset-48)*15%60+60)%60:00} (0x{dt.TimezoneOffset.ToString("X2")})";
            builder.AppendLine(tz, "      Timezone Offset");
        }

        private static bool IsDigits(byte[] arr)
        {
            foreach (byte b in arr)
            {
                if (b < 48 || b > 57)
                    return false;
            }
            return true;
        }
    }
}
