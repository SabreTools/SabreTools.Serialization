using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a blank set from an input DAT
    /// </summary>
    [JsonObject("blank"), XmlRoot("blank")]
    public sealed class Blank : DatItem<Data.Models.Metadata.Blank>
    {
        #region Fields

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Blank;

        #endregion

        #region Constructors

        public Blank() : base() { }

        public Blank(Data.Models.Metadata.Blank item) : base(item) { }

        public Blank(Data.Models.Metadata.Blank item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => null;

        /// <inheritdoc/>
        public override void SetName(string? name) { }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Blank(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Blank GetInternalClone()
            => (_internal as Data.Models.Metadata.Blank)?.Clone() as Data.Models.Metadata.Blank ?? [];

        #endregion
    }
}
