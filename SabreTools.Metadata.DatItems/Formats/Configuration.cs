using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Configuration(s) is associated with a set
    /// </summary>
    [JsonObject("configuration"), XmlRoot("configuration")]
    public sealed class Configuration : DatItem<Data.Models.Metadata.Configuration>
    {
        #region Fields

        public Condition? Condition { get; set; }

        [JsonIgnore]
        public bool ConditionSpecified => Condition is not null;

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Configuration;

        [JsonIgnore]
        public bool LocationsSpecified
        {
            get
            {
                var locations = Read<ConfLocation[]?>(Data.Models.Metadata.Configuration.ConfLocationKey);
                return locations is not null && locations.Length > 0;
            }
        }

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

        [JsonIgnore]
        public bool SettingsSpecified
        {
            get
            {
                var settings = Read<ConfSetting[]?>(Data.Models.Metadata.Configuration.ConfSettingKey);
                return settings is not null && settings.Length > 0;
            }
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

            var confLocations = item.ReadArray<Data.Models.Metadata.ConfLocation>(Data.Models.Metadata.Configuration.ConfLocationKey);
            if (confLocations is not null)
            {
                ConfLocation[] confLocationItems = Array.ConvertAll(confLocations, confLocation => new ConfLocation(confLocation));
                Write<ConfLocation[]?>(Data.Models.Metadata.Configuration.ConfLocationKey, confLocationItems);
            }

            var confSettings = item.ReadArray<Data.Models.Metadata.ConfSetting>(Data.Models.Metadata.Configuration.ConfSettingKey);
            if (confSettings is not null)
            {
                ConfSetting[] confSettingItems = Array.ConvertAll(confSettings, confSetting => new ConfSetting(confSetting));
                Write<ConfSetting[]?>(Data.Models.Metadata.Configuration.ConfSettingKey, confSettingItems);
            }
        }

        public Configuration(Data.Models.Metadata.Configuration item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Configuration(_internal.DeepClone() as Data.Models.Metadata.Configuration ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.Configuration GetInternalClone()
        {
            var configurationItem = base.GetInternalClone();

            if (Condition is not null)
                configurationItem.Condition = Condition.GetInternalClone();

            var confLocations = Read<ConfLocation[]?>(Data.Models.Metadata.Configuration.ConfLocationKey);
            if (confLocations is not null)
            {
                Data.Models.Metadata.ConfLocation[] confLocationItems = Array.ConvertAll(confLocations, confLocation => confLocation.GetInternalClone());
                configurationItem[Data.Models.Metadata.Configuration.ConfLocationKey] = confLocationItems;
            }

            var confSettings = Read<ConfSetting[]?>(Data.Models.Metadata.Configuration.ConfSettingKey);
            if (confSettings is not null)
            {
                Data.Models.Metadata.ConfSetting[] confSettingItems = Array.ConvertAll(confSettings, confSetting => confSetting.GetInternalClone());
                configurationItem[Data.Models.Metadata.Configuration.ConfSettingKey] = confSettingItems;
            }

            return configurationItem;
        }

        #endregion
    }
}
