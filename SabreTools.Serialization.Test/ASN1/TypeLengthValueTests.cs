using System;
using System.IO;
using SabreTools.Serialization.ASN1;
using Xunit;

namespace SabreTools.Serialization.Test.ASN1
{
    public class TypeLengthValueTests
    {
        #region Construction

        [Fact]
        public void Constructor_EmptyArray_Throws()
        {
            int index = 0;
            byte[] data = [];
            Assert.Throws<InvalidDataException>(() => new TypeLengthValue(data, ref index));
        }

        [Fact]
        public void Constructor_ValidArrayNegativeIndex_Throws()
        {
            int index = -1;
            byte[] data = [0x00];
            Assert.Throws<IndexOutOfRangeException>(() => new TypeLengthValue(data, ref index));
        }

        [Fact]
        public void Constructor_ValidArrayOverIndex_Throws()
        {
            int index = 10;
            byte[] data = [0x00];
            Assert.Throws<IndexOutOfRangeException>(() => new TypeLengthValue(data, ref index));
        }

        [Fact]
        public void Constructor_ValidMinimalArray()
        {
            int index = 0;
            byte[] data = [0x00];
            var tlv = new TypeLengthValue(data, ref index);

            Assert.Equal(ASN1Type.V_ASN1_EOC, tlv.Type);
            Assert.Equal(default, tlv.Length);
            Assert.Null(tlv.Value);
        }

        [Fact]
        public void Constructor_EmptyStream_Throws()
        {
            Stream data = new MemoryStream([], 0, 0, false, false);
            Assert.Throws<InvalidDataException>(() => new TypeLengthValue(data));
        }

        [Fact]
        public void Constructor_ValidMinimalStream()
        {
            Stream data = new MemoryStream([0x00]);
            var tlv = new TypeLengthValue(data);

            Assert.Equal(ASN1Type.V_ASN1_EOC, tlv.Type);
            Assert.Equal(default, tlv.Length);
            Assert.Null(tlv.Value);
        }

