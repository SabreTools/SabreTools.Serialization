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
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.DataArea;

        #endregion

        #region Constructors

        public DataArea() : base() { }

        public DataArea(Data.Models.Metadata.DataArea item) : base(item)
        {
            // Process flag values
            if (ReadString(Data.Models.Metadata.DataArea.EndiannessKey) is not null)
                Write<string?>(Data.Models.Metadata.DataArea.EndiannessKey, ReadString(Data.Models.Metadata.DataArea.EndiannessKey).AsEndianness().AsStringValue());
            if (ReadLong(Data.Models.Metadata.DataArea.SizeKey) is not null)
                Write<string?>(Data.Models.Metadata.DataArea.SizeKey, ReadLong(Data.Models.Metadata.DataArea.SizeKey).ToString());
            if (ReadLong(Data.Models.Metadata.DataArea.WidthKey) is not null)
                Write<string?>(Data.Models.Metadata.DataArea.WidthKey, ReadLong(Data.Models.Metadata.DataArea.WidthKey).ToString());
        }

        public DataArea(Data.Models.Metadata.DataArea item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
