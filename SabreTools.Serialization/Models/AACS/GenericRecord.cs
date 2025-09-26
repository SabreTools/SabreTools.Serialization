namespace SabreTools.Data.Models.AACS
{
    /// <summary>
    /// This represents any record that does not have a concrete model yet
    /// </summary>
    public sealed class GenericRecord : Record
    {
        /// <summary>
        /// Unparsed data comprising the record after the header
        /// </summary>
        public byte[]? Data { get; set; }
    }
}