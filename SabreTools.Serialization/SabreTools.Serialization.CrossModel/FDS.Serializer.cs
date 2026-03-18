using System.Collections.Generic;
using SabreTools.Data.Models.NES;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    /// <see href="https://www.nesdev.org/wiki/FDS_disk_format"/>
    /// <see href="https://gist.github.com/einstein95/6545066905680466cdf200c4cc8ca4f0"/>
    public partial class FamicomDiskSystem : ICrossModel<FDS, QD>
    {
        /// <inheritdoc/>
        public QD? Serialize(FDS? obj)
            => Serialize(obj, false);

        /// <inheritdoc/>
        /// <param name="nul">True to insert null checksums on conversion, false otherwise</param>
        public QD? Serialize(FDS? obj, bool nul)
        {
            // Ignore invalid data
            if (obj?.Data is null || obj.Data.Length == 0)
                return null;

            // Ignore incomplete data
            if ((obj.Data.Length % 65500) != 0)
                return null;

            // Determine the number of sides
            byte sides = obj.Header != null
                ? obj.Header.DiskSides
                : (byte)(obj.Data.Length / 65500);
            if (sides < 1)
                return null;

            // Create the QD for output
            var qd = new QD();

            // Loop over the sides and convert
            List<byte> converted = [];
            for (int i = 0; i < sides; i++)
            {
                // Convert the data to manipulate
                List<byte> data = [.. obj.Data];

                // If the data is unexpected
                if (data[0] != 0x01)
                    break;

                // Insert block 01 checksum
                InsertCrc(data, 0, 0x38, nul);

                // Insert block 02 checksum
                int pos = 0x3A;
                InsertCrc(data, pos, pos + 2, nul);

                // Loop while there are more files
                pos = 0x3E;
                while (data[pos] == 3)
                {
                    // Get the filesize
                    ushort filesize = (ushort)((data[pos + 0x0E] << 8) | data[pos + 0x0D]);

                    // Insert block 03 checksum
                    InsertCrc(data, pos, pos + 0x10, nul);

                    // Skip the file data
                    pos += 0x10 + 2;

                    // Insert block 04 checksum
                    InsertCrc(data, pos, pos + 1 + filesize, nul);
                    pos += 1 + filesize + 2;
                }

                // Ensure the data is correctly sized
                if (data.Count > 65536)
                    data = data.GetRange(0, 65536);
                else if (data.Count < 65536)
                    data.AddRange(new byte[65536 - data.Count]);

                // Add the data to the output
                converted.AddRange(data);
            }

            // Assign the converted data and return
            qd.Data = [.. converted];
            return qd;
        }

        private void InsertCrc(List<byte> data, int start, int end, bool nul)
        {
            if (!nul)
            {
                byte[] temp = new byte[end - start];
                data.CopyTo(start, temp, 0, temp.Length);
                ushort crc = FdsCrc(temp);
                data.Insert(end + 0, (byte)(crc & 0xFF));
                data.Insert(end + 1, (byte)(crc >> 0x08));
            }
            else
            {
                data.Insert(end + 0, 0);
                data.Insert(end + 1, 0);
            }
        }

        // TODO: Replace with CRC-16/KERMIT from Hashing
        private ushort FdsCrc(byte[] data)
        {
            // http://forums.nesdev.com/viewtopic.php?p=194867
            // Do not include any existing checksum, not even the blank checksums 00 00 or FF FF.
            // The formula will automatically count 2 0x00 bytes without the programmer adding them manually.
            // Also, do not include the gap terminator (0x80) in the data.
            // If you wish to do so, change sum to 0x0000.
            ushort s = 0x8000;
            byte[] padded = [.. data, 0x00, 0x00];
            foreach (byte b in padded)
            {
                s |= (ushort)(b << 16);
                for (int i = 0; i < 8; i++)
                {
                    if ((s & 1) != 0)
                    {
                        unchecked { s ^= (ushort)(0x8408 << 1); }
                    }

                    s >>= 1;
                }
            }

            return s;
        }
    }
}
