using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Models.PlayStation3;
using static SabreTools.Serialization.Models.PlayStation3.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class SFO : BaseBinaryDeserializer<SabreTools.Serialization.Models.PlayStation3.SFO>
    {
        /// <inheritdoc/>
        public override SabreTools.Serialization.Models.PlayStation3.SFO? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new SFO to fill
                var sfo = new SabreTools.Serialization.Models.PlayStation3.SFO();

                #region Header

                // Try to parse the header
                var header = ParseSFOHeader(data);
                if (header.Magic != SFOMagic)
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
        /// Parse a Stream into an SFOHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SFOHeader on success, null on error</returns>
        public static SFOHeader ParseSFOHeader(Stream data)
        {
            var obj = new SFOHeader();

            obj.Magic = data.ReadUInt32BigEndian();
            obj.Version = data.ReadUInt32LittleEndian();
            obj.KeyTableStart = data.ReadUInt32LittleEndian();
            obj.DataTableStart = data.ReadUInt32LittleEndian();
            obj.TablesEntries = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an SFOIndexTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SFOIndexTableEntry on success, null on error</returns>
        public static SFOIndexTableEntry ParseIndexTableEntry(Stream data)
        {
            var obj = new SFOIndexTableEntry();

            obj.KeyOffset = data.ReadUInt16LittleEndian();
            obj.DataFormat = (DataFormat)data.ReadUInt16LittleEndian();
            obj.DataLength = data.ReadUInt32LittleEndian();
            obj.DataMaxLength = data.ReadUInt32LittleEndian();
            obj.DataOffset = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
