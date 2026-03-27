using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

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
            string? overall = ReadString(Data.Models.Metadata.Feature.OverallKey);
            if (overall is not null)
                Write<string?>(Data.Models.Metadata.Feature.OverallKey, overall.AsFeatureStatus()?.AsStringValue());

            string? status = ReadString(Data.Models.Metadata.Feature.StatusKey);
            if (status is not null)
                Write<string?>(Data.Models.Metadata.Feature.StatusKey, status.AsFeatureStatus()?.AsStringValue());

            string? featureType = ReadString(Data.Models.Metadata.Feature.FeatureTypeKey);
            if (featureType is not null)
                Write<string?>(Data.Models.Metadata.Feature.FeatureTypeKey, featureType.AsFeatureType()?.AsStringValue());
        }

        public PartFeature(Data.Models.Metadata.Feature item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
