using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which SoftwareList(s) is associated with a set
    /// </summary>
    [JsonObject("softwarelist"), XmlRoot("softwarelist")]
    public sealed class SoftwareList : DatItem<Data.Models.Metadata.SoftwareList>
    {
        #region Fields

        public string? Description
        {
            get => (_internal as Data.Models.Metadata.SoftwareList)?.Description;
            set => (_internal as Data.Models.Metadata.SoftwareList)?.Description = value;
        }

        public Data.Models.Metadata.SoftwareListStatus? Status
        {
            get => (_internal as Data.Models.Metadata.SoftwareList)?.Status;
            set => (_internal as Data.Models.Metadata.SoftwareList)?.Status = value;
        }

        public string? Tag
        {
            get => (_internal as Data.Models.Metadata.SoftwareList)?.Tag;
            set => (_internal as Data.Models.Metadata.SoftwareList)?.Tag = value;
        }

        #endregion

        #region Constructors

        public SoftwareList() : base() { }

        public SoftwareList(Data.Models.Metadata.SoftwareList item) : base(item)
        {
            // Handle subitems
            // TODO: Handle the Software subitem
        }

        public SoftwareList(Data.Models.Metadata.SoftwareList item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new SoftwareList(_internal.Clone() as Data.Models.Metadata.SoftwareList ?? []);

        #endregion
    }
}
