using System.Collections.Generic;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.InstallShieldCabinet;

namespace SabreTools.Serialization.Wrappers
{
    public partial class InstallShieldCabinet : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("InstallShield Cabinet Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Headers
            Print(builder, Model.CommonHeader, MajorVersion);
            Print(builder, Model.VolumeHeader, MajorVersion);
            Print(builder, Model.Descriptor);

            // File Descriptors
            Print(builder, Model.FileDescriptorOffsets);
            Print(builder, Model.DirectoryNames);
            Print(builder, Model.FileDescriptors);

            // File Groups
            Print(builder, Model.FileGroupOffsets, "File Group");
            Print(builder, Model.FileGroups);

            // Components
            Print(builder, Model.ComponentOffsets, "Component");
            Print(builder, Model.Components);
        }

        private static void Print(StringBuilder builder, CommonHeader? header, int majorVersion)
        {
            builder.AppendLine("  Common Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine(value: "  No common header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine($"  Version: {header.Version} (0x{header.Version:X}) [{majorVersion}]");
            builder.AppendLine(header.VolumeInfo, "  Volume info");
            builder.AppendLine(header.DescriptorOffset, "  Descriptor offset");
            builder.AppendLine(header.DescriptorSize, "  Descriptor size");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, VolumeHeader? header, int majorVersion)
        {
            builder.AppendLine("  Volume Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine(value: "  No volume header");
                builder.AppendLine();
                return;
            }

            if (majorVersion <= 5)
            {
                builder.AppendLine(header.DataOffset, "  Data offset");
                builder.AppendLine(header.FirstFileIndex, "  First file index");
                builder.AppendLine(header.LastFileIndex, "  Last file index");
                builder.AppendLine(header.FirstFileOffset, "  First file offset");
                builder.AppendLine(header.FirstFileSizeExpanded, "  First file size expanded");
                builder.AppendLine(header.FirstFileSizeCompressed, "  First file size compressed");
                builder.AppendLine(header.LastFileOffset, "  Last file offset");
                builder.AppendLine(header.LastFileSizeExpanded, "  Last file size expanded");
                builder.AppendLine(header.LastFileSizeCompressed, "  Last file size compressed");
            }
            else
            {
                builder.AppendLine(header.DataOffset, "  Data offset");
                builder.AppendLine(header.DataOffsetHigh, "  Data offset high");
                builder.AppendLine(header.FirstFileIndex, "  First file index");
                builder.AppendLine(header.LastFileIndex, "  Last file index");
                builder.AppendLine(header.FirstFileOffset, "  First file offset");
                builder.AppendLine(header.FirstFileOffsetHigh, "  First file offset high");
                builder.AppendLine(header.FirstFileSizeExpanded, "  First file size expanded");
                builder.AppendLine(header.FirstFileSizeExpandedHigh, "  First file size expanded high");
                builder.AppendLine(header.FirstFileSizeCompressed, "  First file size compressed");
                builder.AppendLine(header.FirstFileSizeCompressedHigh, "  First file size compressed high");
                builder.AppendLine(header.LastFileOffset, "  Last file offset");
                builder.AppendLine(header.LastFileOffsetHigh, "  Last file offset high");
                builder.AppendLine(header.LastFileSizeExpanded, "  Last file size expanded");
                builder.AppendLine(header.LastFileSizeExpandedHigh, "  Last file size expanded high");
                builder.AppendLine(header.LastFileSizeCompressed, "  Last file size compressed");
                builder.AppendLine(header.LastFileSizeCompressedHigh, "  Last file size compressed high");
            }
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Descriptor? descriptor)
        {
            builder.AppendLine("  Descriptor Information:");
            builder.AppendLine("  -------------------------");
            if (descriptor == null)
            {
                builder.AppendLine("  No descriptor");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(descriptor.StringsOffset, "  Strings offset");
            builder.AppendLine(descriptor.Reserved0, "  Reserved 0");
            builder.AppendLine(descriptor.ComponentListOffset, "  Component list offset");
            builder.AppendLine(descriptor.FileTableOffset, "  File table offset");
            builder.AppendLine(descriptor.Reserved1, "  Reserved 1");
            builder.AppendLine(descriptor.FileTableSize, "  File table size");
            builder.AppendLine(descriptor.FileTableSize2, "  File table size 2");
            builder.AppendLine(descriptor.DirectoryCount, "  Directory count");
            builder.AppendLine(descriptor.Reserved2, "  Reserved 2");
            builder.AppendLine(descriptor.Reserved3, "  Reserved 3");
            builder.AppendLine(descriptor.Reserved4, "  Reserved 4");
            builder.AppendLine(descriptor.FileCount, "  File count");
            builder.AppendLine(descriptor.FileTableOffset2, "  File table offset 2");
            builder.AppendLine(descriptor.ComponentTableInfoCount, "  Component table info count");
            builder.AppendLine(descriptor.ComponentTableOffset, "  Component table offset");
            builder.AppendLine(descriptor.Reserved5, "  Reserved 5");
            builder.AppendLine(descriptor.Reserved6, "  Reserved 6");
            builder.AppendLine();

            builder.AppendLine("  File group offsets:");
            builder.AppendLine("  -------------------------");
            if (descriptor.FileGroupOffsets == null || descriptor.FileGroupOffsets.Length == 0)
            {
                builder.AppendLine("  No file group offsets");
            }
            else
            {
                for (int i = 0; i < descriptor.FileGroupOffsets.Length; i++)
                {
                    builder.AppendLine(descriptor.FileGroupOffsets[i], $"      File Group Offset {i}");
                }
            }

            builder.AppendLine();

            builder.AppendLine("  Component offsets:");
            builder.AppendLine("  -------------------------");
            if (descriptor.ComponentOffsets == null || descriptor.ComponentOffsets.Length == 0)
            {
                builder.AppendLine("  No component offsets");
            }
            else
            {
                for (int i = 0; i < descriptor.ComponentOffsets.Length; i++)
                {
                    builder.AppendLine(descriptor.ComponentOffsets[i], $"      Component Offset {i}");
                }
            }

            builder.AppendLine();

            builder.AppendLine(descriptor.SetupTypesOffset, "  Setup types offset");
            builder.AppendLine(descriptor.SetupTableOffset, "  Setup table offset");
            builder.AppendLine(descriptor.Reserved7, "  Reserved 7");
            builder.AppendLine(descriptor.Reserved8, "  Reserved 8");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, uint[]? entries)
        {
            builder.AppendLine("  File Descriptor Offsets:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No file descriptor offsets");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                builder.AppendLine(entries[i], $"    File Descriptor Offset {i}");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, string[]? entries)
        {
            builder.AppendLine("  Directory Names:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory names");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                builder.AppendLine(entries[i], $"    Directory Name {i}");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, FileDescriptor[]? entries)
        {
            builder.AppendLine("  File Descriptors:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No file descriptors");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  File Descriptor {i}:");
                builder.AppendLine(entry.NameOffset, "    Name offset");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.DirectoryIndex, "    Directory index");
                builder.AppendLine($"    Flags: {entry.Flags} (0x{entry.Flags:X})");
                builder.AppendLine(entry.ExpandedSize, "    Expanded size");
                builder.AppendLine(entry.CompressedSize, "    Compressed size");
                builder.AppendLine(entry.DataOffset, "    Data offset");
                builder.AppendLine(entry.MD5, "    MD5");
                builder.AppendLine(entry.Volume, "    Volume");
                builder.AppendLine(entry.LinkPrevious, "    Link previous");
                builder.AppendLine(entry.LinkNext, "    Link next");
                builder.AppendLine($"    Link flags: {entry.LinkFlags} (0x{entry.LinkFlags:X})");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Dictionary<long, OffsetList?>? entries, string name)
        {
            builder.AppendLine($"  {name} Offsets:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Count == 0)
            {
                builder.AppendLine($"  No {name.ToLowerInvariant()} offsets");
                builder.AppendLine();
                return;
            }

            foreach (var kvp in entries)
            {
                long offset = kvp.Key;
                var value = kvp.Value;

                builder.AppendLine($"  {name} Offset {offset}:");
                if (value == null)
                {
                    builder.AppendLine($"    Unassigned {name.ToLowerInvariant()}");
                    continue;
                }

                builder.AppendLine(value.NameOffset, "    Name offset");
                builder.AppendLine(value.Name, "    Name");
                builder.AppendLine(value.DescriptorOffset, "    Descriptor offset");
                builder.AppendLine(value.NextOffset, "    Next offset");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, FileGroup[]? entries)
        {
            builder.AppendLine("  File Groups:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No file groups");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  File Group {i}:");
                builder.AppendLine(entry.NameOffset, "    Name offset");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.ExpandedSize, "    Expanded size");
                builder.AppendLine(entry.CompressedSize, "    Compressed size");
                builder.AppendLine($"    Attributes: {entry.Attributes} (0x{entry.Attributes:X})");
                builder.AppendLine(entry.FirstFile, "    First file");
                builder.AppendLine(entry.LastFile, "    Last file");
                builder.AppendLine(entry.UnknownStringOffset, "    Unknown string offset");
                builder.AppendLine(entry.OperatingSystemOffset, "    Operating system offset");
                builder.AppendLine(entry.LanguageOffset, "    Language offset");
                builder.AppendLine(entry.HTTPLocationOffset, "    HTTP location offset");
                builder.AppendLine(entry.FTPLocationOffset, "    FTP location offset");
                builder.AppendLine(entry.MiscOffset, "    Misc. offset");
                builder.AppendLine(entry.TargetDirectoryOffset, "    Target directory offset");
                builder.AppendLine($"    Overwrite flags: {entry.OverwriteFlags} (0x{entry.OverwriteFlags:X})");
                builder.AppendLine(entry.Reserved, "    Reserved");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Component[]? entries)
        {
            builder.AppendLine("  Components:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No components");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Component {i}:");
                builder.AppendLine(entry.IdentifierOffset, "    Identifier offset");
                builder.AppendLine(entry.Identifier, "    Identifier");
                builder.AppendLine(entry.DescriptorOffset, "    Descriptor offset");
                builder.AppendLine(entry.DisplayNameOffset, "    Display name offset");
                builder.AppendLine(entry.DisplayName, "    Display name");
                builder.AppendLine($"    Status: {entry.Status} (0x{entry.Status:X})");
                builder.AppendLine(entry.PasswordOffset, "    Password offset");
                builder.AppendLine(entry.MiscOffset, "    Misc. offset");
                builder.AppendLine(entry.ComponentIndex, "    Component index");
                builder.AppendLine(entry.NameOffset, "    Name offset");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.CDRomFolderOffset, "    CD-ROM folder offset");
                builder.AppendLine(entry.HTTPLocationOffset, "    HTTP location offset");
                builder.AppendLine(entry.FTPLocationOffset, "    FTP location offset");
                builder.AppendLine(entry.Guid, "    GUIDs");
                builder.AppendLine(entry.CLSIDOffset, "    CLSID offset");
                builder.AppendLine(entry.CLSID, "    CLSID");
                builder.AppendLine(entry.Reserved2, "    Reserved 2");
                builder.AppendLine(entry.Reserved3, "    Reserved 3");
                builder.AppendLine(entry.DependsCount, "    Depends count");
                builder.AppendLine(entry.DependsOffset, "    Depends offset");
                builder.AppendLine(entry.FileGroupCount, "    File group count");
                builder.AppendLine(entry.FileGroupNamesOffset, "    File group names offset");
                builder.AppendLine();

                builder.AppendLine("    File group names:");
                builder.AppendLine("    -------------------------");
                if (entry.FileGroupNames == null || entry.FileGroupNames.Length == 0)
                {
                    builder.AppendLine("    No file group names");
                }
                else
                {
                    for (int j = 0; j < entry.FileGroupNames.Length; j++)
                    {
                        builder.AppendLine(entry.FileGroupNames[j], $"      File Group Name {j}");
                    }
                }

                builder.AppendLine();

                builder.AppendLine(entry.X3Count, "    X3 count");
                builder.AppendLine(entry.X3Offset, "    X3 offset");
                builder.AppendLine(entry.SubComponentsCount, "    Sub-components count");
                builder.AppendLine(entry.SubComponentsOffset, "    Sub-components offset");
                builder.AppendLine(entry.NextComponentOffset, "    Next component offset");
                builder.AppendLine(entry.OnInstallingOffset, "    On installing offset");
                builder.AppendLine(entry.OnInstalledOffset, "    On installed offset");
                builder.AppendLine(entry.OnUninstallingOffset, "    On uninstalling offset");
                builder.AppendLine(entry.OnUninstalledOffset, "    On uninstalled offset");
            }

            builder.AppendLine();
        }
    }
}
