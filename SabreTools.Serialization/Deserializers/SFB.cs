using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using static SabreTools.Serialization.Models.PlayStation3.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class SFB : BaseBinaryDeserializer<Models.PlayStation3.SFB>
    {
        /// <inheritdoc/>
        public override Models.PlayStation3.SFB? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Deserialize the SFB
                var sfb = new Models.PlayStation3.SFB();

                sfb.Magic = data.ReadUInt32BigEndian();
                if (sfb.Magic != SFBMagic)
                    return null;

                sfb.FileVersion = data.ReadUInt32BigEndian();
                sfb.Reserved1 = data.ReadBytes(0x18);
                byte[] flagsType = data.ReadBytes(0x10);
                sfb.FlagsType = Encoding.ASCII.GetString(flagsType).TrimEnd('\0');
                sfb.DiscContentDataOffset = data.ReadUInt32BigEndian();
                sfb.DiscContentDataLength = data.ReadUInt32BigEndian();
                sfb.Reserved2 = data.ReadBytes(0x08);
                byte[] discTitleName = data.ReadBytes(0x08);
                sfb.DiscTitleName = Encoding.ASCII.GetString(discTitleName).TrimEnd('\0');
                sfb.Reserved3 = data.ReadBytes(0x08);
                sfb.DiscVersionDataOffset = data.ReadUInt32BigEndian();
                sfb.DiscVersionDataLength = data.ReadUInt32BigEndian();
                sfb.Reserved4 = data.ReadBytes(0x188);
                byte[] discContent = data.ReadBytes(0x20);
                sfb.DiscContent = Encoding.ASCII.GetString(discContent).TrimEnd('\0');
                byte[] discTitle = data.ReadBytes(0x10);
                sfb.DiscTitle = Encoding.ASCII.GetString(discTitle).TrimEnd('\0');
                byte[] discVersion = data.ReadBytes(0x10);
                sfb.DiscVersion = Encoding.ASCII.GetString(discVersion).TrimEnd('\0');
                sfb.Reserved5 = data.ReadBytes(0x3C0);

                return sfb;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}
