namespace SabreTools.Data.Models.PKZIP
{
    /// <remarks>Header ID = 0x0018</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class RecordManagementControls : ExtensibleDataField
    {
        /// <summary>
        /// Record management control attribute tags
        /// </summary>
        public TagSizeVar[]? TagSizeVars { get; set; }
    }
}
