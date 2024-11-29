using System.IO;
using SabreTools.Models.N3DS;

namespace SabreTools.Serialization.Wrappers
{
    public class CIA : WrapperBase<Models.N3DS.CIA>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "CTR Importable Archive (CIA)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public CIA(Models.N3DS.CIA? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public CIA(Models.N3DS.CIA? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a CIA archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A CIA archive wrapper on success, null on failure</returns>
        public static CIA? Create(byte[]? data, int offset)
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
        /// Create a CIA archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A CIA archive wrapper on success, null on failure</returns>
        public static CIA? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var cia = Deserializers.CIA.DeserializeStream(data);
                if (cia == null)
                    return null;

                return new CIA(cia, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        // TODO: Hook these up for external use
        #region Currently Unused Extensions

        #region Ticket

        /// <summary>
        /// Denotes if the ticket denotes a demo or not
        /// </summary>
        public static bool IsDemo(Ticket? ticket)
        {
            if (ticket?.Limits == null || ticket.Limits.Length == 0)
                return false;

            return ticket.Limits[0] == 0x0004;
        }

        /// <summary>
        /// Denotes if the max playcount for a demo
        /// </summary>
        public static uint PlayCount(Ticket ticket)
        {
            if (ticket?.Limits == null || ticket.Limits.Length == 0)
                return 0;

            return ticket.Limits[1];
        }

        #endregion

        #endregion
    }
}