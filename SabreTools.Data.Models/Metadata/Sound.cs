using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("sound"), XmlRoot("sound")]
    public class Sound : DatItem, ICloneable, IEquatable<Sound>
    {
        #region Properties

        public long? Channels { get; set; }

        #endregion

        public Sound() => ItemType = ItemType.Sound;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Sound();

            obj.Channels = Channels;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Sound? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (Channels != other.Channels)
                return false;

            return true;
        }
    }
}
