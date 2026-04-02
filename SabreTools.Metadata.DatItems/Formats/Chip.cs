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

        [JsonIgnore]
        public bool? SoundOnly
        {
            get => (_internal as Data.Models.Metadata.Chip)?.SoundOnly;
            set => (_internal as Data.Models.Metadata.Chip)?.SoundOnly = value;
        }

        #endregion

        #region Constructors

        public Chip() : base() { }

        public Chip(Data.Models.Metadata.Chip item) : base(item)
        {
            // Process flag values
            string? chipType = ReadString(Data.Models.Metadata.Chip.ChipTypeKey);
            if (chipType is not null)
                Write<string?>(Data.Models.Metadata.Chip.ChipTypeKey, chipType.AsChipType()?.AsStringValue());
        }

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
