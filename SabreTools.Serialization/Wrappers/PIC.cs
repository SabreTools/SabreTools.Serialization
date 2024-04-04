using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    // TODO: Figure out extension properties
    public class PIC : WrapperBase<Models.PIC.DiscInformation>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Disc Information";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PIC(Models.PIC.DiscInformation? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public PIC(Models.PIC.DiscInformation? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }
        
        /// <summary>
         /// Create a PIC disc information object from a byte array and offset
         /// </summary>
         /// <param name="data">Byte array representing the information</param>
         /// <param name="offset">Offset within the array to parse</param>
         /// <returns>A PIC disc information wrapper on success, null on failure</returns>
        public static PIC? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a PIC disc information object from a Stream
        /// </summary>
        /// <param name="data">Stream representing the information</param>
        /// <returns>A PIC disc information wrapper on success, null on failure</returns>
        public static PIC? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var info = Deserializers.PIC.DeserializeStream(data);
            if (info == null)
                return null;

            try
            {
                return new PIC(info, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}