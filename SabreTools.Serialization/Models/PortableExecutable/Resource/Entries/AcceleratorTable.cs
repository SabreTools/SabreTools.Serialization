
namespace SabreTools.Data.Models.PortableExecutable.Resource.Entries
{
    /// <summary>
    /// The ACCELTABLEENTRY structure is repeated for all accelerator table entries in the resource.
    /// The last entry in the table is flagged with the value 0x0080.
    /// 
    /// You can compute the number of elements in the table if you divide the length of the resource
    /// by eight. Then your application can randomly access the individual fixed-length entries.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/acceltableentry"/>
    public sealed class AcceleratorTable
    {
        /// <summary>
        /// Accelerator table entries
        /// </summary>
        public AcceleratorTableEntry[]? Entries { get; set; }
    }
}
