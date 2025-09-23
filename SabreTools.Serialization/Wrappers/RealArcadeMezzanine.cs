using System.IO;
using SabreTools.Models.RealArcade;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public class RealArcadeMezzanine : WrapperBase<Mezzanine>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "RealArcade Mezzanine";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public RealArcadeMezzanine(Mezzanine model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public RealArcadeMezzanine(Mezzanine model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public RealArcadeMezzanine(Mezzanine model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public RealArcadeMezzanine(Mezzanine model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public RealArcadeMezzanine(Mezzanine model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public RealArcadeMezzanine(Mezzanine model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a RealArcade mezzanine from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A RealArcade mezzanine wrapper on success, null on failure</returns>
        public static RealArcadeMezzanine? Create(byte[]? data, int offset)
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
        /// Create a RealArcade mezzanine from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A RealArcade mezzanine wrapper on success, null on failure</returns>
        public static RealArcadeMezzanine? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            return new RealArcadeMezzanine(new Mezzanine(), data);
        }

        #endregion

        #region JSON Export

#if NETCOREAPP
        /// <inheritdoc/>
        public override string ExportJSON() => throw new System.NotImplementedException();
#endif

        #endregion
    }
}
