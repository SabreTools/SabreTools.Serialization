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
        #region Constructors

        public Instance() : base() { }

        public Instance(Data.Models.Metadata.Instance item) : base(item) { }

        public Instance(Data.Models.Metadata.Instance item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Instance(_internal.Clone() as Data.Models.Metadata.Instance ?? []);

        #endregion
    }
}
