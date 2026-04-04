using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Chip(s) is associated with a set
    /// </summary>
    [JsonObject("chip"), XmlRoot("chip")]
    public sealed class Chip : DatItem<Data.Models.Metadata.Chip>
    {
        #region Properties

        public Data.Models.Metadata.ChipType? ChipType
        {
            get => (_internal as Data.Models.Metadata.Chip)?.ChipType;
            set => (_internal as Data.Models.Metadata.Chip)?.ChipType = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Chip;

        public long? Clock
        {
            get => (_internal as Data.Models.Metadata.Chip)?.Clock;
            set => (_internal as Data.Models.Metadata.Chip)?.Clock = value;
        }

        public string? Flags
        {
            get => (_internal as Data.Models.Metadata.Chip)?.Flags;
            set => (_internal as Data.Models.Metadata.Chip)?.Flags = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Chip)?.Name;
            set => (_internal as Data.Models.Metadata.Chip)?.Name = value;
        }

        public bool? SoundOnly
        {
            get => (_internal as Data.Models.Metadata.Chip)?.SoundOnly;
            set => (_internal as Data.Models.Metadata.Chip)?.SoundOnly = value;
        }

        public string? Tag
        {
            get => (_internal as Data.Models.Metadata.Chip)?.Tag;
            set => (_internal as Data.Models.Metadata.Chip)?.Tag = value;
        }

        #endregion

        #region Constructors

        public Chip() : base() { }

        public Chip(Data.Models.Metadata.Chip item) : base(item) { }

        public Chip(Data.Models.Metadata.Chip item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Chip(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Chip GetInternalClone()
            => (_internal as Data.Models.Metadata.Chip)?.Clone() as Data.Models.Metadata.Chip ?? [];

        #endregion
    }
}
