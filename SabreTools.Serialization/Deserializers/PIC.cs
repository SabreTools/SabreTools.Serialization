using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.PIC;
using static SabreTools.Models.PIC.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class PIC : BaseBinaryDeserializer<DiscInformation>
    {
        #region IStreamDeserializer

        /// <inheritdoc/>
        public override DiscInformation? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var di = new DiscInformation();

                // Read the initial disc information
                di.DataStructureLength = data.ReadUInt16BigEndian();
                if (di.DataStructureLength > data.Length)
                    return null;

                di.Reserved0 = data.ReadByteValue();
                di.Reserved1 = data.ReadByteValue();

                // Create a list for the units
                var diUnits = new List<DiscInformationUnit>();

                // Loop and read all available units
                for (int i = 0; i < 32; i++)
                {
                    var unit = ParseDiscInformationUnit(data);
                    if (unit == null)
                        continue;

                    diUnits.Add(unit);
                }

                // Assign the units and return
                di.Units = [.. diUnits];
                return di;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a disc information unit
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled disc information unit on success, null on error</returns>
        private static DiscInformationUnit? ParseDiscInformationUnit(Stream data)
        {
            var unit = new DiscInformationUnit();

            #region Header

            // We only accept Disc Information units, not Emergency Brake or other
            var header = data.ReadType<DiscInformationUnitHeader>();
            if (header?.DiscInformationIdentifier != "DI")
                return null;

            // Set the information unit header
            unit.Header = header;

            #endregion

            #region Body

            // Try to parse the body
            var body = ParseDiscInformationUnitBody(data);
            if (body == null)
                return null;

            // Set the information unit body
            unit.Body = body;

            #endregion

            #region Trailer

            if (unit.Body.DiscTypeIdentifier == DiscTypeIdentifierReWritable || unit.Body.DiscTypeIdentifier == DiscTypeIdentifierRecordable)
            {
                // Try to parse the trailer
                var trailer = data.ReadType<DiscInformationUnitTrailer>();
                if (trailer == null)
                    return null;

                // Set the information unit trailer
                unit.Trailer = trailer;
            }

            #endregion

            return unit;
        }

        /// <summary>
        /// Parse a Stream into a disc information unit body
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled disc information unit body on success, null on error</returns>
        private static DiscInformationUnitBody? ParseDiscInformationUnitBody(Stream data)
        {
            var body = new DiscInformationUnitBody();

            byte[] dti = data.ReadBytes(3);
            body.DiscTypeIdentifier = Encoding.ASCII.GetString(dti);
            body.DiscSizeClassVersion = data.ReadByteValue();
            switch (body.DiscTypeIdentifier)
            {
                case DiscTypeIdentifierROM:
                case DiscTypeIdentifierROMUltra:
                case DiscTypeIdentifierXGD4:
                    body.FormatDependentContents = data.ReadBytes(52);
                    break;
                case DiscTypeIdentifierReWritable:
                case DiscTypeIdentifierRecordable:
                    body.FormatDependentContents = data.ReadBytes(100);
                    break;
            }

            return body;
        }

        #endregion
    }
}