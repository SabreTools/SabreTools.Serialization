using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.XDVDFS;

namespace SabreTools.Wrappers
{
    public partial class XDVDFS : WrapperBase<Volume>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xbox DVD Filesystem";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Volume.ReservedArea"/>
        public byte[] ReservedArea => Model.ReservedArea;

        /// <inheritdoc cref="Volume.VolumeDescriptor"/>
        public VolumeDescriptor VolumeDescriptor => Model.VolumeDescriptor;

        /// <inheritdoc cref="Volume.LayoutDescriptor"/>
        public LayoutDescriptor LayoutDescriptor => Model.LayoutDescriptor;

        /// <inheritdoc cref="Volume.DirectoryDescriptors"/>
        public Dictionary<int, DirectoryDescriptor> DirectoryDescriptors => Model.DirectoryDescriptors;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XDVDFS(Volume model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public XDVDFS(Volume model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XDVDFS(Volume model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public XDVDFS(Volume model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public XDVDFS(Volume model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XDVDFS(Volume model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an XDVDFS Volume from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the XDVDFS Volume</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An XDVDFS Volume wrapper on success, null on failure</returns>
        public static XDVDFS? Create(byte[]? data, int offset)
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
        /// Create an XDVDFS Volume from a Stream
        /// </summary>
        /// <param name="data">Stream representing the XDVDFS Volume</param>
        /// <returns>An XDVDFS Volume wrapper on success, null on failure</returns>
        public static XDVDFS? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Serialization.Readers.XDVDFS().Deserialize(data);
                if (model is null)
                    return null;

                return new XDVDFS(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
