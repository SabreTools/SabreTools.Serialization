namespace SabreTools.Serialization.Models.TAR
{
    public sealed class Block
    {
        /// <summary>
        /// Data
        /// </summary>
        /// <remarks>512 bytes</remarks>
        public byte[]? Data { get; set; }
    }
}