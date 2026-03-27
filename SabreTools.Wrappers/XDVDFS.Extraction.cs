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
            // Extract files from all directories from root directory
            return ExtractFromDirectory(outputDirectory, includeDebug, Model.VolumeDescriptor.RootOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExtractFromDirectory(string outputDirectory, bool includeDebug, uint sectorNumber)
        {
            //
        }
    }
}
