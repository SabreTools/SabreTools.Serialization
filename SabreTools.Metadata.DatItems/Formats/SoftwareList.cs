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

        public string? Filter
        {
            get => (_internal as Data.Models.Metadata.SoftwareList)?.Filter;
            set => (_internal as Data.Models.Metadata.SoftwareList)?.Filter = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.SoftwareList;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.SoftwareList)?.Name;
            set => (_internal as Data.Models.Metadata.SoftwareList)?.Name = value;
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

        public SoftwareList(Data.Models.Metadata.SoftwareList item) : base(item) { }

        public SoftwareList(Data.Models.Metadata.SoftwareList item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new SoftwareList(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.SoftwareList GetInternalClone()
            => (_internal as Data.Models.Metadata.SoftwareList)?.Clone() as Data.Models.Metadata.SoftwareList ?? [];

        #endregion
    }
}
