#pragma warning disable IDE0290 // Use primary constructor
namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents a tab-separated value file
    /// </summary>
    public sealed class TabSeparatedValue : SeparatedValue
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public TabSeparatedValue(DatFile? datFile) : base(datFile)
        {
            _delim = '\t';
            Header.SetFieldValue(DatHeader.DatFormatKey, DatFormat.TSV);
        }
    }
}
