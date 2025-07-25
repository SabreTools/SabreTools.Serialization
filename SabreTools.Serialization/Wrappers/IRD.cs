using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class IRD : WrapperBase<Models.IRD.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PS3 IRD file";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public IRD(Models.IRD.File? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public IRD(Models.IRD.File? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an IRD from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An IRD wrapper on success, null on failure</returns>
        public static IRD? Create(byte[]? data, int offset)
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
        /// Create an IRD from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An IRD wrapper on success, null on failure</returns>
        public static IRD? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var file = Deserializers.IRD.DeserializeStream(data);
                if (file == null)
                    return null;

                return new IRD(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
