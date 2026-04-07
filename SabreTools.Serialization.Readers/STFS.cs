using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.STFS;
using SabreTools.IO.Extensions;
using SabreTools.Numerics;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class STFS : BaseBinaryReader<Volume>
    {
        /// <inheritdoc/>
        public override Volume? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            // Simple check for a valid stream length
            if (Constants.StandardHeaderSize > data.Length - data.Position)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new Volume to fill
                var volume = new Volume();

                // Read and validate the header
                var header = ParseHeader(data);
                if (header is null)
                    return null;

                volume.Header = header;

                // TODO: Parse the hash table blocks

                // Don't parse the data blocks into memory

                return volume;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header? ParseHeader(Stream data)
        {
            var obj = new Header();

            obj.MagicBytes = data.ReadBytes(4);
            var signature = System.Text.Encoding.ASCII.GetString(obj.MagicBytes);
            bool remoteSigned = signature.Equals(Constants.MagicStringLIVE) | signature.Equals(Constants.MagicStringPIRS);
            if (!remoteSigned && !signature.Equals(Constants.MagicStringCON))
                return null;

            obj.Signature = ParseSignature(data, remoteSigned);

            obj.LicensingData = ParseLicensingData(data);

            obj.HeaderHash = data.ReadBytes(20);
            obj.HeaderSize = data.ReadUInt32BigEndian();
            obj.ContentType = data.ReadInt32BigEndian();
            obj.MetadataVersion = data.ReadInt32BigEndian();
            obj.ContentSize = data.ReadInt64BigEndian();
            obj.MediaID = data.ReadUInt32BigEndian();
            obj.Version = data.ReadInt32BigEndian();
            obj.BaseVersion = data.ReadInt32BigEndian();
            obj.TitleID = data.ReadUInt32BigEndian();
            obj.Platform = data.ReadByteValue();
            obj.ExecutableType = data.ReadByteValue();
            obj.DiscNumber = data.ReadByteValue();
            obj.DiscInSet = data.ReadByteValue();
            obj.SaveGameID = data.ReadUInt32BigEndian();
            obj.ConsoleID = data.ReadBytes(5);
            obj.ProfileID = data.ReadBytes(8);

            // Peek forward to read whether VolumeDescriptor is SVOD or STFS
            byte[] peeked = data.PeekBytes(52);
            bool svod = peeked[51] == 0x01;
            obj.VolumeDescriptor = ParseVolumeDescriptor(data, svod);

            obj.DataFileCount = data.ReadInt32BigEndian();
            obj.DataFileCombinedSize = data.ReadInt64BigEndian();
            obj.DescriptorType = data.ReadUInt32BigEndian();
            obj.Reserved = data.ReadUInt32BigEndian();

            if (obj.MetadataVersion == 2)
            {
                obj.SeriesID = data.ReadBytes(16);
                obj.SeasonID = data.ReadBytes(16);
                obj.SeasonNumber = data.ReadInt16BigEndian();
                obj.EpisodeNumber = data.ReadInt16BigEndian();
                obj.Padding = data.ReadBytes(40);
            }
            else
            {
                obj.Padding = data.ReadBytes(76);
            }

            obj.DeviceID = data.ReadBytes(20);
            obj.DisplayName = data.ReadBytes(2304);
            obj.DisplayDescription = data.ReadBytes(2304);
            obj.PublisherName = data.ReadBytes(128);
            obj.TitleName = data.ReadBytes(128);
            obj.TransferFlags = data.ReadByteValue();
            obj.ThumbnailImageSize = data.ReadInt32BigEndian();
            obj.TitleThumbnailImageSize = data.ReadInt32BigEndian();

            if (obj.MetadataVersion == 2)
            {
                obj.ThumbnailImage = data.ReadBytes(0x3D00);
                obj.AdditionalDisplayNames = data.ReadBytes(768);
                obj.TitleThumbnailImage = data.ReadBytes(0x3D00);
                obj.AdditionalDisplayDescriptions = data.ReadBytes(768);
            }
            else
            {
                obj.ThumbnailImage = data.ReadBytes(0x4000);
                obj.TitleThumbnailImage = data.ReadBytes(0x4000);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Signature
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Signature</returns>
        public static Signature ParseSignature(Stream data, bool remoteSigned)
        {
            if (remoteSigned)
            {
                var obj = new MicrosoftSignature();

                obj.PackageSignature = data.ReadBytes(256);
                obj.Padding = data.ReadBytes(296);

                return obj;
            }
            else
            {
                var obj = new ConsoleSignature();

                obj.CertificateSize = data.ReadUInt16BigEndian();
                obj.ConsoleID = data.ReadBytes(5);
                obj.PartNumber = data.ReadBytes(20);
                obj.ConsoleType = data.ReadByteValue();
                obj.CertificateDate = data.ReadBytes(8);
                obj.PublicExponent = data.ReadBytes(4);
                obj.PublicModulus = data.ReadBytes(128);
                obj.CertificateSignature = data.ReadBytes(256);
                obj.Signature = data.ReadBytes(128);

                return obj;
            }
        }

        /// <summary>
        /// Parse a Stream into an array of LicenseEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled array of LicenseEntry</returns>
        public static LicenseEntry[] ParseLicensingData(Stream data)
        {
            var obj = new LicenseEntry[16];

            for (int i = 0; i < 16; i++)
            {
                obj[i].LicenseID = data.ReadInt64BigEndian();
                obj[i].LicenseBits = data.ReadInt32BigEndian();
                obj[i].LicenseFlags = data.ReadInt32BigEndian();
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a VolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled VolumeDescriptor</returns>
        public static VolumeDescriptor ParseVolumeDescriptor(Stream data, bool svod)
        {
            if (svod)
            {
                var obj = new SVODDescriptor();

                obj.VolumeDescriptorSize = data.ReadByteValue();
                obj.BlockCacheElementCount = data.ReadByteValue();
                obj.WorkerThreadProcessor = data.ReadByteValue();
                obj.WorkerThreadPriority = data.ReadByteValue();
                obj.Hash = data.ReadBytes(20);
                obj.DataBlockCount = data.ReadUInt24BigEndian();
                obj.DataBlockOffset = data.ReadUInt24BigEndian();
                obj.Hash = data.ReadBytes(5);

                return obj;
            }
            else
            {
                var obj = new STFSDescriptor();

                obj.VolumeDescriptorSize = data.ReadByteValue();
                obj.Reserved = data.ReadByteValue();
                obj.BlockSeparation = data.ReadByteValue();
                obj.FileTableBlockCount = data.ReadInt16BigEndian();
                obj.FileTableBlockNumber = data.ReadInt24BigEndian();
                obj.TopHashTableHash = data.ReadBytes(20);
                obj.TotalAllocatedBlockCount = data.ReadInt32BigEndian();
                obj.TotalUnallocatedBlockCount = data.ReadInt32BigEndian();

                return obj;
            }
        }
    }
}
