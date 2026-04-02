using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one conflocation
    /// </summary>
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public sealed class ConfLocation : DatItem<Data.Models.Metadata.ConfLocation>
    {
        #region Fields

        public bool? Inverted
        {
            get => (_internal as Data.Models.Metadata.ConfLocation)?.Inverted;
            set => (_internal as Data.Models.Metadata.ConfLocation)?.Inverted = value;
        }

        #endregion

        #region Constructors

        public ConfLocation() : base() { }

        public ConfLocation(Data.Models.Metadata.ConfLocation item) : base(item) { }

        public ConfLocation(Data.Models.Metadata.ConfLocation item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new ConfLocation(_internal.Clone() as Data.Models.Metadata.ConfLocation ?? []);

        #endregion
    }
}
