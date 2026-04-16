using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single device on the machine
    /// </summary>
    [JsonObject("device"), XmlRoot("device")]
    public sealed class Device : DatItem<Data.Models.Metadata.Device>
    {
        #region Properties

        public Data.Models.Metadata.DeviceType? DeviceType
        {
            get => _internal.DeviceType;
            set => _internal.DeviceType = value;
        }

        public string[]? ExtensionName
        {
            get => _internal.ExtensionName;
            set => _internal.ExtensionName = value;
        }

        public string? FixedImage
        {
            get => _internal.FixedImage;
            set => _internal.FixedImage = value;
        }

        public string? InstanceBriefName
        {
            get => _internal.InstanceBriefName;
            set => _internal.InstanceBriefName = value;
        }

        public string? InstanceName
        {
            get => _internal.InstanceName;
            set => _internal.InstanceName = value;
        }

        public string? Interface
        {
            get => _internal.Interface;
            set => _internal.Interface = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Device;

        public bool? Mandatory
        {
            get => _internal.Mandatory;
            set => _internal.Mandatory = value;
        }

        public string? Tag
        {
            get => _internal.Tag;
            set => _internal.Tag = value;
        }

        #endregion

        #region Constructors

        public Device() : base() { }

        public Device(Data.Models.Metadata.Device item) : base(item) { }

        public Device(Data.Models.Metadata.Device item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        public Device(Data.Models.Metadata.Device item, long machineIndex, long sourceIndex) : this(item)
        {
            SourceIndex = sourceIndex;
            MachineIndex = machineIndex;
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => null;

        /// <inheritdoc/>
        public override void SetName(string? name) { }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Device(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Device GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Device ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Device otherDevice)
                return _internal.Equals(otherDevice._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
