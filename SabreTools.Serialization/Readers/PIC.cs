using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.PIC;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.PIC.Constants;

namespace SabreTools.Serialization.Readers
{
    public class PIC : BaseBinaryReader<DiscInformation>
    {
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
        /// Parse a Stream into a DiscInformationUnit
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DiscInformationUnit on success, null on error</returns>
        public static DiscInformationUnit ParseDiscInformationUnit(Stream data)
        {
            var obj = new DiscInformationUnit();

            #region Header

            // We only accept Disc Information units, not Emergency Brake or other
            obj.Header = ParseDiscInformationUnitHeader(data);
            if (obj.Header.DiscInformationIdentifier != "DI")
                return obj;

            #endregion

            #region Body

            // Set the information unit body
            obj.Body = ParseDiscInformationUnitBody(data);

            #endregion

            #region Trailer

            // Set the information unit trailer
            if (obj.Body.DiscTypeIdentifier == DiscTypeIdentifierReWritable || obj.Body.DiscTypeIdentifier == DiscTypeIdentifierRecordable)
                obj.Trailer = ParseDiscInformationUnitTrailer(data);

            #endregion

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a disc information unit body
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled disc information unit body on success, null on error</returns>
        private static DiscInformationUnitBody ParseDiscInformationUnitBody(Stream data)
        {
            var obj = new DiscInformationUnitBody();

            byte[] dti = data.ReadBytes(3);
            obj.DiscTypeIdentifier = Encoding.ASCII.GetString(dti);
            obj.DiscSizeClassVersion = data.ReadByteValue();
            switch (obj.DiscTypeIdentifier)
            {
                case DiscTypeIdentifierROM:
                case DiscTypeIdentifierROMUltra:
                case DiscTypeIdentifierXGD4:
                    obj.FormatDependentContents = data.ReadBytes(52);
                    break;
                case DiscTypeIdentifierReWritable:
                case DiscTypeIdentifierRecordable:
                    obj.FormatDependentContents = data.ReadBytes(100);
                    break;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DiscInformationUnitHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>DiscInformationUnitHeader on success, null on error</returns>
        public static DiscInformationUnitHeader ParseDiscInformationUnitHeader(Stream data)
        {
            var obj = new DiscInformationUnitHeader();

            byte[] discInformationIdentifier = data.ReadBytes(2);
            obj.DiscInformationIdentifier = Encoding.ASCII.GetString(discInformationIdentifier);
            if (obj.DiscInformationIdentifier != "DI")
                return obj;

            obj.DiscInformationFormat = data.ReadByteValue();
            obj.NumberOfUnitsInBlock = data.ReadByteValue();
            obj.Reserved0 = data.ReadByteValue();
            obj.SequenceNumber = data.ReadByteValue();
            obj.BytesInUse = data.ReadByteValue();
            obj.Reserved1 = data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DiscInformationUnitTrailer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>DiscInformationUnitTrailer on success, null on error</returns>
        public static DiscInformationUnitTrailer ParseDiscInformationUnitTrailer(Stream data)
        {
            var obj = new DiscInformationUnitTrailer();

            obj.DiscManufacturerID = data.ReadBytes(6);
            obj.MediaTypeID = data.ReadBytes(3);
            obj.TimeStamp = data.ReadUInt16LittleEndian();
            obj.ProductRevisionNumber = data.ReadByteValue();

            return obj;
        }
    }
}
