using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single serials item
    /// </summary>
    [JsonObject("serials"), XmlRoot("serials")]
    public sealed class Serials : DatItem<Data.Models.Metadata.Serials>
    {
        #region Fields

        public string? BoxBarcode
        {
            get => (_internal as Data.Models.Metadata.Serials)?.BoxBarcode;
            set => (_internal as Data.Models.Metadata.Serials)?.BoxBarcode = value;
        }

        public string? BoxSerial
        {
            get => (_internal as Data.Models.Metadata.Serials)?.BoxSerial;
            set => (_internal as Data.Models.Metadata.Serials)?.BoxSerial = value;
        }

        public string? ChipSerial
        {
            get => (_internal as Data.Models.Metadata.Serials)?.ChipSerial;
            set => (_internal as Data.Models.Metadata.Serials)?.ChipSerial = value;
        }

        public string? DigitalSerial1
        {
            get => (_internal as Data.Models.Metadata.Serials)?.DigitalSerial1;
            set => (_internal as Data.Models.Metadata.Serials)?.DigitalSerial1 = value;
        }

        public string? DigitalSerial2
        {
            get => (_internal as Data.Models.Metadata.Serials)?.DigitalSerial2;
            set => (_internal as Data.Models.Metadata.Serials)?.DigitalSerial2 = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Serials;

        public string? LockoutSerial
        {
            get => (_internal as Data.Models.Metadata.Serials)?.LockoutSerial;
            set => (_internal as Data.Models.Metadata.Serials)?.LockoutSerial = value;
        }

        public string? MediaSerial1
        {
            get => (_internal as Data.Models.Metadata.Serials)?.MediaSerial1;
            set => (_internal as Data.Models.Metadata.Serials)?.MediaSerial1 = value;
        }

        public string? MediaSerial2
        {
            get => (_internal as Data.Models.Metadata.Serials)?.MediaSerial2;
            set => (_internal as Data.Models.Metadata.Serials)?.MediaSerial2 = value;
        }

        public string? MediaSerial3
        {
            get => (_internal as Data.Models.Metadata.Serials)?.MediaSerial3;
            set => (_internal as Data.Models.Metadata.Serials)?.MediaSerial3 = value;
        }

        public string? MediaStamp
        {
            get => (_internal as Data.Models.Metadata.Serials)?.MediaStamp;
            set => (_internal as Data.Models.Metadata.Serials)?.MediaStamp = value;
        }

        public string? PCBSerial
        {
            get => (_internal as Data.Models.Metadata.Serials)?.PCBSerial;
            set => (_internal as Data.Models.Metadata.Serials)?.PCBSerial = value;
        }

        public string? RomChipSerial1
        {
            get => (_internal as Data.Models.Metadata.Serials)?.RomChipSerial1;
            set => (_internal as Data.Models.Metadata.Serials)?.RomChipSerial1 = value;
        }

        public string? RomChipSerial2
        {
            get => (_internal as Data.Models.Metadata.Serials)?.RomChipSerial2;
            set => (_internal as Data.Models.Metadata.Serials)?.RomChipSerial2 = value;
        }

        public string? SaveChipSerial
        {
            get => (_internal as Data.Models.Metadata.Serials)?.SaveChipSerial;
            set => (_internal as Data.Models.Metadata.Serials)?.SaveChipSerial = value;
        }

        #endregion

        #region Constructors

        public Serials() : base() { }

        public Serials(Data.Models.Metadata.Serials item) : base(item) { }

        public Serials(Data.Models.Metadata.Serials item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Serials(_internal.DeepClone() as Data.Models.Metadata.Serials ?? []);

        #endregion
    }
}
