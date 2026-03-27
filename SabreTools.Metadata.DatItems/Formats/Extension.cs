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
        protected override ItemType ItemType => ItemType.Extension;

        #endregion

        #region Constructors

        public Extension() : base() { }

        public Extension(Data.Models.Metadata.Extension item) : base(item) { }

        public Extension(Data.Models.Metadata.Extension item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Extension(_internal.Clone() as Data.Models.Metadata.Extension ?? []);

        #endregion
    }
}
