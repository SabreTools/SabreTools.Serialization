using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public abstract class WrapperBase<T> : WrapperBase, IWrapper<T>
    {
        #region Properties

        /// <inheritdoc/>
        public T GetModel() => Model;

        /// <summary>
        /// Internal model
        /// </summary>
        public T Model { get; }

        #endregion

        #region Byte Array Constructors

        /// <summary>
        /// Construct a new instance of the wrapper from a byte array
        /// </summary>
        protected WrapperBase(T model, byte[] data)
            : this(model, data, 0, data.Length)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a byte array
        /// </summary>
        protected WrapperBase(T model, byte[] data, int offset)
            : this(model, data, offset, data.Length - offset)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a byte array
        /// </summary>
        protected WrapperBase(T model, byte[] data, int offset, int length)
            : base(data, offset, length)
        {
            Model = model;
        }

        #endregion

        #region Stream Constructors

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        /// <remarks>Uses the current stream position as the offset</remarks>
        protected WrapperBase(T model, Stream data)
            : this(model, data, data.Position, data.Length - data.Position)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        protected WrapperBase(T model, Stream data, long offset)
            : this(model, data, offset, data.Length - offset)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        protected WrapperBase(T model, Stream data, long offset, long length)
            : base(data, offset, length)
        {
            Model = model;
        }

        #endregion

        #region JSON Export

#if NETCOREAPP
        /// <inheritdoc/>
        public override string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        #endregion
    }
}
