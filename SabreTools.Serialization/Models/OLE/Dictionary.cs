namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The Dictionary packet represents all mappings between property identifiers and
    /// property names in a property set
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public class Dictionary
    {
        /// <summary>
        /// n unsigned integer representing the number of entries in the Dictionary
        /// </summary>
        public uint NumEntries { get; set; }

        /// <summary>
        /// All Entry fields MUST be a sequence of DictionaryEntry packets. Entries are
        /// not required to appear in any particular order.
        /// </summary>
        public DictionaryEntry[]? Entries { get; set; }

        // Padding (variable): Padding, if necessary, to a total length that is a multiple of 4 bytes.
        // Padding would be after each dictionary entry
    }
}
