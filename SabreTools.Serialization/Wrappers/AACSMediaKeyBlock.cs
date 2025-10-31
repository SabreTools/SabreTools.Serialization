using System;
using System.IO;
using SabreTools.Data.Models.AACS;

namespace SabreTools.Serialization.Wrappers
{
    public partial class AACSMediaKeyBlock : WrapperBase<MediaKeyBlock>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "AACS Media Key Block";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Media key block records
        /// </summary>
        public Record[] Records => Model.Records;

        /// <summary>
        /// Reported version of the media key block
        /// </summary>
        public string? Version
        {
            get
            {
                var record = Array.Find(Records, r => r.RecordType == RecordType.TypeAndVersion);
                if (record is TypeAndVersionRecord tavr)
                    return tavr.VersionNumber.ToString();

                return null;
            }
        }

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public AACSMediaKeyBlock(MediaKeyBlock model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public AACSMediaKeyBlock(MediaKeyBlock model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public AACSMediaKeyBlock(MediaKeyBlock model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public AACSMediaKeyBlock(MediaKeyBlock model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public AACSMediaKeyBlock(MediaKeyBlock model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public AACSMediaKeyBlock(MediaKeyBlock model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an AACS media key block from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An AACS media key block wrapper on success, null on failure</returns>
        public static AACSMediaKeyBlock? Create(byte[]? data, int offset)
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
        /// Create an AACS media key block from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An AACS media key block wrapper on success, null on failure</returns>
        public static AACSMediaKeyBlock? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.AACS().Deserialize(data);
                if (model == null)
                    return null;

                return new AACSMediaKeyBlock(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
