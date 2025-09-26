namespace SabreTools.Serialization.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    /// TODO: Should standard and high values be combined?
    public sealed class VolumeHeader
    {
        public uint DataOffset { get; set; }

        public uint DataOffsetHigh { get; set; }

        public uint FirstFileIndex { get; set; }

        public uint LastFileIndex { get; set; }

        public uint FirstFileOffset { get; set; }

        public uint FirstFileOffsetHigh { get; set; }

        public uint FirstFileSizeExpanded { get; set; }

        public uint FirstFileSizeExpandedHigh { get; set; }

        public uint FirstFileSizeCompressed { get; set; }

        public uint FirstFileSizeCompressedHigh { get; set; }

        public uint LastFileOffset { get; set; }

        public uint LastFileOffsetHigh { get; set; }

        public uint LastFileSizeExpanded { get; set; }

        public uint LastFileSizeExpandedHigh { get; set; }

        public uint LastFileSizeCompressed { get; set; }
        
        public uint LastFileSizeCompressedHigh { get; set; }
    }
}