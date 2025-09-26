namespace SabreTools.Serialization.Models.TAR
{
    public sealed class Archive
    {
        /// <summary>
        /// 1 or more entries
        /// </summary>
        public Entry[]? Entries { get; set; }
    }
}