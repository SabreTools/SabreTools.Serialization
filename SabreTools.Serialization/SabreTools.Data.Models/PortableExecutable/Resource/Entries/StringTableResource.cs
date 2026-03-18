using System.Collections.Generic;

namespace SabreTools.Data.Models.PortableExecutable.Resource.Entries
{
    /// <summary>
    /// Represents a string table resource
    /// </summary>
    public sealed class StringTableResource : ResourceDataType
    {
        /// <summary>
        /// Set of integer-keyed values
        /// </summary>
        public Dictionary<int, string?> Data { get; set; } = [];
    }
}
