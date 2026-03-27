using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.XDVDFS;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class XDVDFS : BaseBinaryReader<Volume>
    {
        /// <inheritdoc/>
        public override Volume? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            // Simple check for a valid stream length
            if ((Constants.ReservedSectors + 2) * Constants.SectorSize > data.Length - data.Position)
                return null;

            try
            {
                // Create a new Volume to fill
                var volume = new Volume();

                // Read the Reserved Area
                volume.ReservedArea = data.ReadBytes(Constants.ReservedSectors * Constants.SectorSize);

                // Read the set of Volume Descriptors
                var vd = ParseVolumeDescriptor(data);
                if (vd is null)
                    return null;

                volume.VolumeDescriptor = vd;

                // Parse the path table group(s) for each base volume descriptor
                var ld = ParseLayoutDescriptor(data);
                if (ld is null)
                    return null;

                volume.LayoutDescriptor = ld;

                // Parse the root directory descriptor
                var dd = ParseDirectoryDescriptors(data, volume.VolumeDescriptor);
                if (dd is null)
                    return null;

                volume.DirectoryDescriptors = dd;

                return volume;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a VolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled VolumeDescriptor on success, null on error</returns>
        public static VolumeDescriptor? ParseVolumeDescriptor(Stream data)
        {
            var obj = new VolumeDescriptor();

            obj.StartSignature = data.ReadBytes(20);
            obj.RootOffset = data.ReadUInt32();
            obj.RootSize = data.ReadUInt32();
            obj.MasteringTimestamp = data.ReadUInt64();
            obj.UnknownByte = data.ReadByteValue();
            obj.Reserved = data.ReadBytes(1991);
            obj.EndSignature = data.ReadBytes(20);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a LayoutDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LayoutDescriptor on success, null on error</returns>
        public static LayoutDescriptor? ParseLayoutDescriptor(Stream data)
        {
            var obj = new LayoutDescriptor();

            obj.Signature = data.ReadBytes(24);
            obj.Unusued8Bytes = data.ReadBytes(8);
            obj.XBLayoutVersion = ParseFourPartVersionType(data) ?? 0;
            obj.XBPremasterVersion = ParseFourPartVersionType(data) ?? 0;
            obj.XBGameDiscVersion = ParseFourPartVersionType(data) ?? 0;
            obj.XBOther1Version = ParseFourPartVersionType(data) ?? 0;
            obj.XBOther2Version = ParseFourPartVersionType(data) ?? 0;
            obj.XBOther3Version = ParseFourPartVersionType(data) ?? 0;
            obj.Reserved = data.ReadBytes(1968);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FourPartVersionType
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FourPartVersionType on success, null on error</returns>
        public static FourPartVersionType? ParseFourPartVersionType(Stream data)
        {
            var obj = new FourPartVersionType();

            obj.Major = data.ReadUInt16();
            obj.Minor = data.ReadUInt16();
            obj.Build = data.ReadUInt16();
            obj.Revision = data.ReadUInt16();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Dictionary of int to DirectoryDescriptors
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="vd">VolumeDescriptor pointing to root directory</param>
        /// <returns>Filled Dictionary of int to DirectoryDescriptors on success, null on error</returns>
        public static Dictionary<uint, DirectoryDescriptor>? ParseDirectoryDescriptors(Stream data, VolumeDescriptor vd)
        {
            var obj = new Dictionary<uint, DirectoryDescriptor>();

            var dd = ParseDirectoryDescriptor(data, vd.RootOffset);
            if (dd is null)
                return null;

            obj.Add(vd.RootOffset, dd);

            // TODO: Parse child directory descriptors

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryDescriptor on success, null on error</returns>
        public static DirectoryDescriptor? ParseDirectoryDescriptor(Stream data, uint size)
        {
            var obj = new DirectoryDescriptor();

            var records = new List<DirectoryRecord>();

            // TODO: Seek to start of directory descriptor
            var dr = ParseDirectoryRecord(data);
            if (dr is not null)
                obj.Add(dr);

            // TODO: Parse remaining records, check if next bytes are 0xFF ?

            obj.DirectoryRecords = [.. records];

            // TODO: Parse padding bytes
            int remainder = 0 % 2048;
            if (remainder > 0)
                obj.Padding = data.ReadBytes(remainder);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryRecord on success, null on error</returns>
        public static DirectoryRecord? ParseDirectoryRecord(Stream data)
        {
            var obj = new DirectoryRecord();

            obj.LeftChildOffset = data.ReadUInt16();
            obj.RightChildOffset = data.ReadUInt16();
            obj.ExtentOffset = data.ReadUInt32();
            obj.ExtentSize = data.ReadUInt32();
            obj.FileFlags = (FileFlags)data.ReadByteValue();
            obj.FilenameLength = data.ReadByteValue();
            obj.Filename = data.ReadBytes(obj.FilenameLength);
            int remainder = (2 + obj.FilenameLength) % 4;
            if (remainder > 0)
                obj.Padding = data.ReadBytes(remainder);

            return obj;
        }
    }
}
