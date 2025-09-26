using System.IO;
using System.Text;
using SabreTools.Data.Models.SecuROM;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.SecuROM.Constants;

namespace SabreTools.Serialization.Readers
{
    // TODO: Cache data blocks during parse
    public class SecuROMMatroschkaPackage : BaseBinaryReader<MatroshkaPackage>
    {
        /// <inheritdoc/>
        /// TODO: Unify matroschka spelling to "Matroschka"
        public override MatroshkaPackage? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the initial offset
                long initialOffset = data.Position;

                // Try to parse the header
                var package = ParseMatroshkaPackage(data);
                if (package == null)
                    return null;

                // Try to parse the entries
                package.Entries = ParseEntries(data, package.EntryCount);

                return package;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a MatroshkaPackage
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled MatroshkaPackage on success, null on error</returns>
        public static MatroshkaPackage? ParseMatroshkaPackage(Stream data)
        {
            var obj = new MatroshkaPackage();

            byte[] magic = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(magic);
            if (obj.Signature != MatroshkaMagicString)
                return null;

            obj.EntryCount = data.ReadUInt32LittleEndian();
            if (obj.EntryCount == 0)
                return null;

            // Check if "matrosch" section is a longer header one or not based on whether the next uint is 0 or 1. Anything
            // else will just already be starting the filename string, which is never going to start with this.
            // Previously thought that the longer header was correlated with RC, but at least one executable
            // (NecroVisioN.exe from the GamersGate patch NecroVisioN_Patch1.2_GG.exe) isn't RC and still has it.
            long tempPosition = data.Position;
            uint tempValue = data.ReadUInt32LittleEndian();
            data.Seek(tempPosition, SeekOrigin.Begin);

            // Only 0 or 1 have been observed for long sections
            if (tempValue < 2)
            {
                obj.UnknownRCValue1 = data.ReadUInt32LittleEndian();
                obj.UnknownRCValue2 = data.ReadUInt32LittleEndian();
                obj.UnknownRCValue3 = data.ReadUInt32LittleEndian();

                var keyHexBytes = data.ReadBytes(32);
                obj.KeyHexString = Encoding.ASCII.GetString(keyHexBytes);
                if (!data.ReadBytes(4).EqualsExactly([0x00, 0x00, 0x00, 0x00]))
                    return null;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a MatroshkaEntry array
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="entryCount">Number of entries in the array</param>
        /// <returns>Filled MatroshkaEntry array on success, null on error</returns>
        private static MatroshkaEntry[] ParseEntries(Stream data, uint entryCount)
        {
            var obj = new MatroshkaEntry[entryCount];

            // Determine if file path size is 256 or 512 bytes
            long tempPosition = data.Position;
            data.Seek(data.Position + 256, SeekOrigin.Begin);
            var tempValue = data.ReadUInt32LittleEndian();
            data.Seek(tempPosition, SeekOrigin.Begin);
            int pathSize = tempValue == 0 ? 512 : 256;

            // Set default value for unknown value checking
            bool? hasUnknown = null;

            // Read entries
            for (int i = 0; i < obj.Length; i++)
            {
                var entry = new MatroshkaEntry();

                byte[] pathBytes = data.ReadBytes(pathSize);
                entry.Path = Encoding.ASCII.GetString(pathBytes);
                entry.EntryType = (MatroshkaEntryType)data.ReadUInt32LittleEndian();
                entry.Size = data.ReadUInt32LittleEndian();
                entry.Offset = data.ReadUInt32LittleEndian();

                // On the first entry, determine if the unknown value exists
                if (hasUnknown == null)
                {
                    tempPosition = data.Position;
                    tempValue = data.ReadUInt32LittleEndian();
                    data.Seek(tempPosition, SeekOrigin.Begin);
                    hasUnknown = tempValue == 0;
                }

                // TODO: Validate it's zero?
                if (hasUnknown == true)
                    entry.Unknown = data.ReadUInt32LittleEndian();

                entry.ModifiedTime = data.ReadUInt64LittleEndian();
                entry.CreatedTime = data.ReadUInt64LittleEndian();
                entry.AccessedTime = data.ReadUInt64LittleEndian();
                entry.MD5 = data.ReadBytes(16);

                obj[i] = entry;
            }

            return obj;
        }
    }
}