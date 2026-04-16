using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents release information about a set
    /// </summary>
    [JsonObject("release"), XmlRoot("release")]
    public sealed class Release : DatItem<Data.Models.Metadata.Release>
    {
        #region Properties

        public string? Date
        {
            get => _internal.Date;
            set => _internal.Date = value;
        }

        public bool? Default
        {
            get => _internal.Default;
            set => _internal.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Release;

        public string? Language
        {
            get => _internal.Language;
            set => _internal.Language = value;
        }

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public string? Region
        {
            get => _internal.Region;
            set => _internal.Region = value;
        }

        #endregion

        #region Constructors

        public Release() : base() { }

        public Release(Data.Models.Metadata.Release item) : base(item) { }

        public Release(Data.Models.Metadata.Release item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        public Release(Data.Models.Metadata.Release item, long machineIndex, long sourceIndex) : this(item)
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
        public override object Clone() => new Release(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Release GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Release ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Release otherRelease)
                return _internal.Equals(otherRelease._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
