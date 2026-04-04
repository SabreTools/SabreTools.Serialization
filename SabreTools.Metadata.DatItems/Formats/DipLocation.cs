using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one diplocation
    /// </summary>
    [JsonObject("diplocation"), XmlRoot("diplocation")]
    public sealed class DipLocation : DatItem<Data.Models.Metadata.DipLocation>
    {
        #region Properties

        public bool? Inverted
        {
            get => (_internal as Data.Models.Metadata.DipLocation)?.Inverted;
            set => (_internal as Data.Models.Metadata.DipLocation)?.Inverted = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DipLocation;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.DipLocation)?.Name;
            set => (_internal as Data.Models.Metadata.DipLocation)?.Name = value;
        }

        public long? Number
        {
            get => (_internal as Data.Models.Metadata.DipLocation)?.Number;
            set => (_internal as Data.Models.Metadata.DipLocation)?.Number = value;
        }

        #endregion

        #region Constructors

        public DipLocation() : base() { }

        public DipLocation(Data.Models.Metadata.DipLocation item) : base(item) { }

        public DipLocation(Data.Models.Metadata.DipLocation item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new DipLocation(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.DipLocation GetInternalClone()
            => (_internal as Data.Models.Metadata.DipLocation)?.Clone() as Data.Models.Metadata.DipLocation ?? [];

        #endregion
    }
}
