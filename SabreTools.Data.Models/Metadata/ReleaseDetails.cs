using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("release_details"), XmlRoot("release_details")]
    public class ReleaseDetails : DatItem, ICloneable, IEquatable<ReleaseDetails>
    {
        #region Properties

        public string? AppendToNumber { get; set; }

        public string? ArchiveName { get; set; }

        public string? Category { get; set; }

        public string? Comment { get; set; }

        public string? Date { get; set; }

        public string? DirName { get; set; }

        public string? Group { get; set; }

        public string? Id { get; set; }

        public string? NfoCRC { get; set; }

        public string? NfoName { get; set; }

        public string? NfoSize { get; set; }

        public string? Origin { get; set; }

        public string? OriginalFormat { get; set; }

        public string? Region { get; set; }

        public string? RomInfo { get; set; }

        public string? Tool { get; set; }

        #endregion

        public ReleaseDetails() => ItemType = ItemType.ReleaseDetails;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new ReleaseDetails();

            obj.AppendToNumber = AppendToNumber;
            obj.ArchiveName = ArchiveName;
            obj.Category = Category;
            obj.Comment = Comment;
            obj.Date = Date;
            obj.DirName = DirName;
            obj.Group = Group;
            obj.Id = Id;
            obj.NfoCRC = NfoCRC;
            obj.NfoName = NfoName;
            obj.NfoSize = NfoSize;
            obj.Origin = Origin;
            obj.OriginalFormat = OriginalFormat;
            obj.Region = Region;
            obj.RomInfo = RomInfo;
            obj.Tool = Tool;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(ReleaseDetails? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((AppendToNumber is null) ^ (other.AppendToNumber is null))
                return false;
            else if (AppendToNumber is not null && !AppendToNumber.Equals(other.AppendToNumber, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((ArchiveName is null) ^ (other.ArchiveName is null))
                return false;
            else if (ArchiveName is not null && !ArchiveName.Equals(other.ArchiveName, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Category is null) ^ (other.Category is null))
                return false;
            else if (Category is not null && !Category.Equals(other.Category, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Comment is null) ^ (other.Comment is null))
                return false;
            else if (Comment is not null && !Comment.Equals(other.Comment, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Date is null) ^ (other.Date is null))
                return false;
            else if (Date is not null && !Date.Equals(other.Date, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((DirName is null) ^ (other.DirName is null))
                return false;
            else if (DirName is not null && !DirName.Equals(other.DirName, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Group is null) ^ (other.Group is null))
                return false;
            else if (Group is not null && !Group.Equals(other.Group, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Id is null) ^ (other.Id is null))
                return false;
            else if (Id is not null && !Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((NfoCRC is null) ^ (other.NfoCRC is null))
                return false;
            else if (NfoCRC is not null && !NfoCRC.Equals(other.NfoCRC, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((NfoName is null) ^ (other.NfoName is null))
                return false;
            else if (NfoName is not null && !NfoName.Equals(other.NfoName, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((NfoSize is null) ^ (other.NfoSize is null))
                return false;
            else if (NfoSize is not null && !NfoSize.Equals(other.NfoSize, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Origin is null) ^ (other.Origin is null))
                return false;
            else if (Origin is not null && !Origin.Equals(other.Origin, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((OriginalFormat is null) ^ (other.OriginalFormat is null))
                return false;
            else if (OriginalFormat is not null && !OriginalFormat.Equals(other.OriginalFormat, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Region is null) ^ (other.Region is null))
                return false;
            else if (Region is not null && !Region.Equals(other.Region, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((RomInfo is null) ^ (other.RomInfo is null))
                return false;
            else if (RomInfo is not null && !RomInfo.Equals(other.RomInfo, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Tool is null) ^ (other.Tool is null))
                return false;
            else if (Tool is not null && !Tool.Equals(other.Tool, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
