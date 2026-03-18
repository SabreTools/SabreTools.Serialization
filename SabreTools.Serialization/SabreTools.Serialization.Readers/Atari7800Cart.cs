using System.IO;
using SabreTools.Data.Models.Atari7800;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.Atari7800.Constants;

namespace SabreTools.Serialization.Readers
{
    public class Atari7800Cart : BaseBinaryReader<Cart>
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
                if (!header.MagicText.EqualsExactly(MagicBytesWithNull)
                    && !header.MagicText.EqualsExactly(MagicBytesWithSpace))
                {
                    return null;
                }
                else if (!header.EndMagicText.EqualsExactly(HeaderEndMagicBytes))
                {
                    return null;
                }

                // Set the header
                cart.Header = header;

                #endregion

                // Read the cart data, if necessary
                if (cart.Header.RomSizeWithoutHeader > 0)
                    cart.Data = data.ReadBytes((int)cart.Header.RomSizeWithoutHeader);

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

            obj.HeaderVersion = data.ReadByteValue();
            obj.MagicText = data.ReadBytes(16);
            obj.CartTitle = data.ReadBytes(32);
            if (obj.HeaderVersion == 1)
                obj.RomSizeWithoutHeader = data.ReadUInt32BigEndian();
            else
                obj.RomSizeWithoutHeader = data.ReadUInt32LittleEndian();

            obj.CartType = (CartType)data.ReadUInt16LittleEndian();
            obj.Controller1Type = (ControllerType)data.ReadByteValue();
            obj.Controller2Type = (ControllerType)data.ReadByteValue();
            obj.TVType = (TVType)data.ReadByteValue();
            obj.SaveDevice = (SaveDevice)data.ReadByteValue();
            obj.Reserved = data.ReadBytes(4);
            obj.SlotPassthroughDevice = (SlotPassthroughDevice)data.ReadByteValue();

            if (obj.HeaderVersion >= 4)
            {
                obj.Mapper = (Mapper)data.ReadByteValue();
                obj.MapperOptions = (MapperOptions)data.ReadByteValue();
                obj.AudioDevice = (AudioDevice)data.ReadUInt16LittleEndian();
                obj.Interrupt = (Interrupt)data.ReadUInt16LittleEndian();
                obj.Padding = data.ReadBytes(30);
            }
            else
            {
                obj.Padding = data.ReadBytes(36);
            }

            obj.EndMagicText = data.ReadBytes(28);

            return obj;
        }
    }
}
