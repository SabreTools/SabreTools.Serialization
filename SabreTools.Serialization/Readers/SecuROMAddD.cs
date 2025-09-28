using System.IO;
using System.Text;
using SabreTools.Data.Models.SecuROM;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.SecuROM.Constants;

namespace SabreTools.Serialization.Readers
{
    public class SecuROMAddD : BaseBinaryReader<AddD>
    {
        /// <inheritdoc/>
        public override AddD? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                var addD = ParseAddD(data);
                if (addD.Signature != AddDMagicString)
                    return null;

                return addD;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an AddD
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled AddD on success, null on error</returns>
        private static AddD ParseAddD(Stream data)
        {
            var obj = new AddD();

            byte[] signatureBytes = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(signatureBytes);
            obj.EntryCount = data.ReadUInt32LittleEndian();
            obj.Version = data.ReadNullTerminatedAnsiString();
            byte[] buildBytes = data.ReadBytes(4);
            obj.Build = Encoding.ASCII.GetString(buildBytes);

            // TODO: Figure out how to determine how many bytes are here consistently
            obj.Unknown = data.ReadBytes(1);

            obj.Entries = new AddDEntry[obj.EntryCount];
            for (int i = 0; i < obj.Entries.Length; i++)
            {
                var entry = ParseAddDEntry(data);
                obj.Entries[i] = entry;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an AddDEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled AddDEntry on success, null on error</returns>
        private static AddDEntry ParseAddDEntry(Stream data)
        {
            var obj = new AddDEntry();

            obj.PhysicalOffset = data.ReadUInt32LittleEndian();
            obj.Length = data.ReadUInt32LittleEndian();
            obj.Unknown08h = data.ReadUInt32LittleEndian();
            obj.Unknown0Ch = data.ReadUInt32LittleEndian();
            obj.Unknown10h = data.ReadUInt32LittleEndian();
            obj.Unknown14h = data.ReadUInt32LittleEndian();
            obj.Unknown18h = data.ReadUInt32LittleEndian();
            obj.Unknown1Ch = data.ReadUInt32LittleEndian();
            obj.FileName = data.ReadNullTerminatedAnsiString();
            obj.Unknown2Ch = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
