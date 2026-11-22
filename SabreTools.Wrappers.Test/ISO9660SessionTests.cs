using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Numerics;
using SabreTools.Numerics.Extensions;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class ISO9660SessionTests
    {
        private const FileFlags NoFlags = 0;

        [Fact]
        public void FirstSession_Extract()
        {
            var wrapper = ISO9660.Create(new MemoryStream(BuildTwoSessionImage()), 0);
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal(Sector(0xA1), File.ReadAllBytes(Path.Combine(output, "FILE.BIN")));
                Assert.False(File.Exists(Path.Combine(output, "NEW.BIN")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void LaterSession_ExtentsFromDiscStart()
        {
            var wrapper = ISO9660.Create(new MemoryStream(BuildTwoSessionImage()), 20);
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));

                // FILE.BIN lies in the first session, NEW.BIN in the second; both locations count from sector 0
                Assert.Equal(Sector(0xA1), File.ReadAllBytes(Path.Combine(output, "FILE.BIN")));
                Assert.Equal(Sector(0xB2), File.ReadAllBytes(Path.Combine(output, "NEW.BIN")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void NegativeStart_Null()
        {
            var actual = ISO9660.Create(new MemoryStream(BuildTwoSessionImage()), -1);
            Assert.Null(actual);
        }

        [Fact]
        public void NoVolumeDescriptor_Null()
        {
            var actual = ISO9660.Create(new MemoryStream(BuildTwoSessionImage()), 10);
            Assert.Null(actual);
        }

        #region Helpers

        /// <summary>
        /// Two-session disc: session 1 at sector 0 (FILE.BIN at sector 10), session 2 at sector 20 whose root
        /// directory lists FILE.BIN at sector 10 and NEW.BIN at sector 40; sector 30 (= 20 + 10) holds other data
        /// </summary>
        private static byte[] BuildTwoSessionImage()
        {
            byte[] image = new byte[41 * Constants.MinimumSectorSize];
            WriteVolume(image, 0, ("FILE.BIN;1", 10));
            WriteVolume(image, 20, ("FILE.BIN;1", 10), ("NEW.BIN;1", 40));
            Sector(0xA1).CopyTo(image, 10 * Constants.MinimumSectorSize);
            Sector(0xC3).CopyTo(image, 30 * Constants.MinimumSectorSize);
            Sector(0xB2).CopyTo(image, 40 * Constants.MinimumSectorSize);
            return image;
        }

        /// <summary>
        /// One sector filled with a value
        /// </summary>
        private static byte[] Sector(byte value)
            => [.. Enumerable.Repeat(value, Constants.MinimumSectorSize)];

        /// <summary>
        /// Write a directory record for a one-sector extent, returns the offset after the record
        /// </summary>
        private static int WriteRecord(byte[] image, int offset, byte[] identifier, int location, FileFlags flags)
        {
            int length = 33 + identifier.Length + (identifier.Length % 2 == 0 ? 1 : 0);
            image[offset] = (byte)length;

            int field = offset + 2;
            image.WriteBothEndian(ref field, (BothInt32)location);
            image.WriteBothEndian(ref field, (BothInt32)Constants.MinimumSectorSize);

            image[offset + 25] = (byte)flags;

            field = offset + 28;
            image.WriteBothEndian(ref field, (BothInt16)1);
            image.Write(ref field, (byte)identifier.Length);
            image.Write(ref field, identifier);

            return offset + length;
        }

        /// <summary>
        /// Write a primary volume descriptor, terminator, path table and root directory for a session
        /// </summary>
        private static void WriteVolume(byte[] image, int start, params (string Name, int Location)[] files)
        {
            int descriptor = (start + Constants.SystemAreaSectors) * Constants.MinimumSectorSize;
            int pathTable = start + Constants.SystemAreaSectors + 2;
            int root = start + Constants.SystemAreaSectors + 3;

            // Primary volume descriptor: logical block size, path table size and location, root directory record
            int offset = descriptor;
            image.Write(ref offset, (byte)VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR);
            image.Write(ref offset, Constants.StandardIdentifier);
            image.Write(ref offset, (byte)1);

            offset = descriptor + 128;
            image.WriteBothEndian(ref offset, (BothInt16)Constants.MinimumSectorSize);
            image.WriteBothEndian(ref offset, (BothInt32)10);
            image.WriteLittleEndian(ref offset, pathTable);
            WriteRecord(image, descriptor + 156, Constants.CurrentDirectory, root, FileFlags.DIRECTORY);

            // Volume descriptor set terminator
            offset = descriptor + Constants.MinimumSectorSize;
            image.Write(ref offset, (byte)VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR);
            image.Write(ref offset, Constants.StandardIdentifier);
            image.Write(ref offset, (byte)1);

            // Path table with the root directory only
            offset = pathTable * Constants.MinimumSectorSize;
            image.Write(ref offset, (byte)Constants.CurrentDirectory.Length);
            image.Write(ref offset, (byte)0);
            image.WriteLittleEndian(ref offset, root);
            image.WriteLittleEndian(ref offset, (short)1);
            image.Write(ref offset, Constants.CurrentDirectory);

            // Root directory
            offset = root * Constants.MinimumSectorSize;
            offset = WriteRecord(image, offset, Constants.CurrentDirectory, root, FileFlags.DIRECTORY);
            offset = WriteRecord(image, offset, Constants.ParentDirectory, root, FileFlags.DIRECTORY);
            foreach (var (name, location) in files)
            {
                offset = WriteRecord(image, offset, Encoding.ASCII.GetBytes(name), location, NoFlags);
            }
        }

        #endregion
    }
}
