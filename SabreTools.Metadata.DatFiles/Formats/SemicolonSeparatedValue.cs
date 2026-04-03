namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents a semicolon-separated value file
    /// </summary>
    public sealed class SemicolonSeparatedValue : SeparatedValue
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SemicolonSeparatedValue(DatFile? datFile) : base(datFile)
        {
            _delim = ';';
            Header.DatFormat = DatFormat.SSV;
        }
    }
}
