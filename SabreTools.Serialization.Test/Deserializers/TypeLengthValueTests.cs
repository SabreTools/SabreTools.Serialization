using System.IO;
using System.Linq;
using SabreTools.Serialization.Deserializers;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class TypeLengthValueTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new TypeLengthValue();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new TypeLengthValue();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new TypeLengthValue();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new TypeLengthValue();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new TypeLengthValue();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new TypeLengthValue();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void ValidMinimalStream_NotNull()
        {
            Stream data = new MemoryStream([0x00]);
            var deserializer = new TypeLengthValue();

            var actual = deserializer.Deserialize(data);
            Assert.NotNull(actual);
            Assert.Equal(Serialization.ASN1.ASN1Type.V_ASN1_EOC, actual.Type);
            Assert.Equal(default, actual.Length);
            Assert.Null(actual.Value);
        }

        [Fact]
        public void ValidBoolean_NotNull()
        {
            Stream data = new MemoryStream([0x01, 0x01, 0x01]);
            var deserializer = new TypeLengthValue();

            var actual = deserializer.Deserialize(data);
            Assert.NotNull(actual);
            Assert.Equal(Serialization.ASN1.ASN1Type.V_ASN1_BOOLEAN, actual.Type);
            Assert.Equal(1UL, actual.Length);
            Assert.NotNull(actual.Value);

            byte[]? valueAsArray = actual.Value as byte[];
            Assert.NotNull(valueAsArray);
            byte actualValue = Assert.Single(valueAsArray);
            Assert.Equal(0x01, actualValue);
        }

        [Theory]
        [InlineData(new byte[] { 0x26, 0x81, 0x03, 0x01, 0x01, 0x01 })]
        [InlineData(new byte[] { 0x26, 0x82, 0x00, 0x03, 0x01, 0x01, 0x01 })]
        [InlineData(new byte[] { 0x26, 0x83, 0x00, 0x00, 0x03, 0x01, 0x01, 0x01 })]
        [InlineData(new byte[] { 0x26, 0x84, 0x00, 0x00, 0x00, 0x03, 0x01, 0x01, 0x01 })]
        [InlineData(new byte[] { 0x26, 0x85, 0x00, 0x00, 0x00, 0x00, 0x03, 0x01, 0x01, 0x01 })]
        [InlineData(new byte[] { 0x26, 0x86, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x01, 0x01, 0x01 })]
        [InlineData(new byte[] { 0x26, 0x87, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x01, 0x01, 0x01 })]
        [InlineData(new byte[] { 0x26, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x01, 0x01, 0x01 })]
        public void ComplexValue_NotNull(byte[] arr)
        {
            Stream data = new MemoryStream(arr);
            var deserializer = new TypeLengthValue();

            var actual = deserializer.Deserialize(data);
            Assert.NotNull(actual);
            Assert.Equal(Serialization.ASN1.ASN1Type.V_ASN1_CONSTRUCTED | Serialization.ASN1.ASN1Type.V_ASN1_OBJECT, actual.Type);
            Assert.Equal(3UL, actual.Length);
            Assert.NotNull(actual.Value);

            Serialization.ASN1.TypeLengthValue[]? valueAsArray = actual.Value as Serialization.ASN1.TypeLengthValue[];
            Assert.NotNull(valueAsArray);
            Serialization.ASN1.TypeLengthValue actualSub = Assert.Single(valueAsArray);

            Assert.Equal(Serialization.ASN1.ASN1Type.V_ASN1_BOOLEAN, actualSub.Type);
            Assert.Equal(1UL, actualSub.Length);
            Assert.NotNull(actualSub.Value);
        }

        [Theory]
        [InlineData(new byte[] { 0x26, 0x80 })]
        [InlineData(new byte[] { 0x26, 0x89 })]
        public void ComplexValueInvalidLength_Null(byte[] arr)
        {
            Stream data = new MemoryStream(arr);
            var deserializer = new TypeLengthValue();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }
    }
}