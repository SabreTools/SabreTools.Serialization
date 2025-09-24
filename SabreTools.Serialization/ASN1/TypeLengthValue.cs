namespace SabreTools.Serialization.ASN1
{
    /// <summary>
    /// ASN.1 type/length/value class that all types are based on
    /// </summary>
    public class TypeLengthValue
    {
        /// <summary>
        /// The ASN.1 type
        /// </summary>
        public ASN1Type Type { get; set; }

        /// <summary>
        /// Length of the value
        /// </summary>
        public ulong Length { get; set; }

        /// <summary>
        /// Generic value associated with <see cref="Type"/>
        /// </summary>
        public object? Value { get; set; }
    }
}
