using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// SoftwareList part information
    /// </summary>
    /// <remarks>One Part can contain multiple PartFeature, DataArea, DiskArea, and DipSwitch items</remarks>
    [JsonObject("part"), XmlRoot("part")]
    public sealed class Part : DatItem<Data.Models.Metadata.Part>
    {
        #region Fields

        [JsonIgnore]
        public bool FeaturesSpecified
        {
            get
            {
                var features = Read<PartFeature[]?>(Data.Models.Metadata.Part.FeatureKey);
                return features is not null && features.Length > 0;
            }
        }

        public string? Interface
        {
            get => (_internal as Data.Models.Metadata.Part)?.Interface;
            set => (_internal as Data.Models.Metadata.Part)?.Interface = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Part)?.Name;
            set => (_internal as Data.Models.Metadata.Part)?.Name = value;
        }

        #endregion

        #region Constructors

        public Part() : base() { }

        public Part(Data.Models.Metadata.Part item) : base(item) { }

        public Part(Data.Models.Metadata.Part item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Part(_internal.DeepClone() as Data.Models.Metadata.Part ?? []);

        #endregion
    }
}
