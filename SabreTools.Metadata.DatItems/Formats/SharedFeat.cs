using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one shared feature object
    /// </summary>
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public sealed class SharedFeat : DatItem<Data.Models.Metadata.SharedFeat>
    {
        #region Fields

        /// <inheritdoc>/>
        internal override Data.Models.Metadata.ItemType ItemType => Data.Models.Metadata.ItemType.SharedFeat;

        #endregion

        #region Constructors

        public SharedFeat() : base() { }

        public SharedFeat(Data.Models.Metadata.SharedFeat item) : base(item) { }

        public SharedFeat(Data.Models.Metadata.SharedFeat item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new SharedFeat(_internal.Clone() as Data.Models.Metadata.SharedFeat ?? []);

        #endregion
    }
}
