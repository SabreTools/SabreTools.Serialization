namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents a comma-separated value file
    /// </summary>
    public sealed class CommaSeparatedValue : SeparatedValue
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public CommaSeparatedValue(DatFile? datFile) : base(datFile)
        {
            _delim = ',';
            Header.Write(DatHeader.DatFormatKey, DatFormat.CSV);
        }
    }
}
