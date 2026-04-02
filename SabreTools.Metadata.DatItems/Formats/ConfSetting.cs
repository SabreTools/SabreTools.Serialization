using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML confsetting
    /// </summary>
    [JsonObject("confsetting"), XmlRoot("confsetting")]
    public sealed class ConfSetting : DatItem<Data.Models.Metadata.ConfSetting>
    {
        #region Fields

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = Read<Condition[]?>(Data.Models.Metadata.ConfSetting.ConditionKey);
                return conditions is not null && conditions.Length > 0;
            }
        }

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.ConfSetting)?.Default;
            set => (_internal as Data.Models.Metadata.ConfSetting)?.Default = value;
        }

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
            var condition = Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.ConfSetting.ConditionKey);
            if (condition is not null)
                Write<Condition?>(Data.Models.Metadata.ConfSetting.ConditionKey, new Condition(condition));
        }

        public ConfSetting(Data.Models.Metadata.ConfSetting item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new ConfSetting(_internal.Clone() as Data.Models.Metadata.ConfSetting ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.ConfSetting GetInternalClone()
        {
            var confSettingItem = base.GetInternalClone();

            // Handle subitems
            var condition = Read<Condition>(Data.Models.Metadata.ConfSetting.ConditionKey);
            if (condition is not null)
                confSettingItem[Data.Models.Metadata.ConfSetting.ConditionKey] = condition.GetInternalClone();

            return confSettingItem;
        }

        #endregion
    }
}
