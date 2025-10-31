namespace SabreTools.Data.Models.SGA
{
    /// <summary>
    /// SGA game archive
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public sealed class Archive
    {
        /// <summary>
        /// Header data
        /// </summary>
        public Header Header { get; set; } = new Header4();

        /// <summary>
        /// Directory data
        /// </summary>
        public Directory Directory { get; set; } = new Directory4();
    }
}
