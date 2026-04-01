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
        #region Constructors

        public RamOption() : base() { }

        public RamOption(Data.Models.Metadata.RamOption item) : base(item)
        {
            // Process flag values
            bool? defaultValue = ReadBool(Data.Models.Metadata.RamOption.DefaultKey);
            if (defaultValue is not null)
                Write<string?>(Data.Models.Metadata.RamOption.DefaultKey, defaultValue.FromYesNo());
        }

        public RamOption(Data.Models.Metadata.RamOption item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new RamOption(_internal.Clone() as Data.Models.Metadata.RamOption ?? []);

        #endregion
    }
}
