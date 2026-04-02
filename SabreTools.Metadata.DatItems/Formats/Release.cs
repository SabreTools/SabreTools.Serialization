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

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.Release)?.Default;
            set => (_internal as Data.Models.Metadata.Release)?.Default = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Release)?.Name;
            set => (_internal as Data.Models.Metadata.Release)?.Name = value;
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

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Release(_internal.DeepClone() as Data.Models.Metadata.Release ?? []);

        #endregion
    }
}
