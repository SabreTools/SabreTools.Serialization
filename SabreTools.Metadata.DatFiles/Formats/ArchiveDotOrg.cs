namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents a Archive.org file list
    /// </summary>
    public sealed class ArchiveDotOrg : SerializableDatFile<Data.Models.ArchiveDotOrg.Files, Serialization.Readers.ArchiveDotOrg, Serialization.Writers.ArchiveDotOrg, Serialization.CrossModel.ArchiveDotOrg>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.ItemType[] SupportedTypes
            => [
                Data.Models.Metadata.ItemType.Rom,
            ];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public ArchiveDotOrg(DatFile? datFile) : base(datFile)
        {
            Header.Write(DatHeader.DatFormatKey, DatFormat.ArchiveDotOrg);
        }
    }
}
