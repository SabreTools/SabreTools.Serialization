using System.Collections.Generic;
using SabreTools.Metadata.DatFiles;

namespace SabreTools.Metadata.Reports.Formats
{
    /// <summary>
    /// Represents a comma-separated value file
    /// </summary>
    public sealed class CommaSeparatedValue : SeparatedValue
    {
        /// <summary>
        /// Create a new report from the filename
        /// </summary>
        /// <param name="statsList">List of statistics objects to set</param>
        public CommaSeparatedValue(List<DatStatistics> statsList) : base(statsList)
        {
            _delim = ',';
        }
    }
}
