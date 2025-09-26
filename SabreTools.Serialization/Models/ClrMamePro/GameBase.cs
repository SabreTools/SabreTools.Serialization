using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.ClrMamePro
{
    /// <summary>
    /// Base class to unify the various game-like types
    /// </summary>
    public abstract class GameBase
    {
        /// <remarks>name</remarks>
        [Required]
        public string? Name { get; set; }

        /// <remarks>description</remarks>
        public string? Description { get; set; }

        /// <remarks>year</remarks>
        public string? Year { get; set; }

        /// <remarks>manufacturer</remarks>
        public string? Manufacturer { get; set; }

        /// <remarks>category</remarks>
        public string? Category { get; set; }

        /// <remarks>cloneof</remarks>
        public string? CloneOf { get; set; }

        /// <remarks>romof</remarks>
        public string? RomOf { get; set; }

        /// <remarks>sampleof</remarks>
        public string? SampleOf { get; set; }

        /// <remarks>release</remarks>
        public Release[]? Release { get; set; }

        /// <remarks>biosset</remarks>
        public BiosSet[]? BiosSet { get; set; }

        /// <remarks>rom</remarks>
        public Rom[]? Rom { get; set; }

        /// <remarks>disk</remarks>
        public Disk[]? Disk { get; set; }

        /// <remarks>sample</remarks>
        public Sample[]? Sample { get; set; }

        /// <remarks>archive</remarks>
        public Archive[]? Archive { get; set; }

        #region Aaru Extensions

        /// <remarks>media, Appears after Disk</remarks>
        public Media[]? Media { get; set; }

        #endregion

        #region MAME Extensions

        /// <remarks>chip, Appears after Archive</remarks>
        public Chip[]? Chip { get; set; }

        /// <remarks>video, Appears after Chip</remarks>
        public Video[]? Video { get; set; }

        /// <remarks>sound, Appears after Video</remarks>
        public Sound? Sound { get; set; }

        /// <remarks>input, Appears after Sound</remarks>
        public Input? Input { get; set; }

        /// <remarks>dipswitch, Appears after Input</remarks>
        public DipSwitch[]? DipSwitch { get; set; }

        /// <remarks>driver, Appears after DipSwitch</remarks>
        public Driver? Driver { get; set; }

        #endregion
    }
}