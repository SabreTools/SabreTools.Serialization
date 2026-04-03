using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which DIP Switch(es) is associated with a set
    /// </summary>
    [JsonObject("dipswitch"), XmlRoot("dipswitch")]
    public sealed class DipSwitch : DatItem<Data.Models.Metadata.DipSwitch>
    {
        #region Constants

        /// <summary>
        /// Non-standard key for inverted logic
        /// </summary>
        public const string PartKey = "PART";

        #endregion

        #region Fields

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = Read<Condition[]?>(Data.Models.Metadata.DipSwitch.ConditionKey);
                return conditions is not null && conditions.Length > 0;
            }
        }

        [JsonIgnore]
        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.DipSwitch)?.Default;
            set => (_internal as Data.Models.Metadata.DipSwitch)?.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DipSwitch;

        [JsonIgnore]
        public bool LocationsSpecified
        {
            get
            {
                var locations = Read<DipLocation[]?>(Data.Models.Metadata.DipSwitch.DipLocationKey);
                return locations is not null && locations.Length > 0;
            }
        }

        public string? Mask
        {
            get => (_internal as Data.Models.Metadata.DipSwitch)?.Mask;
            set => (_internal as Data.Models.Metadata.DipSwitch)?.Mask = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.DipSwitch)?.Name;
            set => (_internal as Data.Models.Metadata.DipSwitch)?.Name = value;
        }

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                var part = Read<Part?>(PartKey);
                return part is not null
                    && (!string.IsNullOrEmpty(part.Name)
                        || !string.IsNullOrEmpty(part.Interface));
            }
        }

        public string? Tag
        {
            get => (_internal as Data.Models.Metadata.DipSwitch)?.Tag;
            set => (_internal as Data.Models.Metadata.DipSwitch)?.Tag = value;
        }

        [JsonIgnore]
        public bool ValuesSpecified
        {
            get
            {
                var values = Read<DipValue[]?>(Data.Models.Metadata.DipSwitch.DipValueKey);
                return values is not null && values.Length > 0;
            }
        }

        #endregion

        #region Constructors

        public DipSwitch() : base() { }

        public DipSwitch(Data.Models.Metadata.DipSwitch item) : base(item)
        {
            // Handle subitems
            var condition = item.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.DipSwitch.ConditionKey);
            if (condition is not null)
                Write<Condition?>(Data.Models.Metadata.DipSwitch.ConditionKey, new Condition(condition));

            var dipLocations = item.ReadArray<Data.Models.Metadata.DipLocation>(Data.Models.Metadata.DipSwitch.DipLocationKey);
            if (dipLocations is not null)
            {
                DipLocation[] dipLocationItems = Array.ConvertAll(dipLocations, dipLocation => new DipLocation(dipLocation));
                Write<DipLocation[]?>(Data.Models.Metadata.DipSwitch.DipLocationKey, dipLocationItems);
            }

            var dipValues = item.ReadArray<Data.Models.Metadata.DipValue>(Data.Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues is not null)
            {
                DipValue[] dipValueItems = Array.ConvertAll(dipValues, dipValue => new DipValue(dipValue));
                Write<DipValue[]?>(Data.Models.Metadata.DipSwitch.DipValueKey, dipValueItems);
            }
        }

        public DipSwitch(Data.Models.Metadata.DipSwitch item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new DipSwitch(_internal.DeepClone() as Data.Models.Metadata.DipSwitch ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.DipSwitch GetInternalClone()
        {
            var dipSwitchItem = base.GetInternalClone();

            var condition = Read<Condition?>(Data.Models.Metadata.DipSwitch.ConditionKey);
            if (condition is not null)
                dipSwitchItem[Data.Models.Metadata.DipSwitch.ConditionKey] = condition.GetInternalClone();

            var dipLocations = Read<DipLocation[]?>(Data.Models.Metadata.DipSwitch.DipLocationKey);
            if (dipLocations is not null)
            {
                Data.Models.Metadata.DipLocation[] dipLocationItems = Array.ConvertAll(dipLocations, dipLocation => dipLocation.GetInternalClone());
                dipSwitchItem[Data.Models.Metadata.DipSwitch.DipLocationKey] = dipLocationItems;
            }

            var dipValues = Read<DipValue[]?>(Data.Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues is not null)
            {
                Data.Models.Metadata.DipValue[] dipValueItems = Array.ConvertAll(dipValues, dipValue => dipValue.GetInternalClone());
                dipSwitchItem[Data.Models.Metadata.DipSwitch.DipValueKey] = dipValueItems;
            }

            return dipSwitchItem;
        }

        #endregion
    }
}
