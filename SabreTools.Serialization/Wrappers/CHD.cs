using System.IO;
using SabreTools.Data.Models.CHD;

namespace SabreTools.Serialization.Wrappers
{
    public class CHD : WrapperBase<Header>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "MAME Compressed Hunks of Data";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Internal MD5 hash, if available
        /// </summary>
        public byte[]? MD5
        {
            get
            {
                return Model switch
                {
                    HeaderV1 v1 => v1.MD5,
                    HeaderV2 v2 => v2.MD5,
                    HeaderV3 v3 => v3.MD5,
                    HeaderV4 v4 => null,
                    HeaderV5 v5 => null,
                    _ => null,
                };
            }
        }

        /// <summary>
        /// Internal SHA1 hash, if available
        /// </summary>
        public byte[]? SHA1
        {
            get
            {
                return Model switch
                {
                    HeaderV1 v1 => null,
                    HeaderV2 v2 => null,
                    HeaderV3 v3 => v3.SHA1,
                    HeaderV4 v4 => v4.SHA1,
                    HeaderV5 v5 => v5.SHA1,
                    _ => null,
                };
            }
        }

        /// <inheritdoc cref="Header.Version"/>
        public uint Version => Model.Version;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public CHD(Header model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public CHD(Header model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public CHD(Header model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public CHD(Header model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public CHD(Header model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public CHD(Header model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a CHD header from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A CHD header wrapper on success, null on failure</returns>
        public static CHD? Create(byte[]? data, int offset)
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
        /// Create a CHD header from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A CHD header on success, null on failure</returns>
        public static CHD? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.CHD().Deserialize(data);
                if (model == null)
                    return null;

                return new CHD(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
