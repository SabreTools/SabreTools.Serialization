namespace SabreTools.Serialization.Models.DosCenter
{
    public class MetadataFile
    {
        /// <remarks>doscenter</remarks>
        public DosCenter? DosCenter { get; set; }

        /// <remarks>game</remarks>
        public Game[]? Game { get; set; }
    }
}