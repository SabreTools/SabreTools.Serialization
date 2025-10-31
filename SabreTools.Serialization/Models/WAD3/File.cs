namespace SabreTools.Data.Models.WAD3
{
    /// <summary>
    /// Half-Life Texture Package File
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/WADFile.h"/>
    public sealed class File
    {
        /// <summary>
        /// Deserialized header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Deserialized dir entry data
        /// </summary>
        public DirEntry[] DirEntries { get; set; }

        /// <summary>
        /// Deserialized file entry data
        /// </summary>
        public FileEntry[] FileEntries { get; set; }
    }
}
