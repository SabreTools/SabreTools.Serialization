using System.IO;
using SabreTools.Serialization.Models.BDPlus;

namespace SabreTools.Serialization.Wrappers
{
    public class BDPlusSVM : WrapperBase<SVM>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "BD+ SVM";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Human-readable version of the date
        /// </summary>
        public string Date => $"{Model.Year:0000}-{Model.Month:00}-{Model.Day:00}";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public BDPlusSVM(SVM model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public BDPlusSVM(SVM model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public BDPlusSVM(SVM model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public BDPlusSVM(SVM model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public BDPlusSVM(SVM model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public BDPlusSVM(SVM model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a BD+ SVM from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BD+ SVM wrapper on success, null on failure</returns>
        public static BDPlusSVM? Create(byte[]? data, int offset)
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
        /// Create a BD+ SVM from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A BD+ SVM wrapper on success, null on failure</returns>
        public static BDPlusSVM? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.BDPlus().Deserialize(data);
                if (model == null)
                    return null;

                return new BDPlusSVM(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
