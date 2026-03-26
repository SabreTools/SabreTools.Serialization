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
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Release;

        #endregion

        #region Constructors

        public Release() : base() { }

        public Release(Data.Models.Metadata.Release item) : base(item)
        {
            // Process flag values
            bool? defaultValue = ReadBool(Data.Models.Metadata.Release.DefaultKey);
            if (defaultValue is not null)
                Write<string?>(Data.Models.Metadata.Release.DefaultKey, defaultValue.FromYesNo());
        }

        public Release(Data.Models.Metadata.Release item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
