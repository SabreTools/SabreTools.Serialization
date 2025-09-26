namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Determine what type of file an item is
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Adjuster,
        Analog,
        Archive,
        BiosSet,
        Chip,
        Condition,
        Configuration,
        ConfLocation,
        ConfSetting,
        Control,
        DataArea,
        Device,
        DeviceRef,
        DipLocation,
        DipSwitch,
        DipValue,
        Disk,
        DiskArea,
        Display,
        Driver,
        Dump,
        Extension,
        Feature,
        Info,
        Input,
        Instance,
        Media,
        Original,
        Part,
        Port,
        RamOption,
        Release,
        Rom,
        Sample,
        SharedFeat,
        Slot,
        SlotOption,
        Software,
        SoftwareList,
        Sound,
        Video,

        /// <summary>
        /// This is not a real type, only used internally
        /// </summary>
        Blank = int.MaxValue,
    }
}