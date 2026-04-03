using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single source details item
    /// </summary>
    [JsonObject("source_details"), XmlRoot("source_details")]
    public sealed class SourceDetails : DatItem<Data.Models.Metadata.SourceDetails>
    {
        #region Fields

        public string? AppendToNumber
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.AppendToNumber;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.AppendToNumber = value;
        }

        public string? Comment1
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Comment1;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Comment1 = value;
        }

        public string? Comment2
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Comment2;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Comment2 = value;
        }

        public string? DumpDate
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.DumpDate;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.DumpDate = value;
        }

        public bool? DumpDateInfo
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.DumpDateInfo;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.DumpDateInfo = value;
        }

        public string? Dumper
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Dumper;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Dumper = value;
        }

        public string? Id
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Id;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Id = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.SourceDetails;

        public string? Link1
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Link1;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Link1 = value;
        }

        public bool? Link1Public
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Link1Public;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Link1Public = value;
        }

        public string? Link2
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Link2;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Link2 = value;
        }

        public bool? Link2Public
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Link2Public;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Link2Public = value;
        }

        public string? Link3
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Link3;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Link3 = value;
        }

        public bool? Link3Public
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Link3Public;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Link3Public = value;
        }

        public string? MediaTitle
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.MediaTitle;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.MediaTitle = value;
        }

        public bool? Nodump
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Nodump;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Nodump = value;
        }

        public string? Origin
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Origin;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Origin = value;
        }

        public string? OriginalFormat
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.OriginalFormat;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.OriginalFormat = value;
        }

        public string? Project
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Project;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Project = value;
        }

        public string? Region
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Region;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Region = value;
        }

        public string? ReleaseDate
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.ReleaseDate;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.ReleaseDate = value;
        }

        public bool? ReleaseDateInfo
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.ReleaseDateInfo;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.ReleaseDateInfo = value;
        }

        public string? RomInfo
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.RomInfo;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.RomInfo = value;
        }

        public string? Section
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Section;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Section = value;
        }

        public string? Tool
        {
            get => (_internal as Data.Models.Metadata.SourceDetails)?.Tool;
            set => (_internal as Data.Models.Metadata.SourceDetails)?.Tool = value;
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

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new SourceDetails(_internal.DeepClone() as Data.Models.Metadata.SourceDetails ?? []);

        #endregion
    }
}
