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
        #region Properties

        public long? Channels
        {
            get => _internal.Channels;
            set => _internal.Channels = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Sound;

        #endregion

        #region Constructors

        public Sound() : base() { }

        public Sound(Data.Models.Metadata.Sound item) : base(item) { }

        public Sound(Data.Models.Metadata.Sound item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        public Sound(Data.Models.Metadata.Sound item, long machineIndex, long sourceIndex) : this(item)
        {
            SourceIndex = sourceIndex;
            MachineIndex = machineIndex;
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
        public override object Clone() => new Sound(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Sound GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Sound ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Sound otherSound)
                return _internal.Equals(otherSound._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
