using System.IO;
using SabreTools.IO.Extensions;
using static SabreTools.Models.PlayStation3.Constants;

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
                var sfb = data.ReadType<Models.PlayStation3.SFB>();
                if (sfb?.Magic == SFBMagic)
                    return null;

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