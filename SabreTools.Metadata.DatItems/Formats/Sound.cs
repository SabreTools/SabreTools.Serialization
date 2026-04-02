using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents the sound output for a machine
    /// </summary>
    [JsonObject("sound"), XmlRoot("sound")]
    public sealed class Sound : DatItem<Data.Models.Metadata.Sound>
    {
        #region Constructors

        public Sound() : base() { }

        public Sound(Data.Models.Metadata.Sound item) : base(item)
        {
            // Process flag values
            long? channels = ReadLong(Data.Models.Metadata.Sound.ChannelsKey);
            if (channels is not null)
                Write<string?>(Data.Models.Metadata.Sound.ChannelsKey, channels.ToString());
        }

        public Sound(Data.Models.Metadata.Sound item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Sound(_internal.DeepClone() as Data.Models.Metadata.Sound ?? []);

        #endregion
    }
}
