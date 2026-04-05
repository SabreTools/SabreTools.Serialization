using System;
using System.IO;
using SabreTools.Hashing;
using SabreTools.Text.Extensions;

namespace SabreTools.Metadata.DatFiles
{
    /// <summary>
    /// Depot information wrapper
    /// </summary>
    public class DepotInformation : ICloneable
    {
        /// <summary>
        /// Name or path of the Depot
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Whether to use this Depot or not
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Depot byte-depth
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Maximum depth allowed for path generation
        /// </summary>
        private static readonly int MaximumDepth = HashType.SHA1.ZeroBytes.Length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isActive">Set active state</param>
        /// <param name="depth">Set depth between 0 and SHA-1's byte length</param>
        public DepotInformation(bool isActive, int depth)
            : this(null, isActive, depth)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Identifier for the depot</param>
        /// <param name="isActive">Set active state</param>
        /// <param name="depth">Set depth between 0 and SHA-1's byte length</param>
        public DepotInformation(string? name, bool isActive, int depth)
        {
            Name = name;
            IsActive = isActive;
            Depth = depth;

            // Limit depth value
            if (Depth == int.MinValue)
                Depth = 4;
            else if (Depth < 0)
                Depth = 0;
            else if (Depth > MaximumDepth)
                Depth = MaximumDepth;
        }

        #region Cloning

        /// <summary>
        /// Clone the current object
        /// </summary>
        public object Clone() => new DepotInformation(Name, IsActive, Depth);

        #endregion

        #region Path Generation

        /// <summary>
        /// Get a proper romba subpath
        /// </summary>
        /// <param name="hash">SHA-1 hash to get the path for</param>
        /// <param name="depth">Positive value representing the depth of the depot</param>
        /// <returns>Subfolder path for the given hash, including the filename and .gz extension</returns>
        public string? GetDepotPath(byte[]? hash)
        {
            string? sha1 = hash.ToHexString();
            return GetDepotPath(sha1);
        }

        /// <summary>
        /// Get a proper romba subpath
        /// </summary>
        /// <param name="hash">SHA-1 hash to get the path for</param>
        /// <returns>Subfolder path for the given hash, including the filename and .gz extension</returns>
        public string? GetDepotPath(string? hash)
        {
            // If the hash is null, then we return null
            if (hash is null)
                return null;

            // If the hash isn't the right size, then we return null
            if (hash.Length != HashType.SHA1.ZeroString.Length)
                return null;

            // Cap the depth between 0 and 20, for now
            int depth = Depth;
            if (depth < 0)
                depth = 0;
            else if (depth > MaximumDepth)
                depth = MaximumDepth;

            // Loop through and generate the subdirectory
            string path = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                path += hash.Substring(i * 2, 2) + Path.DirectorySeparatorChar;
            }

            // Now append the filename
            path += $"{hash}.gz";
            return path;
        }

        #endregion
    }
}
