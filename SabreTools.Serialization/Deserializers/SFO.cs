using System.IO;
using System.Text;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Deserializers
{
    public class SFO : BaseBinaryDeserializer<Models.PlayStation3.SFO>
    {
        /// <inheritdoc/>
        public override Models.PlayStation3.SFO? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new SFO to fill
            var sfo = new Models.PlayStation3.SFO();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
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

        /// <summary>
        /// Parse a Stream into an SFO header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SFO header on success, null on error</returns>
        public Models.PlayStation3.SFOHeader? ParseHeader(Stream data)
        {
            var sfoHeader = data.ReadType<Models.PlayStation3.SFOHeader>();

            if (sfoHeader == null)
                return null;

            string magic = Encoding.ASCII.GetString(sfoHeader.Magic);
            if (magic != "\0PSF")
                return null;

            return sfoHeader;
        }
    
        /// <summary>
        /// Parse a Stream into an SFO index table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SFO index table entry on success, null on error</returns>
        public Models.PlayStation3.SFOIndexTableEntry? ParseIndexTableEntry(Stream data)
        {
            return data.ReadType<Models.PlayStation3.SFOIndexTableEntry>();
        }
    }
}