using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.XDVDFS;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;

#pragma warning disable IDE0017 // Simplify object initialization
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
                // Cache the current offset
                long initialOffset = data.Position;

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
                var dd = ParseDirectoryDescriptors(data, initialOffset, vd.RootOffset, vd.RootSize);
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
            var signature = System.Text.Encoding.ASCII.GetString(obj.StartSignature);
            if (!signature.Equals(Constants.VolumeDescriptorSignature))
                return null;

            obj.RootOffset = data.ReadUInt32LittleEndian();
            obj.RootSize = data.ReadUInt32LittleEndian();
            obj.MasteringTimestamp = data.ReadInt64LittleEndian();
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
            var signature = System.Text.Encoding.ASCII.GetString(obj.Signature);
            if (!signature.Equals(Constants.LayoutDescriptorSignature))
                return null;
            obj.Unusued8Bytes = data.ReadBytes(8);

            obj.XBLayoutVersion = ParseFourPartVersionType(data);
            obj.XBPremasterVersion = ParseFourPartVersionType(data);
            obj.XBGameDiscVersion = ParseFourPartVersionType(data);
            obj.XBOther1Version = ParseFourPartVersionType(data);
            obj.XBOther2Version = ParseFourPartVersionType(data);
            obj.XBOther3Version = ParseFourPartVersionType(data);

            obj.Reserved = data.ReadBytes(1968);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FourPartVersionType
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FourPartVersionType on success, null on error</returns>
        public static FourPartVersionType ParseFourPartVersionType(Stream data)
        {
            var obj = new FourPartVersionType();

            obj.Major = data.ReadUInt16LittleEndian();
            obj.Minor = data.ReadUInt16LittleEndian();
            obj.Build = data.ReadUInt16LittleEndian();
            obj.Revision = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Dictionary of int to DirectoryDescriptors
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="offset">Sector number descriptor is located at</param>
        /// <param name="size">Number of bytes descriptor contains</param>
        /// <returns>Filled Dictionary of int to DirectoryDescriptors on success, null on error</returns>
        public static Dictionary<uint, DirectoryDescriptor>? ParseDirectoryDescriptors(Stream data, long initialOffset, uint offset, uint size)
        {
            // Ensure descriptor size is valid
            if (size < 14)
                return null;

            // Ensure offset is valid
            if ((offset * Constants.SectorSize) + size > data.Length)
                return null;

            var obj = new Dictionary<uint, DirectoryDescriptor>();

            var dd = ParseDirectoryDescriptor(data, initialOffset, offset, size);
            if (dd is null)
                return null;

            obj.Add(offset, dd);

            // Parse all child descriptors
            foreach (var dr in dd.DirectoryRecords)
            {
                if ((dr.FileFlags & FileFlags.DIRECTORY) == FileFlags.DIRECTORY)
                {
                    // Ensure same descriptor is never parsed twice
                    if (obj.ContainsKey(dr.ExtentOffset))
                        continue;

                    // Get all descriptors from child
                    var descriptors = ParseDirectoryDescriptors(data, initialOffset, dr.ExtentOffset, dr.ExtentSize);
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
        public static DirectoryDescriptor? ParseDirectoryDescriptor(Stream data, long initialOffset, uint offset, uint size)
        {
            // Ensure descriptor size is valid
            if (size < Constants.MinimumRecordLength)
                return null;

            // Ensure offset is valid
            if ((((long)offset) * Constants.SectorSize) + size > data.Length)
                return null;

            var obj = new DirectoryDescriptor();
            var records = new List<DirectoryRecord>();

            data.SeekIfPossible(initialOffset + (((long)offset) * Constants.SectorSize), SeekOrigin.Begin);
            long curPosition;
            while (size > data.Position - (initialOffset + (((long)offset) * Constants.SectorSize)))
            {
                curPosition = data.Position;
                var dr = ParseDirectoryRecord(data);
                if (dr is not null)
                    records.Add(dr);

                // If invalid record read or next descriptor cannot fit in the current sector, skip ahead
                if (dr is null || (data.Position - initialOffset) % Constants.SectorSize > (Constants.SectorSize - Constants.MinimumRecordLength))
                {
                    data.SeekIfPossible(Constants.SectorSize - (int)((data.Position - initialOffset) % Constants.SectorSize), SeekOrigin.Current);
                    continue;
                }

                // Exit loop if stream has not advanced
                if (curPosition == data.Position)
                    break;
            }

            obj.DirectoryRecords = [.. records];

            int remainder = Constants.SectorSize - (int)(size % Constants.SectorSize);
            if (remainder > 0 && remainder < Constants.SectorSize)
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

            obj.LeftChildOffset = data.ReadUInt16LittleEndian();
            obj.RightChildOffset = data.ReadUInt16LittleEndian();
            if (obj.LeftChildOffset == 0xFFFF && obj.RightChildOffset == 0xFFFF)
                return null;

            obj.ExtentOffset = data.ReadUInt32LittleEndian();
            obj.ExtentSize = data.ReadUInt32LittleEndian();
            obj.FileFlags = (FileFlags)data.ReadByteValue();
            obj.FilenameLength = data.ReadByteValue();
            obj.Filename = data.ReadBytes(obj.FilenameLength);
            int remainder = 4 - (int)(data.Position % 4);
            if (remainder > 0 && remainder < 4)
                obj.Padding = data.ReadBytes(remainder);

            return obj;
        }
    }
}
