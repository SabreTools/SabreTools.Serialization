using System.Collections.Generic;
using SabreTools.Data.Models.NES;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    /// <see href="https://gist.github.com/einstein95/6545066905680466cdf200c4cc8ca4f0"/>
    public partial class FamicomDiskSystem : ICrossModel<FDS, QD>
    {
        /// <inheritdoc/>
        public bool Debug { get; set; } = false;

        /// <inheritdoc/>
        public FDS? Deserialize(QD? obj)
        {
            // Ignore invalid data
            if (obj?.Data is null || obj.Data.Length == 0)
                return null;

            // Ignore incomplete data
            if ((obj.Data.Length % 65536) != 0)
                return null;

            // Determine the number of sides
            byte sides = (byte)(obj.Data.Length / 65536);
            if (sides < 1)
                return null;

            // Create the FDS for output
            var fds = new FDS
            {
                Header = new FDSHeader
                {
                    IdentificationString = Constants.FDSSignatureBytes,
                    DiskSides = sides,
                }
            };

            // Loop over the sides and convert
            List<byte> converted = [];
            for (int i = 0; i < sides; i++)
            {
                // Convert the data to manipulate
                List<byte> data = [.. obj.Data];

                // Delete block 01 checksum
                data.RemoveRange(0x38, 2);

                // Delete block 02 checksum
                int pos = 0x3A;
                data.RemoveRange(pos, 2);

                // Loop while there are more files
                while (data[pos] == 3)
                {
                    // Get the filesize
                    ushort filesize = (ushort)((data[pos + 0x0E] << 8) | data[pos + 0x0D]);

                    // Delete block 03 checksum
                    data.RemoveRange(pos + 0x10, 2);

                    // Skip the file data
                    pos = pos + 0x10 + 1 + filesize;

                    // Delete block 04 checksum
                    data.RemoveRange(pos, 2);
                }

                // Ensure the data is correctly sized
                if (data.Count > 65500)
                    data = data.GetRange(0, 65500);
                else if (data.Count < 65500)
                    data.AddRange(new byte[65500 - data.Count]);

                // Add the data to the output
                converted.AddRange(data);
            }

            // Assign the converted data and return
            fds.Data = [.. converted];
            return fds;
        }
    }
}
