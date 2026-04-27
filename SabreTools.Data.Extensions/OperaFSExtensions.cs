using SabreTools.Data.Models.OperaFS;
using SabreTools.Matching;

namespace SabreTools.Data.Extensions
{
    public static class OperaFSExtensions
    {
        /// <summary>
        /// Compare two DirectoryDescriptor objects
        /// </summary>
        /// <param name="dir1">The provided DirectoryDescriptor</param>
        /// <param name="dir2">The DirectoryDescriptor to compare against</param>
        /// <returns>True if all fields are equal, otherwise false</returns>
        public static bool EqualsExactly(this DirectoryDescriptor dir1, DirectoryDescriptor dir2)
        {
            if (dir1.NextBlock != dir2.NextBlock)
                return false;
            if (dir1.PreviousBlock != dir2.PreviousBlock)
                return false;
            if (dir1.Flags != dir2.Flags)
                return false;
            if (dir1.FirstFreeByte != dir2.FirstFreeByte)
                return false;
            if (dir1.FirstEntryOffset != dir2.FirstEntryOffset)
                return false;
            if (dir1.DirectoryRecords.Length != dir2.DirectoryRecords.Length)
                return false;

            for (int i = 0; i < dir1.DirectoryRecords.Length; i++)
            {
                if (!dir1.DirectoryRecords[i].EqualsExactly(dir2.DirectoryRecords[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Compare two DirectoryRecord objects
        /// </summary>
        /// <param name="dir1">The provided DirectoryRecord</param>
        /// <param name="dir2">The DirectoryRecord to compare against</param>
        /// <returns>True if all fields are equal, otherwise false</returns>
        public static bool EqualsExactly(this DirectoryRecord dr1, DirectoryRecord dr2)
        {
            if ((byte)dr1.DirectoryRecordFlags != (byte)dr2.DirectoryRecordFlags)
                return false;
            if (!dr1.UniqueIdentifier.EqualsExactly(dr2.UniqueIdentifier))
                return false;
            if (!dr1.Type.EqualsExactly(dr2.Type))
                return false;
            if (dr1.BlockSize != dr2.BlockSize)
                return false;
            if (dr1.ByteCount != dr2.ByteCount)
                return false;
            if (dr1.BlockCount != dr2.BlockCount)
                return false;
            if (dr1.Burst != dr2.Burst)
                return false;
            if (dr1.Gap != dr2.Gap)
                return false;
            if (!dr1.Filename.EqualsExactly(dr2.Filename))
                return false;
            if (dr1.LastAvatarIndex != dr2.LastAvatarIndex)
                return false;
            if (dr1.AvatarList.Length != dr2.AvatarList.Length)
                return false;

            for (int i = 0; i < dr1.AvatarList.Length; i++)
            {
                if (dr1.AvatarList[i] != dr2.AvatarList[i])
                    return false;
            }

            return true;
        }
    }
}
