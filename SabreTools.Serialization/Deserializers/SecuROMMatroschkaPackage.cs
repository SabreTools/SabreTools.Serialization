using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Models.SecuROM;
using static SabreTools.Models.SecuROM.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class SecuROMMatroschkaPackage : BaseBinaryDeserializer<MatroshkaPackage>
    {
        /// <inheritdoc/>
        public override MatroshkaPackage? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the initial offset
                long initialOffset = data.Position;
                
                // TODO: Unify matroschka spelling. They spell it matroschka in all official stuff, as far as has been observed. Will double check.
                // Try to parse the header
                var package = ParsePreEntryHeader(data);
                if (package == null)
                    return null;

                var entries = ParseEntries(data, package);
                if (entries == null)
                    return null;
                
                package.Entries = entries;
                return package;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        public MatroshkaPackage? ParsePreEntryHeader(Stream data)
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
            data.Position = tempPosition;

            if (tempValue < 2) // Only big-endian 0 or 1 have been observed for long sections.
            {
                obj.UnknownRCValue1 = data.ReadUInt32LittleEndian();
                obj.UnknownRCValue2 = data.ReadUInt32LittleEndian();
                obj.UnknownRCValue3 = data.ReadUInt32LittleEndian();

                // Exact byte count has to be used because non-RC executables have all 0x00 here.
                var keyHexBytes = data.ReadBytes(32);
                obj.KeyHexString = Encoding.ASCII.GetString(keyHexBytes);
                if (!data.ReadBytes(4).EqualsExactly([0x00, 0x00, 0x00, 0x00]))
                    return null;
            }
            return obj;
        }

        public MatroshkaEntry[]? ParseEntries(Stream data, MatroshkaPackage package)
        {
                
                // If we have any entries
                var entries = new MatroshkaEntry[package.EntryCount];

                int matGapType = 0;
                bool? matHasUnknown = null;
                
                // Read entries
                for (int i = 0; i < entries.Length; i++) 
                {
                    MatroshkaEntry entry = new MatroshkaEntry();
                    // Determine if file path size is 256 or 512 bytes
                    if (matGapType == 0)
                        matGapType = GapHelper(data);
                                      
                    // TODO: Spaces/non-ASCII have not yet been observed. Still, probably safer to store as byte array?
                    // TODO: Read as string and trim once models is bumped. For now, this needs to be trimmed by anything reading it.
                    entry.Path = data.ReadBytes((int)matGapType); 
                    
                    // Entry type isn't currently validated as it's always predictable anyways, nor necessary to know.
                    entry.EntryType = (MatroshkaEntryType)data.ReadUInt32LittleEndian();
                    entry.Size = data.ReadUInt32LittleEndian();
                    entry.Offset = data.ReadUInt32LittleEndian();
                    
                    // Check for unknown 4-byte 0x00 value. Not correlated with 256 vs 512-byte gaps.
                    if (matHasUnknown == null)
                        matHasUnknown = UnknownHelper(data, entry);
                    
                    if (matHasUnknown == true) // If already known, read or don't read the unknown value.
                        entry.Unknown = data.ReadUInt32LittleEndian(); // TODO: Validate it's zero?
                                       
                    entry.ModifiedTime = data.ReadUInt64LittleEndian();
                    entry.CreatedTime = data.ReadUInt64LittleEndian();
                    entry.AccessedTime = data.ReadUInt64LittleEndian();
                    entry.MD5 = data.ReadBytes(16);
                    entries[i] = entry;
                }

                return entries;
        }

        private static int GapHelper(Stream data)
        {
            var tempPosition = data.Position;
            data.Position = tempPosition + 256;
            var tempValue = data.ReadUInt32LittleEndian();
            data.Position = tempPosition;
            if (tempValue <= 0) // Gap is 512 bytes. Actually just == 0, but ST prefers ranges.
                return 512;
            
            // Otherwise, gap is 256 bytes. 
            return 256;
        }
        
        private static bool UnknownHelper(Stream data, MatroshkaEntry entry)
        {
            var tempPosition = data.Position;
            var tempValue = data.ReadUInt32LittleEndian();
            data.Position = tempPosition;

            if (tempValue > 0) // Entry does not have the Unknown value.
                return false;

            // Entry does have the unknown value.
            return true; 
        }
    }
}