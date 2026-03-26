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

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Device;

        [JsonIgnore]
        public bool InstancesSpecified
        {
            get
            {
                var instances = Read<Instance[]?>(Data.Models.Metadata.Device.InstanceKey);
                return instances is not null && instances.Length > 0;
            }
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

        #endregion

        #region Constructors

        public Device() : base() { }

        public Device(Data.Models.Metadata.Device item) : base(item)
        {
            // Process flag values
            if (ReadBool(Data.Models.Metadata.Device.MandatoryKey) is not null)
                Write<string?>(Data.Models.Metadata.Device.MandatoryKey, ReadBool(Data.Models.Metadata.Device.MandatoryKey).FromYesNo());
            if (ReadString(Data.Models.Metadata.Device.DeviceTypeKey) is not null)
                Write<string?>(Data.Models.Metadata.Device.DeviceTypeKey, ReadString(Data.Models.Metadata.Device.DeviceTypeKey).AsDeviceType().AsStringValue());

            // Handle subitems
            var instance = item.Read<Data.Models.Metadata.Instance>(Data.Models.Metadata.Device.InstanceKey);
            if (instance is not null)
                Write<Instance?>(Data.Models.Metadata.Device.InstanceKey, new Instance(instance));

            var extensions = item.ReadItemArray<Data.Models.Metadata.Extension>(Data.Models.Metadata.Device.ExtensionKey);
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
        public override Data.Models.Metadata.Device GetInternalClone()
        {
            var deviceItem = base.GetInternalClone();

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
