namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "ES" Extension Selector, the entries following it belong to the extension with the given sequence number,
    /// counted in the order of the "ER" entries from 0
    /// </summary>
    /// <see href="http://www.o3one.org/hwdocs/cdrom_formats/susp112.doc"/>
    public sealed class ExtensionSelector : BaseEntry
    {
        /// <summary>
        /// Sequence number of the extension
        /// </summary>
        public byte ExtensionSequence { get; set; }
    }
}
