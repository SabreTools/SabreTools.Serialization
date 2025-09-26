namespace SabreTools.Serialization.Models.ClrMamePro
{
    public class MetadataFile
    {
        /// <remarks>clrmamepro</remarks>
        public ClrMamePro? ClrMamePro { get; set; }

        /// <remarks>game, machine, resource, set</remarks>
        public GameBase[]? Game { get; set; }
    }
}