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
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Driver;

        #endregion

        #region Constructors

        public Driver() : base() { }

        public Driver(Data.Models.Metadata.Driver item) : base(item)
        {
            // Process flag values
            if (ReadString(Data.Models.Metadata.Driver.CocktailKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.CocktailKey, ReadString(Data.Models.Metadata.Driver.CocktailKey).AsSupportStatus().AsStringValue());
            if (ReadString(Data.Models.Metadata.Driver.ColorKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.ColorKey, ReadString(Data.Models.Metadata.Driver.ColorKey).AsSupportStatus().AsStringValue());
            if (ReadString(Data.Models.Metadata.Driver.EmulationKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.EmulationKey, ReadString(Data.Models.Metadata.Driver.EmulationKey).AsSupportStatus().AsStringValue());
            if (ReadBool(Data.Models.Metadata.Driver.IncompleteKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.IncompleteKey, ReadBool(Data.Models.Metadata.Driver.IncompleteKey).FromYesNo());
            if (ReadBool(Data.Models.Metadata.Driver.NoSoundHardwareKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.NoSoundHardwareKey, ReadBool(Data.Models.Metadata.Driver.NoSoundHardwareKey).FromYesNo());
            if (ReadLong(Data.Models.Metadata.Driver.PaletteSizeKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.PaletteSizeKey, ReadLong(Data.Models.Metadata.Driver.PaletteSizeKey).ToString());
            if (ReadBool(Data.Models.Metadata.Driver.RequiresArtworkKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.RequiresArtworkKey, ReadBool(Data.Models.Metadata.Driver.RequiresArtworkKey).FromYesNo());
            if (ReadString(Data.Models.Metadata.Driver.SaveStateKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.SaveStateKey, ReadString(Data.Models.Metadata.Driver.SaveStateKey).AsSupported().AsStringValue(useSecond: true));
            if (ReadString(Data.Models.Metadata.Driver.SoundKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.SoundKey, ReadString(Data.Models.Metadata.Driver.SoundKey).AsSupportStatus().AsStringValue());
            if (ReadString(Data.Models.Metadata.Driver.StatusKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.StatusKey, ReadString(Data.Models.Metadata.Driver.StatusKey).AsSupportStatus().AsStringValue());
            if (ReadBool(Data.Models.Metadata.Driver.UnofficialKey) is not null)
                Write<string?>(Data.Models.Metadata.Driver.UnofficialKey, ReadBool(Data.Models.Metadata.Driver.UnofficialKey).FromYesNo());
        }

        public Driver(Data.Models.Metadata.Driver item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}
