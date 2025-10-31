namespace SabreTools.Data.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Section
    {
        public string Alias { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }

    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Section<T> : Section where T : notnull
    {
        public T? FolderStartIndex { get; set; }

        public T? FolderEndIndex { get; set; }

        public T? FileStartIndex { get; set; }

        public T? FileEndIndex { get; set; }

        public T? FolderRootIndex { get; set; }
    }
}
