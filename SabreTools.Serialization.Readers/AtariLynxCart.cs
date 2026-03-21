using System.IO;
using SabreTools.Data.Models.AtariLynx;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.AtariLynx.Constants;

namespace SabreTools.Serialization.Readers
{
    public class AtariLynxCart : BaseBinaryReader<Cart>
    {
        /// <inheritdoc/>
        public override Cart? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new A78 cart image to fill
                var cart = new Cart();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (!header.Magic.EqualsExactly(MagicBytes))
                    return null;
                else if (header.Version != 0x0001)
                    return null;

                // Set the header
                cart.Header = header;

                #endregion

                // Read the cart data
                cart.Data = data.ReadBytes((int)(data.Length - data.Position));

                return cart;
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

            obj.Magic = data.ReadBytes(4);
            obj.Bank0PageSize = data.ReadUInt16LittleEndian();
            obj.Bank1PageSize = data.ReadUInt16LittleEndian();
            obj.Version = data.ReadUInt16LittleEndian();
            obj.CartName = data.ReadBytes(32);
            obj.Manufacturer = data.ReadBytes(16);
            obj.Rotation = (Rotation)data.ReadByteValue();
            obj.Spare = data.ReadBytes(5);

            return obj;
        }
    }
}
