using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents the a driver of the machine
    /// </summary>
    [JsonObject("driver"), XmlRoot("driver")]
    public sealed class Driver : DatItem<Data.Models.Metadata.Driver>
    {
        #region Fields

        public bool? Incomplete
        {
            get => (_internal as Data.Models.Metadata.Driver)?.Incomplete;
            set => (_internal as Data.Models.Metadata.Driver)?.Incomplete = value;
        }

        public bool? NoSoundHardware
        {
            get => (_internal as Data.Models.Metadata.Driver)?.NoSoundHardware;
            set => (_internal as Data.Models.Metadata.Driver)?.NoSoundHardware = value;
        }

        public bool? RequiresArtwork
        {
            get => (_internal as Data.Models.Metadata.Driver)?.RequiresArtwork;
            set => (_internal as Data.Models.Metadata.Driver)?.RequiresArtwork = value;
        }

        public bool? Unofficial
        {
            get => (_internal as Data.Models.Metadata.Driver)?.Unofficial;
            set => (_internal as Data.Models.Metadata.Driver)?.Unofficial = value;
        }

        #endregion

        #region Constructors

        public Driver() : base() { }

        public Driver(Data.Models.Metadata.Driver item) : base(item)
        {
            // Process flag values
            string? cocktail = ReadString(Data.Models.Metadata.Driver.CocktailKey);
            if (cocktail is not null)
                Write<string?>(Data.Models.Metadata.Driver.CocktailKey, cocktail.AsSupportStatus()?.AsStringValue());

            string? color = ReadString(Data.Models.Metadata.Driver.ColorKey);
            if (color is not null)
                Write<string?>(Data.Models.Metadata.Driver.ColorKey, color.AsSupportStatus()?.AsStringValue());

            string? emulation = ReadString(Data.Models.Metadata.Driver.EmulationKey);
            if (emulation is not null)
                Write<string?>(Data.Models.Metadata.Driver.EmulationKey, emulation.AsSupportStatus()?.AsStringValue());

            long? paletteSize = ReadLong(Data.Models.Metadata.Driver.PaletteSizeKey);
            if (paletteSize is not null)
                Write<string?>(Data.Models.Metadata.Driver.PaletteSizeKey, paletteSize.ToString());

            string? saveState = ReadString(Data.Models.Metadata.Driver.SaveStateKey);
            if (saveState is not null)
                Write<string?>(Data.Models.Metadata.Driver.SaveStateKey, saveState.AsSupported()?.AsStringValue(useSecond: true));

            string? sound = ReadString(Data.Models.Metadata.Driver.SoundKey);
            if (sound is not null)
                Write<string?>(Data.Models.Metadata.Driver.SoundKey, sound.AsSupportStatus()?.AsStringValue());

            string? status = ReadString(Data.Models.Metadata.Driver.StatusKey);
            if (status is not null)
                Write<string?>(Data.Models.Metadata.Driver.StatusKey, status.AsSupportStatus()?.AsStringValue());
        }

        public Driver(Data.Models.Metadata.Driver item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Driver(_internal.Clone() as Data.Models.Metadata.Driver ?? []);

        #endregion
    }
}
