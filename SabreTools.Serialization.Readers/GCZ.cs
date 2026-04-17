using System.IO;
using SabreTools.Data.Models.GCZ;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;

#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Serialization.Readers
{
    public class GCZ : BaseBinaryReader<Archive>
    {
        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            // Need at least the header
            if (data.Length - data.Position < Constants.HeaderSize)
                return null;

            try
            {
                long initialOffset = data.Position;

                var archive = new Archive();

                // Parse the header
                archive.Header = ParseGczHeader(data);

                // Validate magic
                if (archive.Header.MagicCookie != Constants.MagicCookie)
                    return null;

                // Validate block count — guard against absurdly large tables
                if (archive.Header.NumBlocks == 0 || archive.Header.NumBlocks > 0x100000)
                    return null;

                int numBlocks = (int)archive.Header.NumBlocks;

                // Read block pointer table (8 bytes per block)
                archive.BlockPointers = new ulong[numBlocks];
                byte[] ptrBuf = data.ReadBytes(numBlocks * 8);
                for (int i = 0; i < numBlocks; i++)
                    archive.BlockPointers[i] = System.BitConverter.ToUInt64(ptrBuf, i * 8);

                // Read block hash table (4 bytes per block, Adler-32)
                archive.BlockHashes = new uint[numBlocks];
                byte[] hashBuf = data.ReadBytes(numBlocks * 4);
                for (int i = 0; i < numBlocks; i++)
                    archive.BlockHashes[i] = System.BitConverter.ToUInt32(hashBuf, i * 4);

                // Compressed data begins immediately after the tables
                archive.DataOffset = (initialOffset + Constants.HeaderSize)
                    + ((long)numBlocks * 8)
                    + ((long)numBlocks * 4);

                return archive;
            }
            catch
            {
                return null;
            }
        }

        private static GczHeader ParseGczHeader(Stream data)
        {
            var header = new GczHeader();
            header.MagicCookie = data.ReadUInt32LittleEndian();
            header.SubType = data.ReadUInt32LittleEndian();
            header.CompressedDataSize = data.ReadUInt64LittleEndian();
            header.DataSize = data.ReadUInt64LittleEndian();
            header.BlockSize = data.ReadUInt32LittleEndian();
            header.NumBlocks = data.ReadUInt32LittleEndian();
            return header;
        }
    }
}
