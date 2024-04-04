using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO;
using SabreTools.Models.PIC;
using SabreTools.Serialization.Interfaces;
using static SabreTools.Models.PIC.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class PIC :
        BaseBinaryDeserializer<DiscInformation>,
        IByteDeserializer<DiscInformation>,
        IFileDeserializer<DiscInformation>,
        IStreamDeserializer<DiscInformation>
    {
        #region IByteDeserializer

        /// <inheritdoc/>
        public DiscInformation? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc/>
        public DiscInformation? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return DeserializeStream(stream);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc/>
        public DiscInformation? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            var di = new DiscInformation();

            // Read the initial disc information
            di.DataStructureLength = data.ReadUInt16BigEndian();
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
            di.Units = diUnits.ToArray();
            return di;
        }

        /// <summary>
        /// Parse a Stream into a disc information unit
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled disc information unit on success, null on error</returns>
        private static DiscInformationUnit? ParseDiscInformationUnit(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var unit = new DiscInformationUnit();

            #region Header

            // Try to parse the header
            var header = ParseDiscInformationUnitHeader(data);
            if (header == null)
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
                var trailer = ParseDiscInformationUnitTrailer(data);
                if (trailer == null)
                    return null;

                // Set the information unit trailer
                unit.Trailer = trailer;
            }

            #endregion

            return unit;
        }

        /// <summary>
        /// Parse a Stream into a disc information unit header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled disc information unit header on success, null on error</returns>
        private static DiscInformationUnitHeader? ParseDiscInformationUnitHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var header = new DiscInformationUnitHeader();

            // We only accept Disc Information units, not Emergency Brake or other
            byte[]? dic = data.ReadBytes(2);
            if (dic == null)
                return null;

            header.DiscInformationIdentifier = Encoding.ASCII.GetString(dic);
            if (header.DiscInformationIdentifier != "DI")
                return null;

            header.DiscInformationFormat = data.ReadByteValue();
            header.NumberOfUnitsInBlock = data.ReadByteValue();
            header.Reserved0 = data.ReadByteValue();
            header.SequenceNumber = data.ReadByteValue();
            header.BytesInUse = data.ReadByteValue();
            header.Reserved1 = data.ReadByteValue();

            return header;
        }

        /// <summary>
        /// Parse a Stream into a disc information unit body
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled disc information unit body on success, null on error</returns>
        private static DiscInformationUnitBody? ParseDiscInformationUnitBody(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var body = new DiscInformationUnitBody();

            byte[]? dti = data.ReadBytes(3);
            if (dti == null)
                return null;

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

        /// <summary>
        /// Parse a Stream into a disc information unit trailer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled disc information unit trailer on success, null on error</returns>
        private static DiscInformationUnitTrailer ParseDiscInformationUnitTrailer(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var trailer = new DiscInformationUnitTrailer();

            trailer.DiscManufacturerID = data.ReadBytes(6);
            trailer.MediaTypeID = data.ReadBytes(3);
            trailer.TimeStamp = data.ReadUInt16();
            trailer.ProductRevisionNumber = data.ReadByteValue();

            return trailer;
        }

        #endregion
    }
}