using System;
using System.Text;
using SabreTools.Data.Models.ZArchive;
using SabreTools.Numerics;

namespace SabreTools.Data.Extensions
{
    public static class ZArchiveExtensions
    {
        /// <summary>
        /// Retrieves the name of the specified node from the NameTable
        /// </summary>
        /// <param name="node">Node in the file tree</param>
        /// <param name="nameTable">ZArchive NameTable</param>
        /// <returns>UTF-8 string representing node's name</returns>
        public static string? GetName(this FileDirectoryEntry node, NameTable nameTable)
        {
            // Check for a valid offset into the NameTable
            uint nameOffset = node.NameOffsetAndTypeFlag & Constants.RootNode;
            if (nameOffset == Constants.RootNode)
                return null;

            // Get the index into the name table
            var index = Array.IndexOf(nameTable.NameTableOffsets, nameOffset);
            if (index < 0)
                return null;

            // Get the name entry for the requested index
            var nameEntry = nameTable.NameEntries[index];
            if (nameEntry is null)
                return null;

            // Decode name to UTF-8
            return Encoding.UTF8.GetString(nameEntry.NodeName);
        }
    }
}
