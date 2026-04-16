using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single serials item
    /// </summary>
    [JsonObject("serials"), XmlRoot("serials")]
    public sealed class Serials : DatItem<Data.Models.Metadata.Serials>
    {
        #region Properties

        public string? BoxBarcode
        {
            get => _internal.BoxBarcode;
            set => _internal.BoxBarcode = value;
        }

        public string? BoxSerial
        {
            get => _internal.BoxSerial;
            set => _internal.BoxSerial = value;
        }

        public string? ChipSerial
        {
            get => _internal.ChipSerial;
            set => _internal.ChipSerial = value;
        }

        public string? DigitalSerial1
        {
            get => _internal.DigitalSerial1;
            set => _internal.DigitalSerial1 = value;
        }

        public string? DigitalSerial2
        {
            get => _internal.DigitalSerial2;
            set => _internal.DigitalSerial2 = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Serials;

        public string? LockoutSerial
        {
            get => _internal.LockoutSerial;
            set => _internal.LockoutSerial = value;
        }

        public string? MediaSerial1
        {
            get => _internal.MediaSerial1;
            set => _internal.MediaSerial1 = value;
        }

        public string? MediaSerial2
        {
            get => _internal.MediaSerial2;
            set => _internal.MediaSerial2 = value;
        }

        public string? MediaSerial3
        {
            get => _internal.MediaSerial3;
            set => _internal.MediaSerial3 = value;
        }

        public string? MediaStamp
        {
            get => _internal.MediaStamp;
            set => _internal.MediaStamp = value;
        }

        public string? PCBSerial
        {
            get => _internal.PCBSerial;
            set => _internal.PCBSerial = value;
        }

        public string? RomChipSerial1
        {
            get => _internal.RomChipSerial1;
            set => _internal.RomChipSerial1 = value;
        }

        public string? RomChipSerial2
        {
            get => _internal.RomChipSerial2;
            set => _internal.RomChipSerial2 = value;
        }

        public string? SaveChipSerial
        {
            get => _internal.SaveChipSerial;
            set => _internal.SaveChipSerial = value;
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

        public Serials(Data.Models.Metadata.Serials item, long machineIndex, long sourceIndex) : this(item)
        {
            SourceIndex = sourceIndex;
            MachineIndex = machineIndex;
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
        public override object Clone() => new Serials(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Serials GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Serials ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Serials otherSerials)
                return _internal.Equals(otherSerials._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
