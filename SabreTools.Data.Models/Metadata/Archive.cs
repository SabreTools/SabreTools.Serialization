using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("archive"), XmlRoot("archive")]
    public class Archive : DatItem, ICloneable, IEquatable<Archive>
    {
        #region Properties

        /// <remarks>No-Intro DB extension</remarks>
        public string? Additional { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? Adult { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? Alt { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? Bios { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? Categories { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? CloneTag { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? Complete { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? Dat { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? DatterNote { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? Description { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? DevStatus { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? GameId1 { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? GameId2 { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? LangChecked { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? Languages { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? Licensed { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? Listed { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? MergeOf { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? MergeName { get; set; }

        public string? Name { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? NameAlt { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? Number { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? Physical { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? Pirate { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? Private { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? Region { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? RegParent { get; set; }

        /// <remarks>(0|1) "0", No-Intro DB extension</remarks>
        public bool? ShowLang { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? Special1 { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? Special2 { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? StickyNote { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? Version1 { get; set; }

        /// <remarks>No-Intro DB extension</remarks>
        public string? Version2 { get; set; }

        #endregion

        public Archive() => ItemType = ItemType.Archive;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Archive();

            obj.Additional = Additional;
            obj.Adult = Adult;
            obj.Alt = Alt;
            obj.Bios = Bios;
            obj.Categories = Categories;
            obj.CloneTag = CloneTag;
            obj.Complete = Complete;
            obj.Dat = Dat;
            obj.DatterNote = DatterNote;
            obj.Description = Description;
            obj.DevStatus = DevStatus;
            obj.GameId1 = GameId1;
            obj.GameId2 = GameId2;
            obj.LangChecked = LangChecked;
            obj.Languages = Languages;
            obj.Licensed = Licensed;
            obj.Listed = Listed;
            obj.MergeOf = MergeOf;
            obj.MergeName = MergeName;
            obj.Name = Name;
            obj.NameAlt = NameAlt;
            obj.Number = Number;
            obj.Physical = Physical;
            obj.Pirate = Pirate;
            obj.Private = Private;
            obj.Region = Region;
            obj.RegParent = RegParent;
            obj.ShowLang = ShowLang;
            obj.Special1 = Special1;
            obj.Special2 = Special2;
            obj.StickyNote = StickyNote;
            obj.Version1 = Version1;
            obj.Version2 = Version2;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Archive? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Additional is null) ^ (other.Additional is null))
                return false;
            else if (Additional is not null && !Additional.Equals(other.Additional, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Adult != other.Adult)
                return false;

            if (Alt != other.Alt)
                return false;

            if (Bios != other.Bios)
                return false;

            if ((Categories is null) ^ (other.Categories is null))
                return false;
            else if (Categories is not null && !Categories.Equals(other.Categories, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((CloneTag is null) ^ (other.CloneTag is null))
                return false;
            else if (CloneTag is not null && !CloneTag.Equals(other.CloneTag, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Complete != other.Complete)
                return false;

            if (Dat != other.Dat)
                return false;

            if ((DatterNote is null) ^ (other.DatterNote is null))
                return false;
            else if (DatterNote is not null && !DatterNote.Equals(other.DatterNote, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Description is null) ^ (other.Description is null))
                return false;
            else if (Description is not null && !Description.Equals(other.Description, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((DevStatus is null) ^ (other.DevStatus is null))
                return false;
            else if (DevStatus is not null && !DevStatus.Equals(other.DevStatus, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((GameId1 is null) ^ (other.GameId1 is null))
                return false;
            else if (GameId1 is not null && !GameId1.Equals(other.GameId1, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((GameId2 is null) ^ (other.GameId2 is null))
                return false;
            else if (GameId2 is not null && !GameId2.Equals(other.GameId2, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((LangChecked is null) ^ (other.LangChecked is null))
                return false;
            else if (LangChecked is not null && !LangChecked.Equals(other.LangChecked, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Languages is null) ^ (other.Languages is null))
                return false;
            else if (Languages is not null && !Languages.Equals(other.Languages, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Licensed != other.Licensed)
                return false;

            if (Listed != other.Listed)
                return false;

            if ((MergeOf is null) ^ (other.MergeOf is null))
                return false;
            else if (MergeOf is not null && !MergeOf.Equals(other.MergeOf, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((MergeName is null) ^ (other.MergeName is null))
                return false;
            else if (MergeName is not null && !MergeName.Equals(other.MergeName, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((NameAlt is null) ^ (other.NameAlt is null))
                return false;
            else if (NameAlt is not null && !NameAlt.Equals(other.NameAlt, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Number is null) ^ (other.Number is null))
                return false;
            else if (Number is not null && !Number.Equals(other.Number, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Physical != other.Physical)
                return false;

            if (Pirate != other.Pirate)
                return false;

            if (Private != other.Private)
                return false;

            if ((Region is null) ^ (other.Region is null))
                return false;
            else if (Region is not null && !Region.Equals(other.Region, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((RegParent is null) ^ (other.RegParent is null))
                return false;
            else if (RegParent is not null && !RegParent.Equals(other.RegParent, StringComparison.OrdinalIgnoreCase))
                return false;

            if (ShowLang != other.ShowLang)
                return false;

            if ((Special1 is null) ^ (other.Special1 is null))
                return false;
            else if (Special1 is not null && !Special1.Equals(other.Special1, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Special2 is null) ^ (other.Special2 is null))
                return false;
            else if (Special2 is not null && !Special2.Equals(other.Special2, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((StickyNote is null) ^ (other.StickyNote is null))
                return false;
            else if (StickyNote is not null && !StickyNote.Equals(other.StickyNote, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Version1 is null) ^ (other.Version1 is null))
                return false;
            else if (Version1 is not null && !Version1.Equals(other.Version1, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Version2 is null) ^ (other.Version2 is null))
                return false;
            else if (Version2 is not null && !Version2.Equals(other.Version2, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
