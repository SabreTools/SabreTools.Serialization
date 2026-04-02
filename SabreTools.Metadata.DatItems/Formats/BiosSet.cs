using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which BIOS(es) is associated with a set
    /// </summary>
    [JsonObject("biosset"), XmlRoot("biosset")]
    public sealed class BiosSet : DatItem<Data.Models.Metadata.BiosSet>
    {
        #region Properties

        public bool? Default
        {
            get => (_internal as Data.Models.Metadata.BiosSet)?.Default;
            set => (_internal as Data.Models.Metadata.BiosSet)?.Default = value;
        }

        public string? Description
        {
            get => (_internal as Data.Models.Metadata.BiosSet)?.Description;
            set => (_internal as Data.Models.Metadata.BiosSet)?.Description = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.BiosSet)?.Name;
            set => (_internal as Data.Models.Metadata.BiosSet)?.Name = value;
        }

        #endregion

        #region Constructors

        public BiosSet() : base() { }

        public BiosSet(Data.Models.Metadata.BiosSet item) : base(item) { }

        public BiosSet(Data.Models.Metadata.BiosSet item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new BiosSet(_internal.DeepClone() as Data.Models.Metadata.BiosSet ?? []);

        #endregion
    }
}
