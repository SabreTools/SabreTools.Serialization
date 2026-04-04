using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a (usually WAV-formatted) sample to be included for use in the set
    /// </summary>
    [JsonObject("sample"), XmlRoot("sample")]
    public class Sample : DatItem<Data.Models.Metadata.Sample>
    {
        #region Properties

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Sample;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Sample)?.Name;
            set => (_internal as Data.Models.Metadata.Sample)?.Name = value;
        }

        #endregion

        #region Constructors

        public Sample() : base() { }

        public Sample(Data.Models.Metadata.Sample item) : base(item) { }

        public Sample(Data.Models.Metadata.Sample item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Sample(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Sample GetInternalClone()
            => (_internal as Data.Models.Metadata.Sample)?.Clone() as Data.Models.Metadata.Sample ?? [];

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Sample>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Sample otherSample)
                return _internal.Equals(otherSample._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
