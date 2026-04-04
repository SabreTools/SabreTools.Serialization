using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single release details item
    /// </summary>
    [JsonObject("release_details"), XmlRoot("release_details")]
    public sealed class ReleaseDetails : DatItem<Data.Models.Metadata.ReleaseDetails>
    {
        #region Fields

        public string? AppendToNumber
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.AppendToNumber;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.AppendToNumber = value;
        }

        public string? ArchiveName
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.ArchiveName;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.ArchiveName = value;
        }

        public string? Category
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.Category;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.Category = value;
        }

        public string? Comment
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.Comment;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.Comment = value;
        }

        public string? Date
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.Date;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.Date = value;
        }

        public string? DirName
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.DirName;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.DirName = value;
        }

        public string? Group
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.Group;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.Group = value;
        }

        public string? Id
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.Id;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.Id = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.ReleaseDetails;

        public string? NfoCRC
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.NfoCRC;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.NfoCRC = value;
        }

        public string? NfoName
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.NfoName;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.NfoName = value;
        }

        public string? NfoSize
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.NfoSize;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.NfoSize = value;
        }

        public string? Origin
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.Origin;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.Origin = value;
        }

        public string? OriginalFormat
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.OriginalFormat;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.OriginalFormat = value;
        }

        public string? Region
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.Region;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.Region = value;
        }

        public string? RomInfo
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.RomInfo;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.RomInfo = value;
        }

        public string? Tool
        {
            get => (_internal as Data.Models.Metadata.ReleaseDetails)?.Tool;
            set => (_internal as Data.Models.Metadata.ReleaseDetails)?.Tool = value;
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
        public override object Clone() => new ReleaseDetails(_internal.DeepClone() as Data.Models.Metadata.ReleaseDetails ?? []);

        #endregion
    }
}
