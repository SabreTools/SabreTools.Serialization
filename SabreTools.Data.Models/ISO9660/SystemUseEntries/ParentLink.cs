using SabreTools.Numerics;

namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "PL" Parent link, recorded in the ".." record of a relocated directory
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    public sealed class ParentLink : BaseEntry
    {
        /// <summary>
        /// Logical block number of the original parent directory of the relocated directory
        /// </summary>
        public BothInt32 ParentDirectoryLocation { get; set; } = 0;
    }
}
