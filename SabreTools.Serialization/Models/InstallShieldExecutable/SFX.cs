namespace SabreTools.Data.Models.InstallShieldExecutable
{
    /// <summary>
    /// Represents the layout of of the overlay area of an
    /// InstallShield executable.
    ///
    /// The layout of this is derived from the layout in the
    /// physical file.
    /// </summary>
    /// <remarks>
    /// According to ISx source, there are two categories of installshield executables,
    /// plain and encrypted. Encrypted has two different "types" it can be, specified by
    /// a header. "InstallShield" and a newer format from 2015?-onwards called "ISSetupStream".
    /// Plain executables have no central header, and each file is unencrypted. Files
    /// in "InstallShield" encrypted executables have encryption applied over block sizes
    /// of 1024 bytes, and files in "ISSetupStream" encrypted executables are encrypted
    /// per-file. There's also something about leading data that isn't explained
    /// (at least not clearly), and these encrypted executables can also additionally have
    /// their files compressed with inflate.
    ///
    /// While not stated in ISx; from experience, executables with "InstallShield" often
    /// (if not always?) mainly consist of a singular, large MSI installer along with some
    /// helper files, whereas plain executables often (if not always?) mainly consist of
    /// regular installshield cabinets within. At the moment, this code only supports and
    /// documents the plain variant. Clearer naming and separation between the types is yet
    /// to come.
    /// </remarks>
    /// TODO: Look into making the array a dictionary
    /// There is no unified header or footer that indicates a file
    /// table, so having either each file entry cache the data
    /// or be associated with an offset may make it more useful.
    ///
    /// It will need to be made very apparent that the dictionary
    /// does not directly represent the structure.
    public class SFX
    {
        /// <summary>
        /// Set of file entries
        /// </summary>
        public FileEntry[] Entries { get; set; }
    }
}
