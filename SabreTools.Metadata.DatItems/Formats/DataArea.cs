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
            get => _internal.Endianness;
            set => _internal.Endianness = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.DataArea;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public long? Size
        {
            get => _internal.Size;
            set => _internal.Size = value;
        }

        public Data.Models.Metadata.Width? Width
        {
            get => _internal.Width;
            set => _internal.Width = value;
        }

        #endregion

        #region Constructors

        public DataArea() : base() { }

        public DataArea(Data.Models.Metadata.DataArea item)
        {
            _internal = item.Clone() as Data.Models.Metadata.DataArea ?? new();

            // Clear all lists
            _internal.Rom = null;
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

        /// <inheritdoc/>
        public override Data.Models.Metadata.DataArea GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.DataArea ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is DataArea otherDataArea)
                return _internal.Equals(otherDataArea._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
