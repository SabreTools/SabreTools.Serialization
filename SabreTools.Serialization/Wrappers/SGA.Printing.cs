using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.SGA;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SGA : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("SGA Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Header
            Print(builder, Model.Header);

            // Directory
            Print(builder, Model.Directory);
            // TODO: Should we print the string table?
        }

        private static void Print(StringBuilder builder, Header header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine(header.MajorVersion, "  Major version");
            builder.AppendLine(header.MinorVersion, "  Minor version");
            switch (header)
            {
                case Header4 header4:
                    builder.AppendLine(header4.FileMD5, "  File MD5");
                    builder.AppendLine(header4.Name, "  Name");
                    builder.AppendLine(header4.HeaderMD5, "  Header MD5");
                    builder.AppendLine(header4.HeaderLength, "  Header length");
                    builder.AppendLine(header4.FileDataOffset, "  File data offset");
                    builder.AppendLine(header4.Dummy0, "  Dummy 0");
                    break;

                case Header6 header6:
                    builder.AppendLine(header6.Name, "  Name");
                    builder.AppendLine(header6.HeaderLength, "  Header length");
                    builder.AppendLine(header6.FileDataOffset, "  File data offset");
                    builder.AppendLine(header6.Dummy0, "  Dummy 0");
                    break;

                default:
                    builder.AppendLine($"  Unrecognized header type");
                    break;
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Directory directory)
        {
            builder.AppendLine("  Directory Information:");
            builder.AppendLine("  -------------------------");
            switch (directory)
            {
                case Directory4 directory4:
                    Print(builder, directory4.DirectoryHeader!);
                    Print(builder, directory4.Sections);
                    Print(builder, directory4.Folders);
                    Print(builder, directory4.Files);
                    break;

                case Directory5 directory5:
                    Print(builder, directory5.DirectoryHeader!);
                    Print(builder, directory5.Sections);
                    Print(builder, directory5.Folders);
                    Print(builder, directory5.Files);
                    break;

                case Directory6 directory6:
                    Print(builder, directory6.DirectoryHeader!);
                    Print(builder, directory6.Sections);
                    Print(builder, directory6.Folders);
                    Print(builder, directory6.Files);
                    break;

                case Directory7 directory7:
                    Print(builder, directory7.DirectoryHeader!);
                    Print(builder, directory7.Sections);
                    Print(builder, directory7.Folders);
                    Print(builder, directory7.Files);
                    break;

                default:
                    builder.AppendLine($"  Unrecognized directory type");
                    break;
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryHeader4 header)
        {
            builder.AppendLine("  Directory Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.SectionOffset, "  Section offset");
            builder.AppendLine(header.SectionCount, "  Section count");
            builder.AppendLine(header.FolderOffset, "  Folder offset");
            builder.AppendLine(header.FolderCount, "  Folder count");
            builder.AppendLine(header.FileOffset, "  File offset");
            builder.AppendLine(header.FileCount, "  File count");
            builder.AppendLine(header.StringTableOffset, "  String table offset");
            builder.AppendLine(header.StringTableCount, "  String table count");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryHeader5 header)
        {
            builder.AppendLine("  Directory Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.SectionOffset, "  Section offset");
            builder.AppendLine(header.SectionCount, "  Section count");
            builder.AppendLine(header.FolderOffset, "  Folder offset");
            builder.AppendLine(header.FolderCount, "  Folder count");
            builder.AppendLine(header.FileOffset, "  File offset");
            builder.AppendLine(header.FileCount, "  File count");
            builder.AppendLine(header.StringTableOffset, "  String table offset");
            builder.AppendLine(header.StringTableCount, "  String table count");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryHeader7 header)
        {
            builder.AppendLine("  Directory Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.SectionOffset, "  Section offset");
            builder.AppendLine(header.SectionCount, "  Section count");
            builder.AppendLine(header.FolderOffset, "  Folder offset");
            builder.AppendLine(header.FolderCount, "  Folder count");
            builder.AppendLine(header.FileOffset, "  File offset");
            builder.AppendLine(header.FileCount, "  File count");
            builder.AppendLine(header.StringTableOffset, "  String table offset");
            builder.AppendLine(header.StringTableCount, "  String table count");
            builder.AppendLine(header.HashTableOffset, "  Hash table offset");
            builder.AppendLine(header.BlockSize, "  Block size");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Section4[] sections)
        {
            builder.AppendLine("  Sections Information:");
            builder.AppendLine("  -------------------------");
            if (sections.Length == 0)
            {
                builder.AppendLine("  No sections");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < sections.Length; i++)
            {
                var section = sections[i];

                builder.AppendLine($"  Section {i}");
                builder.AppendLine(section.Alias, "    Alias");
                builder.AppendLine(section.Name, "    Name");
                builder.AppendLine(section.FolderStartIndex, "    Folder start index");
                builder.AppendLine(section.FolderEndIndex, "    Folder end index");
                builder.AppendLine(section.FileStartIndex, "    File start index");
                builder.AppendLine(section.FileEndIndex, "    File end index");
                builder.AppendLine(section.FolderRootIndex, "    Folder root index");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Section5[] sections)
        {
            builder.AppendLine("  Sections Information:");
            builder.AppendLine("  -------------------------");
            if (sections.Length == 0)
            {
                builder.AppendLine("  No sections");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < sections.Length; i++)
            {
                var section = sections[i];

                builder.AppendLine($"  Section {i}");
                builder.AppendLine(section.Alias, "    Alias");
                builder.AppendLine(section.Name, "    Name");
                builder.AppendLine(section.FolderStartIndex, "    Folder start index");
                builder.AppendLine(section.FolderEndIndex, "    Folder end index");
                builder.AppendLine(section.FileStartIndex, "    File start index");
                builder.AppendLine(section.FileEndIndex, "    File end index");
                builder.AppendLine(section.FolderRootIndex, "    Folder root index");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Folder4[] folders)
        {
            builder.AppendLine("  Folders Information:");
            builder.AppendLine("  -------------------------");
            if (folders.Length == 0)
            {
                builder.AppendLine("  No folders");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < folders.Length; i++)
            {
                var folder = folders[i];

                builder.AppendLine($"  Folder {i}");
                builder.AppendLine(folder.NameOffset, "    Name offset");
                builder.AppendLine(folder.Name, "    Name");
                builder.AppendLine(folder.FolderStartIndex, "    Folder start index");
                builder.AppendLine(folder.FolderEndIndex, "    Folder end index");
                builder.AppendLine(folder.FileStartIndex, "    File start index");
                builder.AppendLine(folder.FileEndIndex, "    File end index");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Folder5[] folders)
        {
            builder.AppendLine("  Folders Information:");
            builder.AppendLine("  -------------------------");
            if (folders.Length == 0)
            {
                builder.AppendLine("  No folders");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < folders.Length; i++)
            {
                var folder = folders[i];

                builder.AppendLine($"  Folder {i}");
                builder.AppendLine(folder.NameOffset, "    Name offset");
                builder.AppendLine(folder.Name, "    Name");
                builder.AppendLine(folder.FolderStartIndex, "    Folder start index");
                builder.AppendLine(folder.FolderEndIndex, "    Folder end index");
                builder.AppendLine(folder.FileStartIndex, "    File start index");
                builder.AppendLine(folder.FileEndIndex, "    File end index");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, File4[] files)
        {
            builder.AppendLine("  Files Information:");
            builder.AppendLine("  -------------------------");
            if (files.Length == 0)
            {
                builder.AppendLine("  No files");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                builder.AppendLine($"  File {i}");
                builder.AppendLine(file.NameOffset, "    Name offset");
                builder.AppendLine(file.Name, "    Name");
                builder.AppendLine(file.Offset, "    Offset");
                builder.AppendLine(file.SizeOnDisk, "    Size on disk");
                builder.AppendLine(file.Size, "    Size");
                builder.AppendLine(file.TimeModified, "    Time modified");
                builder.AppendLine(file.Dummy0, "    Dummy 0");
                builder.AppendLine(file.Type, "    Type");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, File6[] files)
        {
            builder.AppendLine("  Files Information:");
            builder.AppendLine("  -------------------------");
            if (files.Length == 0)
            {
                builder.AppendLine("  No files");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                builder.AppendLine($"  File {i}");
                builder.AppendLine(file.NameOffset, "    Name offset");
                builder.AppendLine(file.Name, "    Name");
                builder.AppendLine(file.Offset, "    Offset");
                builder.AppendLine(file.SizeOnDisk, "    Size on disk");
                builder.AppendLine(file.Size, "    Size");
                builder.AppendLine(file.TimeModified, "    Time modified");
                builder.AppendLine(file.Dummy0, "    Dummy 0");
                builder.AppendLine(file.Type, "    Type");
                builder.AppendLine(file.CRC32, "    CRC32");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, File7[] files)
        {
            builder.AppendLine("  Files Information:");
            builder.AppendLine("  -------------------------");
            if (files.Length == 0)
            {
                builder.AppendLine("  No files");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                builder.AppendLine($"  File {i}");
                builder.AppendLine(file.NameOffset, "    Name offset");
                builder.AppendLine(file.Name, "    Name");
                builder.AppendLine(file.Offset, "    Offset");
                builder.AppendLine(file.SizeOnDisk, "    Size on disk");
                builder.AppendLine(file.Size, "    Size");
                builder.AppendLine(file.TimeModified, "    Time modified");
                builder.AppendLine(file.Dummy0, "    Dummy 0");
                builder.AppendLine(file.Type, "    Type");
                builder.AppendLine(file.CRC32, "    CRC32");
                builder.AppendLine(file.HashOffset, "    Hash offset");
            }

            builder.AppendLine();
        }
    }
}
