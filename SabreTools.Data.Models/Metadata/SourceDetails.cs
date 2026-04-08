using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("source_details"), XmlRoot("source_details")]
    public class SourceDetails : DatItem, ICloneable, IEquatable<SourceDetails>
    {
        #region Properties

        public string? AppendToNumber { get; set; }

        public string? Comment1 { get; set; }

        public string? Comment2 { get; set; }

        public string? DumpDate { get; set; }

        /// <remarks>(0|1) "0"</remarks>
        public bool? DumpDateInfo { get; set; }

        public string? Dumper { get; set; }

        public string? Id { get; set; }

        public string? Link1 { get; set; }

        /// <remarks>(0|1) "0"</remarks>
        public bool? Link1Public { get; set; }

        public string? Link2 { get; set; }

        /// <remarks>(0|1) "0"</remarks>
        public bool? Link2Public { get; set; }

        public string? Link3 { get; set; }

        /// <remarks>(0|1) "0"</remarks>
        public bool? Link3Public { get; set; }

        public string? MediaTitle { get; set; }

        /// <remarks>(0|1) "0"</remarks>
        public bool? Nodump { get; set; }

        public string? Origin { get; set; }

        public string? OriginalFormat { get; set; }

        public string? Project { get; set; }

        public string? Region { get; set; }

        public string? ReleaseDate { get; set; }

        /// <remarks>(0|1) "0"</remarks>
        public bool? ReleaseDateInfo { get; set; }

        public string? RomInfo { get; set; }

        public string? Section { get; set; }

        public string? Tool { get; set; }

        #endregion

        public SourceDetails() => ItemType = ItemType.SourceDetails;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new SourceDetails();

            obj.AppendToNumber = AppendToNumber;
            obj.Comment1 = Comment1;
            obj.Comment2 = Comment2;
            obj.DumpDate = DumpDate;
            obj.DumpDateInfo = DumpDateInfo;
            obj.Dumper = Dumper;
            obj.Id = Id;
            obj.Link1 = Link1;
            obj.Link1Public = Link1Public;
            obj.Link2 = Link2;
            obj.Link2Public = Link2Public;
            obj.Link3 = Link3;
            obj.Link3Public = Link3Public;
            obj.MediaTitle = MediaTitle;
            obj.Nodump = Nodump;
            obj.Origin = Origin;
            obj.OriginalFormat = OriginalFormat;
            obj.Project = Project;
            obj.Region = Region;
            obj.ReleaseDate = ReleaseDate;
            obj.ReleaseDateInfo = ReleaseDateInfo;
            obj.RomInfo = RomInfo;
            obj.Section = Section;
            obj.Tool = Tool;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(SourceDetails? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((AppendToNumber is null) ^ (other.AppendToNumber is null))
                return false;
            else if (AppendToNumber is not null && !AppendToNumber.Equals(other.AppendToNumber, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Comment1 is null) ^ (other.Comment1 is null))
                return false;
            else if (Comment1 is not null && !Comment1.Equals(other.Comment1, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Comment2 is null) ^ (other.Comment2 is null))
                return false;
            else if (Comment2 is not null && !Comment2.Equals(other.Comment2, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((DumpDate is null) ^ (other.DumpDate is null))
                return false;
            else if (DumpDate is not null && !DumpDate.Equals(other.DumpDate, StringComparison.OrdinalIgnoreCase))
                return false;

            if (DumpDateInfo != other.DumpDateInfo)
                return false;

            if ((Dumper is null) ^ (other.Dumper is null))
                return false;
            else if (Dumper is not null && !Dumper.Equals(other.Dumper, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Id is null) ^ (other.Id is null))
                return false;
            else if (Id is not null && !Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Link1 is null) ^ (other.Link1 is null))
                return false;
            else if (Link1 is not null && !Link1.Equals(other.Link1, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Link1Public != other.Link1Public)
                return false;

            if ((Link2 is null) ^ (other.Link2 is null))
                return false;
            else if (Link2 is not null && !Link2.Equals(other.Link2, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Link2Public != other.Link2Public)
                return false;

            if ((Link3 is null) ^ (other.Link3 is null))
                return false;
            else if (Link3 is not null && !Link3.Equals(other.Link3, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Link3Public != other.Link3Public)
                return false;

            if ((MediaTitle is null) ^ (other.MediaTitle is null))
                return false;
            else if (MediaTitle is not null && !MediaTitle.Equals(other.MediaTitle, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Nodump != other.Nodump)
                return false;

            if ((Origin is null) ^ (other.Origin is null))
                return false;
            else if (Origin is not null && !Origin.Equals(other.Origin, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((OriginalFormat is null) ^ (other.OriginalFormat is null))
                return false;
            else if (OriginalFormat is not null && !OriginalFormat.Equals(other.OriginalFormat, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Project is null) ^ (other.Project is null))
                return false;
            else if (Project is not null && !Project.Equals(other.Project, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Region is null) ^ (other.Region is null))
                return false;
            else if (Region is not null && !Region.Equals(other.Region, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((ReleaseDate is null) ^ (other.ReleaseDate is null))
                return false;
            else if (ReleaseDate is not null && !ReleaseDate.Equals(other.ReleaseDate, StringComparison.OrdinalIgnoreCase))
                return false;

            if (ReleaseDateInfo != other.ReleaseDateInfo)
                return false;

            if ((RomInfo is null) ^ (other.RomInfo is null))
                return false;
            else if (RomInfo is not null && !RomInfo.Equals(other.RomInfo, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Section is null) ^ (other.Section is null))
                return false;
            else if (Section is not null && !Section.Equals(other.Section, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Tool is null) ^ (other.Tool is null))
                return false;
            else if (Tool is not null && !Tool.Equals(other.Tool, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
