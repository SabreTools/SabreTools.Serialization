using System.Collections.Generic;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Models.GZIP;
using static SabreTools.Models.GZIP.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class GZip : BaseBinaryDeserializer<Archive>
    {
        /// <inheritdoc/>
        protected override bool SkipCompression => true;

        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                var archive = new Archive();

                #region Header

                var header = ParseHeader(data);
                if (header == null)
                    return null;

                archive.Header = header;

                #endregion

                // Seek to the end to read the trailer
                data.Seek(-8, SeekOrigin.End);

                #region Trailer

                var trailer = ParseTrailer(data);
                if (header == null)
                    return null;

                archive.Trailer = trailer;

                #endregion

                return archive;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header? ParseHeader(Stream data)
        {
            var obj = new Header();

            obj.ID1 = data.ReadByteValue();
            obj.ID2 = data.ReadByteValue();
            if (obj.ID1 != ID1 || obj.ID2 != ID2)
                return null;

            obj.CM = (CompressionMethod)data.ReadByteValue();
            obj.FLG = (Flags)data.ReadByteValue();
            obj.MTIME = data.ReadUInt32LittleEndian();
            obj.XFL = (ExtraFlags)data.ReadByteValue();
            obj.OS = (OperatingSystem)data.ReadByteValue();

#if NET20 || NET35
            if ((obj.FLG & Flags.FEXTRA) != 0)
#else
            if (obj.FLG.HasFlag(Flags.FEXTRA))
#endif
            {
                obj.XLEN = data.ReadUInt16LittleEndian();

                // Cache the current position
                long currentPosition = data.Position;

                List<ExtraFieldData> extraFields = [];
                while (data.Position < currentPosition + obj.XLEN)
                {
                    var extraField = ParseExtraFieldData(data);
                    if (extraField == null)
                        break;

                    extraFields.Add(extraField);
                }

                obj.ExtraField = [.. extraFields];
            }

#if NET20 || NET35
            if ((obj.FLG & Flags.FNAME) != 0)
#else
            if (obj.FLG.HasFlag(Flags.FNAME))
#endif
                obj.OriginalFileName = data.ReadNullTerminatedAnsiString();

#if NET20 || NET35
            if ((obj.FLG & Flags.FCOMMENT) != 0)
#else
            if (obj.FLG.HasFlag(Flags.FCOMMENT))
#endif
                obj.FileComment = data.ReadNullTerminatedAnsiString();


#if NET20 || NET35
            if ((obj.FLG & Flags.FHCRC) != 0)
#else
            if (obj.FLG.HasFlag(Flags.FHCRC))
#endif
                obj.CRC16 = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ExtraFieldData
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExtraFieldData on success, null on error</returns>
        public static ExtraFieldData? ParseExtraFieldData(Stream data)
        {
            var obj = new ExtraFieldData();

            obj.SI1 = data.ReadByteValue();
            obj.SI2 = data.ReadByteValue();
            obj.LEN = data.ReadUInt16LittleEndian();
            obj.Data = data.ReadBytes(obj.LEN);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Trailer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Trailer on success, null on error</returns>
        public static Trailer? ParseTrailer(Stream data)
        {
            var obj = new Trailer();

            obj.CRC32 = data.ReadUInt32LittleEndian();
            obj.ISIZE = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
