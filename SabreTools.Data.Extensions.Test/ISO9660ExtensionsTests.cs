using SabreTools.Data.Models.ISO9660;
using SabreTools.Numerics;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class ISO9660ExtensionsTests
    {
        [Fact]
        public void GetLogicalBlockSize_Generic_SectorLength()
        {
            VolumeDescriptor vd = new GenericVolumeDescriptor();
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(sectorLength, actual);
        }

        [Fact]
        public void GetLogicalBlockSize_PVD_ValidBothEndian_ValidBlockSize_BlockSize()
        {
            VolumeDescriptor vd = new PrimaryVolumeDescriptor
            {
                LogicalBlockSize = new BothInt16(2048, 2048)
            };
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(2048, actual);
        }

        [Fact]
        public void GetLogicalBlockSize_PVD_ValidBothEndian_InvalidBlockSize_SectorLength()
        {
            VolumeDescriptor vd = new PrimaryVolumeDescriptor
            {
                LogicalBlockSize = new BothInt16(2352, 2352)
            };
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(4096, actual);
        }

        [Fact]
        public void GetLogicalBlockSize_PVD_InvalidBothEndian_ValidLE_LEValue()
        {
            VolumeDescriptor vd = new PrimaryVolumeDescriptor
            {
                LogicalBlockSize = new BothInt16(2048, -1)
            };
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(2048, actual);
        }

        [Fact]
        public void GetLogicalBlockSize_PVD_InvalidBothEndian_ValidBE_BEValue()
        {
            VolumeDescriptor vd = new PrimaryVolumeDescriptor
            {
                LogicalBlockSize = new BothInt16(-1, 2048)
            };
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(2048, actual);
        }

        [Fact]
        public void GetLogicalBlockSize_PVD_InvalidBothEndian_BothInvalid_SectorLength()
        {
            VolumeDescriptor vd = new PrimaryVolumeDescriptor
            {
                LogicalBlockSize = new BothInt16(-1, -2)
            };
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(sectorLength, actual);
        }

        [Fact]
        public void GetLogicalBlockSize_SVD_ValidBothEndian_ValidBlockSize_BlockSize()
        {
            VolumeDescriptor vd = new SupplementaryVolumeDescriptor
            {
                LogicalBlockSize = new BothInt16(2048, 2048)
            };
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(2048, actual);
        }

        [Fact]
        public void GetLogicalBlockSize_SVD_ValidBothEndian_InvalidBlockSize_SectorLength()
        {
            VolumeDescriptor vd = new SupplementaryVolumeDescriptor
            {
                LogicalBlockSize = new BothInt16(2352, 2352)
            };
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(4096, actual);
        }

        [Fact]
        public void GetLogicalBlockSize_SVD_InvalidBothEndian_ValidLE_LEValue()
        {
            VolumeDescriptor vd = new SupplementaryVolumeDescriptor
            {
                LogicalBlockSize = new BothInt16(2048, -1)
            };
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(2048, actual);
        }

        [Fact]
        public void GetLogicalBlockSize_SVD_InvalidBothEndian_ValidBE_BEValue()
        {
            VolumeDescriptor vd = new SupplementaryVolumeDescriptor
            {
                LogicalBlockSize = new BothInt16(-1, 2048)
            };
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(2048, actual);
        }

        [Fact]
        public void GetLogicalBlockSize_SVD_InvalidBothEndian_BothInvalid_SectorLength()
        {
            VolumeDescriptor vd = new SupplementaryVolumeDescriptor
            {
                LogicalBlockSize = new BothInt16(-1, -2)
            };
            short sectorLength = 4096;
            short actual = vd.GetLogicalBlockSize(sectorLength);
            Assert.Equal(sectorLength, actual);
        }

        [Theory]
        [InlineData("0000000041115841000000000000", true)]
        [InlineData("000000004111584100000000000041410E0254455854747478740000", true)]
        [InlineData("0000000040005841000000000000", true)]
        [InlineData("000000000D555841000000000000", false)]
        [InlineData("00000000BF005841000000000000", false)]
        [InlineData("0000000041115858000000000000", false)]
        [InlineData("0000000041110041000000000000", false)]
        [InlineData("00000000411158410000000000", false)]
        [InlineData("00", false)]
        [InlineData("", false)]
        public void IsCDDA_SystemUse_ReadsCDDAAttribute(string systemUse, bool expected)
        {
            var dr = new DirectoryRecord
            {
                SystemUse = System.Convert.FromHexString(systemUse)
            };

            bool actual = dr.IsCDDA();
            Assert.Equal(expected, actual);
        }
    }
}
