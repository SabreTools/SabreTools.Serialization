namespace SabreTools.Serialization.Models.DosCenter
{
    /// <remarks>game</remarks>
    public class Game
    {
        /// <remarks>name</remarks>
        [SabreTools.Models.Required]
        public string? Name { get; set; }

        /// <remarks>file</remarks>
        public File[]? File { get; set; }
    }
}