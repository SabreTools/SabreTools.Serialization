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

        public Data.Models.Metadata.FeatureType? FeatureType
        {
            get => (_internal as Data.Models.Metadata.Feature)?.FeatureType;
            set => (_internal as Data.Models.Metadata.Feature)?.FeatureType = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Feature)?.Name;
            set => (_internal as Data.Models.Metadata.Feature)?.Name = value;
        }

        public Data.Models.Metadata.FeatureStatus? Overall
        {
            get => (_internal as Data.Models.Metadata.Feature)?.Overall;
            set => (_internal as Data.Models.Metadata.Feature)?.Overall = value;
        }

        public Data.Models.Metadata.FeatureStatus? Status
        {
            get => (_internal as Data.Models.Metadata.Feature)?.Status;
            set => (_internal as Data.Models.Metadata.Feature)?.Status = value;
        }

        public string? Value
        {
            get => (_internal as Data.Models.Metadata.Feature)?.Value;
            set => (_internal as Data.Models.Metadata.Feature)?.Value = value;
        }

        #endregion

        #region Constructors

        public Feature() : base() { }

        public Feature(Data.Models.Metadata.Feature item) : base(item) { }

        public Feature(Data.Models.Metadata.Feature item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Feature(_internal.DeepClone() as Data.Models.Metadata.Feature ?? []);

        #endregion
    }
}
