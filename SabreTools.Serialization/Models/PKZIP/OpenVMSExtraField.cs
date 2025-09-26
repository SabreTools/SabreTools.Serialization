namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The following is the layout of the OpenVMS attributes
    /// "extra" block.
    /// 
    /// OpenVMS Extra Field Rules:
    /// 
    /// - There will be one or more attributes present, which
    /// will each be preceded by the above TagX & SizeX values.
    /// These values are identical to the ATR$C_XXXX and ATR$S_XXXX
    /// constants which are defined in ATR.H under OpenVMS C.  Neither
    /// of these values will ever be zero.
    /// 
    /// - No word alignment or padding is performed.
    /// 
    /// - A well-behaved PKZIP/OpenVMS program SHOULD NOT produce
    /// more than one sub-block with the same TagX value.  Also, there MUST
    /// NOT be more than one "extra" block of type 0x000c in a particular
    /// directory record.
    /// </summary>
    /// <remarks>Header ID = 0x000C</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class OpenVMSExtraField : ExtensibleDataField
    {
        /// <summary>
        /// 32-bit CRC for remainder of the block
        /// </summary>
        public uint CRC { get; set; }

        /// <summary>
        /// OpenVMS attribute tags
        /// </summary>
        public TagSizeVar[]? TagSizeVars { get; set; }
    }
}
