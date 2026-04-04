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

        public Condition? Condition { get; set; }

        [JsonIgnore]
        public bool ConditionSpecified => Condition is not null;

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.Adjuster)?.Default;
            set => (_internal as Data.Models.Metadata.Adjuster)?.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Adjuster;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Adjuster)?.Name;
            set => (_internal as Data.Models.Metadata.Adjuster)?.Name = value;
        }

        #endregion

        #region Constructors

        public Adjuster() : base() { }

        public Adjuster(Data.Models.Metadata.Adjuster item) : base(item)
        {
            // Handle subitems
            if (item.Condition is not null)
                Condition = new Condition(item.Condition);
        }

        public Adjuster(Data.Models.Metadata.Adjuster item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Adjuster(_internal.DeepClone() as Data.Models.Metadata.Adjuster ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.Adjuster GetInternalClone()
        {
            var adjusterItem = base.GetInternalClone();

            if (Condition is not null)
                adjusterItem.Condition = Condition.GetInternalClone();

            return adjusterItem;
        }

        #endregion
    }
}
