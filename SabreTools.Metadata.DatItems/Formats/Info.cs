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
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Info(_internal.Clone() as Data.Models.Metadata.Info ?? []);

        #endregion
    }
}
