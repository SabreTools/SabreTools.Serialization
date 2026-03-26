using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which BIOS(es) is associated with a set
    /// </summary>
    [JsonObject("biosset"), XmlRoot("biosset")]
    public sealed class BiosSet : DatItem<Data.Models.Metadata.BiosSet>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.BiosSet;

        #endregion

        #region Constructors

        public BiosSet() : base() { }

        public BiosSet(Data.Models.Metadata.BiosSet item) : base(item)
        {
            // Process flag values
            if (ReadBool(Data.Models.Metadata.BiosSet.DefaultKey) is not null)
                Write<string?>(Data.Models.Metadata.BiosSet.DefaultKey, ReadBool(Data.Models.Metadata.BiosSet.DefaultKey).FromYesNo());
        }

        public BiosSet(Data.Models.Metadata.BiosSet item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
