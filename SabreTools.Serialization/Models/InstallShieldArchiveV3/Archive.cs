namespace SabreTools.Data.Models.InstallShieldArchiveV3
{
    /// <see href="https://github.com/wfr/unshieldv3/blob/master/ISArchiveV3.cpp"/>
    public class Archive
    {
        /// <summary>
        /// Archive header information
        /// </summary>
        public Header Header { get; set; } = new();

        /// <summary>
        /// Directories found in the archive
        /// </summary>
        public Directory[] Directories {  get; set; } = [];

        /// <summary>
        /// Files found in the archive
        /// </summary>
        public File[] Files { get; set; } = [];
    }
}
