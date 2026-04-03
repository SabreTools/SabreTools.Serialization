using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single source details item
    /// </summary>
    [JsonObject("source_details"), XmlRoot("source_details")]
    public sealed class SourceDetails : DatItem<Data.Models.Metadata.SourceDetails>
    {
        #region Fields

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.SourceDetails;

        #endregion

        #region Constructors

        public SourceDetails() : base() { }

        public SourceDetails(Data.Models.Metadata.SourceDetails item) : base(item) { }

        public SourceDetails(Data.Models.Metadata.SourceDetails item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new SourceDetails(_internal.DeepClone() as Data.Models.Metadata.SourceDetails ?? []);

        #endregion
    }
}
