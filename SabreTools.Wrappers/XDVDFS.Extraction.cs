using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.XDVDFS;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class XDVDFS : IExtractable
    {
        #region Extraction State

        /// <summary>
        /// List of extracted files by their sector offset
        /// </summary>
        private readonly Dictionary<int, uint> extractedFiles = [];

        #endregion

        /// <inheritdoc/>
        public virtual bool Extract(string outputDirectory, bool includeDebug)
        {
            // Clear the extraction state
            extractedFiles.Clear();

            bool allExtracted = true;

            // Extract all files from all directories

            return allExtracted;
        }
    }
}
