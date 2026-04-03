using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one conflocation
    /// </summary>
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public sealed class ConfLocation : DatItem<Data.Models.Metadata.ConfLocation>
    {
        #region Fields

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.ConfLocation;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.ConfLocation)?.Name;
            set => (_internal as Data.Models.Metadata.ConfLocation)?.Name = value;
        }

        public bool? Inverted
        {
            get => (_internal as Data.Models.Metadata.ConfLocation)?.Inverted;
            set => (_internal as Data.Models.Metadata.ConfLocation)?.Inverted = value;
        }

        #endregion

        #region Constructors

        public ConfLocation() : base() { }

        public ConfLocation(Data.Models.Metadata.ConfLocation item) : base(item) { }

        public ConfLocation(Data.Models.Metadata.ConfLocation item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new ConfLocation(_internal.DeepClone() as Data.Models.Metadata.ConfLocation ?? []);

        #endregion
    }
}
