namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "ER" Extensions Reference, identifies an extension specification used in the directory hierarchy
    /// Recorded in the System Use Area of the "." record of the root directory
    /// </summary>
    /// <see href="http://www.o3one.org/hwdocs/cdrom_formats/susp112.doc"/>
    public sealed class ExtensionsReference : BaseEntry
    {
        /// <summary>
        /// Length of the Extension Identifier in bytes
        /// </summary>
        public byte IdentifierLength { get; set; }

        /// <summary>
        /// Length of the Extension Descriptor in bytes
        /// </summary>
        public byte DescriptorLength { get; set; }

        /// <summary>
        /// Length of the Extension Source in bytes
        /// </summary>
        public byte SourceLength { get; set; }

        /// <summary>
        /// Version of the extension specification
        /// </summary>
        public byte ExtensionVersion { get; set; }

        /// <summary>
        /// Extension Identifier, like "RRIP_1991A" or "IEEE_P1282" for RRIP
        /// </summary>
        public byte[] Identifier { get; set; } = [];

        /// <summary>
        /// Extension Descriptor, a description of the extension
        /// </summary>
        public byte[] Descriptor { get; set; } = [];

        /// <summary>
        /// Extension Source, where to find the extension specification
        /// </summary>
        public byte[] Source { get; set; } = [];
    }
}
