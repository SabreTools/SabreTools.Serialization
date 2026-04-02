using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one diplocation
    /// </summary>
    [JsonObject("diplocation"), XmlRoot("diplocation")]
    public sealed class DipLocation : DatItem<Data.Models.Metadata.DipLocation>
    {
        #region Fields

        public bool? Inverted
        {
            get => (_internal as Data.Models.Metadata.DipLocation)?.Inverted;
            set => (_internal as Data.Models.Metadata.DipLocation)?.Inverted = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.DipLocation)?.Name;
            set => (_internal as Data.Models.Metadata.DipLocation)?.Name = value;
        }

        #endregion

        #region Constructors

        public DipLocation() : base() { }

        public DipLocation(Data.Models.Metadata.DipLocation item) : base(item) { }

        public DipLocation(Data.Models.Metadata.DipLocation item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new DipLocation(_internal.DeepClone() as Data.Models.Metadata.DipLocation ?? []);

        #endregion
    }
}
