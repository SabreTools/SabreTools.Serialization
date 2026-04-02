using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML slotoption
    /// </summary>
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public sealed class SlotOption : DatItem<Data.Models.Metadata.SlotOption>
    {
        #region Properties

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.SlotOption)?.Default;
            set => (_internal as Data.Models.Metadata.SlotOption)?.Default = value;
        }

        public string? DevName
        {
            get => (_internal as Data.Models.Metadata.SlotOption)?.DevName;
            set => (_internal as Data.Models.Metadata.SlotOption)?.DevName = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.SlotOption)?.Name;
            set => (_internal as Data.Models.Metadata.SlotOption)?.Name = value;
        }

        #endregion

        #region Constructors

        public SlotOption() : base() { }

        public SlotOption(Data.Models.Metadata.SlotOption item) : base(item) { }

        public SlotOption(Data.Models.Metadata.SlotOption item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new SlotOption(_internal.DeepClone() as Data.Models.Metadata.SlotOption ?? []);

        #endregion
    }
}
