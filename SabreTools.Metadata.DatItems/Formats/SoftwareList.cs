using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which SoftwareList(s) is associated with a set
    /// </summary>
    [JsonObject("softwarelist"), XmlRoot("softwarelist")]
    public sealed class SoftwareList : DatItem<Data.Models.Metadata.SoftwareList>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.SoftwareList;

        #endregion

        #region Constructors

        public SoftwareList() : base() { }

        public SoftwareList(Data.Models.Metadata.SoftwareList item) : base(item)
        {
            // Process flag values
            if (ReadString(Data.Models.Metadata.SoftwareList.StatusKey) is not null)
                Write<string?>(Data.Models.Metadata.SoftwareList.StatusKey, ReadString(Data.Models.Metadata.SoftwareList.StatusKey).AsSoftwareListStatus().AsStringValue());

            // Handle subitems
            // TODO: Handle the Software subitem
        }

        public SoftwareList(Data.Models.Metadata.SoftwareList item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
