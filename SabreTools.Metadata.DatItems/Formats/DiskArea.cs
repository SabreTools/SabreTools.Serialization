using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// SoftwareList diskarea information
    /// </summary>
    /// <remarks>One DiskArea can contain multiple Disk items</remarks>
    [JsonObject("diskarea"), XmlRoot("diskarea")]
    public sealed class DiskArea : DatItem<Data.Models.Metadata.DiskArea>
    {
        #region Fields

        /// <inheritdoc>/>
        internal override Data.Models.Metadata.ItemType ItemType => Data.Models.Metadata.ItemType.DiskArea;

        #endregion

        #region Constructors

        public DiskArea() : base() { }

        public DiskArea(Data.Models.Metadata.DiskArea item) : base(item) { }

        public DiskArea(Data.Models.Metadata.DiskArea item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new DiskArea(_internal.Clone() as Data.Models.Metadata.DiskArea ?? []);

        #endregion
    }
}
