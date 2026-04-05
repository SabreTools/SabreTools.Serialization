using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// SoftwareList dataarea information
    /// </summary>
    /// <remarks>One DataArea can contain multiple Rom items</remarks>
    [JsonObject("dataarea"), XmlRoot("dataarea")]
    public sealed class DataArea : DatItem<Data.Models.Metadata.DataArea>
    {
        #region Properties

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
        public override object Clone() => new DataArea(GetInternalClone());

        public override Data.Models.Metadata.DataArea GetInternalClone()
        {
            var partItem = (_internal as Data.Models.Metadata.DataArea)?.Clone() as Data.Models.Metadata.DataArea ?? [];

            if (Rom is not null)
                partItem.Rom = Array.ConvertAll(Rom, rom => rom.GetInternalClone());

            return partItem;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is DataArea otherDataArea)
                return ((Data.Models.Metadata.DataArea)_internal).Equals((Data.Models.Metadata.DataArea)otherDataArea._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Data.Models.Metadata.DatItem>? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is DataArea otherDataArea)
                return ((Data.Models.Metadata.DataArea)_internal).Equals((Data.Models.Metadata.DataArea)otherDataArea._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is DataArea otherDataArea)
                return ((Data.Models.Metadata.DataArea)_internal).Equals((Data.Models.Metadata.DataArea)otherDataArea._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.DataArea>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is DataArea otherDataArea)
                return ((Data.Models.Metadata.DataArea)_internal).Equals((Data.Models.Metadata.DataArea)otherDataArea._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
