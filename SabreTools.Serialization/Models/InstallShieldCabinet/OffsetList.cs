namespace SabreTools.Data.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    public sealed class OffsetList
    {
        public uint NameOffset { get; set; }

        public string Name { get; set; }

        public uint DescriptorOffset { get; set; }

        public uint NextOffset { get; set; }
    }
}
