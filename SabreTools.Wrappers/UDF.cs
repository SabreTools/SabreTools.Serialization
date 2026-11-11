using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.UDF;
using SabreTools.IO;

namespace SabreTools.Wrappers
{
    public partial class UDF : WrapperBase<Volume>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Universal Disk Format (UDF) Volume";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Volume.VolumeRecognitionSequence"/>
        public VolumeStructureDescriptor[] VolumeRecognitionSequence => Model.VolumeRecognitionSequence;

        /// <inheritdoc cref="Volume.VolumeDescriptorSequence"/>
        public Descriptor[] VolumeDescriptorSequence => Model.VolumeDescriptorSequence;

        /// <inheritdoc cref="Volume.FileSetDescriptor"/>
        public FileSetDescriptor? FileSetDescriptor => Model.FileSetDescriptor;

        /// <inheritdoc cref="Volume.FileEntries"/>
        public Dictionary<long, FileEntry> FileEntries => Model.FileEntries;

        /// <inheritdoc cref="Volume.Directories"/>
        public Dictionary<long, FileIdentifierDescriptor[]> Directories => Model.Directories;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public UDF(Volume model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public UDF(Volume model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public UDF(Volume model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public UDF(Volume model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public UDF(Volume model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public UDF(Volume model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a UDF Volume from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the UDF Volume</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A UDF Volume wrapper on success, null on failure</returns>
        public static UDF? Create(byte[]? data, int offset)
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
        /// Create a UDF Volume from a Stream
        /// </summary>
        /// <param name="data">Stream representing the UDF Volume</param>
        /// <returns>A UDF Volume wrapper on success, null on failure</returns>
        public static UDF? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead || !data.CanSeek)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                // The reader addresses logical sectors from the start of the stream, so it reads a view that starts here
                var model = new Serialization.Readers.UDF().Deserialize(new ReadOnlyViewStream(data, currentOffset));
                if (model is null)
                    return null;

                return new UDF(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
