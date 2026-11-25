namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "RR" Rock Ridge extensions in-use indicator of RRIP 1.09, the RRIP entries recorded for the file
    /// </summary>
    /// <see href="https://github.com/torvalds/linux/blob/master/fs/isofs/rock.h"/>
    public sealed class ExtensionsInUse : BaseEntry
    {
        /// <summary>
        /// Flags of the entry, the recorded RRIP entries
        /// </summary>
        public ExtensionsInUseFlags Flags { get; set; }
    }
}
