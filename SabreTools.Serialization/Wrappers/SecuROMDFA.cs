using System.IO;
using SabreTools.Models.SecuROM;

namespace SabreTools.Serialization.Wrappers
{
    public class SecuROMDFA : WrapperBase<DFAFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "SecuROM DFA File";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public SecuROMDFA(DFAFile? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public SecuROMDFA(DFAFile? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a SecuROM DFA file from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A SecuROM DFA file wrapper on success, null on failure</returns>
        public static SecuROMDFA? Create(byte[]? data, int offset)
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
        /// Create a SecuROM DFA file from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A SecuROM DFA file wrapper on success, null on failure</returns>
        public static SecuROMDFA? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.SecuROMDFA.DeserializeStream(data);
                if (model == null)
                    return null;

                data.Seek(currentOffset, SeekOrigin.Begin);
                return new SecuROMDFA(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
