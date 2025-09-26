using System.IO;
using System.Linq;
using SabreTools.Data.Models.ASN1;
using SabreTools.Serialization.Readers;
using Xunit;

namespace SabreTools.Serialization.Test.Readers
{
    public class AbstractSyntaxNotationOneTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new AbstractSyntaxNotationOne();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new AbstractSyntaxNotationOne();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new AbstractSyntaxNotationOne();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new AbstractSyntaxNotationOne();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new AbstractSyntaxNotationOne();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new AbstractSyntaxNotationOne();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void ValidMinimalStream_NotNull()
        {
            Stream data = new MemoryStream([0x00]);
            var deserializer = new AbstractSyntaxNotationOne();

            var actual = deserializer.Deserialize(data);
            Assert.NotNull(actual);

            var actualSingle = Assert.Single(actual);
            Assert.Equal(ASN1Type.V_ASN1_EOC, actualSingle.Type);
            Assert.Equal(default, actualSingle.Length);
            Assert.Null(actualSingle.Value);
        }

        [Fact]
        public void ValidBoolean_NotNull()
        {
            Stream data = new MemoryStream([0x01, 0x01, 0x01]);
            var deserializer = new AbstractSyntaxNotationOne();

            var actual = deserializer.Deserialize(data);
            Assert.NotNull(actual);

            var actualSingle = Assert.Single(actual);
            Assert.Equal(ASN1Type.V_ASN1_BOOLEAN, actualSingle.Type);
            Assert.Equal(1UL, actualSingle.Length);
            Assert.NotNull(actualSingle.Value);

            byte[]? valueAsArray = actualSingle.Value as byte[];
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
            var deserializer = new AbstractSyntaxNotationOne();

            var actual = deserializer.Deserialize(data);
            Assert.NotNull(actual);

            var actualSingle = Assert.Single(actual);
            Assert.Equal(ASN1Type.V_ASN1_CONSTRUCTED | ASN1Type.V_ASN1_OBJECT, actualSingle.Type);
            Assert.Equal(3UL, actualSingle.Length);
            Assert.NotNull(actualSingle.Value);

            TypeLengthValue[]? valueAsArray = actualSingle.Value as TypeLengthValue[];
            Assert.NotNull(valueAsArray);
            TypeLengthValue actualSub = Assert.Single(valueAsArray);

            Assert.Equal(ASN1Type.V_ASN1_BOOLEAN, actualSub.Type);
            Assert.Equal(1UL, actualSub.Length);
            Assert.NotNull(actualSub.Value);
        }

        [Theory]
        [InlineData(new byte[] { 0x26, 0x80 })]
        [InlineData(new byte[] { 0x26, 0x89 })]
        public void ComplexValueInvalidLength_Null(byte[] arr)
        {
            Stream data = new MemoryStream(arr);
            var deserializer = new AbstractSyntaxNotationOne();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }
    }
}