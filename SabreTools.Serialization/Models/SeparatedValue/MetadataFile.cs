namespace SabreTools.Serialization.Models.SeparatedValue
{
    public class MetadataFile
    {
        [SabreTools.Models.Required]
        public string[]? Header { get; set; }

        public Row[]? Row { get; set; }
    }
}