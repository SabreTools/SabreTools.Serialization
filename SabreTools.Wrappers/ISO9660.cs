using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO;

namespace SabreTools.Wrappers
{
    public partial class ISO9660 : WrapperBase<Volume>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "ISO 9660 Volume";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Volume.SystemArea"/>
        public byte[] SystemArea => Model.SystemArea;

        /// <inheritdoc cref="Volume.VolumeDescriptorSet"/>
        public VolumeDescriptor[] VolumeDescriptorSet => Model.VolumeDescriptorSet;

        /// <inheritdoc cref="Volume.PathTableGroups"/>
        public PathTableGroup[] PathTableGroups => Model.PathTableGroups;

        /// <inheritdoc cref="Volume.DirectoryDescriptors"/>
        public Dictionary<int, FileExtent> DirectoryDescriptors => Model.DirectoryDescriptors;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public ISO9660(Volume model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public ISO9660(Volume model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public ISO9660(Volume model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public ISO9660(Volume model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public ISO9660(Volume model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public ISO9660(Volume model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an ISO9660 Volume from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the ISO9660 Volume</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An ISO 9660 Volume wrapper on success, null on failure</returns>
        public static ISO9660? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data is null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create an ISO9660 Volume from a Stream
        /// </summary>
        /// <param name="data">Stream representing the ISO9660 Volume</param>
        /// <returns>An ISO9660 Volume wrapper on success, null on failure</returns>
        public static ISO9660? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                // The reader addresses logical blocks from the start of the stream, so it reads a view that starts here
                var model = new Serialization.Readers.ISO9660().Deserialize(new ReadOnlyViewStream(data, currentOffset));
                if (model is null)
                    return null;

                return new ISO9660(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Create an ISO9660 Volume from one session of a multisession disc
        /// </summary>
        /// <param name="data">Stream holding the whole disc, one 2048-byte sector per sector address, sector 0 first</param>
        /// <param name="sessionStart">Sector address of the first track of the session</param>
        /// <returns>An ISO9660 Volume wrapper on success, null on failure</returns>
        /// <remarks>
        /// The volume descriptors of a session follow its own system area, but the extent locations recorded
        /// in it count from sector 0 of the disc, so files are read relative to the start of the stream
        /// </remarks>
        public static ISO9660? Create(Stream? data, long sessionStart)
        {
            // If the data is invalid
            if (data is null)
                return null;

            try
            {
                // Seek throws for a negative start or a stream that cannot seek
                data.Seek(sessionStart * Constants.MinimumSectorSize, SeekOrigin.Begin);

                var model = new Serialization.Readers.ISO9660().Deserialize(data);
                if (model is null)
                    return null;

                // Extent locations count from sector 0, not from the session
                return new ISO9660(model, data, 0);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
