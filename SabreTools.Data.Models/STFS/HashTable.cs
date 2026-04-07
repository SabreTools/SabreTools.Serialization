using System.Collections.Generic;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// STFS Hash Table in a Hash Table Block 
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public class HashTable
    {
        /// <summary>
        /// 170 hash table entries in a single table/block
        /// </summary>
        public HashTableEntry[] HashTableEntries { get; set; } = new HashTableEntry[170];
    }
}
