using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.InstallShieldArchiveV3;

namespace SabreTools.Serialization.Deserializers
{
    public class InstallShieldArchiveV3 : BaseBinaryDeserializer<Archive>
    {
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new archive to fill
                var archive = new Archive();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header.Signature1 != Constants.HeaderSignature)
                    return null;
                if (header.TocAddress >= data.Length)
                    return null;

                // Set the archive header
                archive.Header = header;

                #endregion

                #region Directories

                // Get the directories offset
                uint directoriesOffset = header.TocAddress;
                if (directoriesOffset < 0 || directoriesOffset >= data.Length)
                    return null;

                // Seek to the directories
                data.Seek(directoriesOffset, SeekOrigin.Begin);

                // Try to parse the directories
                var directories = new List<Models.InstallShieldArchiveV3.Directory>();
                for (int i = 0; i < header.DirCount; i++)
                {
                    var directory = ParseDirectory(data);
                    directories.Add(directory);
                    data.Seek(directory.ChunkSize - directory.Name!.Length - 6, SeekOrigin.Current);
                }

                // Set the directories
                archive.Directories = [.. directories];

                #endregion

                #region Files

                // Try to parse the files
                var files = new List<Models.InstallShieldArchiveV3.File>();
                for (int i = 0; i < archive.Directories.Length; i++)
                {
                    var directory = archive.Directories[i];
                    for (int j = 0; j < directory.FileCount; j++)
                    {
                        var file = ParseFile(data);
                        files.Add(file);
                        data.Seek(file.ChunkSize - file.Name!.Length - 30, SeekOrigin.Current);
                    }
                }

                // Set the files
                archive.Files = [.. files];

                #endregion

                return archive;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a Directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Directory on success, null on error</returns>
        public static Models.InstallShieldArchiveV3.Directory ParseDirectory(Stream data)
        {
            var obj = new Models.InstallShieldArchiveV3.Directory();

            obj.FileCount = data.ReadUInt16();
            obj.ChunkSize = data.ReadUInt16();

            ushort nameLength = data.ReadUInt16();
            byte[] nameBytes = data.ReadBytes(nameLength);
            obj.Name = Encoding.ASCII.GetString(nameBytes);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a File
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled File on success, null on error</returns>
        public static Models.InstallShieldArchiveV3.File ParseFile(Stream data)
        {
            var obj = new Models.InstallShieldArchiveV3.File();

            obj.VolumeEnd = data.ReadByteValue();
            obj.Index = data.ReadUInt16LittleEndian();
            obj.UncompressedSize = data.ReadUInt32LittleEndian();
            obj.CompressedSize = data.ReadUInt32LittleEndian();
            obj.Offset = data.ReadUInt32LittleEndian();
            obj.DateTime = data.ReadUInt32LittleEndian();
            obj.Reserved0 = data.ReadUInt32LittleEndian();
            obj.ChunkSize = data.ReadUInt16LittleEndian();
            obj.Attrib = (Attributes)data.ReadByteValue();
            obj.IsSplit = data.ReadByteValue();
            obj.Reserved1 = data.ReadByteValue();
            obj.VolumeStart = data.ReadByteValue();
            obj.Name = data.ReadPrefixedAnsiString();

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

            obj.Signature1 = data.ReadUInt32LittleEndian();
            obj.Signature2 = data.ReadUInt32LittleEndian();
            obj.Reserved0 = data.ReadUInt16LittleEndian();
            obj.IsMultivolume = data.ReadUInt16LittleEndian();
            obj.FileCount = data.ReadUInt16LittleEndian();
            obj.DateTime = data.ReadUInt32LittleEndian();
            obj.CompressedSize = data.ReadUInt32LittleEndian();
            obj.UncompressedSize = data.ReadUInt32LittleEndian();
            obj.Reserved1 = data.ReadUInt32LittleEndian();
            obj.VolumeTotal = data.ReadByteValue();
            obj.VolumeNumber = data.ReadByteValue();
            obj.Reserved2 = data.ReadByteValue();
            obj.SplitBeginAddress = data.ReadUInt32LittleEndian();
            obj.SplitEndAddress = data.ReadUInt32LittleEndian();
            obj.TocAddress = data.ReadUInt32LittleEndian();
            obj.Reserved3 = data.ReadUInt32LittleEndian();
            obj.DirCount = data.ReadUInt16LittleEndian();
            obj.Reserved4 = data.ReadUInt32LittleEndian();
            obj.Reserved5 = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
