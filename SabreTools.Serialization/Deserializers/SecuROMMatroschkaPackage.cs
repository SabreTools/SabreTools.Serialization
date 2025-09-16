using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Models.SecuROM;
using static SabreTools.Models.SecuROM.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public enum MatroschkaHeaderType : int
    {
        Error = -1,
        ShortHeader = 0,
        LongHeader = 1,
    }
    
    public enum MatroschkaGapType : int
    {
        Error = -1,
        ShortGap = 0, // 256 bytes
        LongGap = 1, // 512 bytes
    }
    
    public enum MatroschkaHasUnknown : int
    {
        Error = -1,
        NoUnknown = 0,
        HasUnknown = 1,
    }
    
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
                long startPosition = data.Position;
                // Create a new file to fill
                // TODO: Unify matroschka spelling. They spell it matroschka in all official stuff, as far as has been observed. Will double check.
                var matroschka = new MatroshkaPackage();

                #region Header

                // Try to parse the header
                byte[] magic = data.ReadBytes(4);
                matroschka.Signature = Encoding.ASCII.GetString(magic);
                if (matroschka.Signature != MatroshkaMagicString)
                    return null;

                matroschka.EntryCount = data.ReadUInt32LittleEndian();
                if (matroschka.EntryCount == 0)
                    return null; // TODO: This should never occur, log output should happen even without debug.
                
                // Check if "matrosch" section is a longer header one or not based on whether the next uint is 0 or 1. Anything
                // else will just already be starting the filename string, which is never going to start with this.
                // Previously thought that the longer header was correlated with RC, but at least one executable
                // (NecroVisioN.exe from the GamersGate patch NecroVisioN_Patch1.2_GG.exe) isn't RC and still has it.
                long tempPosition = data.Position;
                uint tempValue = data.ReadUInt32LittleEndian();
                data.Position = tempPosition;
                
                MatroschkaHeaderType matHeaderType;
                if (tempValue < 2) // Only big-endian 0 or 1 have been observed for long sections.
                {
                    matHeaderType = MatroschkaHeaderType.LongHeader;
                    matroschka.UnknownRCValue1 = data.ReadUInt32LittleEndian();
                    matroschka.UnknownRCValue2 = data.ReadUInt32LittleEndian();
                    matroschka.UnknownRCValue3 = data.ReadUInt32LittleEndian();

                    // TODO: Not actually reliable for distinguishing keys, update models documentation to reflect.
                    // Exact byte count has to be used because non-RC executables have all 0x00 here.
                    matroschka.KeyHexString = Encoding.ASCII.GetString(data.ReadBytes(32));
                    if (!data.ReadBytes(4).EqualsExactly([0x00, 0x00, 0x00, 0x00]))
                        return null; // TODO: This should never occur, log output should happen even without debug.
                }
                else
                    matHeaderType = MatroschkaHeaderType.ShortHeader;

                if (matHeaderType > MatroschkaHeaderType.LongHeader)
                    return null; // Only here as a placeholder before this value gets used elsewhere to avoid compiler error.

                #endregion

                #region Entries

                // If we have any entries
                MatroshkaEntry[] entries = new MatroshkaEntry[matroschka.EntryCount];

                MatroschkaGapType matGapType = MatroschkaGapType.Error;
                MatroschkaHasUnknown matHasUnknown = MatroschkaHasUnknown.Error;
                
                // Read entries
                for (int i = 0; i < entries.Length; i++) 
                {
                    MatroshkaEntry entry = new MatroshkaEntry();
                    tempPosition = data.Position;
                    // TODO: Spaces/non-ASCII have not yet been observed. Still, probably safer to store as byte array?
                    string? pathString = data.ReadNullTerminatedString(Encoding.ASCII);
                    if (pathString != null)
                        entry.Path = Encoding.ASCII.GetBytes(pathString);
                    else
                        return null; // TODO: This should never occur, log output should happen even without debug.
                    data.Position = tempPosition;
                    
                    // Determine if gap size is 256 or 512 bytes, then jump the gap.
                    if (matGapType == MatroschkaGapType.Error)
                    {
                        data.Position = tempPosition + 256;
                        tempPosition = data.Position;
                        tempValue = data.ReadUInt32LittleEndian();
                        data.Position = tempPosition;
                        if (tempValue <= 0) // Gap is 512 bytes. Actually just == 0, but ST prefers ranges.
                        {
                            matGapType = MatroschkaGapType.LongGap;
                            data.Position += 256;
                        }
                        else // Gap is 256 bytes.
                            matGapType = MatroschkaGapType.ShortGap;
                        
                    } // If already known, simply jump the gap
                    else if (matGapType == MatroschkaGapType.ShortGap)
                        data.Position += 256;
                    else if (matGapType == MatroschkaGapType.LongGap)
                        data.Position += 512;
                    else
                        return null; // TODO: This should never occur, log output should happen even without debug.
                    
                    // Entry type isn't currently validated as it's always predictable anyways, nor necessary to know.
                    entry.EntryType = (MatroshkaEntryType)data.ReadUInt32LittleEndian();
                    entry.Size = data.ReadUInt32LittleEndian();
                    entry.Offset = data.ReadUInt32LittleEndian();
                    
                    // Check for unknown 4-byte 0x00 value. Not correlated with 256 vs 512-byte gaps.
                    if (matHasUnknown == MatroschkaHasUnknown.Error)
                    {
                        tempPosition = data.Position;
                        tempValue = data.ReadUInt32LittleEndian();
                        if (tempValue <= 0) // Entry has the Unknown value.
                        {
                            matHasUnknown = MatroschkaHasUnknown.HasUnknown;
                            entry.Unknown = tempValue;
                        } 
                        else
                        {
                            matHasUnknown = MatroschkaHasUnknown.NoUnknown;
                            data.Position = tempPosition;
                        }
                        
                    } // If already known, read or don't read the unknown value.
                    else if (matHasUnknown == MatroschkaHasUnknown.HasUnknown)
                        entry.Unknown = data.ReadUInt32LittleEndian(); // TODO: Validate it's zero?
                    else if (matHasUnknown != MatroschkaHasUnknown.NoUnknown)
                        return null; // TODO: This should never occur, log output should happen even without debug.
                    
                    entry.ModifiedTime = data.ReadUInt64LittleEndian();
                    entry.CreatedTime = data.ReadUInt64LittleEndian();
                    entry.AccessedTime = data.ReadUInt64LittleEndian();
                    entry.MD5 = data.ReadBytes(16);
                    tempPosition = data.Position;
                    data.Position = startPosition + entry.Offset;
                    entry.FileData = data.ReadBytes((int)entry.Size); // TODO: Handle out of bounds reading, other errors?
                    data.Position = tempPosition;
                    entries[i] = entry;
                }
                
                matroschka.Entries = entries;
                
                #endregion

                return matroschka;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}