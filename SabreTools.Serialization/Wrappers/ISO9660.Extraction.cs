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
            // If we have no path tables or directory descriptors, there is nothing to extract
            if (PathTableGroups.Length == 0 && DirectoryDescriptors.Count == 0)
                return true;

            bool allExtracted = false;

            // TODO: Extract all directories and file extents

            return allExtracted;
        }
    }
}
