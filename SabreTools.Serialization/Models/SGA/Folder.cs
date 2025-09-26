namespace SabreTools.Data.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Folder
    {
        public uint NameOffset { get; set; }

        public string? Name { get; set; }
    }

    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Folder<T> : Folder
    {
        public T? FolderStartIndex { get; set; }

        public T? FolderEndIndex { get; set; }

        public T? FileStartIndex { get; set; }

        public T? FileEndIndex { get; set; }
    }
}
