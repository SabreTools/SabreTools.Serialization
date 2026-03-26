using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents the sound output for a machine
    /// </summary>
    [JsonObject("sound"), XmlRoot("sound")]
    public sealed class Sound : DatItem<Data.Models.Metadata.Sound>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Sound;

        #endregion

        #region Constructors

        public Sound() : base() { }

        public Sound(Data.Models.Metadata.Sound item) : base(item)
        {
            // Process flag values
            if (ReadLong(Data.Models.Metadata.Sound.ChannelsKey) is not null)
                Write<string?>(Data.Models.Metadata.Sound.ChannelsKey, ReadLong(Data.Models.Metadata.Sound.ChannelsKey).ToString());
        }

        public Sound(Data.Models.Metadata.Sound item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
