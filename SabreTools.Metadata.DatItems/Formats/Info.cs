using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents special information about a machine
    /// </summary>
    [JsonObject("info"), XmlRoot("info")]
    public sealed class Info : DatItem<Data.Models.Metadata.Info>
    {
        #region Fields

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
        public override object Clone() => new Info(_internal.DeepClone() as Data.Models.Metadata.Info ?? []);

        #endregion
    }
}
