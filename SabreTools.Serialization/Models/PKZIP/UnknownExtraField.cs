namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// This represents any unknown extras fields that are either
    /// unmapped or undiscovered. All unknown header IDs map to
    /// this data type.
    /// </summary>
    public class UnknownExtraField : ExtensibleDataField
    {
        /// <summary>
        ///
        /// </summary>
        public byte[] Data { get; set; }
    }
}
