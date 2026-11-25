namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// System Use Entry, recorded in the System Use field of a directory record or in a Continuation Area
    /// </summary>
    /// <remarks>
    /// The System Use Sharing Protocol (SUSP) divides the System Use field into entries, each identified
    /// by a Signature Word, which extensions like the Rock Ridge Interchange Protocol (RRIP) define
    /// </remarks>
    /// <see href="http://www.o3one.org/hwdocs/cdrom_formats/susp112.doc"/>
    public abstract class BaseEntry
    {
        /// <summary>
        /// Signature Word identifying the type of the entry
        /// </summary>
        /// <remarks>2 bytes</remarks>
        public byte[] Signature { get; set; } = new byte[2];

        /// <summary>
        /// Length of the entry in bytes, including the Signature Word, this field and the version
        /// </summary>
        public byte Length { get; set; }

        /// <summary>
        /// Version of the entry
        /// </summary>
        public byte Version { get; set; }
    }
}
