using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one shared feature object
    /// </summary>
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public sealed class SharedFeat : DatItem<Data.Models.Metadata.SharedFeat>
    {
        #region Properties

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.SharedFeat;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.SharedFeat)?.Name;
            set => (_internal as Data.Models.Metadata.SharedFeat)?.Name = value;
        }

        public string? Value
        {
            get => (_internal as Data.Models.Metadata.SharedFeat)?.Value;
            set => (_internal as Data.Models.Metadata.SharedFeat)?.Value = value;
        }

        #endregion

        #region Constructors

        public SharedFeat() : base() { }

        public SharedFeat(Data.Models.Metadata.SharedFeat item) : base(item) { }

        public SharedFeat(Data.Models.Metadata.SharedFeat item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new SharedFeat(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.SharedFeat GetInternalClone()
            => (_internal as Data.Models.Metadata.SharedFeat)?.Clone() as Data.Models.Metadata.SharedFeat ?? [];

        #endregion
    }
}
