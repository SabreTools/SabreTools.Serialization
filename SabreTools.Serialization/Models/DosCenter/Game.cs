using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.DosCenter
{
    /// <remarks>game</remarks>
    public class Game
    {
        /// <remarks>name</remarks>
        [Required]
        public string? Name { get; set; }

        /// <remarks>file</remarks>
        public File[]? File { get; set; }
    }
}