        [Fact]
        public void Constructor_ValidBoolean()
        {
            Stream data = new MemoryStream([0x01, 0x01, 0x01]);
            var tlv = new TypeLengthValue(data);

            Assert.Equal(ASN1Type.V_ASN1_BOOLEAN, tlv.Type);
            Assert.Equal(1UL, tlv.Length);
            Assert.NotNull(tlv.Value);

            byte[]? valueAsArray = tlv.Value as byte[];
            Assert.NotNull(valueAsArray);
            byte actual = Assert.Single(valueAsArray);
            Assert.Equal(0x01, actual);
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
        public void Constructor_ComplexValue(byte[] arr)
        {
            Stream data = new MemoryStream(arr);
            var tlv = new TypeLengthValue(data);

            Assert.Equal(ASN1Type.V_ASN1_CONSTRUCTED | ASN1Type.V_ASN1_OBJECT, tlv.Type);
            Assert.Equal(3UL, tlv.Length);
            Assert.NotNull(tlv.Value);

            TypeLengthValue[]? valueAsArray = tlv.Value as TypeLengthValue[];
            Assert.NotNull(valueAsArray);
            TypeLengthValue actual = Assert.Single(valueAsArray);

            Assert.Equal(ASN1Type.V_ASN1_BOOLEAN, actual.Type);
            Assert.Equal(1UL, actual.Length);
            Assert.NotNull(actual.Value);
        }

        [Theory]
        [InlineData(new byte[] { 0x26, 0x80 })]
        [InlineData(new byte[] { 0x26, 0x89 })]
        public void Constructor_ComplexValueInvalidLength_Throws(byte[] arr)
        {
            Stream data = new MemoryStream(arr);
            Assert.Throws<InvalidOperationException>(() => new TypeLengthValue(data));
        }

        #endregion

        #region Formatting

        [Fact]
        public void Format_EOC()
        {
            string expected = "Type: V_ASN1_EOC";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_EOC, 0, null);
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ZeroLength()
        {
            string expected = "Type: V_ASN1_NULL, Length: 0";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_NULL, 0, null);
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidConstructed()
        {
            string expected = "Type: V_ASN1_OBJECT, V_ASN1_CONSTRUCTED, Length: 1, Value: [INVALID DATA TYPE]";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_OBJECT | ASN1Type.V_ASN1_CONSTRUCTED, 1, (object?)false);
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidConstructed()
        {
            string expected = "Type: V_ASN1_OBJECT, V_ASN1_CONSTRUCTED, Length: 3, Value:\n Type: V_ASN1_BOOLEAN, Length: 1, Value: True";
            var boolTlv = new TypeLengthValue(ASN1Type.V_ASN1_BOOLEAN, 1, new byte[] { 0x01 });
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_OBJECT | ASN1Type.V_ASN1_CONSTRUCTED, 3, new TypeLengthValue[] { boolTlv });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidDataType()
        {
            string expected = "Type: V_ASN1_OBJECT, Length: 1, Value: [INVALID DATA TYPE]";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_OBJECT, 1, (object?)false);
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidLength()
        {
            string expected = "Type: V_ASN1_NULL, Length: 1, Value: [NO DATA]";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_NULL, 1, Array.Empty<byte>());
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidBooleanLength()
        {
            string expected = "Type: V_ASN1_BOOLEAN, Length: 2 [Expected length of 1], Value: True";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_BOOLEAN, 2, new byte[] { 0x01 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidBooleanArrayLength()
        {
            string expected = "Type: V_ASN1_BOOLEAN, Length: 1 [Expected value length of 1], Value: True";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_BOOLEAN, 1, new byte[] { 0x01, 0x00 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidBoolean()
        {
            string expected = "Type: V_ASN1_BOOLEAN, Length: 1, Value: True";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_BOOLEAN, 1, new byte[] { 0x01 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidInteger()
        {
            string expected = "Type: V_ASN1_INTEGER, Length: 1, Value: 1";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_INTEGER, 1, new byte[] { 0x01 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidBitString_NoBits()
        {
            string expected = "Type: V_ASN1_BIT_STRING, Length: 1, Value with 0 unused bits";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_BIT_STRING, 1, new byte[] { 0x00 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidBitString_Bits()
        {
            string expected = "Type: V_ASN1_BIT_STRING, Length: 1, Value with 1 unused bits: 01";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_BIT_STRING, 1, new byte[] { 0x01, 0x01 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidOctetString()
        {
            string expected = "Type: V_ASN1_OCTET_STRING, Length: 1, Value: 01";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_OCTET_STRING, 1, new byte[] { 0x01 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidObject()
        {
            string expected = "Type: V_ASN1_OBJECT, Length: 3, Value: 0.1.2.3 (/ITU-T/1/2/3)";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_OBJECT, 3, new byte[] { 0x01, 0x02, 0x03 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidUTF8String()
        {
            string expected = "Type: V_ASN1_UTF8STRING, Length: 3, Value: ABC";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_UTF8STRING, 3, new byte[] { 0x41, 0x42, 0x43 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidPrintableString()
        {
            string expected = "Type: V_ASN1_PRINTABLESTRING, Length: 3, Value: ABC";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_PRINTABLESTRING, 3, new byte[] { 0x41, 0x42, 0x43 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidTeletexString()
        {
            string expected = "Type: V_ASN1_TELETEXSTRING, Length: 3, Value: ABC";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_TELETEXSTRING, 3, new byte[] { 0x41, 0x42, 0x43 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidIA5String()
        {
            string expected = "Type: V_ASN1_IA5STRING, Length: 3, Value: ABC";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_IA5STRING, 3, new byte[] { 0x41, 0x42, 0x43 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidUTCTime()
        {
            string expected = "Type: V_ASN1_UTCTIME, Length: 3, Value: ABC";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_UTCTIME, 3, new byte[] { 0x41, 0x42, 0x43 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidUTCTime()
        {
            string expected = "Type: V_ASN1_UTCTIME, Length: 3, Value: 1980-01-01 00:00:00";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_UTCTIME, 3, new byte[] { 0x31, 0x39, 0x38, 0x30, 0x2D, 0x30, 0x31, 0x2D, 0x30, 0x31, 0x20, 0x30, 0x30, 0x3A, 0x30, 0x30, 0x3A, 0x30, 0x30 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidBmpString()
        {
            string expected = "Type: V_ASN1_BMPSTRING, Length: 6, Value: ABC";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_BMPSTRING, 6, new byte[] { 0x41, 0x00, 0x42, 0x00, 0x43, 0x00 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidUnformatted()
        {
            string expected = "Type: V_ASN1_OBJECT_DESCRIPTOR, Length: 1, Value: 01";
            var tlv = new TypeLengthValue(ASN1Type.V_ASN1_OBJECT_DESCRIPTOR, 1, new byte[] { 0x01 });
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}