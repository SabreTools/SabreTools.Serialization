using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.WAD3;
using static SabreTools.Models.WAD3.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class WAD3 : BaseBinaryDeserializer<Models.WAD3.File>
    {
        /// <inheritdoc/>
        public override Models.WAD3.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Create a new Half-Life Texture Package to fill
            var file = new Models.WAD3.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            #region Directory Entries

            // Get the directory offset
            uint dirOffset = header.DirOffset;
            if (dirOffset < 0 || dirOffset >= data.Length)
                return null;

            // Seek to the lump offset
            data.Seek(dirOffset, SeekOrigin.Begin);

            // Create the lump array
            file.DirEntries = new DirEntry[header.NumDirs];
            for (int i = 0; i < header.NumDirs; i++)
            {
                var lump = ParseDirEntry(data);
                if (lump == null)
                    return null;

                file.DirEntries[i] = lump;
            }

            #endregion

            #region File Entries

            // Create the file entry array
            file.FileEntries = new FileEntry?[header.NumDirs];
            for (int i = 0; i < header.NumDirs; i++)
            {
                var dirEntry = file.DirEntries[i];
                if (dirEntry == null)
                    continue;

                // TODO: Handle compressed entries
                if (dirEntry.Compression != 0)
                    continue;

                // Get the file entry offset
                uint fileEntryOffset = dirEntry.Offset;
                if (fileEntryOffset < 0 || fileEntryOffset >= data.Length)
                    continue;

                // Seek to the file entry offset
                data.Seek(fileEntryOffset, SeekOrigin.Begin);

                // Try to parse the file entry
                var fileEntry = ParseFileEntry(data, dirEntry.Type);
                if (fileEntry != null)
                    file.FileEntries[i] = fileEntry;
            }

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Texture Package header on success, null on error</returns>
        private static Header? ParseHeader(Stream data)
        {
            var header = data.ReadType<Header>();

            if (header == null)
                return null;
            if (header.Signature != SignatureString)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package directory entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Texture Package directory entry on success, null on error</returns>
        private static DirEntry? ParseDirEntry(Stream data)
        {
            return data.ReadType<DirEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package file entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="type">File entry type</param>
        /// <returns>Filled Half-Life Texture Package file entry on success, null on error</returns>
        private static FileEntry? ParseFileEntry(Stream data, FileType type)
        {
            return type switch
            {
                FileType.Spraydecal
                    or FileType.Miptex => ParseMipTex(data),
                FileType.Qpic => ParseQpicImage(data),
                FileType.Font => ParseFont(data),
                _ => null,
            };
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package MipTex
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Texture Package MipTex on success, null on error</returns>
        private static MipTex ParseMipTex(Stream data)
        {
            var miptex = new MipTex();

            byte[] nameBytes = data.ReadBytes(16);
            miptex.Name = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');
            miptex.Width = data.ReadUInt32();
            miptex.Height = data.ReadUInt32();
            miptex.MipOffsets = new uint[4];
            for (int i = 0; i < miptex.MipOffsets.Length; i++)
            {
                miptex.MipOffsets[i] = data.ReadUInt32();
            }
            miptex.MipImages = new MipMap[4];
            for (int i = 0; i < miptex.MipImages.Length; i++)
            {
                miptex.MipImages[i] = ParseMipMap(data, miptex.Width, miptex.Height);
            }
            miptex.ColorsUsed = data.ReadUInt16();
            miptex.Palette = new byte[miptex.ColorsUsed, 3];
            for (int i = 0; i < miptex.ColorsUsed; i++)
            for (int j = 0; j < 3; j++)
            {
                miptex.Palette[i, j] = data.ReadByteValue();
            }

            return miptex;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package MipMap
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Texture Package MipMap on success, null on error</returns>
        private static MipMap ParseMipMap(Stream data, uint width, uint height)
        {
            var mipmap = new MipMap();

            mipmap.Data = new byte[width, height];
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                mipmap.Data[i, j] = data.ReadByteValue();
            }

            return mipmap;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package Qpic image
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Texture Package Qpic image on success, null on error</returns>
        private static QpicImage ParseQpicImage(Stream data)
        {
            var qpic = new QpicImage();

            qpic.Width = data.ReadUInt32();
            qpic.Height = data.ReadUInt32();
            qpic.Data = new byte[qpic.Height, qpic.Width];
            for (int i = 0; i < qpic.Height; i++)
            for (int j = 0; j < qpic.Width; j++)
            {
                qpic.Data[i, j] = data.ReadByteValue();
            }
            qpic.ColorsUsed = data.ReadUInt16();
            qpic.Palette = new byte[qpic.ColorsUsed, 3];
            for (int i = 0; i < qpic.ColorsUsed; i++)
            for (int j = 0; j < 3; j++)
            {
                qpic.Palette[i, j] = data.ReadByteValue();
            }

            return qpic;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package font
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Texture Package font on success, null on error</returns>
        private static Font ParseFont(Stream data)
        {
            var font = new Font();

            font.Width = data.ReadUInt32();
            font.Height = data.ReadUInt32();
            font.RowCount = data.ReadUInt32();
            font.RowHeight = data.ReadUInt32();
            font.FontInfo = new CharInfo[256];
            for (int i = 0; i < font.FontInfo.Length; i++)
            {
                font.FontInfo[i] = ParseCharInfo(data);
            }
            font.Data = new byte[font.Height, font.Width];
            for (int i = 0; i < font.Height; i++)
            for (int j = 0; j < font.Width; j++)
            {
                font.Data[i, j] = data.ReadByteValue();
            }
            font.ColorsUsed = data.ReadUInt16();
            font.Palette = new byte[font.ColorsUsed, 3];
            for (int i = 0; i < font.ColorsUsed; i++)
            for (int j = 0; j < 3; j++)
            {
                font.Palette[i, j] = data.ReadByteValue();
            }

            return font;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Texture Package CharInfo
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Texture Package CharInfo on success, null on error</returns>
        private static CharInfo ParseCharInfo(Stream data)
        {
            var charinfo = new CharInfo();

            charinfo.StartOffset = data.ReadUInt16();
            charinfo.CharWidth = data.ReadUInt16();

            return charinfo;
        }
    }
}