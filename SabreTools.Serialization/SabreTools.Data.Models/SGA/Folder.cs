namespace SabreTools.Data.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Folder
    {
        public uint NameOffset { get; set; }

        public string? Name { get; set; } = string.Empty;
    }

    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Folder<TNumeric> : Folder where TNumeric : notnull
    {
        public TNumeric? FolderStartIndex { get; set; }

        public TNumeric? FolderEndIndex { get; set; }

        public TNumeric? FileStartIndex { get; set; }

        public TNumeric? FileEndIndex { get; set; }
    }
}
