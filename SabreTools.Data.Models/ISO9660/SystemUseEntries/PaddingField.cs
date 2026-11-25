namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "PD" Padding Field, fills space in a System Use field or Continuation Area
    /// </summary>
    /// <see href="http://www.o3one.org/hwdocs/cdrom_formats/susp112.doc"/>
    public sealed class PaddingField : BaseEntry
    {
        /// <summary>
        /// Padding bytes, their content is not specified
        /// </summary>
        public byte[] Padding { get; set; } = [];
    }
}
