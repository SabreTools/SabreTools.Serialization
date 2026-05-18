namespace SabreTools.Data.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    public sealed class VolumeHeader
    {
        /// <remarks>32-bit in versions 5 and below</remarks>
        public ulong DataOffset { get; set; }

        public uint FirstFileIndex { get; set; }

        public uint LastFileIndex { get; set; }

        /// <remarks>32-bit in versions 5 and below</remarks>
        public ulong FirstFileOffset { get; set; }

        /// <remarks>32-bit in versions 5 and below</remarks>
        public ulong FirstFileSizeExpanded { get; set; }

        /// <remarks>32-bit in versions 5 and below</remarks>
        public ulong FirstFileSizeCompressed { get; set; }

        /// <remarks>32-bit in versions 5 and below</remarks>
        public ulong LastFileOffset { get; set; }

        /// <remarks>32-bit in versions 5 and below</remarks>
        public ulong LastFileSizeExpanded { get; set; }

        /// <remarks>32-bit in versions 5 and below</remarks>
        public ulong LastFileSizeCompressed { get; set; }
    }
}
