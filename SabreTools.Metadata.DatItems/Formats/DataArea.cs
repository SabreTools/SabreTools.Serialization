using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// SoftwareList dataarea information
    /// </summary>
    /// <remarks>One DataArea can contain multiple Rom items</remarks>
    [JsonObject("dataarea"), XmlRoot("dataarea")]
    public sealed class DataArea : DatItem<Data.Models.Metadata.DataArea>
    {
        #region Fields

        public Data.Models.Metadata.Endianness? Endianness
        {
            get => (_internal as Data.Models.Metadata.DataArea)?.Endianness;
            set => (_internal as Data.Models.Metadata.DataArea)?.Endianness = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DataArea;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.DataArea)?.Name;
            set => (_internal as Data.Models.Metadata.DataArea)?.Name = value;
        }

        public Rom[]? Rom { get; set; }

        [JsonIgnore]
        public bool RomSpecified => Rom is not null && Rom.Length > 0;

        public long? Size
        {
            get => (_internal as Data.Models.Metadata.DataArea)?.Size;
            set => (_internal as Data.Models.Metadata.DataArea)?.Size = value;
        }

        public Data.Models.Metadata.Width? Width
        {
            get => (_internal as Data.Models.Metadata.DataArea)?.Width;
            set => (_internal as Data.Models.Metadata.DataArea)?.Width = value;
        }

        #endregion

        #region Constructors

        public DataArea() : base() { }

        public DataArea(Data.Models.Metadata.DataArea item) : base(item)
        {
            // Handle subitems
            if (item.Rom is not null)
                Rom = Array.ConvertAll(item.Rom, rom => new Rom(rom));
        }

        public DataArea(Data.Models.Metadata.DataArea item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new DataArea(_internal.DeepClone() as Data.Models.Metadata.DataArea ?? []);

        public override Data.Models.Metadata.DataArea GetInternalClone()
        {
            var partItem = base.GetInternalClone();

            if (Rom is not null)
                partItem.Rom = Array.ConvertAll(Rom, rom => rom.GetInternalClone());

            return partItem;
        }

        #endregion
    }
}
