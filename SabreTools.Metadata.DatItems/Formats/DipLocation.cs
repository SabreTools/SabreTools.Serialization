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
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.DipLocation;

        #endregion

        #region Constructors

        public DipLocation() : base() { }

        public DipLocation(Data.Models.Metadata.DipLocation item) : base(item)
        {
            // Process flag values
            if (ReadBool(Data.Models.Metadata.DipLocation.InvertedKey) is not null)
                Write<string?>(Data.Models.Metadata.DipLocation.InvertedKey, ReadBool(Data.Models.Metadata.DipLocation.InvertedKey).FromYesNo());
        }

        public DipLocation(Data.Models.Metadata.DipLocation item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
