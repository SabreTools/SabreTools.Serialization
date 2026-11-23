using System;
using System.IO;
using SabreTools.Data.Models.XDVDFS;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class WrapperFactoryTests
    {
        [Fact]
        public void ISO9660_XDVDFSImage_XDVDFS()
        {
            Stream data = new MemoryStream(CreateXDVDFSImage());
            var actual = WrapperFactory.CreateWrapper(WrapperType.ISO9660, data);
            Assert.IsType<XDVDFS>(actual);
        }

        [Fact]
        public void ISO9660_XDVDFSImageWithISO9660Descriptor_XDVDFS()
        {
            byte[] image = CreateXDVDFSImage();

            // Primary volume descriptor with a logical block size of 2048 but without a root directory,
            // followed by the set terminator, as extract-xiso writes them
            WriteISO9660Descriptor(image, 16, 0x01);
            image[(16 * Constants.SectorSize) + 129] = 0x08;
            image[(16 * Constants.SectorSize) + 130] = 0x08;
            WriteISO9660Descriptor(image, 17, 0xFF);

            Stream data = new MemoryStream(image);
            var actual = WrapperFactory.CreateWrapper(WrapperType.ISO9660, data);
            Assert.IsType<XDVDFS>(actual);
        }

        [Fact]
        public void ISO9660_XDVDFSImageStartingWithGameCubeTitleType_XDVDFS()
        {
            byte[] image = CreateXDVDFSImage();

            // The reserved area of a game partition is filler, whose first byte can be read as a GameCube game ID
            image[0] = (byte)'D';

            Stream data = new MemoryStream(image);
            var actual = WrapperFactory.CreateWrapper(WrapperType.ISO9660, data);
            Assert.IsType<XDVDFS>(actual);
        }

        #region Helpers

        /// <summary>
        /// Sector of the root directory
        /// </summary>
        private const uint RootDirectory = 33;

        /// <summary>
        /// Create an XDVDFS image with an empty reserved area and an empty root directory
        /// </summary>
        private static byte[] CreateXDVDFSImage()
        {
            byte[] image = new byte[(RootDirectory + 1) * Constants.SectorSize];

            var stream = new MemoryStream(image);
            stream.Position = Constants.ReservedSectors * Constants.SectorSize;
            Serialization.Writers.XDVDFS.SerializeVolumeDescriptor(stream, new VolumeDescriptor
            {
                StartSignature = Constants.VolumeDescriptorMagic,
                RootOffset = RootDirectory,
                RootSize = Constants.SectorSize,
                EndSignature = Constants.VolumeDescriptorMagic,
            });

            image.AsSpan((int)RootDirectory * Constants.SectorSize, Constants.SectorSize).Fill(0xFF);
            return image;
        }

        /// <summary>
        /// Write the type, standard identifier and version of an ISO9660 volume descriptor to a sector
        /// </summary>
        private static void WriteISO9660Descriptor(byte[] image, int sector, byte type)
        {
            int offset = sector * Constants.SectorSize;
            image[offset] = type;
            Data.Models.ISO9660.Constants.StandardIdentifier.CopyTo(image, offset + 1);
            image[offset + 6] = 0x01;
        }

        #endregion
    }
}
