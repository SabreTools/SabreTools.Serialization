using System.IO;
using SabreTools.IO.Extensions;
using static SabreTools.Models.PlayStation3.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class SFO : BaseBinaryDeserializer<Models.PlayStation3.SFO>
    {
        /// <inheritdoc/>
        public override Models.PlayStation3.SFO? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new SFO to fill
                var sfo = new Models.PlayStation3.SFO();

                #region Header

                // Try to parse the header
                var header = data.ReadType<Models.PlayStation3.SFOHeader>();
                if (header?.Magic != SFOMagic)
                    return null;

                // Assign the header
                sfo.Header = header;

                #endregion

                #region Index Table

                // TODO: Determine how many entries are in the index table

                #endregion

                #region Key Table

                // TODO: Finish implementation

                #endregion

                // Padding
                // TODO: Finish implementation

                #region Data Table

                // TODO: Finish implementation

                #endregion

                return sfo;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an SFO index table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SFO index table entry on success, null on error</returns>
        public static Models.PlayStation3.SFOIndexTableEntry? ParseIndexTableEntry(Stream data)
        {
            return data.ReadType<Models.PlayStation3.SFOIndexTableEntry>();
        }
    }
}