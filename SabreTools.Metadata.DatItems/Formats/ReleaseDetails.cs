using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single release details item
    /// </summary>
    [JsonObject("release_details"), XmlRoot("release_details")]
    public sealed class ReleaseDetails : DatItem<Data.Models.Metadata.ReleaseDetails>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.ReleaseDetails;

        #endregion

        #region Constructors

        public ReleaseDetails() : base() { }

        public ReleaseDetails(Data.Models.Metadata.ReleaseDetails item) : base(item) { }

        public ReleaseDetails(Data.Models.Metadata.ReleaseDetails item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }
        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new ReleaseDetails(_internal.Clone() as Data.Models.Metadata.ReleaseDetails ?? []);

        #endregion
    }
}
