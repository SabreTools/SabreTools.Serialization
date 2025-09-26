using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.SeparatedValue
{
    public class MetadataFile
    {
        [Required]
        public string[]? Header { get; set; }

        public Row[]? Row { get; set; }
    }
}