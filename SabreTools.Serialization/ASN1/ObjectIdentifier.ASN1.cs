namespace SabreTools.Serialization.ASN1
{
    /// <summary>
    /// Methods related to Object Identifiers (OID) and ASN.1 notation
    /// </summary>
    public static partial class ObjectIdentifier
    {
        /// <summary>
        /// Parse an OID in separated-value notation into ASN.1 notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <returns>ASN.1 formatted string, if possible</returns>
        /// <remarks>
        public static string? ParseOIDToASN1Notation(ulong[]? values)
        {
            // TODO: Once the modified OID-IRI formatting is done, make an ASN.1 notation version
            return null;
        }
    }
}
