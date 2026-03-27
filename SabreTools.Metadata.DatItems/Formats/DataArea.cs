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

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.DataArea;

        #endregion

        #region Constructors

        public DataArea() : base() { }

        public DataArea(Data.Models.Metadata.DataArea item) : base(item)
        {
            // Process flag values
            string? endianness = ReadString(Data.Models.Metadata.DataArea.EndiannessKey);
            if (endianness is not null)
                Write<string?>(Data.Models.Metadata.DataArea.EndiannessKey, endianness.AsEndianness()?.AsStringValue());

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
        public override object Clone() => new DataArea(_internal.Clone() as Data.Models.Metadata.DataArea ?? []);

        #endregion
    }
}
