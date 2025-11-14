using System.IO;
using SabreTools.Data.Models.ZSTD;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.ZSTD.Constants;

namespace SabreTools.Serialization.Readers
{
    public class ZSTD : BaseBinaryReader<Header>
    {
        /// <inheritdoc/>
        public override Header? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                #region Header

                var header = ParseHeader(data);
                if (header == null)
                    return null;

                // Valid versions are 0x1E and 0x22-0x28.
                // According to RFC-8878, the current version is still 0x28, and it should stay that way now that
                // it's a stable format.
                if ((header.VersionByte < 0x22 || header.VersionByte > 0x28) && header.VersionByte != 0x1E)
                    return null;

                #endregion

                return header;
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

            obj.VersionByte = data.ReadByteValue();
            obj.Magic = data.ReadBytes(3);
            if (!obj.Magic.EqualsExactly(SignatureBytes))
                return null;

            return obj;
        }
    }
}
