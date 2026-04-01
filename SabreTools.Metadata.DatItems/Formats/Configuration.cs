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

        /// <inheritdoc>/>
        internal override Data.Models.Metadata.ItemType ItemType => Data.Models.Metadata.ItemType.Configuration;

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = Read<Condition[]?>(Data.Models.Metadata.Configuration.ConditionKey);
                return conditions is not null && conditions.Length > 0;
            }
        }

        [JsonIgnore]
        public bool LocationsSpecified
        {
            get
            {
                var locations = Read<ConfLocation[]?>(Data.Models.Metadata.Configuration.ConfLocationKey);
                return locations is not null && locations.Length > 0;
            }
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

        #endregion

        #region Constructors

        public Configuration() : base() { }

        public Configuration(Data.Models.Metadata.Configuration item) : base(item)
        {
            // Handle subitems
            var condition = item.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.Configuration.ConditionKey);
            if (condition is not null)
                Write<Condition?>(Data.Models.Metadata.Configuration.ConditionKey, new Condition(condition));

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
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Configuration(_internal.Clone() as Data.Models.Metadata.Configuration ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.Configuration GetInternalClone()
        {
            var configurationItem = base.GetInternalClone();

            var condition = Read<Condition?>(Data.Models.Metadata.Configuration.ConditionKey);
            if (condition is not null)
                configurationItem[Data.Models.Metadata.Configuration.ConditionKey] = condition.GetInternalClone();

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
