using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Chip(s) is associated with a set
    /// </summary>
    [JsonObject("chip"), XmlRoot("chip")]
    public sealed class Chip : DatItem<Data.Models.Metadata.Chip>
    {
        #region Fields

        public Data.Models.Metadata.ChipType? ChipType
        {
            get => (_internal as Data.Models.Metadata.Chip)?.ChipType;
            set => (_internal as Data.Models.Metadata.Chip)?.ChipType = value;
        }

        public bool? SoundOnly
        {
            get => (_internal as Data.Models.Metadata.Chip)?.SoundOnly;
            set => (_internal as Data.Models.Metadata.Chip)?.SoundOnly = value;
        }

        public string? Tag
        {
            get => (_internal as Data.Models.Metadata.Chip)?.Tag;
            set => (_internal as Data.Models.Metadata.Chip)?.Tag = value;
        }

        #endregion

        #region Constructors

        public Chip() : base() { }

        public Chip(Data.Models.Metadata.Chip item) : base(item) { }

        public Chip(Data.Models.Metadata.Chip item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Chip(_internal.Clone() as Data.Models.Metadata.Chip ?? []);

        #endregion
    }
}
