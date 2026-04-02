using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single analog item
    /// </summary>
    [JsonObject("analog"), XmlRoot("analog")]
    public sealed class Analog : DatItem<Data.Models.Metadata.Analog>
    {
        #region Fields

        public string? Mask
        {
            get => (_internal as Data.Models.Metadata.Analog)?.Mask;
            set => (_internal as Data.Models.Metadata.Analog)?.Mask = value;
        }

        #endregion

        #region Constructors

        public Analog() : base() { }

        public Analog(Data.Models.Metadata.Analog item) : base(item) { }

        public Analog(Data.Models.Metadata.Analog item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Analog(_internal.DeepClone() as Data.Models.Metadata.Analog ?? []);

        #endregion
    }
}
