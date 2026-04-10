using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML confsetting
    /// </summary>
    [JsonObject("confsetting"), XmlRoot("confsetting")]
    public sealed class ConfSetting : DatItem<Data.Models.Metadata.ConfSetting>
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

        public bool? Default
        {
            get => _internal.Default;
            set => _internal.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.ConfSetting;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public string? Value
        {
            get => _internal.Value;
            set => _internal.Value = value;
        }

        #endregion

        #region Constructors

        public ConfSetting() : base() { }

        public ConfSetting(Data.Models.Metadata.ConfSetting item) : base(item) { }

        public ConfSetting(Data.Models.Metadata.ConfSetting item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new ConfSetting(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.ConfSetting GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.ConfSetting ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is ConfSetting otherConfSetting)
                return _internal.Equals(otherConfSetting._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
