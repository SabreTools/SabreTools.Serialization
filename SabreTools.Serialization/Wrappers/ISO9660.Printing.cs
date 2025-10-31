using System;
using System.Collections.Generic;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.ISO9660;

namespace SabreTools.Serialization.Wrappers
{
    public partial class ISO9660 : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("ISO 9660 Information:");
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

        #region Volume Descriptors

        protected static void Print(StringBuilder builder, VolumeDescriptor[]? vdSet)
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

            // Default to UTF-8 deocding (note: Spec says PVD uses subset of ASCII, but UTF-8 is used here as it is a strict superset)
            Encoding encoding = Encoding.UTF8;
            if (vd is PrimaryVolumeDescriptor pvd)
            {
                builder.AppendLine(pvd.UnusedByte, "    Unused Byte");
            }
            else if (vd is SupplementaryVolumeDescriptor svd)
            {
                // Decode strings using UTF-16 BigEndian (note: Spec says SVD uses UCS-2, but UTF-16 is used here as it is a strict superset)
                encoding = Encoding.BigEndianUnicode;
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

            // TODO: Better string decoding (based on spec, EscapeSequences, and detection)

            builder.AppendLine(encoding.GetString(vd.SystemIdentifier), "    System Identifier");
            builder.AppendLine(encoding.GetString(vd.VolumeIdentifier), "    Volume Identifier");


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

            builder.AppendLine("    Root Directory Record:");
            Print(builder, vd.RootDirectoryRecord, encoding);

            builder.AppendLine(encoding.GetString(vd.VolumeSetIdentifier), "    Volume Set Identifier");
            builder.AppendLine(encoding.GetString(vd.PublisherIdentifier), "    Publisher Identifier");
            builder.AppendLine(encoding.GetString(vd.DataPreparerIdentifier), "    Data Preparer Identifier");
            builder.AppendLine(encoding.GetString(vd.ApplicationIdentifier), "    Application Identifier");
            builder.AppendLine(encoding.GetString(vd.CopyrightFileIdentifier), "    Copyright Identifier");
            builder.AppendLine(encoding.GetString(vd.AbstractFileIdentifier), "    Abstract Identifier");
            builder.AppendLine(encoding.GetString(vd.BibliographicFileIdentifier), "    Bibliographic Identifier");

            builder.AppendLine(Format(vd.VolumeCreationDateTime), "    Volume Creation Date Time:");
            builder.AppendLine(Format(vd.VolumeModificationDateTime), "    Volume Modification Date Time:");
            builder.AppendLine(Format(vd.VolumeExpirationDateTime), "    Volume Expiration Date Time:");
            builder.AppendLine(Format(vd.VolumeEffectiveDateTime), "    Volume Effective Date Time:");

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

        protected static void Print(StringBuilder builder, PathTableGroup[]? ptgs, Encoding encoding)
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
                    Print(builder, ptgs[tableNum].PathTableL, encoding);
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
                    Print(builder, ptgs[tableNum].OptionalPathTableL, encoding);
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
                    Print(builder, ptgs[tableNum].PathTableM, encoding);
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
                    Print(builder, ptgs[tableNum].OptionalPathTableM, encoding);
                }
                else
                {
                    builder.AppendLine($"    No Optional Type-M Path Table {tableNum}:");
                    builder.AppendLine();
                }
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, PathTableRecord[] records, Encoding encoding)
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

        protected static void Print(StringBuilder builder, Dictionary<int, FileExtent>? dirs, Encoding encoding)
        {
            builder.AppendLine("  Directory Descriptors Information:");
            builder.AppendLine("  -------------------------");
            if (dirs == null)
            {
                builder.AppendLine("  No directory descriptors");
                builder.AppendLine();
                return;
            }

            foreach (var kvp in dirs)
            {
                builder.AppendLine($"    Directory at Sector {kvp.Key}");
                builder.AppendLine("    -------------------------");
                Print(builder, kvp.Value, encoding);
            }
        }

        private static void Print(StringBuilder builder, FileExtent? extent, Encoding encoding)
        {
            if (extent == null)
            {
                builder.AppendLine("    No directory descriptor");
                builder.AppendLine();
                return;
            }

            if (extent is DirectoryExtent dir)
            {
                if (dir.DirectoryRecords == null)
                {
                    builder.AppendLine("    No directory records");
                    builder.AppendLine();
                    return;
                }

                // File extent is a directory, print all directory records
                for (int recordNum = 0; recordNum < dir.DirectoryRecords.Length; recordNum++)
                {
                    builder.AppendLine($"      Directory Record {recordNum}:");
                    builder.AppendLine("      -------------------------");
                    Print(builder, dir.DirectoryRecords[recordNum], encoding);
                    builder.AppendLine();
                }
            }
            else
            {
                // File extent is a file, print the file's Extended Attribute Record
                Print(builder, extent.ExtendedAttributeRecord, encoding);
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryRecord? dr, Encoding encoding)
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
            string tz = $"{((drdt.TimezoneOffset - 48) * 15 / 60):+0;-0}:{((drdt.TimezoneOffset - 48) * 15 % 60 + 60) % 60:00} (0x{drdt.TimezoneOffset.ToString("X2")})";
            builder.AppendLine(tz, "        Timezone Offset");
        }

        private static void Print(StringBuilder builder, ExtendedAttributeRecord? ear, Encoding encoding)
        {
            builder.AppendLine("      File Extent");
            if (ear == null)
                return;
            
            builder.AppendLineBothEndian(ear.OwnerIdentification, "        Owner Identification");
            builder.AppendLineBothEndian(ear.OwnerIdentification, "        Group Identification");

            builder.AppendLine("      Permissions:");
            builder.AppendLine((ear.Permissions & Permissions.SYSTEM_USER_CANNOT_READ) != 0, "        System User Cannot Read");
            builder.AppendLine((ear.Permissions & Permissions.SYSTEM_USER_CANNOT_EXECUTE) != 0, "        System User Cannot Execute");
            builder.AppendLine((ear.Permissions & Permissions.OWNER_CANNOT_READ) != 0, "        System User Cannot Execute");
            builder.AppendLine((ear.Permissions & Permissions.OWNER_CANNOT_EXECUTE) != 0, "        System User Cannot Execute");
            builder.AppendLine((ear.Permissions & Permissions.GROUP_MEMBER_CANNOT_READ) != 0, "        System User Cannot Execute");
            builder.AppendLine((ear.Permissions & Permissions.GROUP_MEMBER_CANNOT_EXECUTE) != 0, "        System User Cannot Execute");
            builder.AppendLine((ear.Permissions & Permissions.NON_GROUP_MEMBER_CANNOT_READ) != 0, "        System User Cannot Execute");
            builder.AppendLine((ear.Permissions & Permissions.NON_GROUP_MEMBER_CANNOT_EXECUTE) != 0, "        System User Cannot Execute");
            if ((ear.Permissions & Permissions.PERMISSIONS_MASK) == Permissions.PERMISSIONS_MASK)
                builder.AppendLine("        Fixed Bits: All Set");
            else
                builder.AppendLine("        Fixed Bits: Not All Set");


            builder.AppendLine(Format(ear.FileCreationDateTime), "        File Creation Date Time");
            builder.AppendLine(Format(ear.FileModificationDateTime), "        File Modification Date Time");
            builder.AppendLine(Format(ear.FileExpirationDateTime), "        File Expiration Date Time");
            builder.AppendLine(Format(ear.FileEffectiveDateTime), "        File Effective Date Time");

            builder.AppendLine((byte)ear.RecordFormat, "      Record Format:");
            builder.AppendLine((byte)ear.RecordAttributes, "        Record Attributes");
            builder.AppendLineBothEndian(ear.RecordLength, "        Record Length");
            builder.AppendLine(encoding.GetString(ear.SystemIdentifier), "        System Identifier");
            builder.AppendLine(ear.SystemUse, "        System Use");
            builder.AppendLine(ear.ExtendedAttributeRecordVersion, "        Extended Attribute Record Version");
            builder.AppendLine(ear.EscapeSequencesLength, "        Escape Sequences Length");
            builder.AppendLine(ear.Reserved64Bytes, "        Reserved 64 Bytes");
            builder.AppendLineBothEndian(ear.ApplicationLength, "        Application Length");
            builder.AppendLine(ear.ApplicationUse, "        Application Use");
            builder.AppendLine(ear.EscapeSequences, "        Escape Sequences");
        }

        #endregion

        private static string? Format(DecDateTime? dt)
        {
            if (dt == null)
                return null;

            string year = dt.Year.IsNumericArray()
                ? Encoding.ASCII.GetString(dt.Year)
                : BitConverter.ToString(dt.Year).Replace('-', ' ');
            string month = dt.Month.IsNumericArray()
                ? Encoding.ASCII.GetString(dt.Month)
                : BitConverter.ToString(dt.Month).Replace('-', ' ');
            string day = dt.Day.IsNumericArray()
                ? Encoding.ASCII.GetString(dt.Day)
                : BitConverter.ToString(dt.Day).Replace('-', ' ');

            string hour = dt.Hour.IsNumericArray()
                ? Encoding.ASCII.GetString(dt.Hour)
                : BitConverter.ToString(dt.Hour).Replace('-', ' ');
            string minute = dt.Minute.IsNumericArray()
                ? Encoding.ASCII.GetString(dt.Minute)
                : BitConverter.ToString(dt.Minute).Replace('-', ' ');
            string second = dt.Second.IsNumericArray()
                ? Encoding.ASCII.GetString(dt.Second)
                : BitConverter.ToString(dt.Second).Replace('-', ' ');
            string csecond = dt.Centisecond.IsNumericArray()
                ? Encoding.ASCII.GetString(dt.Centisecond)
                : BitConverter.ToString(dt.Centisecond).Replace('-', ' ');

            string tz = $"{((dt.TimezoneOffset - 48) * 15 / 60):+0;-0}:{((dt.TimezoneOffset - 48) * 15 % 60 + 60) % 60:00} (0x{dt.TimezoneOffset:X2})";

            return $"{year}-{month}-{day} {hour}:{minute}:{second}.{csecond} [{tz}]";
        }
    }
}
