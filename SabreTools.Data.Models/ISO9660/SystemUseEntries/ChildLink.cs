using SabreTools.Numerics;

namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "CL" Child link, recorded in place of a directory that was relocated to keep the directory hierarchy
    /// within the depth limit of ISO 9660, the directory record is a file that stands for the directory
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    public sealed class ChildLink : BaseEntry
    {
        /// <summary>
        /// Logical block number of the relocated directory
        /// </summary>
        public BothInt32 ChildDirectoryLocation { get; set; } = 0;
    }
}
