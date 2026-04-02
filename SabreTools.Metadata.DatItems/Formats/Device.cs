using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single device on the machine
    /// </summary>
    [JsonObject("device"), XmlRoot("device")]
    public sealed class Device : DatItem<Data.Models.Metadata.Device>
    {
        #region Fields

        public Data.Models.Metadata.DeviceType? DeviceType
        {
            get => (_internal as Data.Models.Metadata.Device)?.DeviceType;
            set => (_internal as Data.Models.Metadata.Device)?.DeviceType = value;
        }

        [JsonIgnore]
        public bool ExtensionsSpecified
        {
            get
            {
                var extensions = Read<Extension[]?>(Data.Models.Metadata.Device.ExtensionKey);
                return extensions is not null && extensions.Length > 0;
            }
        }

        public string? FixedImage
        {
            get => (_internal as Data.Models.Metadata.Device)?.FixedImage;
            set => (_internal as Data.Models.Metadata.Device)?.FixedImage = value;
        }

        [JsonIgnore]
        public bool InstancesSpecified
        {
            get
            {
                var instances = Read<Instance[]?>(Data.Models.Metadata.Device.InstanceKey);
                return instances is not null && instances.Length > 0;
            }
        }

        public string? Interface
        {
            get => (_internal as Data.Models.Metadata.Device)?.Interface;
            set => (_internal as Data.Models.Metadata.Device)?.Interface = value;
        }

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
            var instance = item.Read<Data.Models.Metadata.Instance>(Data.Models.Metadata.Device.InstanceKey);
            if (instance is not null)
                Write<Instance?>(Data.Models.Metadata.Device.InstanceKey, new Instance(instance));

            var extensions = item.ReadArray<Data.Models.Metadata.Extension>(Data.Models.Metadata.Device.ExtensionKey);
            if (extensions is not null)
            {
                Extension[] extensionItems = Array.ConvertAll(extensions, extension => new Extension(extension));
                Write<Extension[]?>(Data.Models.Metadata.Device.ExtensionKey, extensionItems);
            }
        }

        public Device(Data.Models.Metadata.Device item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Device(_internal.DeepClone() as Data.Models.Metadata.Device ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.Device GetInternalClone()
        {
            var deviceItem = base.GetInternalClone();

            deviceItem.DeviceType = DeviceType;
            deviceItem.FixedImage = FixedImage;
            deviceItem.Interface = Interface;
            deviceItem.Mandatory = Mandatory;
            deviceItem.Tag = Tag;

            var instance = Read<Instance?>(Data.Models.Metadata.Device.InstanceKey);
            if (instance is not null)
                deviceItem[Data.Models.Metadata.Device.InstanceKey] = instance.GetInternalClone();

            var extensions = Read<Extension[]?>(Data.Models.Metadata.Device.ExtensionKey);
            if (extensions is not null)
            {
                Data.Models.Metadata.Extension[] extensionItems = Array.ConvertAll(extensions, extension => extension.GetInternalClone());
                deviceItem[Data.Models.Metadata.Device.ExtensionKey] = extensionItems;
            }

            return deviceItem;
        }

        #endregion
    }
}
