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
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new Half-Life Texture Package to fill
                var file = new Models.WAD3.File();

                #region Header

                // Try to parse the header
                var header = data.ReadType<Header>();
                if (header?.Signature != SignatureString)
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
                    var lump = data.ReadType<DirEntry>();
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
            catch
            {
                // Ignore the actual error
                return null;
            }
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
            miptex.Palette = new byte[miptex.ColorsUsed][];
            for (int i = 0; i < miptex.ColorsUsed; i++)
            {
                miptex.Palette[i] = data.ReadBytes(3);
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

            mipmap.Data = new byte[width][];
            for (int i = 0; i < width; i++)
            {
                mipmap.Data[i] = data.ReadBytes((int)height);
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
            qpic.Data = new byte[qpic.Height][];
            for (int i = 0; i < qpic.Height; i++)
            {
                qpic.Data[i] = data.ReadBytes((int)qpic.Width);
            }
            qpic.ColorsUsed = data.ReadUInt16();
            qpic.Palette = new byte[qpic.ColorsUsed][];
            for (int i = 0; i < qpic.ColorsUsed; i++)
            {
                qpic.Palette[i] = data.ReadBytes(3);
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
                var fontInfo = data.ReadType<CharInfo>();
                if (fontInfo != null)
                    font.FontInfo[i] = fontInfo;
            }
            font.Data = new byte[font.Height][];
            for (int i = 0; i < font.Height; i++)
            {
                font.Data[i] = data.ReadBytes((int)font.Width);
            }
            font.ColorsUsed = data.ReadUInt16();
            font.Palette = new byte[font.ColorsUsed][];
            for (int i = 0; i < font.ColorsUsed; i++)
            {
                font.Palette[i] = data.ReadBytes(3);
            }

            return font;
        }
    }
}