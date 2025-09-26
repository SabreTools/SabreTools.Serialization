namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// MD2 File
    /// </summary>
    public class MD2
    {
        [SabreTools.Models.Required]
        public string? Hash { get; set; }

        [SabreTools.Models.Required]
        public string? File { get; set; }
    }
}