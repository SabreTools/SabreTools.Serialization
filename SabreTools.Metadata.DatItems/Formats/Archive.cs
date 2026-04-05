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
            get => _internal.Additional;
            set => _internal.Additional = value;
        }

        public bool? Adult
        {
            get => _internal.Adult;
            set => _internal.Adult = value;
        }

        public bool? Alt
        {
            get => _internal.Alt;
            set => _internal.Alt = value;
        }

        public bool? Bios
        {
            get => _internal.Bios;
            set => _internal.Bios = value;
        }

        public string? Categories
        {
            get => _internal.Categories;
            set => _internal.Categories = value;
        }

        public string? CloneTag
        {
            get => _internal.CloneTag;
            set => _internal.CloneTag = value;
        }

        public bool? Complete
        {
            get => _internal.Complete;
            set => _internal.Complete = value;
        }

        public bool? Dat
        {
            get => _internal.Dat;
            set => _internal.Dat = value;
        }

        public string? DatterNote
        {
            get => _internal.DatterNote;
            set => _internal.DatterNote = value;
        }
        public string? Description
        {
            get => _internal.Description;
            set => _internal.Description = value;
        }

        public string? DevStatus
        {
            get => _internal.DevStatus;
            set => _internal.DevStatus = value;
        }

        public string? GameId1
        {
            get => _internal.GameId1;
            set => _internal.GameId1 = value;
        }

        public string? GameId2
        {
            get => _internal.GameId2;
            set => _internal.GameId2 = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Archive;

        public string? LangChecked
        {
            get => _internal.LangChecked;
            set => _internal.LangChecked = value;
        }

        public string? Languages
        {
            get => _internal.Languages;
            set => _internal.Languages = value;
        }

        public bool? Licensed
        {
            get => _internal.Licensed;
            set => _internal.Licensed = value;
        }

        public bool? Listed
        {
            get => _internal.Listed;
            set => _internal.Listed = value;
        }

        public string? MergeOf
        {
            get => _internal.MergeOf;
            set => _internal.MergeOf = value;
        }

        public string? MergeName
        {
            get => _internal.MergeName;
            set => _internal.MergeName = value;
        }

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public string? NameAlt
        {
            get => _internal.NameAlt;
            set => _internal.NameAlt = value;
        }

        public string? Number
        {
            get => _internal.Number;
            set => _internal.Number = value;
        }

        public bool? Physical
        {
            get => _internal.Physical;
            set => _internal.Physical = value;
        }

        public bool? Pirate
        {
            get => _internal.Pirate;
            set => _internal.Pirate = value;
        }

        public bool? Private
        {
            get => _internal.Private;
            set => _internal.Private = value;
        }

        public string? Region
        {
            get => _internal.Region;
            set => _internal.Region = value;
        }

        public string? RegParent
        {
            get => _internal.RegParent;
            set => _internal.RegParent = value;
        }

        public bool? ShowLang
        {
            get => _internal.ShowLang;
            set => _internal.ShowLang = value;
        }

        public string? Special1
        {
            get => _internal.Special1;
            set => _internal.Special1 = value;
        }

        public string? Special2
        {
            get => _internal.Special2;
            set => _internal.Special2 = value;
        }

        public string? StickyNote
        {
            get => _internal.StickyNote;
            set => _internal.StickyNote = value;
        }

        public string? Version1
        {
            get => _internal.Version1;
            set => _internal.Version1 = value;
        }

        public string? Version2
        {
            get => _internal.Version2;
            set => _internal.Version2 = value;
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
            => _internal.Clone() as Data.Models.Metadata.Archive ?? new();

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
                return _internal.Equals(otherArchive._internal);

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
                return _internal.Equals(otherArchive._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
