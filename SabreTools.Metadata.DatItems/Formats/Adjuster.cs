using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Adjuster(s) is associated with a set
    /// </summary>
    [JsonObject("adjuster"), XmlRoot("adjuster")]
    public sealed class Adjuster : DatItem<Data.Models.Metadata.Adjuster>
    {
        #region Fields

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = Read<Condition[]?>(Data.Models.Metadata.Adjuster.ConditionKey);
                return conditions is not null && conditions.Length > 0;
            }
        }

        #endregion

        #region Constructors

        public Adjuster() : base() { }

        public Adjuster(Data.Models.Metadata.Adjuster item) : base(item)
        {
            // Process flag values
            bool? defaultValue = ReadBool(Data.Models.Metadata.Adjuster.DefaultKey);
            if (defaultValue is not null)
                Write<string?>(Data.Models.Metadata.Adjuster.DefaultKey, defaultValue.FromYesNo());

            // Handle subitems
            var condition = item.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.Adjuster.ConditionKey);
            if (condition is not null)
                Write(Data.Models.Metadata.Adjuster.ConditionKey, new Condition(condition));
        }

        public Adjuster(Data.Models.Metadata.Adjuster item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Adjuster(_internal.Clone() as Data.Models.Metadata.Adjuster ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.Adjuster GetInternalClone()
        {
            var adjusterItem = base.GetInternalClone();

            var condition = Read<Condition?>(Data.Models.Metadata.Adjuster.ConditionKey);
            if (condition is not null)
                adjusterItem[Data.Models.Metadata.Adjuster.ConditionKey] = condition.GetInternalClone();

            return adjusterItem;
        }

        #endregion
    }
}
