using System.Linq;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class XZExtensionsTests
    {
        [Fact]
        public void VariableLengthIntegerTest()
        {
            ulong expected = 123456789;
            byte[] encoded = expected.EncodeVariableLength();
            Assert.True(encoded.SequenceEqual<byte>([0x95, 0x9A, 0xEF, 0x3A]));

            ulong actual = encoded.DecodeVariableLength(maxSize: 16, out int length);
            Assert.Equal(expected, actual);
            Assert.Equal(4, length);
        }
    }
}
