using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

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
            get => (_internal as Data.Models.Metadata.Release)?.Date;
            set => (_internal as Data.Models.Metadata.Release)?.Date = value;
        }

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.Release)?.Default;
            set => (_internal as Data.Models.Metadata.Release)?.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Release;

        public string? Language
        {
            get => (_internal as Data.Models.Metadata.Release)?.Language;
            set => (_internal as Data.Models.Metadata.Release)?.Language = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Release)?.Name;
            set => (_internal as Data.Models.Metadata.Release)?.Name = value;
        }

        public string? Region
        {
            get => (_internal as Data.Models.Metadata.Release)?.Region;
            set => (_internal as Data.Models.Metadata.Release)?.Region = value;
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

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Release(_internal.DeepClone() as Data.Models.Metadata.Release ?? []);

        #endregion
    }
}
