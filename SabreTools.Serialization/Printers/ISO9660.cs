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

            // TODO: Check for non-zero contents in System Area
            builder.AppendLine("Unchecked", "System Area");
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
            builder.AppendLine("    Boot Record Volume Descriptor:");
            builder.AppendLine("    -------------------------");

            builder.AppendLine(vd.BootSystemIdentifier, "    Boot System Identifier");
            builder.AppendLine(vd.BootSystemIdentifier, "    Boot Identifier");

            // TODO: Check for non-zero contents in vd.SystemUse
            builder.AppendLine("Unchecked", "    System Use");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, BaseVolumeDescriptor vd)
        {
            var type = (byte?)vd.Type;
            
            // TOOD: Determine encoding based on vd.Type, svd.EscapeSequence (and manual detection?)

            if (type == 0x01)
                builder.AppendLine("    Primary Volume Descriptor:");
            else if (type == 0x02)
                builder.AppendLine("    Supplementary Volume Descriptor:");
            else
                builder.AppendLine("    Unidentified Base Volume Descriptor:");
            builder.AppendLine("    -------------------------");

            // TODO: Check Unused byte
            if (vd is SupplementaryVolumeDescriptor svd)
            {
                builder.AppendLine("    File Flags:");
                // TODO: Check file flags
                builder.AppendLine("Unchecked", "      Existence:");
                builder.AppendLine("Unchecked", "      Directory:");
                builder.AppendLine("Unchecked", "      Associated File:");
                builder.AppendLine("Unchecked", "      Record:");
                builder.AppendLine("Unchecked", "      Protection:");
                builder.AppendLine("Unchecked", "      Reserved Flag 1:");
                builder.AppendLine("Unchecked", "      Reserved Flag 2:");
                builder.AppendLine("Unchecked", "      Multi-Extent:");
            }
            else
            {
                // TODO: Check for non-zero unusued byte
                builder.AppendLine("Unchecked", "    Unused Byte");
            }

            // TODO: Decode all byte arrays into strings (based on encoding above)

            builder.AppendLine(vd.SystemIdentifier, "    System Identifier");
            builder.AppendLine(vd.VolumeIdentifier, "    Volume Identifier");
            // TODO: Check for non-zero vd.Unused8Bytes
            builder.AppendLine("Unchecked", "    Unused 8 Bytes");
            // TODO: Check that MSB/LSB match
            builder.AppendLine(vd.VolumeSpaceSize?.LSB, "    Volume Space Size");

            // TODO: Check for non-zero contents
            if (vd is SupplementaryVolumeDescriptor svd2)
            {
                // TODO: Trim trailing 0x00 and split array into characters (multi-byte encoding detection)
                builder.AppendLine(svd2.EscapeSequences, "    Escape Sequences");
            }
            else
            {
                // TODO: Check for non-zero unusued byte
                builder.AppendLine("Unchecked", "    Unused Bytes");
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
        
            // TODO: Print info on vd.RootDirectoryRecord using ParseDirectoryRecord

            builder.AppendLine(vd.VolumeSetIdentifier, "    Volume Set Identifier");
            builder.AppendLine(vd.PublisherIdentifier, "    Publisher Identifier");
            builder.AppendLine(vd.DataPreparerIdentifier, "    Data Preparer Identifier");
            builder.AppendLine(vd.ApplicationIdentifier, "    Application Identifier");
            builder.AppendLine(vd.CopyrightFileIdentifier, "    Copyright Identifier");
            builder.AppendLine(vd.AbstractFileIdentifier, "    Abstract Identifier");
            builder.AppendLine(vd.BibliographicFileIdentifier, "    Bibliographic Identifier");
            
            // TODO: Complete Print DecDateTime function using ParseDecDateTime
            builder.AppendLine(vd.VolumeCreationDateTime?.Year, "    Volume Creation Date Time");
            builder.AppendLine(vd.VolumeModificationDateTime?.Year, "    Volume Modification Date Time");
            builder.AppendLine(vd.VolumeExpirationDateTime?.Year, "    Volume Expiration Date Time");
            builder.AppendLine(vd.VolumeEffectiveDateTime?.Year, "    Volume Effective Date Time");

            builder.AppendLine(vd.FileStructureVersion, "    File Structure Version");

            // TODO: Check for non-zero reserved byte
            builder.AppendLine("Unchecked", "    Reserved Byte");

            // TODO: Check for non-zero contents in vd.ApplicationUse
            builder.AppendLine("Unchecked", "    Application Use");

            // TODO: Check for non-zero reserved bytes
            builder.AppendLine("Unchecked", "    Reserved Bytes");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, VolumePartitionDescriptor vd)
        {
            builder.AppendLine("    Volume Partition Descriptor:");
            builder.AppendLine("    -------------------------");

            builder.AppendLine(vd.SystemIdentifier, "    System Identifier");
            builder.AppendLine(vd.VolumePartitionIdentifier, "    Volume Partition Identifier");
            // TODO: Check that MSB/LSB match
            builder.AppendLine(vd.VolumePartitionLocation?.LSB, "    Volume Partition Location");
            // TODO: Check that MSB/LSB match
            builder.AppendLine(vd.VolumePartitionSize?.LSB, "    Volume Partition Size");

            // TODO: Check for non-zero contents in vd.SystemUse
            builder.AppendLine("Unchecked", "    System Use");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, VolumeDescriptorSetTerminator vd)
        {
            builder.AppendLine("    Volume Descriptor Set Terminator:");
            builder.AppendLine("    -------------------------");

            // TODO: Check for non-zero contents
            builder.AppendLine("Unchecked", "    Reserved Bytes");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, GenericVolumeDescriptor vd)
        {
            builder.AppendLine("    Unidentified Volume Descriptor:");
            builder.AppendLine("    -------------------------");

            // TODO: Check for non-zero contents
            builder.AppendLine("Unchecked", "    Contents");

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
                    builder.AppendLine("    Type-L Path Table {}", tableNum);
                    Print(builder, ptg.PathTableL);
                }
                else
                {
                    builder.AppendLine("    No Type-L Path Table {}", tableNum);
                    builder.AppendLine();
                }
                if (ptg.OptionalPathTableL != null)
                {
                    builder.AppendLine("    Optional Type-L Path Table {}", tableNum);
                    Print(builder, ptg.OptionalPathTableL);
                }
                else
                {
                    builder.AppendLine("    No Optional Type-L Path Table {}", tableNum);
                    builder.AppendLine();
                }
                if (ptg.PathTableM != null)
                {
                    builder.AppendLine("    Type-M Path Table {}", tableNum);
                    Print(builder, ptg.PathTableM);
                }
                else
                {
                    builder.AppendLine("    No Type-M Path Table {}", tableNum);
                    builder.AppendLine();
                }
                if (ptg.OptionalPathTableM != null)
                {
                    builder.AppendLine("    Optional Type-M Path Table {}", tableNum);
                    Print(builder, ptg.OptionalPathTableM);
                }
                else
                {
                    builder.AppendLine("    No Optional Type-M Path Table {}", tableNum);
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

            builder.AppendLine();
        }

        #endregion
    }
}
