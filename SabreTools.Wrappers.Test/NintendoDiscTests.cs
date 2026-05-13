using System;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    [Collection("NintendoDisc")]
    public class NintendoDiscTests
    {
        #region DecryptTitleKey

        [Fact]
        public void DecryptTitleKey_EmptyKeys_Null()
        {
            NintendoDisc.RetailCommonKey = [];
            NintendoDisc.KoreanCommonKey = [];

            Assert.Null(NintendoDisc.DecryptTitleKey(new byte[16], new byte[8], 0));
        }

        [Fact]
        public void DecryptTitleKey_NullEncKey_Null()
        {
            NintendoDisc.RetailCommonKey = new byte[16];
            NintendoDisc.KoreanCommonKey = new byte[16];

            Assert.Null(NintendoDisc.DecryptTitleKey(null, new byte[8], 0));
        }

        [Fact]
        public void DecryptTitleKey_WrongLengthEncKey_Null()
        {
            NintendoDisc.RetailCommonKey = new byte[16];
            NintendoDisc.KoreanCommonKey = new byte[16];

            Assert.Null(NintendoDisc.DecryptTitleKey(new byte[8], new byte[8], 0));
        }

        [Fact]
        public void DecryptTitleKey_NullTitleId_Null()
        {
            NintendoDisc.RetailCommonKey = new byte[16];
            NintendoDisc.KoreanCommonKey = new byte[16];

            Assert.Null(NintendoDisc.DecryptTitleKey(new byte[16], null, 0));
        }

        [Fact]
        public void DecryptTitleKey_WrongLengthTitleId_Null()
        {
            NintendoDisc.RetailCommonKey = new byte[16];
            NintendoDisc.KoreanCommonKey = new byte[16];

            Assert.Null(NintendoDisc.DecryptTitleKey(new byte[16], new byte[4], 0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        public void DecryptTitleKey_UnknownIndex_Null(int index)
        {
            Assert.Null(NintendoDisc.DecryptTitleKey(new byte[16], new byte[8], index));
        }

        [Fact]
        public void DecryptTitleKey_WithInjectedKey_RoundTrips()
        {
            byte[] commonKey =
            [
                0xDE, 0xAD, 0xBE, 0xEF, 0xCA, 0xFE, 0xF0, 0x0D,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
            ];
            byte[] plainTitleKey =
            [
                0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
                0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
            ];
            byte[] titleId = [0x00, 0x01, 0x00, 0x45, 0x52, 0x53, 0x42, 0x00];

            byte[] iv = new byte[16];
            Array.Copy(titleId, 0, iv, 0, 8);
            byte[] encTitleKey = AesCbc.Encrypt(plainTitleKey, commonKey, iv)
                ?? throw new InvalidOperationException("AesCbc.Encrypt returned null");

            NintendoDisc.RetailCommonKey = commonKey;
            NintendoDisc.KoreanCommonKey = commonKey;

            byte[]? decrypted = NintendoDisc.DecryptTitleKey(encTitleKey, titleId, 0);
            Assert.NotNull(decrypted);
            Assert.Equal(plainTitleKey, decrypted);
        }

        #endregion
    }
}
