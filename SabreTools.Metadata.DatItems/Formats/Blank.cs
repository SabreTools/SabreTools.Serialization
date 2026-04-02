using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a blank set from an input DAT
    /// </summary>
    [JsonObject("blank"), XmlRoot("blank")]
    public sealed class Blank : DatItem<Data.Models.Metadata.Blank>
    {
        #region Constructors

        public Blank() : base() { }

        public Blank(Data.Models.Metadata.Blank item) : base(item) { }

        public Blank(Data.Models.Metadata.Blank item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Blank(_internal.DeepClone() as Data.Models.Metadata.Blank ?? []);

        #endregion
    }
}
