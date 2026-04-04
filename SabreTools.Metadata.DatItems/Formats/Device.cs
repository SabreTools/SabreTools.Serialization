using System;
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
            get => (_internal as Data.Models.Metadata.Device)?.DeviceType;
            set => (_internal as Data.Models.Metadata.Device)?.DeviceType = value;
        }

        public Extension[]? Extension { get; set; }

        [JsonIgnore]
        public bool ExtensionSpecified => Extension is not null && Extension.Length > 0;

        public string? FixedImage
        {
            get => (_internal as Data.Models.Metadata.Device)?.FixedImage;
            set => (_internal as Data.Models.Metadata.Device)?.FixedImage = value;
        }

        public Instance? Instance { get; set; }

        [JsonIgnore]
        public bool InstanceSpecified => Instance is not null;

        public string? Interface
        {
            get => (_internal as Data.Models.Metadata.Device)?.Interface;
            set => (_internal as Data.Models.Metadata.Device)?.Interface = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Device;

        public bool? Mandatory
        {
            get => (_internal as Data.Models.Metadata.Device)?.Mandatory;
            set => (_internal as Data.Models.Metadata.Device)?.Mandatory = value;
        }

        public string? Tag
        {
            get => (_internal as Data.Models.Metadata.Device)?.Tag;
            set => (_internal as Data.Models.Metadata.Device)?.Tag = value;
        }

        #endregion

        #region Constructors

        public Device() : base() { }

        public Device(Data.Models.Metadata.Device item) : base(item)
        {
            // Handle subitems
            if (item.Extension is not null)
                Extension = Array.ConvertAll(item.Extension, extension => new Extension(extension)); ;

            if (item.Instance is not null)
                Instance = new Instance(item.Instance);
        }

        public Device(Data.Models.Metadata.Device item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
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
        {
            var deviceItem = (_internal as Data.Models.Metadata.Device)?.Clone() as Data.Models.Metadata.Device ?? [];

            deviceItem.DeviceType = DeviceType;
            deviceItem.FixedImage = FixedImage;
            deviceItem.Interface = Interface;
            deviceItem.Mandatory = Mandatory;
            deviceItem.Tag = Tag;

            if (Instance is not null)
                deviceItem.Instance = Instance.GetInternalClone();

            if (Extension is not null)
                deviceItem.Extension = Array.ConvertAll(Extension, extension => extension.GetInternalClone()); ;

            return deviceItem;
        }

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
                return ((Data.Models.Metadata.Device)_internal).Equals((Data.Models.Metadata.Device)otherDevice._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Device>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Device otherDevice)
                return ((Data.Models.Metadata.Device)_internal).Equals((Data.Models.Metadata.Device)otherDevice._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
