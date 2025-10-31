namespace SabreTools.Data.Models.MoPaQ
{
    /// <summary>
    /// Each incremental patch file in a patch MPQ starts with a header. It is supposed
    /// to be a structure with variable length.
    /// </summary>
    /// <see href="http://zezula.net/en/mpq/mpqformat.html"/>
    public sealed class PatchHeader
    {
        #region PATCH Header

        /// <summary>
        /// 'PTCH'
        /// </summary>
        public uint PatchSignature { get; set; }

        /// <summary>
        /// Size of the entire patch (decompressed)
        /// </summary>
        public int SizeOfPatchData { get; set; }

        /// <summary>
        /// Size of the file before patch
        /// </summary>
        public int SizeBeforePatch { get; set; }

        /// <summary>
        /// Size of file after patch
        /// </summary>
        public int SizeAfterPatch { get; set; }

        #endregion

        #region MD5 Block

        /// <summary>
        /// 'MD5_'
        /// </summary>
        public uint Md5Signature { get; set; }

        /// <summary>
        /// Size of the MD5 block, including the signature and size itself
        /// </summary>
        public int Md5BlockSize { get; set; }

        /// <summary>
        /// MD5 of the original (unpached) file
        /// </summary>
        public byte[] Md5BeforePatch { get; set; } = new byte[0x10];

        /// <summary>
        /// MD5 of the patched file
        /// </summary>
        public byte[] Md5AfterPatch { get; set; } = new byte[0x10];

        #endregion

        #region XFRM Block

        /// <summary>
        /// 'XFRM'
        /// </summary>
        public uint XfrmSignature { get; set; }

        /// <summary>
        /// Size of the XFRM block, includes XFRM header and patch data
        /// </summary>
        public int XfrmBlockSize { get; set; }

        /// <summary>
        /// Type of patch ('BSD0' or 'COPY')
        /// </summary>
        public PatchType PatchType { get; set; }

        #endregion

        #region Patch Data - BSD0

        /// <summary>
        /// 'BSDIFF40' signature
        /// </summary>
        public ulong BsdiffSignature { get; set; }

        /// <summary>
        /// Size of CTRL block (in bytes)
        /// </summary>
        public long CtrlBlockSize { get; set; }

        /// <summary>
        /// Size of DATA block (in bytes)
        /// </summary>
        public long DataBlockSize { get; set; }

        /// <summary>
        /// Size of file after applying the patch (in bytes)
        /// </summary>
        public long NewFileSize { get; set; }

        // TODO: Fill rest of data from http://zezula.net/en/mpq/patchfiles.html
        // CTRL block
        // DATA block
        // EXTRA block

        #endregion

        #region Patch Data - COPY

        /// <summary>
        /// File data are simply replaced by the data in the patch.
        /// </summary>
        public byte[] PatchDataCopy { get; set; } = [];

        #endregion
    }
}
