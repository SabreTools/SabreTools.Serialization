using System;
using System.Text;
using SabreTools.Data.Models.ISO9660;

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

            // TODO: Better check
            if (IsAllZero(volume.SystemArea))
                builder.AppendLine("Zeroed", "  System Area");
            else
                builder.AppendLine("Not Zeroed", "  System Area");
            builder.AppendLine();

            Print(builder, volume.VolumeDescriptorSet);
            Print(builder, volume.PathTableGroups);
            Print(builder, volume.RootDirectoryDescriptors);
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
                Console.WriteLine("0");
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
            }
        }

        private static void Print(StringBuilder builder, BootRecordVolumeDescriptor vd)
        {
            Console.WriteLine("1");
            builder.AppendLine("    Boot Record Volume Descriptor:");
            builder.AppendLine("    -------------------------");

            builder.AppendLine(vd.BootSystemIdentifier, "    Boot System Identifier");
            builder.AppendLine(vd.BootSystemIdentifier, "    Boot Identifier");

            if (vd.BootSystemUse == null || vd.BootSystemUse.Length == 0)
                builder.AppendLine(vd.BootSystemUse, "    Boot System Use");
            else if (IsAllZero(vd.BootSystemUse))
                builder.AppendLine("Zeroed", "    Boot System Use");
            else
                builder.AppendLine("Not Zeroed", "    Boot System Use");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, BaseVolumeDescriptor vd)
        {
            Console.WriteLine("2");
            var type = (byte?)vd.Type;
            
            // TOOD: Determine encoding based on vd.Type, svd.EscapeSequence (and manual detection?)

            if (type == 0x01)
                builder.AppendLine("    Primary Volume Descriptor:");
            else if (type == 0x02)
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
                if (svd.VolumeFlags == null)
                {
                    builder.AppendLine("      Null");
                }
                else
                {
                    Console.WriteLine("a");
                    builder.AppendLine((svd.VolumeFlags & VolumeFlags.UNREGISTERED_ESCAPE_SEQUENCES) == VolumeFlags.UNREGISTERED_ESCAPE_SEQUENCES, "      Unregistered Escape Sequences");
                    if ((byte)svd.VolumeFlags > 1)
                        builder.AppendLine("Not Zeroed", "      Reserved Flags");
                    else
                        builder.AppendLine("Zeroed", "      Reserved Flags");
                }
                    Console.WriteLine("b");
            }

            // TODO: Decode all byte arrays into strings (based on encoding above)

            builder.AppendLine(vd.SystemIdentifier, "    System Identifier");
            builder.AppendLine(vd.VolumeIdentifier, "    Volume Identifier");

            Console.WriteLine("c");
            
            if (IsAllZero(vd.Unused8Bytes))
                builder.AppendLine("Zeroed", "  Unused 8 Bytes");
            else
                builder.AppendLine(vd.Unused8Bytes, "  Unused 8 Bytes");

            // TODO: Check that MSB/LSB match
            builder.AppendLine(vd.VolumeSpaceSize?.LSB, "    Volume Space Size");

            if (vd is PrimaryVolumeDescriptor pvd2)
            {
                if (IsAllZero(pvd2.Unused32Bytes))
                    builder.AppendLine("Zeroed", "  Unused 32 Bytes");
                else
                    builder.AppendLine(pvd2.Unused32Bytes, "  Unused 32 Bytes");
            }
            if (vd is SupplementaryVolumeDescriptor svd2)
            {
                // TODO: Trim trailing 0x00 and split array into characters (multi-byte encoding detection)
                builder.AppendLine(svd2.EscapeSequences, "    Escape Sequences");
            }

            // TODO: Check that MSB/LSB match
            builder.AppendLine(vd.VolumeSetSize?.LSB, "    Volume Set Size");
            // TODO: Check that MSB/LSB match
            builder.AppendLine(vd.VolumeSequenceNumber?.LSB, "    Volume Sequence Number");
            // TODO: Check that MSB/LSB match
            builder.AppendLine(vd.LogicalBlockSize?.LSB, "    Logical Block Size");
            // TODO: Check that MSB/LSB match
            builder.AppendLine(vd.PathTableSize?.LSB, "    Path Table Size");
            builder.AppendLine(vd.PathTableLocationL, "    Type-L Path Table Location");
            builder.AppendLine(vd.OptionalPathTableLocationL, "    Optional Type-L Path Table Location");
            builder.AppendLine(vd.PathTableLocationM, "    Type-M Path Table Location");
            builder.AppendLine(vd.OptionalPathTableLocationM, "    Optional Type-M Path Table Location");
        
            Console.WriteLine("d");
            Print(builder, vd.RootDirectoryRecord);

            builder.AppendLine(vd.VolumeSetIdentifier, "    Volume Set Identifier");
            builder.AppendLine(vd.PublisherIdentifier, "    Publisher Identifier");
            builder.AppendLine(vd.DataPreparerIdentifier, "    Data Preparer Identifier");
            builder.AppendLine(vd.ApplicationIdentifier, "    Application Identifier");
            builder.AppendLine(vd.CopyrightFileIdentifier, "    Copyright Identifier");
            builder.AppendLine(vd.AbstractFileIdentifier, "    Abstract Identifier");
            builder.AppendLine(vd.BibliographicFileIdentifier, "    Bibliographic Identifier");
            
            builder.AppendLine("    Volume Creation Date Time");
            Print(builder, vd.VolumeCreationDateTime);
            builder.AppendLine("    Volume Modification Date Time");
            Print(builder, vd.VolumeModificationDateTime);
            builder.AppendLine("    Volume Expiration Date Time");
            Print(builder, vd.VolumeExpirationDateTime);
            builder.AppendLine("    Volume Effective Date Time");
            Print(builder, vd.VolumeEffectiveDateTime);

            builder.AppendLine(vd.FileStructureVersion, "    File Structure Version");

            builder.AppendLine(vd.ReservedByte, "    Reserved Byte");

            if (IsAllZero(vd.ApplicationUse))
                builder.AppendLine("Zeroed", "  Application Use");
            else
                builder.AppendLine("Not Zeroed", "  Application Use");

            if (IsAllZero(vd.Reserved653Bytes))
                builder.AppendLine("Zeroed", "  Reserved 653 Bytes");
            else
                builder.AppendLine("Not Zeroed", "  Reserved 653 Bytes");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, VolumePartitionDescriptor vd)
        {
            Console.WriteLine("3");
            builder.AppendLine("    Volume Partition Descriptor:");
            builder.AppendLine("    -------------------------");

            builder.AppendLine(vd.SystemIdentifier, "    System Identifier");
            builder.AppendLine(vd.VolumePartitionIdentifier, "    Volume Partition Identifier");
            // TODO: Check that MSB/LSB match
            builder.AppendLine(vd.VolumePartitionLocation?.LSB, "    Volume Partition Location");
            // TODO: Check that MSB/LSB match
            builder.AppendLine(vd.VolumePartitionSize?.LSB, "    Volume Partition Size");

            if (IsAllZero(vd.SystemUse))
                builder.AppendLine("Zeroed", "  System Use");
            else
                builder.AppendLine("Not Zeroed", "  System Use");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, VolumeDescriptorSetTerminator vd)
        {
            Console.WriteLine("4");
            builder.AppendLine("    Volume Descriptor Set Terminator:");
            builder.AppendLine("    -------------------------");

            if (IsAllZero(vd.Reserved2041Bytes))
                builder.AppendLine("Zeroed", "  Reserved Bytes");
            else
                builder.AppendLine("Not Zeroed", "  Reserved Bytes");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, GenericVolumeDescriptor vd)
        {
            builder.AppendLine("    Unidentified Volume Descriptor:");
            builder.AppendLine("    -------------------------");

            if (IsAllZero(vd.Data))
                builder.AppendLine("Zeroed", "  Data");
            else
                builder.AppendLine("Not Zeroed", "  Data");

            builder.AppendLine();
        }

        #endregion

        #region Directories/Paths

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

            int tableNum = 0;
            foreach(var ptg in ptgs)
            {
                tableNum++;
                if (ptg.PathTableL != null)
                {
                    builder.AppendLine($"    Type-L Path Table {tableNum}");
                    Print(builder, ptg.PathTableL);
                }
                else
                {
                    builder.AppendLine($"    No Type-L Path Table {tableNum}");
                    builder.AppendLine();
                }
                if (ptg.OptionalPathTableL != null)
                {
                    builder.AppendLine($"    Optional Type-L Path Table {tableNum}");
                    Print(builder, ptg.OptionalPathTableL);
                }
                else
                {
                    builder.AppendLine($"    No Optional Type-L Path Table {tableNum}");
                    builder.AppendLine();
                }
                if (ptg.PathTableM != null)
                {
                    builder.AppendLine($"    Type-M Path Table {tableNum}");
                    Print(builder, ptg.PathTableM);
                }
                else
                {
                    builder.AppendLine($"    No Type-M Path Table {tableNum}");
                    builder.AppendLine();
                }
                if (ptg.OptionalPathTableM != null)
                {
                    builder.AppendLine($"    Optional Type-M Path Table {tableNum}");
                    Print(builder, ptg.OptionalPathTableM);
                }
                else
                {
                    builder.AppendLine($"    No Optional Type-M Path Table {tableNum}");
                    builder.AppendLine();
                }
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, PathTableRecord[] records)
        {
            builder.AppendLine("    Path Table Records:");
            builder.AppendLine("    -------------------------");
            if (records.Length == 0)
            {
                builder.AppendLine("    No records");
                builder.AppendLine();
                return;
            }

            foreach (var record in records)
            {
                builder.AppendLine(record.DirectoryIdentifierLength, "    Directory Identifier Length");
                builder.AppendLine(record.ExtendedAttributeRecordLength, "    Extended Attribute Record Length");
                builder.AppendLine(record.ExtentLocation, "    Extent Location");
                builder.AppendLine(record.DirectoryIdentifier, "    Directory Identifier");
                if (record.PaddingField != null)
                    builder.AppendLine(record.PaddingField, "    Padding Field");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryDescriptor[]? dds)
        {
            builder.AppendLine("  Root Directory Descriptor(s) Information:");
            builder.AppendLine("  -------------------------");
            if (dds == null)
            {
                builder.AppendLine("  No root directory descriptors");
                builder.AppendLine();
                return;
            }

            foreach(var dd in dds)
            {
                Print(builder, dd);
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryDescriptor? dd)
        {
            builder.AppendLine("    Directory Descriptor Information:");
            builder.AppendLine("    -------------------------");
            if (dd == null)
            {
                builder.AppendLine("    No directory descriptor");
                builder.AppendLine();
                return;
            }

            // TODO: Implement

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryRecord? dr)
        {
            builder.AppendLine("    Directory Record:");
            builder.AppendLine("    -------------------------");
            if (dr == null)
            {
                builder.AppendLine("    No directory record");
                builder.AppendLine();
                return;
            }

            // TODO: Implement
            
            //builder.AppendLine("    File Flags:");
            //builder.AppendLine(dr.FileFlags.HasFlag(FileFlags.EXISTENCE), "      Existence");
            //builder.AppendLine(dr.FileFlags.HasFlag(FileFlags.DIRECTORY), "      Directory");
            //builder.AppendLine(dr.FileFlags.HasFlag(FileFlags.ASSOCIATED_FILE), "      Associated File");
            //builder.AppendLine(dr.FileFlags.HasFlag(FileFlags.RECORD), "      Record");
            //builder.AppendLine(dr.FileFlags.HasFlag(FileFlags.PROTECTION), "      Protection");
            //builder.AppendLine(dr.FileFlags.HasFlag(FileFlags.RESERVED_BIT5), "      Reserved Flag (Bit 5)");
            //builder.AppendLine(dr.FileFlags.HasFlag(FileFlags.RESERVED_BIT6), "      Reserved Flag (Bit 6)");
            //builder.AppendLine(dr.FileFlags.HasFlag(FileFlags.MULTI_EXTENT), "      Multi-Extent");

            builder.AppendLine();
        }

        #endregion

        private static void Print(StringBuilder builder, DecDateTime? dt)
        {
            if (dt == null)
            {
                builder.AppendLine("      Null");
                return;
            }

            builder.AppendLine(Encoding.ASCII.GetString(dt.Year), "      Year");
            builder.AppendLine(Encoding.ASCII.GetString(dt.Month), "      Month");
            builder.AppendLine(Encoding.ASCII.GetString(dt.Day), "      Day");
            builder.AppendLine(Encoding.ASCII.GetString(dt.Hour), "      Hour");
            builder.AppendLine(Encoding.ASCII.GetString(dt.Minute), "      Minute");
            builder.AppendLine(Encoding.ASCII.GetString(dt.Second), "      Second");
            builder.AppendLine(Encoding.ASCII.GetString(dt.Centisecond), "      Centisecond");
            string tz = $"{((dt.TimezoneOffset-48)*15/60):+0;-0}:{((dt.TimezoneOffset-48)*15%60+60)%60:00}";
            builder.AppendLine(tz, "      Timezone Offset");
            builder.AppendLine();
        }

        private static bool IsAllZero(byte[] array)
        {
            for (byte i = 0; i < array.Length; i++)
            {
                if (array[i] != 0)
                    return false;
            }
            return true;
        }
    }
}




