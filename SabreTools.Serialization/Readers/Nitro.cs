using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.Nitro;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class Nitro : BaseBinaryReader<Cart>
    {
        /// <inheritdoc/>
        public override Cart? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new cart image to fill
                var cart = new Cart();

                #region Header

                // Set the cart image header
                cart.CommonHeader = ParseCommonHeader(data);

                #endregion

                #region Extended DSi Header

                // If we have a DSi-compatible cartridge
                if (cart.CommonHeader.UnitCode == Unitcode.NDSPlusDSi || cart.CommonHeader.UnitCode == Unitcode.DSi)
                    cart.ExtendedDSiHeader = ParseExtendedDSiHeader(data);

                #endregion

                #region Secure Area

                // Try to get the secure area offset
                long secureAreaOffset = initialOffset + 0x4000;
                if (secureAreaOffset > data.Length)
                    return null;

                // Seek to the secure area
                data.SeekIfPossible(secureAreaOffset, SeekOrigin.Begin);

                // Read the secure area without processing
                cart.SecureArea = data.ReadBytes(0x800);

                #endregion

                #region Name Table

                // Try to get the name table offset
                long nameTableOffset = initialOffset + cart.CommonHeader.FileNameTableOffset;
                if (nameTableOffset < initialOffset || nameTableOffset > data.Length)
                    return null;

                // Seek to the name table
                data.SeekIfPossible(nameTableOffset, SeekOrigin.Begin);

                // Set the name table
                cart.NameTable = ParseNameTable(data);

                #endregion

                #region File Allocation Table

                // Try to get the file allocation table offset
                long fileAllocationTableOffset = initialOffset + cart.CommonHeader.FileAllocationTableOffset;
                if (fileAllocationTableOffset < initialOffset || fileAllocationTableOffset > data.Length)
                    return null;

                // Seek to the file allocation table
                data.SeekIfPossible(fileAllocationTableOffset, SeekOrigin.Begin);

                // Create the file allocation table
                var fileAllocationTable = new List<FileAllocationTableEntry>();

                // Try to parse the file allocation table
                while (data.Position - fileAllocationTableOffset < cart.CommonHeader.FileAllocationTableLength)
                {
                    var entry = ParseFileAllocationTableEntry(data);
                    fileAllocationTable.Add(entry);
                }

                // Set the file allocation table
                cart.FileAllocationTable = [.. fileAllocationTable];

                #endregion

                // TODO: Read and optionally parse out the other areas
                // Look for offsets and lengths in the header pieces

                return cart;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a CommonHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled CommonHeader on success, null on error</returns>
        public static CommonHeader ParseCommonHeader(Stream data)
        {
            var obj = new CommonHeader();

            byte[] gameTitle = data.ReadBytes(12);
            obj.GameTitle = Encoding.ASCII.GetString(gameTitle).TrimEnd('\0');
            obj.GameCode = data.ReadUInt32LittleEndian();
            byte[] makerCode = data.ReadBytes(2);
            obj.MakerCode = Encoding.ASCII.GetString(makerCode);
            obj.UnitCode = (Unitcode)data.ReadByteValue();
            obj.EncryptionSeedSelect = data.ReadByteValue();
            obj.DeviceCapacity = data.ReadByteValue();
            obj.Reserved1 = data.ReadBytes(7);
            obj.GameRevision = data.ReadUInt16LittleEndian();
            obj.RomVersion = data.ReadByteValue();
            obj.InternalFlags = data.ReadByteValue();
            obj.ARM9RomOffset = data.ReadUInt32LittleEndian();
            obj.ARM9EntryAddress = data.ReadUInt32LittleEndian();
            obj.ARM9LoadAddress = data.ReadUInt32LittleEndian();
            obj.ARM9Size = data.ReadUInt32LittleEndian();
            obj.ARM7RomOffset = data.ReadUInt32LittleEndian();
            obj.ARM7EntryAddress = data.ReadUInt32LittleEndian();
            obj.ARM7LoadAddress = data.ReadUInt32LittleEndian();
            obj.ARM7Size = data.ReadUInt32LittleEndian();
            obj.FileNameTableOffset = data.ReadUInt32LittleEndian();
            obj.FileNameTableLength = data.ReadUInt32LittleEndian();
            obj.FileAllocationTableOffset = data.ReadUInt32LittleEndian();
            obj.FileAllocationTableLength = data.ReadUInt32LittleEndian();
            obj.ARM9OverlayOffset = data.ReadUInt32LittleEndian();
            obj.ARM9OverlayLength = data.ReadUInt32LittleEndian();
            obj.ARM7OverlayOffset = data.ReadUInt32LittleEndian();
            obj.ARM7OverlayLength = data.ReadUInt32LittleEndian();
            obj.NormalCardControlRegisterSettings = data.ReadUInt32LittleEndian();
            obj.SecureCardControlRegisterSettings = data.ReadUInt32LittleEndian();
            obj.IconBannerOffset = data.ReadUInt32LittleEndian();
            obj.SecureAreaCRC = data.ReadUInt16LittleEndian();
            obj.SecureTransferTimeout = data.ReadUInt16LittleEndian();
            obj.ARM9Autoload = data.ReadUInt32LittleEndian();
            obj.ARM7Autoload = data.ReadUInt32LittleEndian();
            obj.SecureDisable = data.ReadBytes(8);
            obj.NTRRegionRomSize = data.ReadUInt32LittleEndian();
            obj.HeaderSize = data.ReadUInt32LittleEndian();
            obj.Reserved2 = data.ReadBytes(56);
            obj.NintendoLogo = data.ReadBytes(156);
            obj.NintendoLogoCRC = data.ReadUInt16LittleEndian();
            obj.HeaderCRC = data.ReadUInt16LittleEndian();
            obj.DebuggerReserved = data.ReadBytes(0x20);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExtendedDSiHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExtendedDSiHeader on success, null on error</returns>
        public static ExtendedDSiHeader ParseExtendedDSiHeader(Stream data)
        {
            var obj = new ExtendedDSiHeader();

            obj.GlobalMBK15Settings = new uint[5];
            for (int i = 0; i < 5; i++)
            {
                obj.GlobalMBK15Settings[i] = data.ReadUInt32LittleEndian();
            }
            obj.LocalMBK68SettingsARM9 = new uint[3];
            for (int i = 0; i < 3; i++)
            {
                obj.LocalMBK68SettingsARM9[i] = data.ReadUInt32LittleEndian();
            }
            obj.LocalMBK68SettingsARM7 = new uint[3];
            for (int i = 0; i < 3; i++)
            {
                obj.LocalMBK68SettingsARM7[i] = data.ReadUInt32LittleEndian();
            }
            obj.GlobalMBK9Setting = data.ReadUInt32LittleEndian();
            obj.RegionFlags = data.ReadUInt32LittleEndian();
            obj.AccessControl = data.ReadUInt32LittleEndian();
            obj.ARM7SCFGEXTMask = data.ReadUInt32LittleEndian();
            obj.ReservedFlags = data.ReadUInt32LittleEndian();
            obj.ARM9iRomOffset = data.ReadUInt32LittleEndian();
            obj.Reserved3 = data.ReadUInt32LittleEndian();
            obj.ARM9iLoadAddress = data.ReadUInt32LittleEndian();
            obj.ARM9iSize = data.ReadUInt32LittleEndian();
            obj.ARM7iRomOffset = data.ReadUInt32LittleEndian();
            obj.Reserved4 = data.ReadUInt32LittleEndian();
            obj.ARM7iLoadAddress = data.ReadUInt32LittleEndian();
            obj.ARM7iSize = data.ReadUInt32LittleEndian();
            obj.DigestNTRRegionOffset = data.ReadUInt32LittleEndian();
            obj.DigestNTRRegionLength = data.ReadUInt32LittleEndian();
            obj.DigestTWLRegionOffset = data.ReadUInt32LittleEndian();
            obj.DigestTWLRegionLength = data.ReadUInt32LittleEndian();
            obj.DigestSectorHashtableRegionOffset = data.ReadUInt32LittleEndian();
            obj.DigestSectorHashtableRegionLength = data.ReadUInt32LittleEndian();
            obj.DigestBlockHashtableRegionOffset = data.ReadUInt32LittleEndian();
            obj.DigestBlockHashtableRegionLength = data.ReadUInt32LittleEndian();
            obj.DigestSectorSize = data.ReadUInt32LittleEndian();
            obj.DigestBlockSectorCount = data.ReadUInt32LittleEndian();
            obj.IconBannerSize = data.ReadUInt32LittleEndian();
            obj.Unknown1 = data.ReadUInt32LittleEndian();
            obj.NTRTWLRegionRomSize = data.ReadUInt32LittleEndian();
            obj.Unknown2 = data.ReadBytes(12);
            obj.ModcryptArea1Offset = data.ReadUInt32LittleEndian();
            obj.ModcryptArea1Size = data.ReadUInt32LittleEndian();
            obj.ModcryptArea2Offset = data.ReadUInt32LittleEndian();
            obj.ModcryptArea2Size = data.ReadUInt32LittleEndian();
            obj.TitleID = data.ReadBytes(8);
            obj.DSiWarePublicSavSize = data.ReadUInt32LittleEndian();
            obj.DSiWarePrivateSavSize = data.ReadUInt32LittleEndian();
            obj.ReservedZero = data.ReadBytes(176);
            obj.Unknown3 = data.ReadBytes(16);
            obj.ARM9WithSecureAreaSHA1HMACHash = data.ReadBytes(20);
            obj.ARM7SHA1HMACHash = data.ReadBytes(20);
            obj.DigestMasterSHA1HMACHash = data.ReadBytes(20);
            obj.BannerSHA1HMACHash = data.ReadBytes(20);
            obj.ARM9iDecryptedSHA1HMACHash = data.ReadBytes(20);
            obj.ARM7iDecryptedSHA1HMACHash = data.ReadBytes(20);
            obj.Reserved5 = data.ReadBytes(40);
            obj.ARM9NoSecureAreaSHA1HMACHash = data.ReadBytes(20);
            obj.Reserved6 = data.ReadBytes(2636);
            obj.ReservedAndUnchecked = data.ReadBytes(0x180);
            obj.RSASignature = data.ReadBytes(0x80);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FileAllocationTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FileAllocationTableEntry on success, null on error</returns>
        public static FileAllocationTableEntry ParseFileAllocationTableEntry(Stream data)
        {
            var obj = new FileAllocationTableEntry();

            obj.StartOffset = data.ReadUInt32LittleEndian();
            obj.EndOffset = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FolderAllocationTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FolderAllocationTableEntry on success, null on error</returns>
        public static FolderAllocationTableEntry ParseFolderAllocationTableEntry(Stream data)
        {
            var obj = new FolderAllocationTableEntry();

            obj.StartOffset = data.ReadUInt32LittleEndian();
            obj.FirstFileIndex = data.ReadUInt16LittleEndian();
            obj.ParentFolderIndex = data.ReadByteValue();
            obj.Unknown = data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a NameListEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NameListEntry on success, null on error</returns>
        public static NameListEntry? ParseNameListEntry(Stream data)
        {
            var entry = new NameListEntry();

            byte flagAndSize = data.ReadByteValue();
            if (flagAndSize == 0xFF)
                return null;

            entry.Folder = (flagAndSize & 0x80) != 0;

            byte size = (byte)(flagAndSize & ~0x80);
            if (size > 0)
            {
                byte[] name = data.ReadBytes(size);
                entry.Name = Encoding.UTF8.GetString(name);
            }

            if (entry.Folder)
                entry.Index = data.ReadUInt16LittleEndian();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a name table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled name table on success, null on error</returns>
        public static NameTable ParseNameTable(Stream data)
        {
            var nameTable = new NameTable();

            // Create a variable-length table
            var folderAllocationTable = new List<FolderAllocationTableEntry>();
            int entryCount = int.MaxValue;
            while (entryCount > 0)
            {
                var entry = ParseFolderAllocationTableEntry(data);
                folderAllocationTable.Add(entry);

                // If we have the root entry
                if (entryCount == int.MaxValue)
                    entryCount = (entry.Unknown << 8) | entry.ParentFolderIndex;

                // Decrement the entry count
                entryCount--;
            }

            // Assign the folder allocation table
            nameTable.FolderAllocationTable = [.. folderAllocationTable];

            // Create a variable-length table
            var nameList = new List<NameListEntry>();
            while (true)
            {
                var entry = ParseNameListEntry(data);
                if (entry == null)
                    break;

                nameList.Add(entry);
            }

            // Assign the name list
            nameTable.NameList = [.. nameList];

            return nameTable;
        }
    }
}
