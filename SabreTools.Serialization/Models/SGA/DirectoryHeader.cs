namespace SabreTools.Data.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class DirectoryHeader
    {
        // All logic lives in the typed version
    }

    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class DirectoryHeader<TNumeric> where TNumeric : notnull
    {
        public uint SectionOffset { get; set; }

        public TNumeric? SectionCount { get; set; }

        public uint FolderOffset { get; set; }

        public TNumeric? FolderCount { get; set; }

        public uint FileOffset { get; set; }

        public TNumeric? FileCount { get; set; }

        public uint StringTableOffset { get; set; }

        public TNumeric? StringTableCount { get; set; }
    }
}
