namespace SabreTools.Serialization.Models.SecuROM
{
    public enum MatroshkaEntryType : uint
    {
        /// <summary>
        /// Helper or activation executable
        /// </summary>
        Helper = 0x01,

        /// <summary>
        /// Main executable, usually one of the following:
        /// - RC-encrypted executable to be decrypted later
        /// - Main game program executable
        /// - Revoker executable
        /// </summary>
        /// <remarks>Usually the second entry</remarks>
        Main = 0x02,

        /// <summary>
        /// Required libraries for the main executable
        /// </summary>
        /// <remarks>
        /// Examples include:
        /// - DFA.dll for RC-encrypted executables
        /// - paul.dll for PA-protected games
        /// - remover.exe for revocation
        /// executables.
        /// </remarks>
        Dependency = 0x04,

        /// <summary>
        /// Similar use to <see cref="Dependency"/> 
        /// </summary>
        Unknown0x08 = 0x08,
    }
}