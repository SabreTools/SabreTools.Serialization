using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a matchable extension
    /// </summary>
    [JsonObject("extension"), XmlRoot("extension")]
    public sealed class Extension : DatItem<Data.Models.Metadata.Extension>
    {
        #region Fields

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Extension;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Extension)?.Name;
            set => (_internal as Data.Models.Metadata.Extension)?.Name = value;
        }

        #endregion

        #region Constructors

        public Extension() : base() { }

        public Extension(Data.Models.Metadata.Extension item) : base(item) { }

        public Extension(Data.Models.Metadata.Extension item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Extension(_internal.DeepClone() as Data.Models.Metadata.Extension ?? []);

        #endregion
    }
}
