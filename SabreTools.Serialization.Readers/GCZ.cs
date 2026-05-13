using System.IO;
using SabreTools.Data.Models.GCZ;
using SabreTools.Numerics.Extensions;

#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Serialization.Readers
{
    public class GCZ : BaseBinaryReader<DiscImage>
    {
        /// <inheritdoc/>
        public override DiscImage? Deserialize(Stream? data)
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

                var archive = new DiscImage();

                // Parse the header
                archive.Header = ParseGczHeader(data);
                if (archive.Header.MagicCookie != Constants.MagicCookie)
                    return null;

                // Validate block count — guard against absurdly large tables
                // TODO: Determine if this block count is arbitrary
                if (archive.Header.NumBlocks == 0 || archive.Header.NumBlocks > 0x100000)
                    return null;

                int numBlocks = (int)archive.Header.NumBlocks;

                // Read block pointer table (8 bytes per block)
                archive.BlockPointers = new ulong[numBlocks];
                for (int i = 0; i < numBlocks; i++)
                {
                    archive.BlockPointers[i] = data.ReadUInt64LittleEndian();
                }

                // Read block hash table (4 bytes per block, Adler-32)
                archive.BlockHashes = new uint[numBlocks];
                for (int i = 0; i < numBlocks; i++)
                {
                    archive.BlockHashes[i] = data.ReadUInt32LittleEndian();
                }

                return archive;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a GczHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled GczHeader on success, null on error</returns>
        public static GczHeader ParseGczHeader(Stream data)
        {
            var obj = new GczHeader();

            obj.MagicCookie = data.ReadUInt32LittleEndian();
            obj.SubType = data.ReadUInt32LittleEndian();
            obj.CompressedDataSize = data.ReadUInt64LittleEndian();
            obj.DataSize = data.ReadUInt64LittleEndian();
            obj.BlockSize = data.ReadUInt32LittleEndian();
            obj.NumBlocks = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
