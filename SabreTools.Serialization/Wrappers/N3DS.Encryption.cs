using System;
using static SabreTools.Models.N3DS.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public partial class N3DS
    {
        /// <summary>
        /// Get the initial value for the ExeFS counter
        /// </summary>
        public byte[] ExeFSIV(int index)
        {
            if (Partitions == null)
                return [];
            if (index < 0 || index >= Partitions.Length)
                return [];

            var header = Partitions[index];
            if (header == null || header.MagicID != NCCHMagicNumber)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            Array.Reverse(partitionIdBytes);
            return [.. partitionIdBytes, .. ExefsCounter];
        }

        /// <summary>
        /// Get the initial value for the plain counter
        /// </summary>
        public byte[] PlainIV(int index)
        {
            if (Partitions == null)
                return [];
            if (index < 0 || index >= Partitions.Length)
                return [];

            var header = Partitions[index];
            if (header == null || header.MagicID != NCCHMagicNumber)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            Array.Reverse(partitionIdBytes);
            return [.. partitionIdBytes, .. PlainCounter];
        }

        /// <summary>
        /// Get the initial value for the RomFS counter
        /// </summary>
        public byte[] RomFSIV(int index)
        {
            if (Partitions == null)
                return [];
            if (index < 0 || index >= Partitions.Length)
                return [];

            var header = Partitions[index];
            if (header == null || header.MagicID != NCCHMagicNumber)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            Array.Reverse(partitionIdBytes);
            return [.. partitionIdBytes, .. RomfsCounter];
        }
    }
}
