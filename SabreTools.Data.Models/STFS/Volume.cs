using System.Collections.Generic;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// Secure Transacted File System, used by Xbox 360
    /// There are three formats: "LIVE", "PIRS", "CON "
    /// LIVE/PIRS are read-only signed by Microsoft, "CON " is read/write signed by console
    /// LIVE files are only distributed via Xbox Live, PIRS can be found elsewhere (e.g. system updates)
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public class Volume
    {
        /// <summary>
        /// STFS Header data
        /// Should be 0xA000 bytes (10 blocks)
        /// </summary>
        public Header Header { get; set; } = new();

        /// <summary>
        /// An STFS volume contains blocks dedicated to integrity hashes
        /// Each 4096-byte block has 170 integrity hashes for other blocks
        /// A new hash table block exists after every 170 data blocks
        /// i.e. First hash block is at 0x0B000 then next is at 0xB7000
        /// </summary>
        /// <remarks>Reader does not fill this in yet</remarks>
        public HashTables[]? HashTables { get; set; }

        /// <summary>
        /// Data in the STFS, arranged in blocks of 4096-bytes
        /// The Hash Table Blocks are interleaved, and ignored when numbering
        /// i.e. Block 170 is not adjacent to Block 171
        /// </summary>
        /// <remarks>Too large to read into memory, left in model for posterity</remarks>
        public byte[]? Data { get; set; }
    }
}
