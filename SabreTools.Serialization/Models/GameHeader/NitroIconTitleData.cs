namespace SabreTools.Data.Models.GameHeader
{
    /// <summary>
    /// Icon/Title Data section for an NDS cart image
    /// </summary>
    public sealed class NitroIconTitleData
    {
        public ushort IconVersion { get; set; }

        public ushort IconCRC16 { get; set; }

        public string? IconCRCInfo { get; set; }

        public string? JapaneseTitle { get; set; }

        public string? EnglishTitle { get; set; }

        public string? FrenchTitle { get; set; }

        public string? GermanTitle { get; set; }

        public string? SpanishTitle { get; set; }

        public string? ItalianTitle { get; set; }
    }
}