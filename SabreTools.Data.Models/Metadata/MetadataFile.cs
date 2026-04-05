
namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of a full metadata file
    /// </summary>
    /// TODO: ICloneable
    /// TODO: IComparable<MetadataFile>
    public class MetadataFile
    {
        public Header? Header { get; set; }

        public InfoSource? InfoSource { get; set; }

        public Machine[]? Machine { get; set; }
    }
}
