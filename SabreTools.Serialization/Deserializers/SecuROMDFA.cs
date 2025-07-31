using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Models.SecuROM;
using static SabreTools.Models.SecuROM.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class SecuROMDFA : BaseBinaryDeserializer<DFAFile>
    {
        /// <inheritdoc/>
        public override DFAFile? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new file to fill
                var dfa = new DFAFile();

                #region Header

                // Try to parse the header
                dfa.Signature = data.ReadBytes(8);
                if (!dfa.Signature.EqualsExactly(DFAMagicBytes))
                    return null;

                dfa.BlockOrHeaderSize = data.ReadUInt32LittleEndian();

                #endregion

                #region Entries

                // If we have any entries
                List<DFAEntry> entries = [];

                // Read entries while there is data
                while (data.Position < data.Length)
                {
                    var entry = ParseDFAEntry(data);
                    entries.Add(entry);
                }

                // Set the entries list
                dfa.Entries = [.. entries];

                #endregion

                return dfa;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a DFAEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DFAEntry on success, null on error</returns>
        public static DFAEntry ParseDFAEntry(Stream data)
        {
            var obj = new DFAEntry();

            byte[] name = data.ReadBytes(4);
            obj.Name = Encoding.ASCII.GetString(name);
            obj.Length = data.ReadUInt32LittleEndian();
            if (obj.Length > 0)
                obj.Value = data.ReadBytes((int)obj.Length);

            return obj;
        }
    }
}
