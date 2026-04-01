using System;
using System.Text;
using SabreTools.Data.Models.ZArchive;
using SabreTools.Numerics;

namespace SabreTools.Data.Extensions
{
    public static class ZArchiveExtensions
    {
        /// <summary>
        /// Determines whether a node in the file tree is a directory or not
        /// </summary>
        /// <param name="nameTable">NameTable section</param>
        /// <returns>True if the node is a directory</returns>
        public static NameEntry? EntryAtOffset(this NameTable nameTable, uint offset)
        {
            var index = Array.IndexOf(nameTable.NameTableOffsets, offset);
            if (index < 0)
                return null;

            return nameTable.NameEntries[index];
        }

        /// <summary>
        /// Determines whether a node in the file tree is a file or not
        /// </summary>
        /// <param name="node">Node in the file tree</param>
        /// <returns>True if the node is a file</returns>
        public static bool IsFile(this FileDirectoryEntry node)
        {
            return (node.NameOffsetAndTypeFlag & Constants.FileFlag) == Constants.FileFlag;
        }

        /// <summary>
        /// Determines whether a node in the file tree is a directory or not
        /// </summary>
        /// <param name="node">Node in the file tree</param>
        /// <returns>True if the node is a directory</returns>
        public static bool IsDirectory(this FileDirectoryEntry node)
        {
            return (node.NameOffsetAndTypeFlag & Constants.FileFlag) == 0;
        }

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

            // Get the name entry for the requested offset
            var nameEntry = nameTable.EntryAtOffset(nameOffset);
            if (nameEntry is null)
                return null;

            // Decode name to UTF-8
            return Encoding.UTF8.GetString(nameEntry.NodeName);
        }
    }
}
