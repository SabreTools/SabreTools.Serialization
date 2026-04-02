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

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.DataArea)?.Name;
            set => (_internal as Data.Models.Metadata.DataArea)?.Name = value;
        }

        #endregion

        #region Constructors

        public DataArea() : base() { }

        public DataArea(Data.Models.Metadata.DataArea item) : base(item)
        {
            // Process flag values
            long? size = ReadLong(Data.Models.Metadata.DataArea.SizeKey);
            if (size is not null)
                Write<string?>(Data.Models.Metadata.DataArea.SizeKey, size.ToString());

            long? width = ReadLong(Data.Models.Metadata.DataArea.WidthKey);
            if (width is not null)
                Write<string?>(Data.Models.Metadata.DataArea.WidthKey, width.ToString());
        }

        public DataArea(Data.Models.Metadata.DataArea item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new DataArea(_internal.DeepClone() as Data.Models.Metadata.DataArea ?? []);

        #endregion
    }
}
