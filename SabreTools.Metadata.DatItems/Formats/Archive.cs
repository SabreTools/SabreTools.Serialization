using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents generic archive files to be included in a set
    /// </summary>
    [JsonObject("archive"), XmlRoot("archive")]
    public sealed class Archive : DatItem<Data.Models.Metadata.Archive>
    {
        #region Constructors

        public Archive() : base() { }

        public Archive(Data.Models.Metadata.Archive item) : base(item) { }

        public Archive(Data.Models.Metadata.Archive item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Archive(_internal.Clone() as Data.Models.Metadata.Archive ?? []);

        #endregion
    }
}
