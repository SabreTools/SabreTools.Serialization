using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents generic archive files to be included in a set
    /// </summary>
    [JsonObject("archive"), XmlRoot("archive")]
    public sealed class Archive : DatItem<Data.Models.Metadata.Archive>
    {
        #region Properties

        public string? Additional
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Additional;
            set => (_internal as Data.Models.Metadata.Archive)?.Additional = value;
        }

        public bool? Adult
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Adult;
            set => (_internal as Data.Models.Metadata.Archive)?.Adult = value;
        }

        public bool? Alt
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Alt;
            set => (_internal as Data.Models.Metadata.Archive)?.Alt = value;
        }

        public bool? Bios
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Bios;
            set => (_internal as Data.Models.Metadata.Archive)?.Bios = value;
        }

        public string? Categories
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Categories;
            set => (_internal as Data.Models.Metadata.Archive)?.Categories = value;
        }

        public string? CloneTag
        {
            get => (_internal as Data.Models.Metadata.Archive)?.CloneTag;
            set => (_internal as Data.Models.Metadata.Archive)?.CloneTag = value;
        }

        public bool? Complete
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Complete;
            set => (_internal as Data.Models.Metadata.Archive)?.Complete = value;
        }

        public bool? Dat
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Dat;
            set => (_internal as Data.Models.Metadata.Archive)?.Dat = value;
        }

        public string? DatterNote
        {
            get => (_internal as Data.Models.Metadata.Archive)?.DatterNote;
            set => (_internal as Data.Models.Metadata.Archive)?.DatterNote = value;
        }
        public string? Description
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Description;
            set => (_internal as Data.Models.Metadata.Archive)?.Description = value;
        }

        public string? DevStatus
        {
            get => (_internal as Data.Models.Metadata.Archive)?.DevStatus;
            set => (_internal as Data.Models.Metadata.Archive)?.DevStatus = value;
        }

        public string? GameId1
        {
            get => (_internal as Data.Models.Metadata.Archive)?.GameId1;
            set => (_internal as Data.Models.Metadata.Archive)?.GameId1 = value;
        }

        public string? GameId2
        {
            get => (_internal as Data.Models.Metadata.Archive)?.GameId2;
            set => (_internal as Data.Models.Metadata.Archive)?.GameId2 = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Archive;

        public string? LangChecked
        {
            get => (_internal as Data.Models.Metadata.Archive)?.LangChecked;
            set => (_internal as Data.Models.Metadata.Archive)?.LangChecked = value;
        }

        public string? Languages
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Languages;
            set => (_internal as Data.Models.Metadata.Archive)?.Languages = value;
        }

        public bool? Licensed
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Licensed;
            set => (_internal as Data.Models.Metadata.Archive)?.Licensed = value;
        }

        public bool? Listed
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Listed;
            set => (_internal as Data.Models.Metadata.Archive)?.Listed = value;
        }

        public string? MergeOf
        {
            get => (_internal as Data.Models.Metadata.Archive)?.MergeOf;
            set => (_internal as Data.Models.Metadata.Archive)?.MergeOf = value;
        }

        public string? MergeName
        {
            get => (_internal as Data.Models.Metadata.Archive)?.MergeName;
            set => (_internal as Data.Models.Metadata.Archive)?.MergeName = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Name;
            set => (_internal as Data.Models.Metadata.Archive)?.Name = value;
        }

        public string? NameAlt
        {
            get => (_internal as Data.Models.Metadata.Archive)?.NameAlt;
            set => (_internal as Data.Models.Metadata.Archive)?.NameAlt = value;
        }

        public string? Number
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Number;
            set => (_internal as Data.Models.Metadata.Archive)?.Number = value;
        }

        public bool? Physical
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Physical;
            set => (_internal as Data.Models.Metadata.Archive)?.Physical = value;
        }

        public bool? Pirate
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Pirate;
            set => (_internal as Data.Models.Metadata.Archive)?.Pirate = value;
        }

        public bool? Private
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Private;
            set => (_internal as Data.Models.Metadata.Archive)?.Private = value;
        }

        public string? Region
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Region;
            set => (_internal as Data.Models.Metadata.Archive)?.Region = value;
        }

        public string? RegParent
        {
            get => (_internal as Data.Models.Metadata.Archive)?.RegParent;
            set => (_internal as Data.Models.Metadata.Archive)?.RegParent = value;
        }

        public bool? ShowLang
        {
            get => (_internal as Data.Models.Metadata.Archive)?.ShowLang;
            set => (_internal as Data.Models.Metadata.Archive)?.ShowLang = value;
        }

        public string? Special1
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Special1;
            set => (_internal as Data.Models.Metadata.Archive)?.Special1 = value;
        }

        public string? Special2
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Special2;
            set => (_internal as Data.Models.Metadata.Archive)?.Special2 = value;
        }

        public string? StickyNote
        {
            get => (_internal as Data.Models.Metadata.Archive)?.StickyNote;
            set => (_internal as Data.Models.Metadata.Archive)?.StickyNote = value;
        }

        public string? Version1
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Version1;
            set => (_internal as Data.Models.Metadata.Archive)?.Version1 = value;
        }

        public string? Version2
        {
            get => (_internal as Data.Models.Metadata.Archive)?.Version2;
            set => (_internal as Data.Models.Metadata.Archive)?.Version2 = value;
        }

        #endregion

        #region Constructors

        public Archive() : base() { }

        public Archive(Data.Models.Metadata.Archive item) : base(item) { }

        public Archive(Data.Models.Metadata.Archive item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Archive(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Archive GetInternalClone()
            => (_internal as Data.Models.Metadata.Archive)?.Clone() as Data.Models.Metadata.Archive ?? [];

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Archive otherArchive)
                return ((Data.Models.Metadata.Archive)_internal).Equals((Data.Models.Metadata.Archive)otherArchive._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Archive>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Archive otherArchive)
                return ((Data.Models.Metadata.Archive)_internal).Equals((Data.Models.Metadata.Archive)otherArchive._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
