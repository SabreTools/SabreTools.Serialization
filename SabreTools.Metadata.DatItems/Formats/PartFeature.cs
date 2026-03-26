using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one part feature object
    /// </summary>
    [JsonObject("part_feature"), XmlRoot("part_feature")]
    public sealed class PartFeature : DatItem<Data.Models.Metadata.Feature>
    {
        #region Constants

        /// <summary>
        /// Non-standard key for inverted logic
        /// </summary>
        public const string PartKey = "PART";

        #endregion

        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.PartFeature;

        #endregion

        #region Constructors

        public PartFeature() : base() { }

        public PartFeature(Data.Models.Metadata.Feature item) : base(item)
        {
            // Process flag values
            if (ReadString(Data.Models.Metadata.Feature.OverallKey) is not null)
                Write<string?>(Data.Models.Metadata.Feature.OverallKey, ReadString(Data.Models.Metadata.Feature.OverallKey).AsFeatureStatus().AsStringValue());
            if (ReadString(Data.Models.Metadata.Feature.StatusKey) is not null)
                Write<string?>(Data.Models.Metadata.Feature.StatusKey, ReadString(Data.Models.Metadata.Feature.StatusKey).AsFeatureStatus().AsStringValue());
            if (ReadString(Data.Models.Metadata.Feature.FeatureTypeKey) is not null)
                Write<string?>(Data.Models.Metadata.Feature.FeatureTypeKey, ReadString(Data.Models.Metadata.Feature.FeatureTypeKey).AsFeatureType().AsStringValue());
        }

        public PartFeature(Data.Models.Metadata.Feature item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
