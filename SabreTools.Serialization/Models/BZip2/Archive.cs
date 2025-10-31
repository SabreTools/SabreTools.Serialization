namespace SabreTools.Data.Models.BZip2
{
    /// <see href="https://github.com/dsnet/compress/blob/master/doc/bzip2-format.pdf"/>
    public class Archive
    {
        /// <summary>
        /// Stream header
        /// </summary>
        public Header Header { get; set; }

        // TODO: Implement remaining structures

        /// <summary>
        /// Stream footer
        /// </summary>
        public Footer Footer { get; set; }
    }
}
