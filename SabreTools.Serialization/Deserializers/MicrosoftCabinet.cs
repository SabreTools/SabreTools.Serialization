using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Models.MicrosoftCabinet;
using static SabreTools.Serialization.Models.MicrosoftCabinet.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class MicrosoftCabinet : BaseBinaryDeserializer<Cabinet>
    {
        /// <inheritdoc/>
        public override Cabinet? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new cabinet to fill
                var cabinet = new Cabinet();

                #region Cabinet Header

                // Try to parse the cabinet header
                var cabinetHeader = ParseCabinetHeader(data);
                if (cabinetHeader == null)
                    return null;

                // Set the cabinet header
                cabinet.Header = cabinetHeader;

                #endregion

                #region Folders

                // Set the folder array
                cabinet.Folders = new CFFOLDER[cabinetHeader.FolderCount];

                // Try to parse each folder, if we have any
                for (int i = 0; i < cabinetHeader.FolderCount; i++)
                {
                    var folder = ParseFolder(data, cabinetHeader);
                    if (folder == null)
                        return null;

                    // Set the folder
                    cabinet.Folders[i] = folder;
                }

                #endregion

                #region Files

                // Get the files offset
                long filesOffset = initialOffset + cabinetHeader.FilesOffset;
                if (filesOffset > data.Length)
                    return null;

                // Seek to the offset
                data.Seek(filesOffset, SeekOrigin.Begin);

                // Set the file array
                cabinet.Files = new CFFILE[cabinetHeader.FileCount];

                // Try to parse each file, if we have any
                for (int i = 0; i < cabinetHeader.FileCount; i++)
                {
                    var file = ParseFile(data);
                    if (file == null)
                        return null;

                    // Set the file
                    cabinet.Files[i] = file;
                }

                #endregion

                return cabinet;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a cabinet header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled cabinet header on success, null on error</returns>
        private static CFHEADER? ParseCabinetHeader(Stream data)
        {
            var header = new CFHEADER();

            byte[] signature = data.ReadBytes(4);
            header.Signature = Encoding.ASCII.GetString(signature);
            if (header.Signature != SignatureString)
                return null;

            header.Reserved1 = data.ReadUInt32LittleEndian();
            header.CabinetSize = data.ReadUInt32LittleEndian();
            header.Reserved2 = data.ReadUInt32LittleEndian();
            header.FilesOffset = data.ReadUInt32LittleEndian();
            header.Reserved3 = data.ReadUInt32LittleEndian();
            header.VersionMinor = data.ReadByteValue();
            header.VersionMajor = data.ReadByteValue();
            header.FolderCount = data.ReadUInt16LittleEndian();
            header.FileCount = data.ReadUInt16LittleEndian();
            header.Flags = (HeaderFlags)data.ReadUInt16LittleEndian();
            header.SetID = data.ReadUInt16LittleEndian();
            header.CabinetIndex = data.ReadUInt16LittleEndian();

#if NET20 || NET35
            if ((header.Flags & HeaderFlags.RESERVE_PRESENT) != 0)
#else
            if (header.Flags.HasFlag(HeaderFlags.RESERVE_PRESENT))
#endif
            {
                header.HeaderReservedSize = data.ReadUInt16LittleEndian();
                if (header.HeaderReservedSize > 60_000)
                    return null;

                header.FolderReservedSize = data.ReadByteValue();
                header.DataReservedSize = data.ReadByteValue();

                if (header.HeaderReservedSize > 0)
                    header.ReservedData = data.ReadBytes(header.HeaderReservedSize);
            }

#if NET20 || NET35
            if ((header.Flags & HeaderFlags.PREV_CABINET) != 0)
#else
            if (header.Flags.HasFlag(HeaderFlags.PREV_CABINET))
#endif
            {
                header.CabinetPrev = data.ReadNullTerminatedAnsiString();
                header.DiskPrev = data.ReadNullTerminatedAnsiString();
            }

#if NET20 || NET35
            if ((header.Flags & HeaderFlags.NEXT_CABINET) != 0)
#else
            if (header.Flags.HasFlag(HeaderFlags.NEXT_CABINET))
#endif
            {
                header.CabinetNext = data.ReadNullTerminatedAnsiString();
                header.DiskNext = data.ReadNullTerminatedAnsiString();
            }

            return header;
        }

        /// <summary>
        /// Parse a Stream into a folder
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="header">Cabinet header to get flags and sizes from</param>
        /// <returns>Filled folder on success, null on error</returns>
        private static CFFOLDER ParseFolder(Stream data, CFHEADER header)
        {
            var folder = new CFFOLDER();

            folder.CabStartOffset = data.ReadUInt32LittleEndian();
            folder.DataCount = data.ReadUInt16LittleEndian();
            folder.CompressionType = (CompressionType)data.ReadUInt16LittleEndian();

            if (header.FolderReservedSize > 0)
                folder.ReservedData = data.ReadBytes(header.FolderReservedSize);

            if (folder.CabStartOffset > 0)
            {
                long currentPosition = data.Position;
                data.Seek(folder.CabStartOffset, SeekOrigin.Begin);

                folder.DataBlocks = new CFDATA[folder.DataCount];
                for (int i = 0; i < folder.DataCount; i++)
                {
                    CFDATA dataBlock = ParseDataBlock(data, header.DataReservedSize);
                    folder.DataBlocks[i] = dataBlock;
                }

                data.Seek(currentPosition, SeekOrigin.Begin);
            }

            return folder;
        }

        /// <summary>
        /// Parse a Stream into a data block
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="dataReservedSize">Reserved byte size for data blocks</param>
        /// <returns>Filled folder on success, null on error</returns>
        private static CFDATA ParseDataBlock(Stream data, byte dataReservedSize)
        {
            var dataBlock = new CFDATA();

            dataBlock.Checksum = data.ReadUInt32LittleEndian();
            dataBlock.CompressedSize = data.ReadUInt16LittleEndian();
            dataBlock.UncompressedSize = data.ReadUInt16LittleEndian();

            if (dataReservedSize > 0)
                dataBlock.ReservedData = data.ReadBytes(dataReservedSize);

            if (dataBlock.CompressedSize > 0)
                dataBlock.CompressedData = data.ReadBytes(dataBlock.CompressedSize);

            return dataBlock;
        }

        /// <summary>
        /// Parse a Stream into a file
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled file on success, null on error</returns>
        private static CFFILE ParseFile(Stream data)
        {
            var file = new CFFILE();

            file.FileSize = data.ReadUInt32LittleEndian();
            file.FolderStartOffset = data.ReadUInt32LittleEndian();
            file.FolderIndex = (FolderIndex)data.ReadUInt16LittleEndian();
            file.Date = data.ReadUInt16LittleEndian();
            file.Time = data.ReadUInt16LittleEndian();
            file.Attributes = (SabreTools.Serialization.Models.MicrosoftCabinet.FileAttributes)data.ReadUInt16LittleEndian();

#if NET20 || NET35
            if ((file.Attributes & SabreTools.Serialization.Models.MicrosoftCabinet.FileAttributes.NAME_IS_UTF) != 0)
#else
            if (file.Attributes.HasFlag(SabreTools.Serialization.Models.MicrosoftCabinet.FileAttributes.NAME_IS_UTF))
#endif
                file.Name = data.ReadNullTerminatedUnicodeString();
            else
                file.Name = data.ReadNullTerminatedAnsiString();

            return file;
        }
    }
}
