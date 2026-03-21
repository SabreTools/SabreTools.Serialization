namespace SabreTools.Data.Models.COFF.SymbolTableEntries
{
    /// <summary>
    /// Auxiliary Format 4: Files
    ///
    /// This format follows a symbol-table record with storage class FILE (103).
    /// The symbol name itself should be .file, and the auxiliary record that
    /// follows it gives the name of a source-code file.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class FileRecord : BaseEntry
    {
        /// <summary>
        /// An ANSI string that gives the name of the source file. This is padded
        /// with nulls if it is less than the maximum length.
        /// </summary>
        /// <remarks>18 bytes</remarks>
        public byte[] FileName { get; set; } = new byte[18];
    }
}
