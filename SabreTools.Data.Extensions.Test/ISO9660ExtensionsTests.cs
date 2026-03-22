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
    }
}
