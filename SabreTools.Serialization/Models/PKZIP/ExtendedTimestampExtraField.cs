namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The unix modified time, last access time, and creation time, if set
    /// </summary>
    /// <remarks>Header ID = 0x5455</remarks>
    /// <see href="https://github.com/adamhathcock/sharpcompress/blob/master/src/SharpCompress/Common/Zip/Headers/LocalEntryHeaderExtraFactory.cs"/> 
    public class ExtendedTimestampExtraField : ExtensibleDataField
    {
        /// <summary>
        /// Indicates what tiemstamps are included
        /// </summary>
        public RecordedTimeFlag Flags { get; set; }

        /// <summary>
        /// Last modified time
        /// </summary>
        /// <remarks>Only available when <see cref="RecordedTimeFlag.LastModified"/> is set</remarks> 
        public uint? LastModified { get; set; }

        /// <summary>
        /// Last accessed time
        /// </summary>
        /// <remarks>Only available when <see cref="RecordedTimeFlag.LastAccessed"/> is set</remarks> 
        public uint? LastAccessed { get; set; }

        /// <summary>
        /// Created on time
        /// </summary>
        /// <remarks>Only available when <see cref="RecordedTimeFlag.Created"/> is set</remarks> 
        public uint? CreatedOn { get; set; }
    }
}
