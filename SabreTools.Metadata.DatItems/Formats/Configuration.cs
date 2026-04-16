using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Metadata.Filter;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Configuration(s) is associated with a set
    /// </summary>
    [JsonObject("configuration"), XmlRoot("configuration")]
    public sealed class Configuration : DatItem<Data.Models.Metadata.Configuration>
    {
        #region Properties

        public string? ConditionMask
        {
            get => _internal.ConditionMask;
            set => _internal.ConditionMask = value;
        }

        public Data.Models.Metadata.Relation? ConditionRelation
        {
            get => _internal.ConditionRelation;
            set => _internal.ConditionRelation = value;
        }

        public string? ConditionTag
        {
            get => _internal.ConditionTag;
            set => _internal.ConditionTag = value;
        }

        public string? ConditionValue
        {
            get => _internal.ConditionValue;
            set => _internal.ConditionValue = value;
        }

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
            get => _internal.Mask;
            set => _internal.Mask = value;
        }

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public string? Tag
        {
            get => _internal.Tag;
            set => _internal.Tag = value;
        }

        #endregion

        #region Constructors

        public Configuration() : base() { }

        public Configuration(Data.Models.Metadata.Configuration item) : base(item)
        {
            // Handle subitems
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

        public Configuration(Data.Models.Metadata.Configuration item, long machineIndex, long sourceIndex) : this(item)
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
        public override object Clone() => new Configuration(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Configuration GetInternalClone()
        {
            var configurationItem = _internal.Clone() as Data.Models.Metadata.Configuration ?? new();

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
                return _internal.Equals(otherConfiguration._internal);

            // Everything else fails
            return false;
        }

        #endregion

        #region Manipulation

        /// <inheritdoc/>
        public override bool PassesFilter(FilterRunner filterRunner)
        {
            if (Machine is not null && !Machine.PassesFilter(filterRunner))
                return false;

            // TODO: ConfLocation
            // TODO: ConfSetting

            return filterRunner.Run(_internal);
        }

        /// <inheritdoc/>
        public override bool PassesFilterDB(FilterRunner filterRunner)
        {
            // TODO: ConfLocation
            // TODO: ConfSetting

            return filterRunner.Run(_internal);
        }

        #endregion
    }
}
