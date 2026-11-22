using System.IO;
using System.Linq;
using SabreTools.Data.Models.ISO9660;
using Xunit;
using static SabreTools.Wrappers.Test.ISO9660ImageBuilder;

namespace SabreTools.Wrappers.Test
{
    public class ISO9660SessionTests
    {
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
        /// directory lists FILE.BIN at sector 10 and NEW.BIN at sector 41; sector 30 (= 20 + 10) holds other data
        /// </summary>
        private static byte[] BuildTwoSessionImage()
        {
            byte[] image = new byte[42 * Constants.MinimumSectorSize];
            WriteVolume(image, 0, ("FILE.BIN", 10, Constants.MinimumSectorSize, NoFlags, []));
            WriteVolume(image, 20, ("FILE.BIN", 10, Constants.MinimumSectorSize, NoFlags, []), ("NEW.BIN", 41, Constants.MinimumSectorSize, NoFlags, []));
            Sector(0xA1).CopyTo(image, 10 * Constants.MinimumSectorSize);
            Sector(0xC3).CopyTo(image, 30 * Constants.MinimumSectorSize);
            Sector(0xB2).CopyTo(image, 41 * Constants.MinimumSectorSize);
            return image;
        }

        /// <summary>
        /// One sector filled with a value
        /// </summary>
        private static byte[] Sector(byte value)
            => [.. Enumerable.Repeat(value, Constants.MinimumSectorSize)];

        #endregion
    }
}
