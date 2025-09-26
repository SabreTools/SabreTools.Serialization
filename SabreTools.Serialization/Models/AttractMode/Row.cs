namespace SabreTools.Serialization.Models.AttractMode
{
    public class Row
    {
        /// <remarks>Also called Romname</remarks>
        [Required]
        public string? Name { get; set; }

        public string? Title { get; set; }

        public string? Emulator { get; set; }

        public string? CloneOf { get; set; }

        public string? Year { get; set; }

        public string? Manufacturer { get; set; }

        public string? Category { get; set; }

        public string? Players { get; set; }

        public string? Rotation { get; set; }

        public string? Control { get; set; }

        public string? Status { get; set; }

        public string? DisplayCount { get; set; }

        public string? DisplayType { get; set; }

        public string? AltRomname { get; set; }

        public string? AltTitle { get; set; }

        public string? Extra { get; set; }

        public string? Buttons { get; set; }

        public string? Favorite { get; set; }

        public string? Tags { get; set; }

        public string? PlayedCount { get; set; }

        public string? PlayedTime { get; set; }

        public string? FileIsAvailable { get; set; }
    }
}