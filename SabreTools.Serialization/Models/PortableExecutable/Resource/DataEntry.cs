namespace SabreTools.Data.Models.PortableExecutable.Resource
{
    /// <summary>
    /// The resource directory string area consists of Unicode strings, which
    /// are word-aligned. These strings are stored together after the last
    /// Resource Directory entry and before the first Resource Data entry.
    /// This minimizes the impact of these variable-length strings on the
    /// alignment of the fixed-size directory entries.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class DataEntry
    {
        /// <summary>
        /// The address of a unit of resource data in the Resource Data area.
        /// </summary>
        public uint DataRVA { get; set; }

        /// <summary>
        /// The size, in bytes, of the resource data that is pointed to by the
        /// Data RVA field.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// The resource data that is pointed to by the Data RVA field.
        /// </summary>
        public byte[]? Data { get; set; }

        /// <summary>
        /// The code page that is used to decode code point values within the
        /// resource data. Typically, the code page would be the Unicode code page.
        /// </summary>
        public uint Codepage { get; set; }

        /// <summary>
        /// Reserved, must be 0.
        /// </summary>
        public uint Reserved { get; set; }
    }
}
