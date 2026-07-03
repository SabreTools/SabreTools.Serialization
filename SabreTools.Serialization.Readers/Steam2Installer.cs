using System.IO;
using System.Text;
using SabreTools.Data.Models.Steam2Installer;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class Steam2Installer : BaseBinaryReader<Steam2InstallerSet>
    {
         /// <inheritdoc/>
        public override Steam2InstallerSet? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new Steam2 Installer set to populate
                var si = new Steam2InstallerSet();

                // Try to parse the .sim file
                var sim = ParseSim(data);

                // Make sure the sim was parsed properly
                if (sim == null)
                    return null;

                // Set the sim file
                si.Sim = sim;

                return si;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Sim file
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Sim on success, null on error</returns>
        public static SimFile? ParseSim(Stream data)
        {
            // Create a new Sim file to populate
            var si = new SimFile();

            // Try to parse the .sim file header
            var header = ParseHeader(data);

            // Make sure the magic is correct
            if (header.Magic != Constants.SimSignatureUInt32)
                return null;

            // Make sure the version is correct
            if (header.Version != 1)
                return null;

            // Make sure string bytes section doesn't go beyond EOF
            if (header.StringsSize > data.Length - data.Position)
                return null;

            // Set the header
            si.Header = header;

            // Read the string section bytes
            byte[] stringsBytes = data.ReadBytes((int)header.StringsSize);

            // Read the size and number of file entries
            uint fileEntriesSize = data.ReadUInt32LittleEndian();
            uint fileEntriesCount = data.ReadUInt32LittleEndian();

            // Make sure reading the file entries won't go beyond EOF
            if (fileEntriesSize > data.Length - data.Position)
                return null;

            // Try to parse the entry table
            var table = ParseTable(data, fileEntriesCount, stringsBytes);
            if (table is null || table.Length != fileEntriesCount)
                return null;

            // Set the entry table
            si.FileEntries = table;

            return si;
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header ParseHeader(Stream data)
        {
            var obj = new Header();

            obj.Magic = data.ReadUInt32LittleEndian();
            obj.Version = data.ReadUInt32LittleEndian();
            obj.Discs = data.ReadUInt32LittleEndian();
            obj.StringsSize = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an entry table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of entries to parse</param>
        /// <returns>Filled entry table on success, null on error</returns>
        public static FileEntry[]? ParseTable(Stream data, uint count, byte[] stringsBytes)
        {
            if (count == 0)
                return [];

            var obj = new FileEntry[count];
            for (uint i = 0; i < count; i++)
            {
                var fileEntry = ParseFileEntry(data, stringsBytes);
                if (fileEntry is null)
                    return null;

                obj[i] = fileEntry;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FileEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FileEntry on success, null on error</returns>
        public static FileEntry? ParseFileEntry(Stream data, byte[] stringsBytes)
        {
            var obj = new FileEntry();

            obj.NameOffset = data.ReadUInt32LittleEndian();
            obj.PathOffset = data.ReadUInt32LittleEndian();
            obj.DepotId = data.ReadUInt32LittleEndian();
            obj.Offset = data.ReadUInt64LittleEndian();
            obj.Size = data.ReadUInt64LittleEndian();
            obj.DiscNumber = data.ReadByteValue();
            obj.VolumeNumber = data.ReadByteValue();
            obj.Encrypted = data.ReadByte() != 0;
            obj.Unknown = data.ReadByteValue();

            // Read file path and name strings
            int nameOffset = (int)obj.NameOffset;
            string? name = stringsBytes.ReadNullTerminatedAnsiString(ref nameOffset); // Unsure on encoding, assumed ascii/ansi
            if (name is null)
                return null;

            obj.Name = name;
            int pathOffset = (int)obj.PathOffset;
            string? path = stringsBytes.ReadNullTerminatedAnsiString(ref pathOffset); // Unsure on encoding, assumed ascii/ansi
            if (path is null)
                return null;

            obj.Path = path;
            return obj;
        }
    }
}
