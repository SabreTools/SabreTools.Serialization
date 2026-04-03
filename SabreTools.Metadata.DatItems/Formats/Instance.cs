using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single instance of another item
    /// </summary>
    [JsonObject("instance"), XmlRoot("instance")]
    public sealed class Instance : DatItem<Data.Models.Metadata.Instance>
    {
        #region Fields

        public string? BriefName
        {
            get => (_internal as Data.Models.Metadata.Instance)?.BriefName;
            set => (_internal as Data.Models.Metadata.Instance)?.BriefName = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Instance;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Instance)?.Name;
            set => (_internal as Data.Models.Metadata.Instance)?.Name = value;
        }

        #endregion

        #region Constructors

        public Instance() : base() { }

        public Instance(Data.Models.Metadata.Instance item) : base(item) { }

        public Instance(Data.Models.Metadata.Instance item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Instance(_internal.DeepClone() as Data.Models.Metadata.Instance ?? []);

        #endregion
    }
}
