namespace SabreTools.Data.Models.NewExecutable
{
    /// <summary>
    /// The imported-name table follows the module-reference table. This table
    /// contains the names of modules and procedures that are imported by the
    /// executable file. Each entry is composed of a 1-byte field that
    /// contains the length of the string, followed by any number of
    /// characters. The strings are not null-terminated and are case
    /// sensitive.
    /// </summary>
    /// <see href="https://web.archive.org/web/20240422070115/http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    public sealed class ImportedNameTableEntry
    {
        /// <summary>
        /// Length of the name string that follows. A zero value indicates
        /// the end of the name table.
        /// </summary>
        public byte Length { get; set; } // TODO: Remove in lieu of AnsiBStr

        /// <summary>
        /// ASCII text of the name string.
        /// </summary>
        public byte[] NameString { get; set; } = [];
    }
}
