namespace SabreTools.Data.Models.SecuROM
{
    /// <summary>
    /// Represents a single key-length-value tuple in a
    /// SecuROM DFA file
    /// </summary>
    public class DFAEntry
    {
        /// <summary>
        /// Entry name, always 4 ASCII characters
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Length of the value in bytes
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// Value of the entry whose length is given by <see cref="Length"/>
        /// </summary>
        public byte[] Value { get; set; }
    }
}
