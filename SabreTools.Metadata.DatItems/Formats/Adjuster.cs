using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents which Adjuster(s) is associated with a set
    /// </summary>
    [JsonObject("adjuster"), XmlRoot("adjuster")]
    public sealed class Adjuster : DatItem<Data.Models.Metadata.Adjuster>
    {
        #region Properties

        public string? ConditionMask
        {
            get => _internal.ConditionMask;
            set => _internal.ConditionMask = value;
        }

        public Data.Models.Metadata.Relation? ConditionRelation
        {
            get => _internal.ConditionRelation;
            set => _internal.ConditionRelation = value;
        }

        public string? ConditionTag
        {
            get => _internal.ConditionTag;
            set => _internal.ConditionTag = value;
        }

        public string? ConditionValue
        {
            get => _internal.ConditionValue;
            set => _internal.ConditionValue = value;
        }

        public bool? Default
        {
            get => _internal.Default;
            set => _internal.Default = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Adjuster;

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        #endregion

        #region Constructors

        public Adjuster() : base() { }

        public Adjuster(Data.Models.Metadata.Adjuster item) : base(item) { }

        public Adjuster(Data.Models.Metadata.Adjuster item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Adjuster(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Adjuster GetInternalClone()
            =>_internal.Clone() as Data.Models.Metadata.Adjuster ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Adjuster otherAdjuster)
                return _internal.Equals(otherAdjuster._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
