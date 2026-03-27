using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML slotoption
    /// </summary>
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public sealed class SlotOption : DatItem<Data.Models.Metadata.SlotOption>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.SlotOption;

        #endregion

        #region Constructors

        public SlotOption() : base() { }

        public SlotOption(Data.Models.Metadata.SlotOption item) : base(item)
        {
            // Process flag values
            bool? defaultValue = ReadBool(Data.Models.Metadata.SlotOption.DefaultKey);
            if (defaultValue is not null)
                Write<string?>(Data.Models.Metadata.SlotOption.DefaultKey, defaultValue.FromYesNo());
        }

        public SlotOption(Data.Models.Metadata.SlotOption item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new SlotOption(_internal.Clone() as Data.Models.Metadata.SlotOption ?? []);

        #endregion
    }
}
