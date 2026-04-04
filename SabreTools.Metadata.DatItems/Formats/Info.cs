using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents special information about a machine
    /// </summary>
    [JsonObject("info"), XmlRoot("info")]
    public sealed class Info : DatItem<Data.Models.Metadata.Info>
    {
        #region Properties

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Info;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Info)?.Name;
            set => (_internal as Data.Models.Metadata.Info)?.Name = value;
        }

        public string? Value
        {
            get => (_internal as Data.Models.Metadata.Info)?.Value;
            set => (_internal as Data.Models.Metadata.Info)?.Value = value;
        }

        #endregion

        #region Constructors

        public Info() : base() { }

        public Info(Data.Models.Metadata.Info item) : base(item) { }

        public Info(Data.Models.Metadata.Info item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Info(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Info GetInternalClone()
            => (_internal as Data.Models.Metadata.Info)?.Clone() as Data.Models.Metadata.Info ?? [];

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Info otherInfo)
                return ((Data.Models.Metadata.Info)_internal).Equals((Data.Models.Metadata.Info)otherInfo._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Info>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Info otherInfo)
                return ((Data.Models.Metadata.Info)_internal).Equals((Data.Models.Metadata.Info)otherInfo._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
