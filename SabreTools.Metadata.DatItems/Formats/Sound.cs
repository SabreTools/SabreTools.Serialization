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
            get => (_internal as Data.Models.Metadata.Sound)?.Channels;
            set => (_internal as Data.Models.Metadata.Sound)?.Channels = value;
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
            => (_internal as Data.Models.Metadata.Sound)?.Clone() as Data.Models.Metadata.Sound ?? [];

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Sound otherSound)
                return ((Data.Models.Metadata.Sound)_internal).Equals((Data.Models.Metadata.Sound)otherSound._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Data.Models.Metadata.DatItem>? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Sound otherSound)
                return ((Data.Models.Metadata.Sound)_internal).Equals((Data.Models.Metadata.Sound)otherSound._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Sound otherSound)
                return ((Data.Models.Metadata.Sound)_internal).Equals((Data.Models.Metadata.Sound)otherSound._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Sound>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Sound otherSound)
                return ((Data.Models.Metadata.Sound)_internal).Equals((Data.Models.Metadata.Sound)otherSound._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
