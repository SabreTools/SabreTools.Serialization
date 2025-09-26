namespace SabreTools.Data.Models.WAD3
{
    /// <see href="https://twhl.info/wiki/page/Specification:_WAD3"/>
    public enum FileType : byte
    {
        /// <summary>
        /// spraydecal (0x40): Same as <see cref="Miptex"/>.
        /// Only found in entry LOGO (or {LOGO) in tempdecal.wad
        /// </summary>
        Spraydecal = 0x40,

        /// <summary>
        /// qpic (0x42): A simple image of any size.
        /// </summary>
        Qpic = 0x42,

        /// <summary>
        /// miptex (0x43): World texture with dimensions that are multiple-of-16
        /// and 4 mipmaps. This is the typical entry type.
        /// </summary>
        Miptex = 0x43,

        /// <summary>
        /// font (0x46): Fixed-height font for 256 ASCII characters.
        /// </summary>
        Font = 0x46,
    }
}