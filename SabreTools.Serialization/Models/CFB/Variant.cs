namespace SabreTools.Data.Models.CFB
{
    /// <summary>
    /// VARIANT is a container for a union that can hold many types of data.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-oaut/b2ee2b50-665e-43e6-a92c-8f2a29fd7add"/>
    public sealed class Variant
    {
        /// <summary>
        /// MUST be set to the size, in quad words (64 bits), of the structure.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// MUST be set to 0 and MUST be ignored by the recipient.
        /// </summary>
        public uint RpcReserved { get; set; }

        /// <summary>
        /// MUST be set to one of the values specified with a "V".
        /// </summary>
        public VariantType VariantType { get; set; }

        /// <summary>
        /// MAY be set to 0 and MUST be ignored by the recipient.
        /// </summary>
        public ushort Reserved1 { get; set; }

        /// <summary>
        /// MAY be set to 0 and MUST be ignored by the recipient.
        /// </summary>
        public ushort Reserved2 { get; set; }

        /// <summary>
        /// MAY be set to 0 and MUST be ignored by the recipient.
        /// </summary>
        public ushort Reserved3 { get; set; }

        /// <summary>
        /// MUST contain an instance of the type, according to the value
        /// in the <see cref="VariantType"/> field.
        /// </summary>
        public object Union { get; set; }
    }
}
