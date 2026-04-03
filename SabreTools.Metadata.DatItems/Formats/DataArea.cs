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

        public DataArea(Data.Models.Metadata.DataArea item) : base(item) { }

        public DataArea(Data.Models.Metadata.DataArea item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new DataArea(_internal.DeepClone() as Data.Models.Metadata.DataArea ?? []);

        #endregion
    }
}
