using System.IO;
using SabreTools.Models.PKZIP;

namespace SabreTools.Serialization.Wrappers
{
    public class PKZIP : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PKZIP Archive (or Derived Format)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PKZIP(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public PKZIP(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a PKZIP archive (or derived format) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PKZIP wrapper on success, null on failure</returns>
        public static PKZIP? Create(byte[]? data, int offset)
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
        /// Create a PKZIP archive (or derived format) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A PKZIP wrapper on success, null on failure</returns>
        public static PKZIP? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var archive = Deserializers.PKZIP.DeserializeStream(data);
                if (archive == null)
                    return null;

                return new PKZIP(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}