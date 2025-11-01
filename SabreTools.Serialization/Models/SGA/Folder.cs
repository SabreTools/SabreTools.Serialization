namespace SabreTools.Data.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Folder
    {
        public uint NameOffset { get; set; }

        public string? Name { get; set; } = string.Empty;
    }

    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Folder<T> : Folder where T : notnull
    {
        public T? FolderStartIndex { get; set; }

        public T? FolderEndIndex { get; set; }

        public T? FileStartIndex { get; set; }

        public T? FileEndIndex { get; set; }
    }
}
