namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// System Use Entry that is not specified by SUSP or RRIP, or whose length does not match its specification,
    /// like the Apple extensions ("AA") or the zisofs compression of Linux ("ZF")
    /// </summary>
    /// <see href="http://www.o3one.org/hwdocs/cdrom_formats/susp112.doc"/>
    public sealed class UnknownEntry : BaseEntry
    {
        /// <summary>
        /// Data of the entry after its version, as recorded
        /// </summary>
        public byte[] Data { get; set; } = [];
    }
}
