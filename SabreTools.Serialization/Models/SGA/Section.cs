namespace SabreTools.Data.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Section
    {
        public string Alias { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }

    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Section<TNumeric> : Section where TNumeric : notnull
    {
        public TNumeric? FolderStartIndex { get; set; }

        public TNumeric? FolderEndIndex { get; set; }

        public TNumeric? FileStartIndex { get; set; }

        public TNumeric? FileEndIndex { get; set; }

        public TNumeric? FolderRootIndex { get; set; }
    }
}
