namespace SabreTools.Data.Models.SecuROM
{
    /// <summary>
    /// Securom Matroschka Package PE section
    /// </summary>
    /// <remarks>
    /// Offered by SecuROM, its main purpose seems to be managing some sort
    /// of SecuROM-related operation involving multiple temporary files
    /// contained within the package. Observed in Release Control executables,
    /// Product Activation Revocation executables, and in some regular
    /// Product-Activation-protected releases (such as the digital download
    /// releases of Neverwinter Nights 2 and Test Drive Unlimited) where the
    /// game executable, paul.dll and other PA-related files are stored in
    /// the matroschka package.
    /// </remarks>
    public class MatroshkaPackage
    {
        /// <summary>
        /// "MatR"
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public string Signature { get; set; } = string.Empty;

        /// <summary>
        /// Number of internal entries
        /// </summary>
        public uint EntryCount { get; set; }

        #region Longer Header only

        // The combination of the 4 following values have only been seen in
        // one of 4 distinct patterns. The meaning of these patterns is unknown.
        // - 0 0 0
        // - 0 0 1
        // - 0 1 1
        // - 1 1 1
        // These values do not seem to have a link to whether the paths included
        // in entries are 256- or 512-byte. There also do not seem to be any links
        // between these values and the hex string values.
        // There is one example of "0 0 0" which contains a key hex string of all
        // 0x00 values and 256-byte paths.

        /// <summary>
        /// One of four unknown values only observed on longer header matroschka sections
        /// </summary>
        /// <remarks>Only values of 0 or 1 have been found</remarks>
        public uint? UnknownRCValue1 { get; set; }

        /// <summary>
        /// One of four unknown values only observed on longer header matroschka sections
        /// </summary>
        /// <remarks>Only values of 0 or 1 have been found</remarks>
        public uint? UnknownRCValue2 { get; set; }

        /// <summary>
        /// One of four unknown values only observed on longer header matroschka sections
        /// </summary>
        /// <remarks>Only values of 0 or 1 have been found</remarks>
        public uint? UnknownRCValue3 { get; set; }

        /// <summary>
        /// 32-character hex string
        /// </summary>
        /// <remarks>
        /// This is all zeroes for all observed longer header non-RC matroschka sections. For RC sections, this is
        /// always 630A411277DE8A4B9BB8DF2A14AC4C28, except for the executables for Crysis Wars and Crysis
        /// Warhead, which are 9D2593B31A01E041AF6EDC15AEF5B969. Those two are noteworthy as they appear to be the
        /// earliest known RC games, have a strange key in the encrypted executable, and are the only games that cannot
        /// currently be manually unlocked (and thus, cannot be unlocked at all at the moment). So, this may indicate
        /// something about version, functionality, or otherwise?
        /// </remarks>
        public string? KeyHexString { get; set; }

        /// <summary>
        /// Padding for alignment, always 0x00000000
        /// </summary>
        public uint? Padding { get; set; }

        #endregion

        /// <summary>
        /// Entries array whose length is given by <see cref="EntryCount"/>
        /// </summary>
        public MatroshkaEntry[] Entries { get; set; } = [];
    }
}
