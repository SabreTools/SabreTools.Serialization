using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Configuration(s) is associated with a set
    /// </summary>
    [JsonObject("configuration"), XmlRoot("configuration")]
    public sealed class Configuration : DatItem<Data.Models.Metadata.Configuration>
    {
        #region Properties

        public Condition? Condition { get; set; }

        [JsonIgnore]
        public bool ConditionSpecified => Condition is not null;

        public ConfLocation[]? ConfLocation { get; set; }

        [JsonIgnore]
        public bool ConfLocationSpecified => ConfLocation is not null && ConfLocation.Length > 0;

        public ConfSetting[]? ConfSetting { get; set; }

        [JsonIgnore]
        public bool ConfSettingSpecified => ConfSetting is not null && ConfSetting.Length > 0;

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Configuration;

        public string? Mask
        {
            get => (_internal as Data.Models.Metadata.Configuration)?.Mask;
            set => (_internal as Data.Models.Metadata.Configuration)?.Mask = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Configuration)?.Name;
            set => (_internal as Data.Models.Metadata.Configuration)?.Name = value;
        }

        public string? Tag
        {
            get => (_internal as Data.Models.Metadata.Configuration)?.Tag;
            set => (_internal as Data.Models.Metadata.Configuration)?.Tag = value;
        }

        #endregion

        #region Constructors

        public Configuration() : base() { }

        public Configuration(Data.Models.Metadata.Configuration item) : base(item)
        {
            // Handle subitems
            if (item.Condition is not null)
                Condition = new Condition(item.Condition);

            if (item.ConfLocation is not null)
                ConfLocation = Array.ConvertAll(item.ConfLocation, confLocation => new ConfLocation(confLocation));

            if (item.ConfSetting is not null)
                ConfSetting = Array.ConvertAll(item.ConfSetting, confSetting => new ConfSetting(confSetting));
        }

        public Configuration(Data.Models.Metadata.Configuration item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Configuration(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Configuration GetInternalClone()
        {
            var configurationItem = (_internal as Data.Models.Metadata.Configuration)?.Clone() as Data.Models.Metadata.Configuration ?? new();

            if (Condition is not null)
                configurationItem.Condition = Condition.GetInternalClone();

            if (ConfLocation is not null)
                configurationItem.ConfLocation = Array.ConvertAll(ConfLocation, confLocation => confLocation.GetInternalClone());

            if (ConfSetting is not null)
                configurationItem.ConfSetting = Array.ConvertAll(ConfSetting, confSetting => confSetting.GetInternalClone());

            return configurationItem;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Configuration otherConfiguration)
                return ((Data.Models.Metadata.Configuration)_internal).Equals((Data.Models.Metadata.Configuration)otherConfiguration._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Configuration>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Configuration otherConfiguration)
                return ((Data.Models.Metadata.Configuration)_internal).Equals((Data.Models.Metadata.Configuration)otherConfiguration._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
