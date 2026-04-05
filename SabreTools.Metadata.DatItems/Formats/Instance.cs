using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single instance of another item
    /// </summary>
    [JsonObject("instance"), XmlRoot("instance")]
    public sealed class Instance : DatItem<Data.Models.Metadata.Instance>
    {
        #region Properties

        public string? BriefName
        {
            get => _internal.BriefName;
            set => _internal.BriefName = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Instance;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
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

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Instance(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Instance GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Instance ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Instance otherInstance)
                return _internal.Equals(otherInstance._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Instance>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Instance otherInstance)
                return _internal.Equals(otherInstance._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
