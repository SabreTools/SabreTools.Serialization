using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

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

        public DataArea[]? DataArea { get; set; }

        [JsonIgnore]
        public bool DataAreaSpecified => DataArea is not null && DataArea.Length > 0;

        public DiskArea[]? DiskArea { get; set; }

        [JsonIgnore]
        public bool DiskAreaSpecified => DiskArea is not null && DiskArea.Length > 0;

        public DipSwitch[]? DipSwitch { get; set; }

        [JsonIgnore]
        public bool DipSwitchSpecified => DipSwitch is not null && DipSwitch.Length > 0;

        public PartFeature[]? Feature { get; set; }

        [JsonIgnore]
        public bool FeatureSpecified => Feature is not null && Feature.Length > 0;

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

        public Part(Data.Models.Metadata.Part item) : base(item)
        {
            // Handle subitems
            if (item.DataArea is not null)
                DataArea = Array.ConvertAll(item.DataArea, dataArea => new DataArea(dataArea));

            if (item.DiskArea is not null)
                DiskArea = Array.ConvertAll(item.DiskArea, diskArea => new DiskArea(diskArea));

            if (item.DipSwitch is not null)
                DipSwitch = Array.ConvertAll(item.DipSwitch, dipSwitch => new DipSwitch(dipSwitch));

            if (item.Feature is not null)
                Feature = Array.ConvertAll(item.Feature, feature => new PartFeature(feature));
        }

        public Part(Data.Models.Metadata.Part item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Part(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Part GetInternalClone()
        {
            var partItem = (_internal as Data.Models.Metadata.Part)?.Clone() as Data.Models.Metadata.Part ?? [];

            if (DataArea is not null)
                partItem.DataArea = Array.ConvertAll(DataArea, dataArea => dataArea.GetInternalClone());

            if (DiskArea is not null)
                partItem.DiskArea = Array.ConvertAll(DiskArea, diskArea => diskArea.GetInternalClone());

            if (DipSwitch is not null)
                partItem.DipSwitch = Array.ConvertAll(DipSwitch, dipSwitch => dipSwitch.GetInternalClone());

            if (Feature is not null)
                partItem.Feature = Array.ConvertAll(Feature, feature => feature.GetInternalClone());

            return partItem;
        }

        #endregion
    }
}
