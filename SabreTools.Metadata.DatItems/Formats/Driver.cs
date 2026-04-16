using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents the a driver of the machine
    /// </summary>
    [JsonObject("driver"), XmlRoot("driver")]
    public sealed class Driver : DatItem<Data.Models.Metadata.Driver>
    {
        #region Properties

        public Data.Models.Metadata.Blit? Blit
        {
            get => _internal.Blit;
            set => _internal.Blit = value;
        }

        public Data.Models.Metadata.SupportStatus? Cocktail
        {
            get => _internal.Cocktail;
            set => _internal.Cocktail = value;
        }

        public Data.Models.Metadata.SupportStatus? Color
        {
            get => _internal.Color;
            set => _internal.Color = value;
        }

        public Data.Models.Metadata.SupportStatus? Emulation
        {
            get => _internal.Emulation;
            set => _internal.Emulation = value;
        }

        public bool? Incomplete
        {
            get => _internal.Incomplete;
            set => _internal.Incomplete = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Driver;

        public bool? NoSoundHardware
        {
            get => _internal.NoSoundHardware;
            set => _internal.NoSoundHardware = value;
        }

        public string? PaletteSize
        {
            get => _internal.PaletteSize;
            set => _internal.PaletteSize = value;
        }

        public bool? RequiresArtwork
        {
            get => _internal.RequiresArtwork;
            set => _internal.RequiresArtwork = value;
        }

        public Data.Models.Metadata.Supported? SaveState
        {
            get => _internal.SaveState;
            set => _internal.SaveState = value;
        }

        public Data.Models.Metadata.SupportStatus? Sound
        {
            get => _internal.Sound;
            set => _internal.Sound = value;
        }

        public Data.Models.Metadata.SupportStatus? Status
        {
            get => _internal.Status;
            set => _internal.Status = value;
        }

        public bool? Unofficial
        {
            get => _internal.Unofficial;
            set => _internal.Unofficial = value;
        }

        #endregion

        #region Constructors

        public Driver() : base() { }

        public Driver(Data.Models.Metadata.Driver item) : base(item) { }

        public Driver(Data.Models.Metadata.Driver item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        public Driver(Data.Models.Metadata.Driver item, long machineIndex, long sourceIndex) : this(item)
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
        public override object Clone() => new Driver(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Driver GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Driver ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Driver otherDriver)
                return _internal.Equals(otherDriver._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}
