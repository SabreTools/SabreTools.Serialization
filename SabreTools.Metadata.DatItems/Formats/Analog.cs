using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single analog item
    /// </summary>
    [JsonObject("analog"), XmlRoot("analog")]
    public sealed class Analog : DatItem<Data.Models.Metadata.Analog>
    {
        #region Fields

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Analog;

        public string? Mask
        {
            get => (_internal as Data.Models.Metadata.Analog)?.Mask;
            set => (_internal as Data.Models.Metadata.Analog)?.Mask = value;
        }

        #endregion

        #region Constructors

        public Analog() : base() { }

        public Analog(Data.Models.Metadata.Analog item) : base(item) { }

        public Analog(Data.Models.Metadata.Analog item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => null;

        /// <inheritdoc/>
        public override void SetName(string? name) { }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Analog(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Analog GetInternalClone()
            => (_internal as Data.Models.Metadata.Analog)?.Clone() as Data.Models.Metadata.Analog ?? [];

        #endregion
    }
}
