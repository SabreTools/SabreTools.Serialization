using System;
using SabreTools.Data.Models.ASN1;
using SabreTools.Serialization.Extensions;
using Xunit;

namespace SabreTools.Serialization.Test.Extensions
{
    public class TypeLengthValueTests
    {
        #region Formatting

        [Fact]
        public void Format_EOC()
        {
            string expected = "Type: V_ASN1_EOC";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_EOC, Length = 0, Value = null };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ZeroLength()
        {
            string expected = "Type: V_ASN1_NULL, Length: 0";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_NULL, Length = 0, Value = null };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidConstructed()
        {
            string expected = "Type: V_ASN1_OBJECT, V_ASN1_CONSTRUCTED, Length: 1, Value: [INVALID DATA TYPE]";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_OBJECT | ASN1Type.V_ASN1_CONSTRUCTED, Length = 1, Value = (object?)false };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidConstructed()
        {
            string expected = "Type: V_ASN1_OBJECT, V_ASN1_CONSTRUCTED, Length: 3, Value:\n Type: V_ASN1_BOOLEAN, Length: 1, Value: True";
            var boolTlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_BOOLEAN, Length = 1, Value = new byte[] { 0x01 } };
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_OBJECT | ASN1Type.V_ASN1_CONSTRUCTED, Length = 3, Value = new Data.Models.ASN1.TypeLengthValue[] { boolTlv } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidDataType()
        {
            string expected = "Type: V_ASN1_OBJECT, Length: 1, Value: [INVALID DATA TYPE]";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_OBJECT, Length = 1, Value = (object?)false };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidLength()
        {
            string expected = "Type: V_ASN1_NULL, Length: 1, Value: [NO DATA]";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_NULL, Length = 1, Value = Array.Empty<byte>() };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidBooleanLength()
        {
            string expected = "Type: V_ASN1_BOOLEAN, Length: 2 [Expected length of 1], Value: True";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_BOOLEAN, Length = 2, Value = new byte[] { 0x01 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidBooleanArrayLength()
        {
            string expected = "Type: V_ASN1_BOOLEAN, Length: 1 [Expected value length of 1], Value: True";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_BOOLEAN, Length = 1, Value = new byte[] { 0x01, 0x00 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidBoolean()
        {
            string expected = "Type: V_ASN1_BOOLEAN, Length: 1, Value: True";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_BOOLEAN, Length = 1, Value = new byte[] { 0x01 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidInteger()
        {
            string expected = "Type: V_ASN1_INTEGER, Length: 1, Value: 1";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_INTEGER, Length = 1, Value = new byte[] { 0x01 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidBitString_NoBits()
        {
            string expected = "Type: V_ASN1_BIT_STRING, Length: 1, Value with 0 unused bits";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_BIT_STRING, Length = 1, Value = new byte[] { 0x00 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidBitString_Bits()
        {
            string expected = "Type: V_ASN1_BIT_STRING, Length: 1, Value with 1 unused bits: 01";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_BIT_STRING, Length = 1, Value = new byte[] { 0x01, 0x01 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidOctetString()
        {
            string expected = "Type: V_ASN1_OCTET_STRING, Length: 1, Value: 01";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_OCTET_STRING, Length = 1, Value = new byte[] { 0x01 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidObject()
        {
            string expected = "Type: V_ASN1_OBJECT, Length: 3, Value: 0.1.2.3 (/ITU-T/1/2/3)";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_OBJECT, Length = 3, Value = new byte[] { 0x01, 0x02, 0x03 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidUTF8String()
        {
            string expected = "Type: V_ASN1_UTF8STRING, Length: 3, Value: ABC";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_UTF8STRING, Length = 3, Value = new byte[] { 0x41, 0x42, 0x43 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidPrintableString()
        {
            string expected = "Type: V_ASN1_PRINTABLESTRING, Length: 3, Value: ABC";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_PRINTABLESTRING, Length = 3, Value = new byte[] { 0x41, 0x42, 0x43 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidTeletexString()
        {
            string expected = "Type: V_ASN1_TELETEXSTRING, Length: 3, Value: ABC";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_TELETEXSTRING, Length = 3, Value = new byte[] { 0x41, 0x42, 0x43 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidIA5String()
        {
            string expected = "Type: V_ASN1_IA5STRING, Length: 3, Value: ABC";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_IA5STRING, Length = 3, Value = new byte[] { 0x41, 0x42, 0x43 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_InvalidUTCTime()
        {
            string expected = "Type: V_ASN1_UTCTIME, Length: 3, Value: ABC";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_UTCTIME, Length = 3, Value = new byte[] { 0x41, 0x42, 0x43 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidUTCTime()
        {
            string expected = "Type: V_ASN1_UTCTIME, Length: 3, Value: 1980-01-01 00:00:00";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_UTCTIME, Length = 3, Value = new byte[] { 0x31, 0x39, 0x38, 0x30, 0x2D, 0x30, 0x31, 0x2D, 0x30, 0x31, 0x20, 0x30, 0x30, 0x3A, 0x30, 0x30, 0x3A, 0x30, 0x30 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidBmpString()
        {
            string expected = "Type: V_ASN1_BMPSTRING, Length: 6, Value: ABC";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_BMPSTRING, Length = 6, Value = new byte[] { 0x41, 0x00, 0x42, 0x00, 0x43, 0x00 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Format_ValidUnformatted()
        {
            string expected = "Type: V_ASN1_OBJECT_DESCRIPTOR, Length: 1, Value: 01";
            var tlv = new Data.Models.ASN1.TypeLengthValue { Type = ASN1Type.V_ASN1_OBJECT_DESCRIPTOR, Length = 1, Value = new byte[] { 0x01 } };
            string actual = tlv.Format();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}