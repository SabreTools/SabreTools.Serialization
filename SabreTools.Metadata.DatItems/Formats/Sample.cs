using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a (usually WAV-formatted) sample to be included for use in the set
    /// </summary>
    [JsonObject("sample"), XmlRoot("sample")]
    public class Sample : DatItem<Data.Models.Metadata.Sample>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Sample;

        #endregion

        #region Constructors

        public Sample() : base() { }

        public Sample(Data.Models.Metadata.Sample item) : base(item) { }

        public Sample(Data.Models.Metadata.Sample item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Sample(_internal.Clone() as Data.Models.Metadata.Sample ?? []);

        #endregion
    }
}
