using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single serials item
    /// </summary>
    [JsonObject("serials"), XmlRoot("serials")]
    public sealed class Serials : DatItem<Data.Models.Metadata.Serials>
    {
        #region Fields

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Serials;

        #endregion

        #region Constructors

        public Serials() : base() { }

        public Serials(Data.Models.Metadata.Serials item) : base(item) { }

        public Serials(Data.Models.Metadata.Serials item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Serials(_internal.DeepClone() as Data.Models.Metadata.Serials ?? []);

        #endregion
    }
}
