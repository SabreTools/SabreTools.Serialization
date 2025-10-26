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
            // If we have no root directories
            if (RootDirectories == null || RootDirectories.Length == 0)
                return false;

            // If we have no path table groups
            if (PathTableGroups == null || PathTableGroups.Length == 0)
                return false;

            bool allExtracted = true;

            // TODO: Extract all directories and file extents

            return allExtracted;
        }
    }
}
