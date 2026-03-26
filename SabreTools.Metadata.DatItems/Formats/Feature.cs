using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents the a feature of the machine
    /// </summary>
    [JsonObject("feature"), XmlRoot("feature")]
    public sealed class Feature : DatItem<Data.Models.Metadata.Feature>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Feature;

        #endregion

        #region Constructors

        public Feature() : base() { }

        public Feature(Data.Models.Metadata.Feature item) : base(item)
        {
            // Process flag values
            if (ReadString(Data.Models.Metadata.Feature.OverallKey) is not null)
                Write<string?>(Data.Models.Metadata.Feature.OverallKey, ReadString(Data.Models.Metadata.Feature.OverallKey).AsFeatureStatus().AsStringValue());
            if (ReadString(Data.Models.Metadata.Feature.StatusKey) is not null)
                Write<string?>(Data.Models.Metadata.Feature.StatusKey, ReadString(Data.Models.Metadata.Feature.StatusKey).AsFeatureStatus().AsStringValue());
            if (ReadString(Data.Models.Metadata.Feature.FeatureTypeKey) is not null)
                Write<string?>(Data.Models.Metadata.Feature.FeatureTypeKey, ReadString(Data.Models.Metadata.Feature.FeatureTypeKey).AsFeatureType().AsStringValue());
        }

        public Feature(Data.Models.Metadata.Feature item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
