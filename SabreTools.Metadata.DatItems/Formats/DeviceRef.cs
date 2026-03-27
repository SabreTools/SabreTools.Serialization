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
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.DeviceRef;

        #endregion

        #region Constructors

        public DeviceRef() : base() { }

        public DeviceRef(Data.Models.Metadata.DeviceRef item) : base(item) { }

        public DeviceRef(Data.Models.Metadata.DeviceRef item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new DeviceRef(_internal.Clone() as Data.Models.Metadata.DeviceRef ?? []);

        #endregion
    }
}
