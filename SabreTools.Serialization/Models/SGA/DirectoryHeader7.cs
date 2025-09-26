namespace SabreTools.Serialization.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public sealed class DirectoryHeader7 : DirectoryHeader<uint>
    {
        public uint HashTableOffset { get; set; }

        public uint BlockSize { get; set; }
    }
}
