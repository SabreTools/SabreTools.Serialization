namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Determine the chip type
    /// </summary>
    public enum ChipType
    {
        /// <summary>"cpu"</summary>
        CPU,

        /// <summary>"audio"</summary>
        Audio,
    }

    /// <summary>
    /// Determine the control type
    /// </summary>
    public enum ControlType
    {
        /// <summary>"joy"</summary>
        Joy,

        /// <summary>"stick"</summary>
        Stick,

        /// <summary>"paddle"</summary>
        Paddle,

        /// <summary>"pedal"</summary>
        Pedal,

        /// <summary>"lightgun"</summary>
        Lightgun,

        /// <summary>"positional"</summary>
        Positional,

        /// <summary>"dial"</summary>
        Dial,

        /// <summary>"trackball"</summary>
        Trackball,

        /// <summary>"mouse"</summary>
        Mouse,

        /// <summary>"only_buttons"</summary>
        OnlyButtons,

        /// <summary>"keypad"</summary>
        Keypad,

        /// <summary>"keyboard"</summary>
        Keyboard,

        /// <summary>"mahjong"</summary>
        Mahjong,

        /// <summary>"hanafuda"</summary>
        Hanafuda,

        /// <summary>"gambling"</summary>
        Gambling,
    }

    /// <summary>
    /// Determine the device type
    /// </summary>
    public enum DeviceType
    {
        /// <summary>"unknown"</summary>
        Unknown,

        /// <summary>"cartridge"</summary>
        Cartridge,

        /// <summary>"floppydisk"</summary>
        FloppyDisk,

        /// <summary>"harddisk"</summary>
        HardDisk,

        /// <summary>"cylinder"</summary>
        Cylinder,

        /// <summary>"cassette"</summary>
        Cassette,

        /// <summary>"punchcard"</summary>
        PunchCard,

        /// <summary>"punchtape"</summary>
        PunchTape,

        /// <summary>"printout"</summary>
        Printout,

        /// <summary>"serial"</summary>
        Serial,

        /// <summary>"parallel"</summary>
        Parallel,

        /// <summary>"snapshot"</summary>
        Snapshot,

        /// <summary>"quickload"</summary>
        QuickLoad,

        /// <summary>"memcard"</summary>
        MemCard,

        /// <summary>"cdrom"</summary>
        CDROM,

        /// <summary>"magtape"</summary>
        MagTape,

        /// <summary>"romimage"</summary>
        ROMImage,

        /// <summary>"midiin"</summary>
        MIDIIn,

        /// <summary>"midiout"</summary>
        MIDIOut,

        /// <summary>"picture"</summary>
        Picture,

        /// <summary>"vidfile"</summary>
        VidFile,
    }

    /// <summary>
    /// Determine the display type
    /// </summary>
    public enum DisplayType
    {
        /// <summary>"raster"</summary>
        Raster,

        /// <summary>"vector"</summary>
        Vector,

        /// <summary>"lcd"</summary>
        LCD,

        /// <summary>"svg"</summary>
        SVG,

        /// <summary>"unknown"</summary>
        Unknown,
    }

    /// <summary>
    /// Determine the endianness
    /// </summary>
    public enum Endianness
    {
        /// <summary>"big"</summary>
        Big,

        /// <summary>"little"</summary>
        Little,
    }

    /// <summary>
    /// Determine the emulation status
    /// </summary>
    public enum FeatureStatus
    {
        /// <summary>"unemulated"</summary>
        Unemulated,

        /// <summary>"imperfect"</summary>
        Imperfect,
    }

    /// <summary>
    /// Determine the feature type
    /// </summary>
    public enum FeatureType
    {
        /// <summary>"protection"</summary>
        Protection,

        /// <summary>"palette"</summary>
        Palette,

        /// <summary>"graphics"</summary>
        Graphics,

        /// <summary>"sound"</summary>
        Sound,

        /// <summary>"controls"</summary>
        Controls,

        /// <summary>"keyboard"</summary>
        Keyboard,

        /// <summary>"mouse"</summary>
        Mouse,

        /// <summary>"microphone"</summary>
        Microphone,

        /// <summary>"camera"</summary>
        Camera,

        /// <summary>"disk"</summary>
        Disk,

        /// <summary>"printer"</summary>
        Printer,

        /// <summary>"lan"</summary>
        Lan,

        /// <summary>"wan"</summary>
        Wan,

        /// <summary>"timing"</summary>
        Timing,
    }

    /// <summary>
    /// Determine the status of the item
    /// </summary>
    public enum ItemStatus
    {
        /// <summary>"none", "no"</summary>
        None,

        /// <summary>"good"</summary>
        Good,

        /// <summary>"baddump"</summary>
        BadDump,

        /// <summary>"nodump", "yes"</summary>
        Nodump,

        /// <summary>"verified"</summary>
        Verified,
    }

    /// <summary>
    /// Determine the loadflag value
    /// </summary>
    public enum LoadFlag
    {
        /// <summary>"load16_byte"</summary>
        Load16Byte,

        /// <summary>"load16_word"</summary>
        Load16Word,

        /// <summary>"load16_word_swap"</summary>
        Load16WordSwap,

        /// <summary>"load32_byte"</summary>
        Load32Byte,

        /// <summary>"load32_word"</summary>
        Load32Word,

        /// <summary>"load32_word_swap"</summary>
        Load32WordSwap,

        /// <summary>"load32_dword"</summary>
        Load32DWord,

        /// <summary>"load64_word"</summary>
        Load64Word,

        /// <summary>"load64_word_swap"</summary>
        Load64WordSwap,

        /// <summary>"reload"</summary>
        Reload,

        /// <summary>"fill"</summary>
        Fill,

        /// <summary>"continue"</summary>
        Continue,

        /// <summary>"reload_plain"</summary>
        ReloadPlain,

        /// <summary>"ignore"</summary>
        Ignore,
    }

    /// <summary>
    /// Determines merging tag handling for DAT output
    /// </summary>
    public enum MergingFlag
    {
        /// <summary>"none"</summary>
        None = 0,

        /// <summary>"split"</summary>
        Split,

        /// <summary>"merged"</summary>
        Merged,

        /// <summary>"nonmerged", "unmerged"</summary>
        NonMerged,

        /// <summary>"fullmerged"</summary>
        /// <remarks>This is not usually defined for Merging flags</remarks>
        FullMerged,

        /// <summary>"device", "deviceunmerged", "devicenonmerged"</summary>
        /// <remarks>This is not usually defined for Merging flags</remarks>
        DeviceNonMerged,

        /// <summary>"full", "fullunmerged", "fullnonmerged"</summary>
        /// <remarks>This is not usually defined for Merging flags</remarks>
        FullNonMerged,
    }

    /// <summary>
    /// Determines nodump tag handling for DAT output
    /// </summary>
    public enum NodumpFlag
    {
        /// <summary>"none"</summary>
        None = 0,

        /// <summary>"obsolete"</summary>
        Obsolete,

        /// <summary>"required"</summary>
        Required,

        /// <summary>"ignore"</summary>
        Ignore,
    }

    /// <summary>
    /// Determine which OpenMSX subtype an item is
    /// </summary>
    public enum OpenMSXSubType
    {
        /// <summary>"rom"</summary>
        Rom,

        /// <summary>"megarom"</summary>
        MegaRom,

        /// <summary>"sccpluscart"</summary>
        SCCPlusCart,
    }

    /// <summary>
    /// Determines packing tag handling for DAT output
    /// </summary>
    public enum PackingFlag
    {
        /// <summary>"none"</summary>
        None = 0,

        /// <summary>
        /// Force all sets to be in archives, except disk and media
        /// </summary>
        /// <remarks>"zip", "yes"</remarks>
        Zip,

        /// <summary>
        /// Force all sets to be extracted into subfolders
        /// </summary>
        /// <remarks>"unzip", "no"</remarks>
        Unzip,

        /// <summary>
        /// Force sets with single items to be extracted to the parent folder
        /// </summary>
        /// <remarks>"partial"</remarks>
        Partial,

        /// <summary>
        /// Force all sets to be extracted to the parent folder
        /// </summary>
        /// <remarks>"flat"</remarks>
        Flat,

        /// <summary>
        /// Force all sets to have all archives treated as files
        /// </summary>
        /// <remarks>"fileonly"</remarks>
        FileOnly,
    }

    /// <summary>
    /// Determine relation of value to condition
    /// </summary>
    public enum Relation
    {
        /// <summary>"eq"</summary>
        Equal,

        /// <summary>"ne"</summary>
        NotEqual,

        /// <summary>"gt"</summary>
        GreaterThan,

        /// <summary>"le"</summary>
        LessThanOrEqual,

        /// <summary>"lt"</summary>
        LessThan,

        /// <summary>"ge"</summary>
        GreaterThanOrEqual,
    }

    /// <summary>
    /// Determine machine runnable status
    /// </summary>
    public enum Runnable
    {
        /// <summary>"no"</summary>
        No,

        /// <summary>"partial"</summary>
        Partial,

        /// <summary>"yes"</summary>
        Yes,
    }

    /// <summary>
    /// Determine software list status
    /// </summary>
    public enum SoftwareListStatus
    {
        /// <summary>"none"</summary>
        None,

        /// <summary>"original"</summary>
        Original,

        /// <summary>"compatible"</summary>
        Compatible,
    }

    /// <summary>
    /// Determine machine support status
    /// </summary>
    public enum Supported
    {
        /// <summary>"no", "unsupported"</summary>
        No,

        /// <summary>"partial"</summary>
        Partial,

        /// <summary>"yes", "supported"</summary>
        Yes,
    }

    /// <summary>
    /// Determine driver support statuses
    /// </summary>
    public enum SupportStatus
    {
        /// <summary>"good"</summary>
        Good,

        /// <summary>"imperfect"</summary>
        Imperfect,

        /// <summary>"preliminary"</summary>
        Preliminary,
    }
}
