namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// MD2 File
    /// </summary>
    public class MD2
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}