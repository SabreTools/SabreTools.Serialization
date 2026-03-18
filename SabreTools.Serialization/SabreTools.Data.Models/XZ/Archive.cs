namespace SabreTools.Data.Models.XZ
{
    /// <see href="https://tukaani.org/xz/xz-file-format.txt"/>
    public class Archive
    {
        /// <summary>
        /// Pre-blocks header
        /// </summary>
        public Header Header { get; set; } = new();

        /// <summary>
        /// Sequence of 0 or more blocks
        /// </summary>
        public Block[] Blocks { get; set; } = [];

        /// <summary>
        /// Index structure
        /// </summary>
        public Index Index { get; set; } = new();

        /// <summary>
        /// Post-blocks footer
        /// </summary>
        public Footer Footer { get; set; } = new();
    }
}
