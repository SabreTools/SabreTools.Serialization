using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single analog item
    /// </summary>
    [JsonObject("analog"), XmlRoot("analog")]
    public sealed class Analog : DatItem<Data.Models.Metadata.Analog>
    {
        #region Properties

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Analog;

        public string? Mask
        {
            get => _internal.Mask;
            set => _internal.Mask = value;
        }

        #endregion

        #region Constructors

        public Analog() : base() { }

        public Analog(Data.Models.Metadata.Analog item) : base(item) { }

        public Analog(Data.Models.Metadata.Analog item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Analog(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Analog GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Analog ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Analog otherAnalog)
                return _internal.Equals(otherAnalog._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
