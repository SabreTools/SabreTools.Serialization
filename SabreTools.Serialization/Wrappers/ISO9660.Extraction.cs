using System;
using System.IO;
using SabreTools.Data.Models.ISO9660;

namespace SabreTools.Serialization.Wrappers
{
    public partial class ISO9660 : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no root directory
            if (Volume?.RootDirectoryExtent == null)
                return false;

            bool allExtracted = true;

            // TODO: Extract all directories and file extents

            return allExtracted;
        }
    }
}
