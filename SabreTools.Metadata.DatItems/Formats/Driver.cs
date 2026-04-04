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
            get => (_internal as Data.Models.Metadata.Driver)?.Blit;
            set => (_internal as Data.Models.Metadata.Driver)?.Blit = value;
        }

        public Data.Models.Metadata.SupportStatus? Cocktail
        {
            get => (_internal as Data.Models.Metadata.Driver)?.Cocktail;
            set => (_internal as Data.Models.Metadata.Driver)?.Cocktail = value;
        }

        public Data.Models.Metadata.SupportStatus? Color
        {
            get => (_internal as Data.Models.Metadata.Driver)?.Color;
            set => (_internal as Data.Models.Metadata.Driver)?.Color = value;
        }

        public Data.Models.Metadata.SupportStatus? Emulation
        {
            get => (_internal as Data.Models.Metadata.Driver)?.Emulation;
            set => (_internal as Data.Models.Metadata.Driver)?.Emulation = value;
        }

        public bool? Incomplete
        {
            get => (_internal as Data.Models.Metadata.Driver)?.Incomplete;
            set => (_internal as Data.Models.Metadata.Driver)?.Incomplete = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Driver;

        public bool? NoSoundHardware
        {
            get => (_internal as Data.Models.Metadata.Driver)?.NoSoundHardware;
            set => (_internal as Data.Models.Metadata.Driver)?.NoSoundHardware = value;
        }

        public string? PaletteSize
        {
            get => (_internal as Data.Models.Metadata.Driver)?.PaletteSize;
            set => (_internal as Data.Models.Metadata.Driver)?.PaletteSize = value;
        }

        public bool? RequiresArtwork
        {
            get => (_internal as Data.Models.Metadata.Driver)?.RequiresArtwork;
            set => (_internal as Data.Models.Metadata.Driver)?.RequiresArtwork = value;
        }

        public Data.Models.Metadata.Supported? SaveState
        {
            get => (_internal as Data.Models.Metadata.Driver)?.SaveState;
            set => (_internal as Data.Models.Metadata.Driver)?.SaveState = value;
        }

        public Data.Models.Metadata.SupportStatus? Sound
        {
            get => (_internal as Data.Models.Metadata.Driver)?.Sound;
            set => (_internal as Data.Models.Metadata.Driver)?.Sound = value;
        }

        public Data.Models.Metadata.SupportStatus? Status
        {
            get => (_internal as Data.Models.Metadata.Driver)?.Status;
            set => (_internal as Data.Models.Metadata.Driver)?.Status = value;
        }

        public bool? Unofficial
        {
            get => (_internal as Data.Models.Metadata.Driver)?.Unofficial;
            set => (_internal as Data.Models.Metadata.Driver)?.Unofficial = value;
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
            => (_internal as Data.Models.Metadata.Driver)?.Clone() as Data.Models.Metadata.Driver ?? [];

        #endregion
    }
}
