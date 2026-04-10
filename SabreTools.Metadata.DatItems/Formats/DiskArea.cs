using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// SoftwareList diskarea information
    /// </summary>
    /// <remarks>One DiskArea can contain multiple Disk items</remarks>
    [JsonObject("diskarea"), XmlRoot("diskarea")]
    public sealed class DiskArea : DatItem<Data.Models.Metadata.DiskArea>
    {
        #region Properties

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DiskArea;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        #endregion

        #region Constructors

        public DiskArea() : base() { }

        public DiskArea(Data.Models.Metadata.DiskArea item) : base(item)
        {
            _internal = item.Clone() as Data.Models.Metadata.DiskArea ?? new();

            // Clear all lists
            _internal.Disk = null;
        }

        public DiskArea(Data.Models.Metadata.DiskArea item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new DiskArea(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.DiskArea GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.DiskArea ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is DiskArea otherDiskArea)
                return _internal.Equals(otherDiskArea._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
