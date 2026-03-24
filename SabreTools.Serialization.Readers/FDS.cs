using System.IO;
using SabreTools.Data.Models.NES;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;
using static SabreTools.Data.Models.NES.Constants;

namespace SabreTools.Serialization.Readers
{
    public class FDS : BaseBinaryReader<Data.Models.NES.FDS>
    {
        /// <inheritdoc/>
        public override Data.Models.NES.FDS? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new FDS file to fill
                var fds = new Data.Models.NES.FDS();

                // If the size indicates no header
                if ((data.Length - data.Position) > 0 && (data.Length - data.Position) % 65500 == 0)
                {
                    fds.Data = data.ReadBytes((int)(data.Length - data.Position));
                }

                // Otherwise, assume a header is present
                else
                {
                    // Try to parse the header
                    var header = ParseHeader(data);
                    if (header is null)
                        return null;

                    // Set the header
                    fds.Header = header;

                    // Read the disk data
                    if (fds.Header.DiskSides > 0)
                        fds.Data = data.ReadBytes(65500 * fds.Header.DiskSides);
                }

                return fds;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a FDSHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FDSHeader on success, null on error</returns>
        public static FDSHeader? ParseHeader(Stream data)
        {
            var obj = new FDSHeader();

            obj.IdentificationString = data.ReadBytes(4);
            if (!obj.IdentificationString.EqualsExactly(FDSSignatureBytes))
                return null;

            obj.DiskSides = data.ReadByteValue();
            obj.Padding = data.ReadBytes(11);

            return obj;
        }
    }
}
