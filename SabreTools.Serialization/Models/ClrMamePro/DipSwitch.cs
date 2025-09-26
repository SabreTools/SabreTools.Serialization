namespace SabreTools.Serialization.Models.ClrMamePro
{
    /// <remarks>dipswitch</remarks>
    public class DipSwitch
    {
        /// <remarks>name</remarks>
        [Required]
        public string? Name { get; set; }

        /// <remarks>entry</remarks>
        public string[]? Entry { get; set; }

        /// <remarks>default, (yes|no) "no"</remarks>
        public string? Default { get; set; }
    }
}