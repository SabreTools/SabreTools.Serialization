namespace SabreTools.Data.Models.Steam2Installer
{
    public class Steam2InstallerSet
    {
        /// <summary>
        /// The sim file
        /// </summary>
        public SimFile Sim { get; set; } = new();

        /// <summary>
        /// The set of sid file volumes
        /// </summary>
        // public SidFile[] Sid { get; set; } = [];
    }
}
