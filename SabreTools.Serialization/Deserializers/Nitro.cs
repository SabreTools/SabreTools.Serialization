using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.Nitro;

namespace SabreTools.Serialization.Deserializers
{
    public class Nitro : BaseBinaryDeserializer<Cart>
    {
        /// <inheritdoc/>
        public override Cart? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Create a new cart image to fill
            var cart = new Cart();

            #region Header

            // Try to parse the header
            var header = data.ReadType<CommonHeader>();
            if (header == null)
                return null;

            // Set the cart image header
            cart.CommonHeader = header;

            #endregion

            #region Extended DSi Header

            // If we have a DSi-compatible cartridge
            if (header.UnitCode == Unitcode.NDSPlusDSi || header.UnitCode == Unitcode.DSi)
            {
                var extendedDSiHeader = data.ReadType<ExtendedDSiHeader>();
                if (extendedDSiHeader == null)
                    return null;

                cart.ExtendedDSiHeader = extendedDSiHeader;
            }

            #endregion

            #region Secure Area

            // Try to get the secure area offset
            long secureAreaOffset = 0x4000;
            if (secureAreaOffset > data.Length)
                return null;

            // Seek to the secure area
            data.Seek(secureAreaOffset, SeekOrigin.Begin);

            // Read the secure area without processing
            cart.SecureArea = data.ReadBytes(0x800);

            #endregion

            #region Name Table

            // Try to get the name table offset
            long nameTableOffset = header.FileNameTableOffset;
            if (nameTableOffset < 0 || nameTableOffset > data.Length)
                return null;

            // Seek to the name table
            data.Seek(nameTableOffset, SeekOrigin.Begin);

            // Try to parse the name table
            var nameTable = ParseNameTable(data);
            if (nameTable == null)
                return null;

            // Set the name table
            cart.NameTable = nameTable;

            #endregion

            #region File Allocation Table

            // Try to get the file allocation table offset
            long fileAllocationTableOffset = header.FileAllocationTableOffset;
            if (fileAllocationTableOffset < 0 || fileAllocationTableOffset > data.Length)
                return null;

            // Seek to the file allocation table
            data.Seek(fileAllocationTableOffset, SeekOrigin.Begin);

            // Create the file allocation table
            var fileAllocationTable = new List<FileAllocationTableEntry>();

            // Try to parse the file allocation table
            while (data.Position - fileAllocationTableOffset < header.FileAllocationTableLength)
            {
                var entry = data.ReadType<FileAllocationTableEntry>();
                if (entry == null)
                    return null;
                
                fileAllocationTable.Add(entry);
            }

            // Set the file allocation table
            cart.FileAllocationTable = [.. fileAllocationTable];

            #endregion

            // TODO: Read and optionally parse out the other areas
            // Look for offsets and lengths in the header pieces

            return cart;
        }

        /// <summary>
        /// Parse a Stream into a name table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled name table on success, null on error</returns>
        private static NameTable? ParseNameTable(Stream data)
        {
            var nameTable = new NameTable();

            // Create a variable-length table
            var folderAllocationTable = new List<FolderAllocationTableEntry>();
            int entryCount = int.MaxValue;
            while (entryCount > 0)
            {
                var entry = data.ReadType<FolderAllocationTableEntry>();
                if (entry == null)
                    return null;
                
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

        /// <summary>
        /// Parse a Stream into a name list entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled name list entry on success, null on error</returns>
        private static NameListEntry? ParseNameListEntry(Stream data)
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
                entry.Index = data.ReadUInt16();

            return entry;
        }
    }
}