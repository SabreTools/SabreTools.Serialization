using System.IO;
using System.Text;
using SabreTools.Data.Models.WAD3;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.WAD3.Constants;

namespace SabreTools.Serialization.Readers
{
    public class WAD3 : BaseBinaryDeserializer<Data.Models.WAD3.File>
    {
        /// <inheritdoc/>
        public override Data.Models.WAD3.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new Half-Life Texture Package to fill
                var file = new Data.Models.WAD3.File();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header.Signature != SignatureString)
                    return null;

                // Set the package header
                file.Header = header;

                #endregion

                #region Directory Entries

                // Get the directory offset
                long dirOffset = initialOffset + header.DirOffset;
                if (dirOffset < initialOffset || dirOffset >= data.Length)
                    return null;

                // Seek to the lump offset
                data.Seek(dirOffset, SeekOrigin.Begin);

                // Create the lump array
                file.DirEntries = new DirEntry[header.NumDirs];
                for (int i = 0; i < header.NumDirs; i++)
                {
                    file.DirEntries[i] = ParseDirEntry(data);
                }

                #endregion

                #region File Entries

                // Create the file entry array
                file.FileEntries = new FileEntry[header.NumDirs];
                for (int i = 0; i < header.NumDirs; i++)
                {
                    var dirEntry = file.DirEntries[i];
                    if (dirEntry == null)
                        continue;

                    // TODO: Handle compressed entries
                    if (dirEntry.Compression != 0)
                        continue;

                    // Get the file entry offset
                    long fileEntryOffset = initialOffset + dirEntry.Offset;
                    if (fileEntryOffset < initialOffset || fileEntryOffset >= data.Length)
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
        /// Parse a Stream into a CharInfo
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled CharInfo on success, null on error</returns>
        public static CharInfo ParseCharInfo(Stream data)
        {
            var obj = new CharInfo();

            obj.StartOffset = data.ReadUInt16LittleEndian();
            obj.CharWidth = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirEntry on success, null on error</returns>
        public static DirEntry ParseDirEntry(Stream data)
        {
            var obj = new DirEntry();

            obj.Offset = data.ReadUInt32LittleEndian();
            obj.DiskLength = data.ReadUInt32LittleEndian();
            obj.Length = data.ReadUInt32LittleEndian();
            obj.Type = (FileType)data.ReadByteValue();
            obj.Compression = data.ReadByteValue();
            obj.Padding = data.ReadUInt16LittleEndian();
            byte[] name = data.ReadBytes(16);
            obj.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FileEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="type">File entry type</param>
        /// <returns>Filled FileEntry on success, null on error</returns>
        public static FileEntry? ParseFileEntry(Stream data, FileType type)
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
        /// Parse a Stream into a Font
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Font on success, null on error</returns>
        public static Font ParseFont(Stream data)
        {
            var obj = new Font();

            obj.Width = data.ReadUInt32LittleEndian();
            obj.Height = data.ReadUInt32LittleEndian();
            obj.RowCount = data.ReadUInt32LittleEndian();
            obj.RowHeight = data.ReadUInt32LittleEndian();
            obj.FontInfo = new CharInfo[256];
            for (int i = 0; i < obj.FontInfo.Length; i++)
            {
                obj.FontInfo[i] = ParseCharInfo(data);
            }
            obj.Data = new byte[obj.Height][];
            for (int i = 0; i < obj.Height; i++)
            {
                obj.Data[i] = data.ReadBytes((int)obj.Width);
            }
            obj.ColorsUsed = data.ReadUInt16LittleEndian();
            obj.Palette = new byte[obj.ColorsUsed][];
            for (int i = 0; i < obj.ColorsUsed; i++)
            {
                obj.Palette[i] = data.ReadBytes(3);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header ParseHeader(Stream data)
        {
            var obj = new Header();

            byte[] signature = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.NumDirs = data.ReadUInt32LittleEndian();
            obj.DirOffset = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a MipMap
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled MipMap on success, null on error</returns>
        public static MipMap ParseMipMap(Stream data, uint width, uint height)
        {
            var obj = new MipMap();

            obj.Data = new byte[width][];
            for (int i = 0; i < width; i++)
            {
                obj.Data[i] = data.ReadBytes((int)height);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a MipTex
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled MipTex on success, null on error</returns>
        public static MipTex ParseMipTex(Stream data)
        {
            var obj = new MipTex();

            byte[] nameBytes = data.ReadBytes(16);
            obj.Name = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');
            obj.Width = data.ReadUInt32LittleEndian();
            obj.Height = data.ReadUInt32LittleEndian();
            obj.MipOffsets = new uint[4];
            for (int i = 0; i < obj.MipOffsets.Length; i++)
            {
                obj.MipOffsets[i] = data.ReadUInt32LittleEndian();
            }
            obj.MipImages = new MipMap[4];
            for (int i = 0; i < obj.MipImages.Length; i++)
            {
                obj.MipImages[i] = ParseMipMap(data, obj.Width, obj.Height);
            }
            obj.ColorsUsed = data.ReadUInt16LittleEndian();
            obj.Palette = new byte[obj.ColorsUsed][];
            for (int i = 0; i < obj.ColorsUsed; i++)
            {
                obj.Palette[i] = data.ReadBytes(3);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a QpicImage
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled QpicImage on success, null on error</returns>
        public static QpicImage ParseQpicImage(Stream data)
        {
            var obj = new QpicImage();

            obj.Width = data.ReadUInt32LittleEndian();
            obj.Height = data.ReadUInt32LittleEndian();
            obj.Data = new byte[obj.Height][];
            for (int i = 0; i < obj.Height; i++)
            {
                obj.Data[i] = data.ReadBytes((int)obj.Width);
            }
            obj.ColorsUsed = data.ReadUInt16LittleEndian();
            obj.Palette = new byte[obj.ColorsUsed][];
            for (int i = 0; i < obj.ColorsUsed; i++)
            {
                obj.Palette[i] = data.ReadBytes(3);
            }

            return obj;
        }
    }
}
