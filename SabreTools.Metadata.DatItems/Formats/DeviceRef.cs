using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Device Reference(s) is associated with a set
    /// </summary>
    [JsonObject("device_ref"), XmlRoot("device_ref")]
    public sealed class DeviceRef : DatItem<Data.Models.Metadata.DeviceRef>
    {
        #region Properties

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DeviceRef;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.DeviceRef)?.Name;
            set => (_internal as Data.Models.Metadata.DeviceRef)?.Name = value;
        }

        #endregion

        #region Constructors

        public DeviceRef() : base() { }

        public DeviceRef(Data.Models.Metadata.DeviceRef item) : base(item) { }

        public DeviceRef(Data.Models.Metadata.DeviceRef item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new DeviceRef(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.DeviceRef GetInternalClone()
            => (_internal as Data.Models.Metadata.DeviceRef)?.Clone() as Data.Models.Metadata.DeviceRef ?? [];

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.DeviceRef>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is DeviceRef otherDeviceRef)
                return _internal.Equals(otherDeviceRef._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
