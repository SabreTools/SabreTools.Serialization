using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Adjuster(s) is associated with a set
    /// </summary>
    [JsonObject("adjuster"), XmlRoot("adjuster")]
    public sealed class Adjuster : DatItem<Data.Models.Metadata.Adjuster>
    {
        #region Properties

        public Condition? Condition { get; set; }

        [JsonIgnore]
        public bool ConditionSpecified => Condition is not null;

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.Adjuster)?.Default;
            set => (_internal as Data.Models.Metadata.Adjuster)?.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Adjuster;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Adjuster)?.Name;
            set => (_internal as Data.Models.Metadata.Adjuster)?.Name = value;
        }

        #endregion

        #region Constructors

        public Adjuster() : base() { }

        public Adjuster(Data.Models.Metadata.Adjuster item) : base(item)
        {
            // Handle subitems
            if (item.Condition is not null)
                Condition = new Condition(item.Condition);
        }

        public Adjuster(Data.Models.Metadata.Adjuster item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Adjuster(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Adjuster GetInternalClone()
        {
            var adjusterItem = (_internal as Data.Models.Metadata.Adjuster)?.Clone() as Data.Models.Metadata.Adjuster ?? [];

            if (Condition is not null)
                adjusterItem.Condition = Condition.GetInternalClone();

            return adjusterItem;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Adjuster otherAdjuster)
                return ((Data.Models.Metadata.Adjuster)_internal).Equals((Data.Models.Metadata.Adjuster)otherAdjuster._internal);

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
            if (other is Adjuster otherAdjuster)
                return ((Data.Models.Metadata.Adjuster)_internal).Equals((Data.Models.Metadata.Adjuster)otherAdjuster._internal);

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
            if (other is Adjuster otherAdjuster)
                return ((Data.Models.Metadata.Adjuster)_internal).Equals((Data.Models.Metadata.Adjuster)otherAdjuster._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Adjuster>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Adjuster otherAdjuster)
                return ((Data.Models.Metadata.Adjuster)_internal).Equals((Data.Models.Metadata.Adjuster)otherAdjuster._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
