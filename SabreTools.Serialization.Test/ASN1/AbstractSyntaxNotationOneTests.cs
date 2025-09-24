using System;
using System.IO;
using SabreTools.Serialization.ASN1;
using Xunit;

namespace SabreTools.Serialization.Test.ASN1
{
    public class AbstractSyntaxNotationOneTests
    {
        [Fact]
        public void Parse_EmptyArray_Throws()
        {
            byte[] data = [];
            Assert.Throws<InvalidDataException>(() => AbstractSyntaxNotationOne.Parse(data, 0));
        }

        [Fact]
        public void Parse_ValidArrayNegativeIndex_Throws()
        {
            byte[] data = [0x00];
            Assert.Throws<IndexOutOfRangeException>(() => AbstractSyntaxNotationOne.Parse(data, -1));
        }

        [Fact]
        public void Parse_ValidArrayOverIndex_Throws()
        {
            byte[] data = [0x00];
            Assert.Throws<IndexOutOfRangeException>(() => AbstractSyntaxNotationOne.Parse(data, 10));
        }

        [Fact]
        public void Parse_ValidMinimalArray()
        {
            byte[] data = [0x00];
            var tlvs = AbstractSyntaxNotationOne.Parse(data, 0);

            var tlv = Assert.Single(tlvs);
            Assert.Equal(ASN1Type.V_ASN1_EOC, tlv.Type);
            Assert.Equal(default, tlv.Length);
            Assert.Null(tlv.Value);
        }

        [Fact]
        public void Parse_EmptyStream_Throws()
        {
            Stream data = new MemoryStream([], 0, 0, false, false);
            Assert.Throws<InvalidDataException>(() => AbstractSyntaxNotationOne.Parse(data));
        }

        [Fact]
        public void Parse_ValidMinimalStream()
        {
            Stream data = new MemoryStream([0x00]);
            var tlvs = AbstractSyntaxNotationOne.Parse(data);

            var tlv = Assert.Single(tlvs);
            Assert.Equal(ASN1Type.V_ASN1_EOC, tlv.Type);
            Assert.Equal(default, tlv.Length);
            Assert.Null(tlv.Value);
        }
    }
}