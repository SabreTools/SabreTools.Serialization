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

                // Read and validate the volume descriptor
                var vd = ParseVolumeDescriptor(data);
                if (vd is null)
                    return null;

                volume.VolumeDescriptor = vd;

                // Parse the optional layout descriptor
                volume.LayoutDescriptor = ParseLayoutDescriptor(data);

                // Parse the descriptors from the root directory descriptor
                var dd = ParseDirectoryDescriptors(data, vd.RootOffset, vd.RootSize);
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
            if (!obj.EqualsExactly(Constants.VOLUME_DESCRIPTOR_SIG))
                return null;

            obj.RootOffset = data.ReadUInt32();
            obj.RootSize = data.ReadUInt32();
            obj.MasteringTimestamp = data.ReadInt64();
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
            if (!obj.Signature.EqualsExactly(Constants.LAYOUT_DESCRIPTOR_MAGIC))
                return null;
            obj.Unusued8Bytes = data.ReadBytes(8);

            obj.XBLayoutVersion = ParseFourPartVersionType(data) ?? new FourPartVersionType();
            obj.XBPremasterVersion = ParseFourPartVersionType(data) ?? new FourPartVersionType();
            obj.XBGameDiscVersion = ParseFourPartVersionType(data) ?? new FourPartVersionType();
            obj.XBOther1Version = ParseFourPartVersionType(data) ?? new FourPartVersionType();
            obj.XBOther2Version = ParseFourPartVersionType(data) ?? new FourPartVersionType();
            obj.XBOther3Version = ParseFourPartVersionType(data) ?? new FourPartVersionType();

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
        /// <param name="offset">Sector number descriptor is located at</param>
        /// <param name="size">Number of bytes descriptor contains</param>
        /// <returns>Filled Dictionary of int to DirectoryDescriptors on success, null on error</returns>
        public static Dictionary<uint, DirectoryDescriptor>? ParseDirectoryDescriptors(Stream data, uint offset, uint size)
        {
            var obj = new Dictionary<uint, DirectoryDescriptor>();

            var dd = ParseDirectoryDescriptor(data, offset, size);
            if (dd is null)
                return null;

            obj.TryAdd(offset, dd);

            // Parse all child descriptors
            foreach (var dr in vd.DirectoryRecords)
            {
                if (dr.FileFlags & FileFlags.DIRECTORY == FileFlags.DIRECTORY)
                {
                    // Get all descriptors from child
                    var descriptors = ParseDirectoryDescriptors(data, dr.ExtentOffset, dr.ExtentSize);
                    if (descriptors is null)
                        continue;

                    // Merge dictionaries
                    foreach (var kvp in descriptors)
                    {
                        if (!obj.ContainsKey(kvp.Key))
                            obj.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="offset">Sector number descriptor is located at</param>
        /// <param name="size">Number of bytes descriptor contains</param>
        /// <returns>Filled DirectoryDescriptor on success, null on error</returns>
        public static DirectoryDescriptor? ParseDirectoryDescriptor(Stream data, uint offset, uint size)
        {
            var obj = new DirectoryDescriptor();
            var records = new List<DirectoryRecord>();

            data.SeekIfPossible(Constants.SectorSize * offset, SeekOrigin.Begin);
            long curPosition = data.Position;
            while (size > Constants.SectorSize * offset - data.Position)
            {
                var dr = ParseDirectoryRecord(data);
                if (dr is not null)
                    records.TryAdd(dr);

                // Exit early if stream does not advance
                if (curPosition == data.Position)
                    break;
            }

            obj.DirectoryRecords = [.. records];

            int remainder = size % 2048;
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
