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

        public Condition? Condition { get; set; }

        [JsonIgnore]
        public bool ConditionSpecified => Condition is not null;

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.ConfSetting)?.Default;
            set => (_internal as Data.Models.Metadata.ConfSetting)?.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.ConfSetting;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.ConfSetting)?.Name;
            set => (_internal as Data.Models.Metadata.ConfSetting)?.Name = value;
        }

        public string? Value
        {
            get => (_internal as Data.Models.Metadata.ConfSetting)?.Value;
            set => (_internal as Data.Models.Metadata.ConfSetting)?.Value = value;
        }

        #endregion

        #region Constructors

        public ConfSetting() : base() { }

        public ConfSetting(Data.Models.Metadata.ConfSetting item) : base(item)
        {
            // Handle subitems
            if (item.Condition is not null)
                Condition = new Condition(item.Condition);
        }

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
        {
            var confSettingItem = (_internal as Data.Models.Metadata.ConfSetting)?.Clone() as Data.Models.Metadata.ConfSetting ?? [];

            if (Condition is not null)
                confSettingItem.Condition = Condition.GetInternalClone();

            return confSettingItem;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.ConfSetting>? other)
        {
            // If the other value is invalid
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
