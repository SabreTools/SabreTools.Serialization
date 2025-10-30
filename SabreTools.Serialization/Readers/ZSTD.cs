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
            
            obj.VersionByte = data.ReadByte(); // ReadByte returning int is actually preferable here
            obj.Magic = data.ReadBytes(3);
            if (!obj.Magic.EqualsExactly(SignatureBytes))
                return null;

            // Valid versions are 0x1E and 0x22-0x28.
            // According to https://datatracker.ietf.org/doc/html/rfc8878#section-7.1-2.22.2.4, the current version is
            // still 0x28, and it should stay that way now that it's a stable format.
            if (obj.VersionByte is < 0x22 or > 0x28 && obj.VersionByte != 0x1E)
                return null;
 
            return obj;
        }
    }

    
}