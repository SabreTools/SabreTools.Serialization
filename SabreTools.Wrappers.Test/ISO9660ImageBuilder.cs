using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.ISO9660;

namespace SabreTools.Wrappers.Test
{
    /// <summary>
    /// Builds ISO9660 test images in memory
    /// </summary>
    internal static class ISO9660ImageBuilder
    {
        public const int BlockLength = 2048;

        /// <summary>
        /// Block of the root directory in the test image
        /// </summary>
        public const int RootDirectory = 20;

        /// <summary>
        /// File flags of a plain file record
        /// </summary>
        public const FileFlags NoFlags = 0;

        /// <summary>
        /// Build an image with a primary volume descriptor and the given records in its root directory
        /// </summary>
        /// <remarks>Every byte of a block after the root directory holds the low byte of the block number</remarks>
        public static byte[] BuildImage(int blockCount, params (string Name, int Location, int Length, FileFlags Flags, byte[] SystemUse)[] files)
        {
            const int pathTableL = 18, pathTableM = 19;
            byte[] image = new byte[blockCount * BlockLength];
            for (int block = RootDirectory + 1; block < blockCount; block++)
            {
                image.AsSpan(block * BlockLength, BlockLength).Fill((byte)block);
            }

            // Primary volume descriptor
            var pvd = image.AsSpan(16 * BlockLength, BlockLength);
            pvd[0] = 1;
            Encoding.ASCII.GetBytes("CD001").CopyTo(pvd[1..]);
            pvd[6] = 1;
            WriteBothEndian(pvd[80..], blockCount);
            WriteBothEndian16(pvd[120..], 1);
            WriteBothEndian16(pvd[124..], 1);
            WriteBothEndian16(pvd[128..], BlockLength);
            WriteBothEndian(pvd[132..], 10);
            BinaryPrimitives.WriteInt32LittleEndian(pvd[140..], pathTableL);
            BinaryPrimitives.WriteInt32BigEndian(pvd[148..], pathTableM);
            DirectoryRecord([0], RootDirectory, BlockLength, FileFlags.DIRECTORY, []).CopyTo(pvd[156..]);
            pvd[881] = 1;

            // Volume descriptor set terminator
            var terminator = image.AsSpan(17 * BlockLength, BlockLength);
            terminator[0] = 255;
            Encoding.ASCII.GetBytes("CD001").CopyTo(terminator[1..]);
            terminator[6] = 1;

            // Path tables with the root directory only
            var pathTable = image.AsSpan(pathTableL * BlockLength);
            pathTable[0] = 1;
            BinaryPrimitives.WriteInt32LittleEndian(pathTable[2..], RootDirectory);
            BinaryPrimitives.WriteInt16LittleEndian(pathTable[6..], 1);
            pathTable = image.AsSpan(pathTableM * BlockLength);
            pathTable[0] = 1;
            BinaryPrimitives.WriteInt32BigEndian(pathTable[2..], RootDirectory);
            BinaryPrimitives.WriteInt16BigEndian(pathTable[6..], 1);

            // Root directory
            WriteDirectory(image, RootDirectory, Encoding.ASCII, files);
            return image;
        }

        /// <summary>
        /// Write a directory with the given records after its current and parent directory records
        /// </summary>
        public static void WriteDirectory(byte[] image, int block, Encoding encoding, (string Name, int Location, int Length, FileFlags Flags, byte[] SystemUse)[] files)
        {
            List<byte[]> records =
            [
                DirectoryRecord([0], block, BlockLength, FileFlags.DIRECTORY, []),
                DirectoryRecord([1], block, BlockLength, FileFlags.DIRECTORY, []),
            ];
            foreach (var (name, location, length, flags, systemUse) in files)
            {
                records.Add(DirectoryRecord(encoding.GetBytes($"{name};1"), location, length, flags, systemUse));
            }

            int position = block * BlockLength;
            foreach (byte[] record in records)
            {
                record.CopyTo(image, position);
                position += record.Length;
            }
        }

        /// <summary>
        /// Data of a file recorded at the given block, as BuildImage fills it
        /// </summary>
        public static byte[] ExpectedData(int location, int length)
            => [.. Enumerable.Range(0, length).Select(i => (byte)(location + (i / BlockLength)))];

        public static byte[] DirectoryRecord(byte[] identifier, int location, int length, FileFlags flags, byte[] systemUse)
        {
            // Pad the identifier and the System Use field to an even record length
            byte[] record = [.. new byte[33], .. identifier, .. new byte[1 - (identifier.Length % 2)], .. systemUse, .. new byte[systemUse.Length % 2]];
            record[0] = (byte)record.Length;
            WriteBothEndian(record.AsSpan(2), location);
            WriteBothEndian(record.AsSpan(10), length);
            record[25] = (byte)flags;
            WriteBothEndian16(record.AsSpan(28), 1);
            record[32] = (byte)identifier.Length;
            return record;
        }

        public static void WriteBothEndian(Span<byte> destination, int value)
        {
            BinaryPrimitives.WriteInt32LittleEndian(destination, value);
            BinaryPrimitives.WriteInt32BigEndian(destination[4..], value);
        }

        public static void WriteBothEndian16(Span<byte> destination, short value)
        {
            BinaryPrimitives.WriteInt16LittleEndian(destination, value);
            BinaryPrimitives.WriteInt16BigEndian(destination[2..], value);
        }
    }
}
