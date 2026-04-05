using Xunit;

namespace SabreTools.Metadata.Test
{
    public class UtilitiesTests
    {
        #region HasValidDatExtension

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("no-extension", false)]
        [InlineData("no-extension.", false)]
        [InlineData("invalid.ext", false)]
        [InlineData("invalid..ext", false)]
        [InlineData("INVALID.EXT", false)]
        [InlineData("INVALID..EXT", false)]
        [InlineData(".dat", true)]
        [InlineData(".DAT", true)]
        [InlineData("valid_extension.dat", true)]
        [InlineData("valid_extension..dat", true)]
        [InlineData("valid_extension.DAT", true)]
        [InlineData("valid_extension..DAT", true)]
        public void HasValidDatExtensionTest(string? path, bool expected)
        {
            bool actual = Utilities.HasValidDatExtension(path);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
