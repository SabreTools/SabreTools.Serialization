using System.Collections.Generic;
using SabreTools.Metadata.DatFiles;

namespace SabreTools.Metadata.Reports.Formats
{
    /// <summary>
    /// Represents a semicolon-separated value file
    /// </summary>
    public sealed class SemicolonSeparatedValue : SeparatedValue
    {
        /// <summary>
        /// Create a new report from the filename
        /// </summary>
        /// <param name="statsList">List of statistics objects to set</param>
        public SemicolonSeparatedValue(List<DatStatistics> statsList) : base(statsList)
        {
            _delim = ';';
        }
    }
}
