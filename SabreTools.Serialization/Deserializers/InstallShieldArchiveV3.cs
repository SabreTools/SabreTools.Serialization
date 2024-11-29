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
            if (data == null || data.Length == 0 || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Create a new archive to fill
            var archive = new Archive();

            #region Header

            // Try to parse the header
            var header = data.ReadType<Header>();
            if (header?.Signature1 != Constants.HeaderSignature)
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
                if (directory?.Name == null)
                    return null;

                directories.Add(directory);
                data.Seek(directory.ChunkSize - directory.Name.Length - 6, SeekOrigin.Current);
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
                    var file = data.ReadType<Models.InstallShieldArchiveV3.File>();
                    if (file?.Name == null)
                        return null;

                    files.Add(file);
                    data.Seek(file.ChunkSize - file.Name.Length - 30, SeekOrigin.Current);
                }
            }

            // Set the files
            archive.Files = [.. files];

            #endregion

            return archive;
        }

        /// <summary>
        /// Parse a Stream into a directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled directory on success, null on error</returns>
        public static Models.InstallShieldArchiveV3.Directory? ParseDirectory(Stream data)
        {
            var directory = new Models.InstallShieldArchiveV3.Directory();

            directory.FileCount = data.ReadUInt16();
            directory.ChunkSize = data.ReadUInt16();

            ushort nameLength = data.ReadUInt16();
            byte[] nameBytes = data.ReadBytes(nameLength);
            directory.Name = Encoding.ASCII.GetString(nameBytes);

            return directory;
        }
    }
}
