namespace SabreTools.Data.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class DirectoryHeader
    {
        // All logic lives in the typed version
    }

    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class DirectoryHeader<T>
    {
        public uint SectionOffset { get; set; }

        public T? SectionCount { get; set; }

        public uint FolderOffset { get; set; }

        public T? FolderCount { get; set; }

        public uint FileOffset { get; set; }

        public T? FileCount { get; set; }

        public uint StringTableOffset { get; set; }

        public T? StringTableCount { get; set; }
    }
}
