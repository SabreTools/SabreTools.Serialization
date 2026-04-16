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
            get => _internal.ChipType;
            set => _internal.ChipType = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Chip;

        public long? Clock
        {
            get => _internal.Clock;
            set => _internal.Clock = value;
        }

        public string? Flags
        {
            get => _internal.Flags;
            set => _internal.Flags = value;
        }

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public bool? SoundOnly
        {
            get => _internal.SoundOnly;
            set => _internal.SoundOnly = value;
        }

        public string? Tag
        {
            get => _internal.Tag;
            set => _internal.Tag = value;
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

        public Chip(Data.Models.Metadata.Chip item, long machineIndex, long sourceIndex) : this(item)
        {
            SourceIndex = sourceIndex;
            MachineIndex = machineIndex;
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
            => _internal.Clone() as Data.Models.Metadata.Chip ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Chip otherChip)
                return _internal.Equals(otherChip._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
