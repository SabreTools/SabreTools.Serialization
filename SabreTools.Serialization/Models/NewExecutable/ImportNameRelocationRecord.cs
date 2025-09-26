using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.NewExecutable
{
    /// <see href="https://web.archive.org/web/20240422070115/http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ImportNameRelocationRecord
    {
        /// <summary>
        /// Index into module reference table for the imported module.
        /// </summary>
        public ushort Index;

        /// <summary>
        /// Offset within Imported Names Table to procedure name string.
        /// </summary>
        public ushort Offset;
    }
}
