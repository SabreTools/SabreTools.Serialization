using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single source details item
    /// </summary>
    [JsonObject("source_details"), XmlRoot("source_details")]
    public sealed class SourceDetails : DatItem<Data.Models.Metadata.SourceDetails>
    {
        #region Properties

        public string? AppendToNumber
        {
            get => _internal.AppendToNumber;
            set => _internal.AppendToNumber = value;
        }

        public string? Comment1
        {
            get => _internal.Comment1;
            set => _internal.Comment1 = value;
        }

        public string? Comment2
        {
            get => _internal.Comment2;
            set => _internal.Comment2 = value;
        }

        public string? DumpDate
        {
            get => _internal.DumpDate;
            set => _internal.DumpDate = value;
        }

        public bool? DumpDateInfo
        {
            get => _internal.DumpDateInfo;
            set => _internal.DumpDateInfo = value;
        }

        public string? Dumper
        {
            get => _internal.Dumper;
            set => _internal.Dumper = value;
        }

        public string? Id
        {
            get => _internal.Id;
            set => _internal.Id = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.SourceDetails;

        public string? Link1
        {
            get => _internal.Link1;
            set => _internal.Link1 = value;
        }

        public bool? Link1Public
        {
            get => _internal.Link1Public;
            set => _internal.Link1Public = value;
        }

        public string? Link2
        {
            get => _internal.Link2;
            set => _internal.Link2 = value;
        }

        public bool? Link2Public
        {
            get => _internal.Link2Public;
            set => _internal.Link2Public = value;
        }

        public string? Link3
        {
            get => _internal.Link3;
            set => _internal.Link3 = value;
        }

        public bool? Link3Public
        {
            get => _internal.Link3Public;
            set => _internal.Link3Public = value;
        }

        public string? MediaTitle
        {
            get => _internal.MediaTitle;
            set => _internal.MediaTitle = value;
        }

        public bool? Nodump
        {
            get => _internal.Nodump;
            set => _internal.Nodump = value;
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

        public string? Project
        {
            get => _internal.Project;
            set => _internal.Project = value;
        }

        public string? Region
        {
            get => _internal.Region;
            set => _internal.Region = value;
        }

        public string? ReleaseDate
        {
            get => _internal.ReleaseDate;
            set => _internal.ReleaseDate = value;
        }

        public bool? ReleaseDateInfo
        {
            get => _internal.ReleaseDateInfo;
            set => _internal.ReleaseDateInfo = value;
        }

        public string? RomInfo
        {
            get => _internal.RomInfo;
            set => _internal.RomInfo = value;
        }

        public string? Section
        {
            get => _internal.Section;
            set => _internal.Section = value;
        }

        public string? Tool
        {
            get => _internal.Tool;
            set => _internal.Tool = value;
        }

        #endregion

        #region Constructors

        public SourceDetails() : base() { }

        public SourceDetails(Data.Models.Metadata.SourceDetails item) : base(item) { }

        public SourceDetails(Data.Models.Metadata.SourceDetails item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new SourceDetails(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.SourceDetails GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.SourceDetails ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is SourceDetails otherSourceDetails)
                return _internal.Equals(otherSourceDetails._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.SourceDetails>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is SourceDetails otherSourceDetails)
                return _internal.Equals(otherSourceDetails._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
