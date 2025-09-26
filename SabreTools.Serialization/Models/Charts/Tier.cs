namespace SabreTools.Data.Models.Charts
{
    /// <see href="https://github.com/TheNathannator/GuitarGame_ChartFormats/blob/main/doc/FileFormats/Other/Frets%20on%20Fire%20X/Careers.md"/> 
    public class Tier
    {
        /// <summary>
        /// Display name of the tier.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Name used for associating a song with this tier, and for checking unlock requirements.
        /// </summary>
        public string? UnlockId { get; set; }
    }
}