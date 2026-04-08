using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("driver"), XmlRoot("driver")]
    public class Driver : DatItem, ICloneable, IEquatable<Driver>
    {
        #region Properties

        /// <remarks>(plain|dirty)</remarks>
        public Blit? Blit { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public SupportStatus? Cocktail { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public SupportStatus? Color { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public SupportStatus? Emulation { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Incomplete { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? NoSoundHardware { get; set; }

        /// <remarks>Possibly long</remarks>
        public string? PaletteSize { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? RequiresArtwork { get; set; }

        /// <remarks>(supported|unsupported)</remarks>
        public Supported? SaveState { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public SupportStatus? Sound { get; set; }

        /// <remarks>(good|imperfect|preliminary|test)</remarks>
        public SupportStatus? Status { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Unofficial { get; set; }

        #endregion

        public Driver() => ItemType = ItemType.Driver;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Driver();

            obj.Blit = Blit;
            obj.Cocktail = Cocktail;
            obj.Color = Color;
            obj.Emulation = Emulation;
            obj.Incomplete = Incomplete;
            obj.NoSoundHardware = NoSoundHardware;
            obj.PaletteSize = PaletteSize;
            obj.RequiresArtwork = RequiresArtwork;
            obj.SaveState = SaveState;
            obj.Sound = Sound;
            obj.Status = Status;
            obj.Unofficial = Unofficial;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Driver? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (Blit != other.Blit)
                return false;

            if (Cocktail != other.Cocktail)
                return false;

            if (Color != other.Color)
                return false;

            if (Emulation != other.Emulation)
                return false;

            if (Incomplete != other.Incomplete)
                return false;

            if (NoSoundHardware != other.NoSoundHardware)
                return false;

            if ((PaletteSize is null) ^ (other.PaletteSize is null))
                return false;
            else if (PaletteSize is not null && !PaletteSize.Equals(other.PaletteSize, StringComparison.OrdinalIgnoreCase))
                return false;

            if (RequiresArtwork != other.RequiresArtwork)
                return false;

            if (SaveState != other.SaveState)
                return false;

            if (Sound != other.Sound)
                return false;

            if (Status != other.Status)
                return false;

            if (Unofficial != other.Unofficial)
                return false;

            return true;
        }
    }
}
