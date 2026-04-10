using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Metadata.Filter;

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

        public Extension[]? Extension { get; set; }

        [JsonIgnore]
        public bool ExtensionSpecified => Extension is not null && Extension.Length > 0;

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

        public Device(Data.Models.Metadata.Device item) : base(item)
        {
            // Handle subitems
            if (item.Extension is not null)
                Extension = Array.ConvertAll(item.Extension, extension => new Extension(extension));
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
            var deviceItem = _internal.Clone() as Data.Models.Metadata.Device ?? new();

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
                return _internal.Equals(otherDevice._internal);

            // Everything else fails
            return false;
        }

        #endregion

        #region Manipulation

        /// <inheritdoc/>
        public override bool PassesFilter(FilterRunner filterRunner)
        {
            if (Machine is not null && !Machine.PassesFilter(filterRunner))
                return false;

            // TODO: Extension

            return filterRunner.Run(_internal);
        }

        /// <inheritdoc/>
        public override bool PassesFilterDB(FilterRunner filterRunner)
        {
            // TODO: Extension

            return filterRunner.Run(_internal);
        }

        #endregion
    }
}
