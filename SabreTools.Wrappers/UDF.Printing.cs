using System.Collections.Generic;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.UDF;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class UDF : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON(bool recursive) => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#else
        /// <inheritdoc/>
        public string ExportJSON(bool recursive) => Newtonsoft.Json.JsonConvert.SerializeObject(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, bool recursive)
        {
            builder.AppendLine("UDF Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.VolumeRecognitionSequence);
            Print(builder, Model.AnchorVolumeDescriptorPointer);
            Print(builder, Model.VolumeDescriptorSequence);
            Print(builder, Model.FileSetDescriptor);

            if (Model.FileSetDescriptor is not null)
            {
                builder.AppendLine("  Directory Hierarchy:");
                builder.AppendLine("  -------------------------");
                long root = Model.FileSetDescriptor.RootDirectoryICB.ExtentLocation.GetKey();
                PrintDirectory(builder, root, "\\", new HashSet<long> { root });
                builder.AppendLine();
            }
        }

        private static void Print(StringBuilder builder, VolumeStructureDescriptor[] sequence)
        {
            builder.AppendLine("  Volume Recognition Sequence:");
            builder.AppendLine("  -------------------------");
            if (sequence.Length == 0)
            {
                builder.AppendLine("  No volume structure descriptors");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < sequence.Length; i++)
            {
                var vsd = sequence[i];
                builder.AppendLine($"  Volume Structure Descriptor {i}:");
                builder.AppendLine(vsd.StructureType, "    Structure Type");
                builder.AppendLine(Encoding.ASCII.GetString(vsd.StandardIdentifier), "    Standard Identifier");
                builder.AppendLine(vsd.StructureVersion, "    Structure Version");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, AnchorVolumeDescriptorPointer? anchor)
        {
            builder.AppendLine("  Anchor Volume Descriptor Pointer:");
            builder.AppendLine("  -------------------------");
            if (anchor is null)
            {
                builder.AppendLine("  No anchor volume descriptor pointer");
                builder.AppendLine();
                return;
            }

            Print(builder, anchor.DescriptorTag, "    ");
            Print(builder, anchor.MainVolumeDescriptorSequenceExtent, "    Main Volume Descriptor Sequence");
            Print(builder, anchor.ReserveVolumeDescriptorSequenceExtent, "    Reserve Volume Descriptor Sequence");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Descriptor[] sequence)
        {
            builder.AppendLine("  Volume Descriptor Sequence:");
            builder.AppendLine("  -------------------------");
            if (sequence.Length == 0)
            {
                builder.AppendLine("  No volume descriptors");
                builder.AppendLine();
                return;
            }

            foreach (var descriptor in sequence)
            {
                builder.AppendLine($"  {descriptor.DescriptorTag.TagIdentifier} (Sector {descriptor.DescriptorTag.TagLocation}):");
                Print(builder, descriptor.DescriptorTag, "    ");
                switch (descriptor)
                {
                    case PrimaryVolumeDescriptor pvd:
                        builder.AppendLine(pvd.VolumeDescriptorSequenceNumber, "    Volume Descriptor Sequence Number");
                        builder.AppendLine(pvd.PrimaryVolumeDescriptorNumber, "    Primary Volume Descriptor Number");
                        builder.AppendLine(pvd.VolumeIdentifier.DecodeDString(), "    Volume Identifier");
                        builder.AppendLine(pvd.VolumeSequenceNumber, "    Volume Sequence Number");
                        builder.AppendLine(pvd.MaximumVolumeSequenceNumber, "    Maximum Volume Sequence Number");
                        builder.AppendLine(pvd.InterchangeLevel, "    Interchange Level");
                        builder.AppendLine(pvd.MaximumInterchangeLevel, "    Maximum Interchange Level");
                        builder.AppendLine(pvd.VolumeSetIdentifier.DecodeDString(), "    Volume Set Identifier");
                        Print(builder, pvd.ApplicationIdentifier, "    Application Identifier");
                        builder.AppendLine(Format(pvd.RecordingDateAndTime), "    Recording Date and Time");
                        Print(builder, pvd.ImplementationIdentifier, "    Implementation Identifier");
                        builder.AppendLine(pvd.Flags, "    Flags");
                        break;

                    case PartitionDescriptor pd:
                        builder.AppendLine(pd.VolumeDescriptorSequenceNumber, "    Volume Descriptor Sequence Number");
                        builder.AppendLine(pd.PartitionFlags, "    Partition Flags");
                        builder.AppendLine(pd.PartitionNumber, "    Partition Number");
                        Print(builder, pd.PartitionContents, "    Partition Contents");
                        builder.AppendLine(pd.AccessType, "    Access Type");
                        builder.AppendLine(pd.PartitionStartingLocation, "    Partition Starting Location");
                        builder.AppendLine(pd.PartitionLength, "    Partition Length");
                        Print(builder, pd.ImplementationIdentifier, "    Implementation Identifier");
                        break;

                    case LogicalVolumeDescriptor lvd:
                        builder.AppendLine(lvd.VolumeDescriptorSequenceNumber, "    Volume Descriptor Sequence Number");
                        builder.AppendLine(lvd.LogicalVolumeIdentifier.DecodeDString(), "    Logical Volume Identifier");
                        builder.AppendLine(lvd.LogicalBlockSize, "    Logical Block Size");
                        Print(builder, lvd.DomainIdentifier, "    Domain Identifier");
                        Print(builder, lvd.LogicalVolumeContentsUse, "    File Set Descriptor Extent");
                        builder.AppendLine(lvd.MapTableLength, "    Map Table Length");
                        builder.AppendLine(lvd.NumberOfPartitionMaps, "    Number of Partition Maps");
                        Print(builder, lvd.ImplementationIdentifier, "    Implementation Identifier");
                        Print(builder, lvd.IntegritySequenceExtent, "    Integrity Sequence Extent");
                        for (int i = 0; i < lvd.PartitionMaps.Length; i++)
                        {
                            Print(builder, lvd.PartitionMaps[i], i);
                        }

                        break;

                    case VolumeDescriptorPointer vdp:
                        builder.AppendLine(vdp.VolumeDescriptorSequenceNumber, "    Volume Descriptor Sequence Number");
                        Print(builder, vdp.NextVolumeDescriptorSequenceExtent, "    Next Volume Descriptor Sequence");
                        break;

                    default:
                        // Other descriptors are not parsed further
                        break;
                }
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, PartitionMap map, int index)
        {
            builder.AppendLine($"    Partition Map {index} (Partition Reference Number {index}):");
            builder.AppendLine(map.PartitionMapType, "      Partition Map Type");
            builder.AppendLine(map.PartitionMapLength, "      Partition Map Length");
            switch (map)
            {
                case Type1PartitionMap type1:
                    builder.AppendLine(type1.VolumeSequenceNumber, "      Volume Sequence Number");
                    builder.AppendLine(type1.PartitionNumber, "      Partition Number");
                    break;

                case MetadataPartitionMap metadata:
                    Print(builder, metadata.PartitionTypeIdentifier, "      Partition Type Identifier");
                    builder.AppendLine(metadata.VolumeSequenceNumber, "      Volume Sequence Number");
                    builder.AppendLine(metadata.PartitionNumber, "      Partition Number");
                    builder.AppendLine(metadata.MetadataFileLocation, "      Metadata File Location");
                    builder.AppendLine(metadata.MetadataMirrorFileLocation, "      Metadata Mirror File Location");
                    builder.AppendLine(metadata.MetadataBitmapFileLocation, "      Metadata Bitmap File Location");
                    builder.AppendLine(metadata.AllocationUnitSize, "      Allocation Unit Size");
                    builder.AppendLine(metadata.AlignmentUnitSize, "      Alignment Unit Size");
                    builder.AppendLine(metadata.Flags, "      Flags");
                    break;

                case Type2PartitionMap type2:
                    Print(builder, type2.PartitionTypeIdentifier, "      Partition Type Identifier");
                    builder.AppendLine(type2.VolumeSequenceNumber, "      Volume Sequence Number");
                    builder.AppendLine(type2.PartitionNumber, "      Partition Number");
                    builder.AppendLine(type2.PartitionTypeData, "      Partition Type Data");
                    break;

                default:
                    // Unknown partition map types are not parsed further
                    break;
            }
        }

        private static void Print(StringBuilder builder, FileSetDescriptor? fsd)
        {
            builder.AppendLine("  File Set Descriptor:");
            builder.AppendLine("  -------------------------");
            if (fsd is null)
            {
                builder.AppendLine("  No file set descriptor");
                builder.AppendLine();
                return;
            }

            Print(builder, fsd.DescriptorTag, "    ");
            builder.AppendLine(Format(fsd.RecordingDateAndTime), "    Recording Date and Time");
            builder.AppendLine(fsd.InterchangeLevel, "    Interchange Level");
            builder.AppendLine(fsd.MaximumInterchangeLevel, "    Maximum Interchange Level");
            builder.AppendLine(fsd.FileSetNumber, "    File Set Number");
            builder.AppendLine(fsd.FileSetDescriptorNumber, "    File Set Descriptor Number");
            builder.AppendLine(fsd.LogicalVolumeIdentifier.DecodeDString(), "    Logical Volume Identifier");
            builder.AppendLine(fsd.FileSetIdentifier.DecodeDString(), "    File Set Identifier");
            builder.AppendLine(fsd.CopyrightFileIdentifier.DecodeDString(), "    Copyright File Identifier");
            builder.AppendLine(fsd.AbstractFileIdentifier.DecodeDString(), "    Abstract File Identifier");
            Print(builder, fsd.RootDirectoryICB, "    Root Directory ICB");
            Print(builder, fsd.DomainIdentifier, "    Domain Identifier");
            Print(builder, fsd.NextExtent, "    Next Extent");
            Print(builder, fsd.SystemStreamDirectoryICB, "    System Stream Directory ICB");
            builder.AppendLine();
        }

        private void PrintDirectory(StringBuilder builder, long key, string path, HashSet<long> visited)
        {
            if (!Model.Directories.TryGetValue(key, out var fids))
            {
                builder.AppendLine($"  {path} (directory not read)");
                return;
            }

            foreach (var fid in fids)
            {
                if ((fid.FileCharacteristics & (FileCharacteristics.DELETED | FileCharacteristics.PARENT)) != 0)
                    continue;

                string name = fid.FileIdentifier.DecodeDCharacters() ?? "(unknown encoding)";
                string childPath = path + name;
                long childKey = fid.ICB.ExtentLocation.GetKey();
                string hidden = (fid.FileCharacteristics & FileCharacteristics.EXISTENCE) != 0 ? ", hidden" : string.Empty;
                if (!Model.FileEntries.TryGetValue(childKey, out var entry))
                {
                    builder.AppendLine($"  {childPath} (File Entry not read{hidden})");
                    continue;
                }

                string location = $"partition {fid.ICB.ExtentLocation.PartitionReferenceNumber}, block {fid.ICB.ExtentLocation.LogicalBlockNumber}";
                if (entry.ICBTag.FileType == FileType.DIRECTORY)
                {
                    builder.AppendLine($"  {childPath}\\ ({location}{hidden})");
                    if (visited.Add(childKey))
                        PrintDirectory(builder, childKey, childPath + "\\", visited);
                }
                else
                {
                    builder.AppendLine($"  {childPath} ({entry.InformationLength} bytes, {entry.ICBTag.FileType}, {location}{hidden})");
                }
            }
        }

        private static void Print(StringBuilder builder, DescriptorTag tag, string indent)
        {
            builder.AppendLine((ushort)tag.TagIdentifier, $"{indent}Tag Identifier");
            builder.AppendLine(tag.DescriptorVersion, $"{indent}Descriptor Version");
            builder.AppendLine(tag.TagSerialNumber, $"{indent}Tag Serial Number");
            builder.AppendLine(tag.DescriptorCRCLength, $"{indent}Descriptor CRC Length");
            builder.AppendLine(tag.TagLocation, $"{indent}Tag Location");
        }

        private static void Print(StringBuilder builder, ExtentDescriptor extent, string label)
            => builder.AppendLine($"sector {extent.ExtentLocation}, {extent.ExtentLength} bytes", label);

        private static void Print(StringBuilder builder, LongAllocationDescriptor ad, string label)
        {
            builder.AppendLine($"partition {ad.ExtentLocation.PartitionReferenceNumber}, block {ad.ExtentLocation.LogicalBlockNumber}, "
                + $"{UDFExtensions.GetExtentLength(ad.ExtentLength)} bytes", label);
        }

        private static void Print(StringBuilder builder, EntityIdentifier identifier, string label)
        {
            int length = System.Array.IndexOf(identifier.Identifier, (byte)0);
            if (length < 0)
                length = identifier.Identifier.Length;

            string text = Encoding.ASCII.GetString(identifier.Identifier, 0, length);
            builder.AppendLine($"{text} (flags {identifier.Flags}, suffix {BytesToHex(identifier.IdentifierSuffix)})", label);
        }

        private static string Format(Timestamp timestamp)
        {
            if (timestamp.Year == 0 && timestamp.Month == 0 && timestamp.Day == 0)
                return "Not specified";

            // Lower 12 bits: signed offset from UTC in minutes, -2047 = not specified
            int offset = timestamp.TypeAndTimezone & 0x0FFF;
            if ((offset & 0x0800) != 0)
                offset -= 0x1000;

            string zone = offset == -2047 ? string.Empty : $" UTC{(offset < 0 ? "-" : "+")}{System.Math.Abs(offset) / 60:D2}:{System.Math.Abs(offset) % 60:D2}";
            return $"{timestamp.Year:D4}-{timestamp.Month:D2}-{timestamp.Day:D2} {timestamp.Hour:D2}:{timestamp.Minute:D2}:{timestamp.Second:D2}.{timestamp.Centiseconds:D2}{zone}";
        }

        private static string BytesToHex(byte[] data)
        {
            var builder = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                builder.Append(b.ToString("X2"));
            }

            return builder.ToString();
        }
    }
}
