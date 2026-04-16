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
        #region Properties

        public string? Filter
        {
            get => _internal.Filter;
            set => _internal.Filter = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.SoftwareList;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public Data.Models.Metadata.SoftwareListStatus? Status
        {
            get => _internal.Status;
            set => _internal.Status = value;
        }

        public string? Tag
        {
            get => _internal.Tag;
            set => _internal.Tag = value;
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

        public SoftwareList(Data.Models.Metadata.SoftwareList item, long machineIndex, long sourceIndex) : this(item)
        {
            SourceIndex = sourceIndex;
            MachineIndex = machineIndex;
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
            => _internal.Clone() as Data.Models.Metadata.SoftwareList ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is SoftwareList otherSoftwareList)
                return _internal.Equals(otherSoftwareList._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
