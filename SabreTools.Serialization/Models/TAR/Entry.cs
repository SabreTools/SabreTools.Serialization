namespace SabreTools.Data.Models.TAR
{
    public sealed class Entry
    {
        /// <summary>
        /// Entry header
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// 0 or more blocks representing the content
        /// </summary>
        public Block[] Blocks { get; set; }
    }
}
