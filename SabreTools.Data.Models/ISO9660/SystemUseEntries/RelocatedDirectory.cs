namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "RE" Relocated directory, recorded in the directory record of a relocated directory in its new parent directory
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    public sealed class RelocatedDirectory : BaseEntry { }
}
