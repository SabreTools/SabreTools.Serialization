using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public abstract class WrapperBase<TModel> : WrapperBase, IWrapper<TModel>
    {
        #region Properties

        /// <inheritdoc/>
        public TModel GetModel() => Model;

        /// <summary>
        /// Internal model
        /// </summary>
        public TModel Model { get; }

        #endregion

        #region Byte Array Constructors

        /// <summary>
        /// Construct a new instance of the wrapper from a byte array
        /// </summary>
        /// <param name="model">Model to be used in the wrapper</param>
        /// <param name="data">Underlying data for the wrapper</param>
        protected WrapperBase(TModel model, byte[] data)
            : this(model, data, 0, data.Length)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a byte array
        /// </summary>
        /// <param name="model">Model to be used in the wrapper</param>
        /// <param name="data">Underlying data for the wrapper</param>
        /// <param name="offset">Offset into the data to use as the window start</param>
        protected WrapperBase(TModel model, byte[] data, int offset)
            : this(model, data, offset, data.Length - offset)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a byte array
        /// </summary>
        /// <param name="model">Model to be used in the wrapper</param>
        /// <param name="data">Underlying data for the wrapper</param>
        /// <param name="offset">Offset into the data to use as the window start</param>
        /// <param name="length">Length of the window into the data</param>
        protected WrapperBase(TModel model, byte[] data, int offset, int length)
            : base(data, offset, length)
        {
            Model = model;
        }

        #endregion

        #region Stream Constructors

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        /// <param name="model">Model to be used in the wrapper</param>
        /// <param name="data">Underlying data for the wrapper</param>
        /// <remarks>Uses the current stream position as the offset</remarks>
        protected WrapperBase(TModel model, Stream data)
            : this(model, data, data.Position, data.Length - data.Position)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        /// <param name="model">Model to be used in the wrapper</param>
        /// <param name="data">Underlying data for the wrapper</param>
        /// <param name="offset">Offset into the data to use as the window start</param>
        protected WrapperBase(TModel model, Stream data, long offset)
            : this(model, data, offset, data.Length - offset)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        /// <param name="model">Model to be used in the wrapper</param>
        /// <param name="data">Underlying data for the wrapper</param>
        /// <param name="offset">Offset into the data to use as the window start</param>
        /// <param name="length">Length of the window into the data</param>
        protected WrapperBase(TModel model, Stream data, long offset, long length)
            : base(data, offset, length)
        {
            Model = model;
        }

        #endregion
    }
}
