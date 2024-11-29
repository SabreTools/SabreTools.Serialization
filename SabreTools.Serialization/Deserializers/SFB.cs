using System.IO;
using System.Text;
using SabreTools.IO.Extensions;

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

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Deserialize the SFB
            var sfb = data.ReadType<Models.PlayStation3.SFB>();
            if (sfb?.Magic == null)
                return null;

            string magic = Encoding.ASCII.GetString(sfb.Magic);
            if (magic != ".SFB")
                return null;

            return sfb;
        }
    }
}