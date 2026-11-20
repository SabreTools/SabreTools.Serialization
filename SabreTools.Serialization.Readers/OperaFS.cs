using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.OperaFS;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class OperaFS : BaseBinaryReader<FileSystem>
    {
        /// <inheritdoc cref="Deserialize(Stream?)" />
        public override FileSystem? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            // Simple check for a valid stream length
            if (data.Length - data.Position < Constants.SectorSize)
                return null;

            // Simple check on first bytes
            var signature = data.PeekBytes(7);
            if (!signature.EqualsExactly(Constants.MagicBytes))
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new FileSystem to fill
                var volume = new FileSystem();

                var volumeDescriptor = ParseVolumeDescriptor(data);
                if (volumeDescriptor is null)
                    return null;

                volume.VolumeDescriptor = volumeDescriptor;

                var directories = ParseRootDirectory(data, volumeDescriptor, initialOffset);
                if (directories is null)
                    return null;

                volume.Directories = directories;

                return volume;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an OperaFS Volume Descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled VolumeDescriptor on success, null on error</returns>
        public static VolumeDescriptor ParseVolumeDescriptor(Stream data)
        {
            var volumeDescriptor = new VolumeDescriptor();

            volumeDescriptor.RecordType = data.ReadByteValue();
            volumeDescriptor.VolumeSyncBytes = data.ReadBytes(5);
            volumeDescriptor.StructureVersion = data.ReadByteValue();
            volumeDescriptor.VolumeFlags = (VolumeFlags)data.ReadByteValue();
            volumeDescriptor.VolumeCommentary = data.ReadBytes(32);
            volumeDescriptor.VolumeIdentifier = data.ReadBytes(32);
            volumeDescriptor.VolumeUniqueIdentifier = data.ReadUInt32BigEndian();
            volumeDescriptor.VolumeBlockSize = data.ReadUInt32BigEndian();
            volumeDescriptor.VolumeBlockCount = data.ReadUInt32BigEndian();
            volumeDescriptor.RootUniqueIdentifier = data.ReadUInt32BigEndian();
            volumeDescriptor.RootDirectoryBlockCount = data.ReadUInt32BigEndian();
            volumeDescriptor.RootDirectoryBlockSize = data.ReadUInt32BigEndian();
            volumeDescriptor.RootDirectoryLastAvatarIndex = data.ReadUInt32BigEndian();

            for (int i = 0; i < 8; i++)
            {
                volumeDescriptor.RootDirectoryAvatarList[i] = data.ReadUInt32BigEndian();
            }

            if ((volumeDescriptor.VolumeFlags & VolumeFlags.M2) == VolumeFlags.M2)
            {
                volumeDescriptor.RomTagCount = data.ReadUInt32BigEndian();
                volumeDescriptor.ApplicationID = data.ReadUInt32BigEndian();
                volumeDescriptor.Reserved = data.ReadBytes(36);
                volumeDescriptor.Padding = data.ReadBytes(0x750);
            }
            else
            {
                volumeDescriptor.Padding = data.ReadBytes(0x77C);
            }

            return volumeDescriptor;
        }

        /// <summary>
        /// Parse a Stream into an map of OperaFS directories from a volume descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled map of directories on success, null on error</returns>
        public static Dictionary<uint, DirectoryDescriptor> ParseRootDirectory(Stream data, VolumeDescriptor volumeDescriptor, long initialOffset)
        {
            var directories = new Dictionary<uint, DirectoryDescriptor>();

            uint rootSector = volumeDescriptor.RootDirectoryAvatarList[0];
            var rootBlocks = ParseDirectoryBlocks(data, rootSector, volumeDescriptor.RootDirectoryBlockCount, initialOffset);
            for (int i = 0; i <= volumeDescriptor.RootDirectoryLastAvatarIndex; i++)
            {
                foreach (var kvp in rootBlocks)
                {
                    directories.Add(volumeDescriptor.RootDirectoryAvatarList[i] + (kvp.Key - rootSector), kvp.Value);
                }
            }

            foreach (var rootDirectory in rootBlocks.Values)
            {
                ParseChildDirectories(data, rootDirectory, initialOffset, directories);
            }

            return directories;
        }

        /// <summary>
        /// Parse a Stream into an map of OperaFS directories from a directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="parent">Directory block to read the directory records from</param>
        /// <param name="initialOffset">Offset of the file system in the stream</param>
        /// <param name="directories">Map of directories to add to, directories already in it are not read again</param>
        public static void ParseChildDirectories(Stream data, DirectoryDescriptor parent, long initialOffset, Dictionary<uint, DirectoryDescriptor> directories)
        {
            foreach (var dr in parent.DirectoryRecords)
            {
                // Skip file records
                if ((dr.DirectoryRecordFlags & DirectoryRecordFlags.DIRECTORY) == 0)
                    continue;

                // Skip directories that are already read, like a directory that records itself
                if (directories.ContainsKey(dr.AvatarList[0]))
                    continue;

                Dictionary<uint, DirectoryDescriptor> blocks;
                var childDirectories = new Dictionary<uint, DirectoryDescriptor>();
                try
                {
                    blocks = ParseDirectoryBlocks(data, dr.AvatarList[0], dr.BlockCount, initialOffset);
                    foreach (var kvp in blocks)
                    {
                        childDirectories.Add(kvp.Key, kvp.Value);
                    }

                    for (int i = 1; i <= dr.LastAvatarIndex; i++)
                    {
                        // Read avatar
                        var avatarBlocks = ParseDirectoryBlocks(data, dr.AvatarList[i], dr.BlockCount, initialOffset);
                        foreach (var kvp in avatarBlocks)
                        {
                            // Add reference to original directory if avatar is identical
                            // TODO: Check equality of all previously added unique avatars too
                            if (blocks.TryGetValue(dr.AvatarList[0] + (kvp.Key - dr.AvatarList[i]), out var block) && kvp.Value.EqualsExactly(block))
                                childDirectories.Add(kvp.Key, block);
                            else
                                childDirectories.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                catch
                {
                    // Skip directories that cannot be read
                    continue;
                }

                foreach (var kvp in childDirectories)
                {
                    if (!directories.ContainsKey(kvp.Key))
                        directories.Add(kvp.Key, kvp.Value);
                }

                foreach (var directory in blocks.Values)
                {
                    ParseChildDirectories(data, directory, initialOffset, directories);
                }
            }
        }

        /// <summary>
        /// Parse a Stream into a map of the blocks of an OperaFS directory, from its first block and the next blocks
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sector">Sector of the first block</param>
        /// <param name="blockCount">Number of blocks of the directory</param>
        /// <param name="initialOffset">Offset of the file system in the stream</param>
        /// <returns>Filled map of directory blocks by their sector, in the order of the blocks</returns>
        public static Dictionary<uint, DirectoryDescriptor> ParseDirectoryBlocks(Stream data, uint sector, uint blockCount, long initialOffset)
        {
            var blocks = new Dictionary<uint, DirectoryDescriptor>();

            data.SeekIfPossible(initialOffset + (sector * Constants.SectorSize), SeekOrigin.Begin);
            var block = ParseDirectory(data);
            blocks.Add(sector, block);

            // The next block is relative to the first block, and -1 for the last block
            while (block.NextBlock >= 0 && block.NextBlock < blockCount)
            {
                uint blockSector = sector + (uint)block.NextBlock;
                if (blocks.ContainsKey(blockSector))
                    break;

                try
                {
                    data.SeekIfPossible(initialOffset + (blockSector * Constants.SectorSize), SeekOrigin.Begin);
                    block = ParseDirectory(data);
                }
                catch
                {
                    // A block that cannot be read ends the directory
                    break;
                }

                blocks.Add(blockSector, block);
            }

            return blocks;
        }

        /// <summary>
        /// Parse a Stream into an OperaFS Directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Directory on success, null on error</returns>
        public static DirectoryDescriptor ParseDirectory(Stream data)
        {
            var directory = new DirectoryDescriptor();

            directory.NextBlock = data.ReadInt32BigEndian();
            directory.PreviousBlock = data.ReadInt32BigEndian();
            directory.Flags = data.ReadUInt32BigEndian();
            directory.FirstFreeByte = data.ReadUInt32BigEndian();
            directory.FirstEntryOffset = data.ReadUInt32BigEndian();

            var directoryRecords = new List<DirectoryRecord>();

            long startPosition = data.Position;
            while (data.Position < startPosition + directory.FirstFreeByte)
            {
                var directoryRecord = ParseDirectoryRecord(data);
                directoryRecords.Add(directoryRecord);

                if ((directoryRecord.DirectoryRecordFlags & DirectoryRecordFlags.DIRECTORY_FINAL) != 0)
                    break;

                if ((directoryRecord.DirectoryRecordFlags & DirectoryRecordFlags.BLOCK_FINAL) != 0)
                {
                    long nextBlock = Constants.SectorSize - ((data.Position - startPosition) % Constants.SectorSize);
                    data.SeekIfPossible(nextBlock, SeekOrigin.Current);
                }
            }

            directory.DirectoryRecords = [.. directoryRecords];

            return directory;
        }

        /// <summary>
        /// Parse a Stream into an OperaFS Directory Record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Directory Record on success, null on error</returns>
        public static DirectoryRecord ParseDirectoryRecord(Stream data)
        {
            var directoryRecord = new DirectoryRecord();

            directoryRecord.DirectoryRecordFlags = (DirectoryRecordFlags)data.ReadUInt32BigEndian();
            directoryRecord.UniqueIdentifier = data.ReadBytes(4);
            directoryRecord.Type = data.ReadBytes(4);
            directoryRecord.BlockSize = data.ReadUInt32BigEndian();
            directoryRecord.ByteCount = data.ReadUInt32BigEndian();
            directoryRecord.BlockCount = data.ReadUInt32BigEndian();
            directoryRecord.Burst = data.ReadUInt32BigEndian();
            directoryRecord.Gap = data.ReadUInt32BigEndian();
            directoryRecord.Filename = data.ReadBytes(32);
            directoryRecord.LastAvatarIndex = data.ReadUInt32BigEndian();

            directoryRecord.AvatarList = new uint[directoryRecord.LastAvatarIndex + 1];
            for (int i = 0; i <= directoryRecord.LastAvatarIndex; i++)
            {
                directoryRecord.AvatarList[i] = data.ReadUInt32BigEndian();
            }

            return directoryRecord;
        }
    }
}
