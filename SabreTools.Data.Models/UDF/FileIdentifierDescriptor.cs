namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF File Identifier Descriptor (4/14.4), a directory entry naming a file or directory and pointing to its ICB
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class FileIdentifierDescriptor : Descriptor
    {
        /// <summary>
        /// File version number
        /// </summary>
        public ushort FileVersionNumber { get; set; }

        /// <summary>
        /// File characteristics
        /// </summary>
        public FileCharacteristics FileCharacteristics { get; set; }

        /// <summary>
        /// Length of the file identifier in bytes
        /// </summary>
        public byte LengthOfFileIdentifier { get; set; }

        /// <summary>
        /// ICB of the file or directory
        /// </summary>
        public LongAllocationDescriptor ICB { get; set; } = new();

        /// <summary>
        /// Length of the implementation use in bytes
        /// </summary>
        public ushort LengthOfImplementationUse { get; set; }

        /// <summary>
        /// Implementation use
        /// </summary>
        public byte[] ImplementationUse { get; set; } = [];

        /// <summary>
        /// File identifier, d-characters (first byte = compression ID), empty for the parent directory
        /// </summary>
        public byte[] FileIdentifier { get; set; } = [];

        /// <summary>
        /// Padding to a multiple of 4 bytes
        /// </summary>
        public byte[] Padding { get; set; } = [];
    }
}
