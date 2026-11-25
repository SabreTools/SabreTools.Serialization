using SabreTools.Numerics;

namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "CE" Continuation Area, extends the System Use field by an area with more System Use Entries
    /// The Continuation Area is processed after the System Use field or Continuation Area the entry is recorded in
    /// </summary>
    /// <see href="http://www.o3one.org/hwdocs/cdrom_formats/susp112.doc"/>
    public sealed class ContinuationArea : BaseEntry
    {
        /// <summary>
        /// Logical block number from which the start of the Continuation Area is measured
        /// </summary>
        public BothInt32 BlockLocation { get; set; } = 0;

        /// <summary>
        /// Offset in bytes from the start of the block to the start of the Continuation Area
        /// </summary>
        public BothUInt32 Offset { get; set; } = 0;

        /// <summary>
        /// Number of bytes in the Continuation Area
        /// </summary>
        public BothUInt32 AreaLength { get; set; } = 0;
    }
}
