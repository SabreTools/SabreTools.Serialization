using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// PartList part information
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

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Part;

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

        /// <inheritdoc/>
        public override Data.Models.Metadata.Part GetInternalClone()
        {
            var partItem = base.GetInternalClone();

            var dataAreas = Read<DataArea[]?>(Data.Models.Metadata.Part.DataAreaKey);
            if (dataAreas is not null)
            {
                Data.Models.Metadata.DataArea[] dataAreaItems = Array.ConvertAll(dataAreas, dataArea => dataArea.GetInternalClone());
                partItem[Data.Models.Metadata.Part.DataAreaKey] = dataAreaItems;
            }

            var diskAreas = Read<DiskArea[]?>(Data.Models.Metadata.Part.DiskAreaKey);
            if (diskAreas is not null)
            {
                Data.Models.Metadata.DiskArea[] diskAreaItems = Array.ConvertAll(diskAreas, diskArea => diskArea.GetInternalClone());
                partItem[Data.Models.Metadata.Part.DiskAreaKey] = diskAreaItems;
            }

            var dipSwitches = Read<DipSwitch[]?>(Data.Models.Metadata.Part.DipSwitchKey);
            if (dipSwitches is not null)
            {
                Data.Models.Metadata.DipSwitch[] dipSwitchItems = Array.ConvertAll(dipSwitches, dipSwitch => dipSwitch.GetInternalClone());
                partItem[Data.Models.Metadata.Part.DipSwitchKey] = dipSwitchItems;
            }

            var features = Read<Feature[]?>(Data.Models.Metadata.Part.FeatureKey);
            if (features is not null)
            {
                Data.Models.Metadata.Feature[] featureItems = Array.ConvertAll(features, feature => feature.GetInternalClone());
                partItem[Data.Models.Metadata.Part.FeatureKey] = featureItems;
            }

            return partItem;
        }

        #endregion
    }
}
