using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single release details item
    /// </summary>
    [JsonObject("release_details"), XmlRoot("release_details")]
    public sealed class ReleaseDetails : DatItem<Data.Models.Metadata.ReleaseDetails>
    {
        #region Properties

        public string? AppendToNumber
        {
            get => _internal.AppendToNumber;
            set => _internal.AppendToNumber = value;
        }

        public string? ArchiveName
        {
            get => _internal.ArchiveName;
            set => _internal.ArchiveName = value;
        }

        public string? Category
        {
            get => _internal.Category;
            set => _internal.Category = value;
        }

        public string? Comment
        {
            get => _internal.Comment;
            set => _internal.Comment = value;
        }

        public string? Date
        {
            get => _internal.Date;
            set => _internal.Date = value;
        }

        public string? DirName
        {
            get => _internal.DirName;
            set => _internal.DirName = value;
        }

        public string? Group
        {
            get => _internal.Group;
            set => _internal.Group = value;
        }

        public string? Id
        {
            get => _internal.Id;
            set => _internal.Id = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.ReleaseDetails;

        public string? NfoCRC
        {
            get => _internal.NfoCRC;
            set => _internal.NfoCRC = value;
        }

        public string? NfoName
        {
            get => _internal.NfoName;
            set => _internal.NfoName = value;
        }

        public string? NfoSize
        {
            get => _internal.NfoSize;
            set => _internal.NfoSize = value;
        }

        public string? Origin
        {
            get => _internal.Origin;
            set => _internal.Origin = value;
        }

        public string? OriginalFormat
        {
            get => _internal.OriginalFormat;
            set => _internal.OriginalFormat = value;
        }

        public string? Region
        {
            get => _internal.Region;
            set => _internal.Region = value;
        }

        public string? RomInfo
        {
            get => _internal.RomInfo;
            set => _internal.RomInfo = value;
        }

        public string? Tool
        {
            get => _internal.Tool;
            set => _internal.Tool = value;
        }

        #endregion

        #region Constructors

        public ReleaseDetails() : base() { }

        public ReleaseDetails(Data.Models.Metadata.ReleaseDetails item) : base(item) { }

        public ReleaseDetails(Data.Models.Metadata.ReleaseDetails item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }
        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => null;

        /// <inheritdoc/>
        public override void SetName(string? name) { }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new ReleaseDetails(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.ReleaseDetails GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.ReleaseDetails ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is ReleaseDetails otherReleaseDetails)
                return _internal.Equals(otherReleaseDetails._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
