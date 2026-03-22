using SabreTools.Data.Models.NewExecutable;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class NewExecutableExtensionsTests
    {
        #region IsIntegerType

        [Fact]
        public void IsIntegerType_RTIE_HighBitSet_True()
        {
            var entry = new ResourceTypeInformationEntry { TypeID = 0xFFFF };
            bool actual = entry.IsIntegerType();
            Assert.True(actual);
        }

        [Fact]
        public void IsIntegerType_RTIE_HighBitClear_False()
        {
            var entry = new ResourceTypeInformationEntry { TypeID = 0x0000 };
            bool actual = entry.IsIntegerType();
            Assert.False(actual);
        }

        [Fact]
        public void IsIntegerType_RTRE_HighBitSet_True()
        {
            var entry = new ResourceTypeResourceEntry { ResourceID = 0xFFFF };
            bool actual = entry.IsIntegerType();
            Assert.True(actual);
        }

        [Fact]
        public void IsIntegerType_RTRE_HighBitClear_False()
        {
            var entry = new ResourceTypeResourceEntry { ResourceID = 0x0000 };
            bool actual = entry.IsIntegerType();
            Assert.False(actual);
        }

        #endregion

        #region GetEntryType

        [Theory]
        [InlineData(0x00, SegmentEntryType.Unused)]
        [InlineData(0x01, SegmentEntryType.FixedSegment)]
        [InlineData(0xAA, SegmentEntryType.FixedSegment)]
        [InlineData(0xFE, SegmentEntryType.FixedSegment)]
        [InlineData(0xFF, SegmentEntryType.MoveableSegment)]
        public void GetEntryTypeTest(byte segmentIndicator, SegmentEntryType expected)
        {
            var entry = new EntryTableBundle { SegmentIndicator = segmentIndicator };
            SegmentEntryType actual = entry.GetEntryType();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
