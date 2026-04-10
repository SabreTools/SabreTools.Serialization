using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Metadata.Filter;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which DIP Switch(es) is associated with a set
    /// </summary>
    [JsonObject("dipswitch"), XmlRoot("dipswitch")]
    public sealed class DipSwitch : DatItem<Data.Models.Metadata.DipSwitch>
    {
        #region Properties

        public Condition? Condition { get; set; }

        [JsonIgnore]
        public bool ConditionSpecified => Condition is not null;

        public bool? Default
        {
            get => _internal.Default;
            set => _internal.Default = value;
        }

        public DipLocation[]? DipLocation { get; set; }

        [JsonIgnore]
        public bool DipLocationSpecified => DipLocation is not null && DipLocation.Length > 0;

        public DipValue[]? DipValue { get; set; }

        [JsonIgnore]
        public bool DipValueSpecified => DipValue is not null && DipValue.Length > 0;

        public string[]? Entry { get; set; }

        [JsonIgnore]
        public bool EntrySpecified => Entry is not null && Entry.Length > 0;

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DipSwitch;

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

        public Part? Part { get; set; }

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                return Part is not null
                    && (!string.IsNullOrEmpty(Part.Name)
                        || !string.IsNullOrEmpty(Part.Interface));
            }
        }

        public string? Tag
        {
            get => _internal.Tag;
            set => _internal.Tag = value;
        }

        #endregion

        #region Constructors

        public DipSwitch() : base() { }

        public DipSwitch(Data.Models.Metadata.DipSwitch item) : base(item)
        {
            // Handle subitems
            if (item.Condition is not null)
                Condition = new Condition(item.Condition);

            if (item.DipLocation is not null)
                DipLocation = Array.ConvertAll(item.DipLocation, dipLocation => new DipLocation(dipLocation));

            if (item.DipValue is not null)
                DipValue = Array.ConvertAll(item.DipValue, dipValue => new DipValue(dipValue));

            if (item.Entry is not null)
                Entry = Array.ConvertAll(item.Entry, entry => entry);
        }

        public DipSwitch(Data.Models.Metadata.DipSwitch item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new DipSwitch(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.DipSwitch GetInternalClone()
        {
            var dipSwitchItem = _internal.Clone() as Data.Models.Metadata.DipSwitch ?? new();

            if (Condition is not null)
                dipSwitchItem.Condition = Condition.GetInternalClone();

            if (DipLocation is not null)
                dipSwitchItem.DipLocation = Array.ConvertAll(DipLocation, dipLocation => dipLocation.GetInternalClone());

            if (DipValue is not null)
                dipSwitchItem.DipValue = Array.ConvertAll(DipValue, dipValue => dipValue.GetInternalClone());

            if (Entry is not null)
                dipSwitchItem.Entry = Array.ConvertAll(Entry, entry => entry);

            return dipSwitchItem;
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
            if (other is DipSwitch otherDipSwitch)
                return _internal.Equals(otherDipSwitch._internal);

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

            // TODO: Condition
            // TODO: DipLocation
            // TODO: DipValue
            // TODO: Entry

            return filterRunner.Run(_internal);
        }

        /// <inheritdoc/>
        public override bool PassesFilterDB(FilterRunner filterRunner)
        {
            // TODO: Condition
            // TODO: DipLocation
            // TODO: DipValue
            // TODO: Entry

            return filterRunner.Run(_internal);
        }

        #endregion
    }
}
