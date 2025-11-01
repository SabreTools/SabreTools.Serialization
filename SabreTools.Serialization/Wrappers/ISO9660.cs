using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.ISO9660;

namespace SabreTools.Serialization.Wrappers
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

        #region Global Variables

        private var extractedFiles = new Dictionary<int, int>();

        private var multiExtentFiles = new List<byte[]>();

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
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An ISO 9660 Volume wrapper on success, null on failure</returns>
        public static ISO9660? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
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
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An ISO9660 Volume wrapper on success, null on failure</returns>
        public static ISO9660? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.ISO9660().Deserialize(data);
                if (model == null)
                    return null;

                return new ISO9660(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
