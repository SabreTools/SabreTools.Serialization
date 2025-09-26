namespace SabreTools.Serialization.Models.Charts
{
    /// <see href="https://github.com/TheNathannator/GuitarGame_ChartFormats/blob/main/doc/FileFormats/Other/Frets%20on%20Fire%20X/Careers.md"/> 
    /// <remarks>[titles]</remarks>
    public class TitlesIni
    {
        /// <summary>
        /// A space-separated list of .ini sections to include in the career.
        /// </summary>
        /// <remarks>sections</remarks>
        public string[]? SectionList { get; set; }

        /// <summary>
        /// This entry points to other sections that should be used as part of the career.
        /// </summary>
        public Tier[]? Sections { get; set; }
    }
}