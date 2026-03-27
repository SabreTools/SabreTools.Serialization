using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

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

        public Feature(Data.Models.Metadata.Feature item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
