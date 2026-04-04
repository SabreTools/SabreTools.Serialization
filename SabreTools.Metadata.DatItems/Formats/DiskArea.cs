using System;
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

        public Disk[]? Disk { get; set; }

        [JsonIgnore]
        public bool DiskSpecified => Disk is not null && Disk.Length > 0;

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DiskArea;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.DiskArea)?.Name;
            set => (_internal as Data.Models.Metadata.DiskArea)?.Name = value;
        }

        #endregion

        #region Constructors

        public DiskArea() : base() { }

        public DiskArea(Data.Models.Metadata.DiskArea item) : base(item)
        {
            // Handle subitems
            if (item.Disk is not null)
                Disk = Array.ConvertAll(item.Disk, rom => new Disk(rom));
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
        public override object Clone() => new DiskArea(_internal.DeepClone() as Data.Models.Metadata.DiskArea ?? []);

        public override Data.Models.Metadata.DiskArea GetInternalClone()
        {
            var partItem = base.GetInternalClone();

            if (Disk is not null)
                partItem.Disk = Array.ConvertAll(Disk, rom => rom.GetInternalClone());

            return partItem;
        }

        #endregion
    }
}
