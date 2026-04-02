using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which RAM option(s) is associated with a set
    /// </summary>
    [JsonObject("ramoption"), XmlRoot("ramoption")]
    public sealed class RamOption : DatItem<Data.Models.Metadata.RamOption>
    {
        #region Properties

        public string? Content
        {
            get => (_internal as Data.Models.Metadata.RamOption)?.Content;
            set => (_internal as Data.Models.Metadata.RamOption)?.Content = value;
        }

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.RamOption)?.Default;
            set => (_internal as Data.Models.Metadata.RamOption)?.Default = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.RamOption)?.Name;
            set => (_internal as Data.Models.Metadata.RamOption)?.Name = value;
        }

        #endregion

        #region Constructors

        public RamOption() : base() { }

        public RamOption(Data.Models.Metadata.RamOption item) : base(item) { }

        public RamOption(Data.Models.Metadata.RamOption item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new RamOption(_internal.DeepClone() as Data.Models.Metadata.RamOption ?? []);

        #endregion
    }
}
