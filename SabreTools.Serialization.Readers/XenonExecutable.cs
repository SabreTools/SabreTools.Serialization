using System.IO;
using SabreTools.Data.Models.XenonExecutable;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;
using static SabreTools.Data.Models.XenonExecutable.Constants;

namespace SabreTools.Serialization.Readers
{
    public class XenonExecutable : BaseBinaryReader<Executable>
    {
        /// <inheritdoc/>
        public override Executable? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new executable to fill
                var xex = new Executable();

                #region ParseHeader

                // Parse the file header
                var header = ParseHeader(data);
                if (!header.MagicNumber.EqualsExactly(MagicBytes))
                    return null;

                // Set the file header
                xex.Header = header;

                #endregion

                return xex;
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
        public static Header ParseHeader(Stream data)
        {
            var obj = new Header();

            obj.MagicNumber = data.ReadBytes(4);
            obj.ModuleFlags = data.ReadUInt32BigEndian();
            obj.PEDataOffset = data.ReadUInt32BigEndian();
            obj.Reserved = data.ReadUInt32BigEndian();
            obj.SecurityInfoOffset = data.ReadUInt32BigEndian();
            obj.OptionalHeaderCount = data.ReadUInt32BigEndian();

            var optionalHeaders = new OptionalHeader[obj.OptionalHeaderCount];
            for (int i = 0; i < obj.OptionalHeaderCount; i++)
            {
                optionalHeaders[i].HeaderID = data.ReadUInt32BigEndian();
                optionalHeaders[i].HeaderData = data.ReadUInt32BigEndian();
            }

            return obj;
        }
    }
}
