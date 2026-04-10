using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// PartList part information
    /// </summary>
    /// <remarks>One Part can contain multiple PartFeature, DataArea, DiskArea, and DipSwitch items</remarks>
    [JsonObject("part"), XmlRoot("part")]
    public sealed class Part : DatItem<Data.Models.Metadata.Part>
    {
        #region Properties

        public string? Interface
        {
            get => _internal.Interface;
            set => _internal.Interface = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Part;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        #endregion

        #region Constructors

        public Part() : base() { }

        public Part(Data.Models.Metadata.Part item) : base(item)
        {
            _internal = item.Clone() as Data.Models.Metadata.Part ?? new();

            // Clear all lists
            _internal.DataArea = null;
            _internal.DiskArea = null;
            _internal.DipSwitch = null;
            _internal.Feature = null;
        }

        public Part(Data.Models.Metadata.Part item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Part(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Part GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Part ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Part otherPart)
                return _internal.Equals(otherPart._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
