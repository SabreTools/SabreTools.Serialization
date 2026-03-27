using System;

namespace SabreTools.Metadata.DatItems
{
    /// <summary>
    /// Determines which type of duplicate a file is
    /// </summary>
    [Flags]
    public enum DupeType
    {
        // Type of match
        Hash = 1 << 0,
        All = 1 << 1,

        // Location of match
        Internal = 1 << 2,
        External = 1 << 3,
    }

    /// <summary>
    /// A subset of fields that can be used as keys
    /// </summary>
    public enum ItemKey
    {
        NULL = 0,

        Machine,

        CRC16,
        CRC,
        CRC64,
        MD2,
        MD4,
        MD5,
        RIPEMD128,
        RIPEMD160,
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        SpamSum,
    }

    /// <summary>
    /// Determine what type of file an item is
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        // "Actionable" item types

        /// <summary>"rom"</summary>
        Rom,

        /// <summary>"disk"</summary>
        Disk,

        /// <summary>"file"</summary>
        File,

        /// <summary>"media"</summary>
        Media,

        // "Auxiliary" item types

        /// <summary>"adjuster"</summary>
        Adjuster,

        /// <summary>"analog"</summary>
        Analog,

        /// <summary>"archive"</summary>
        Archive,

        /// <summary>"biosset"</summary>
        BiosSet,

        /// <summary>"chip"</summary>
        Chip,

        /// <summary>"condition"</summary>
        Condition,

        /// <summary>"configuration"</summary>
        Configuration,

        /// <summary>"conflocation"</summary>
        ConfLocation,

        /// <summary>"confsetting"</summary>
        ConfSetting,

        /// <summary>"control"</summary>
        Control,

        /// <summary>"dataarea"</summary>
        DataArea,

        /// <summary>"device"</summary>
        Device,

        /// <summary>"device_ref", "deviceref"</summary>
        DeviceRef,

        /// <summary>"diplocation"</summary>
        DipLocation,

        /// <summary>"dipswitch"</summary>
        DipSwitch,

        /// <summary>"dipvalue"</summary>
        DipValue,

        /// <summary>"diskarea"</summary>
        DiskArea,

        /// <summary>"display"</summary>
        Display,

        /// <summary>"driver"</summary>
        Driver,

        /// <summary>"extension"</summary>
        Extension,

        /// <summary>"feature"</summary>
        Feature,

        /// <summary>"info"</summary>
        Info,

        /// <summary>"input"</summary>
        Input,

        /// <summary>"instance"</summary>
        Instance,

        /// <summary>"original"</summary>
        Original,

        /// <summary>"part"</summary>
        Part,

        /// <summary>"part_feature", "partfeature"</summary>
        PartFeature,

        /// <summary>"port"</summary>
        Port,

        /// <summary>"ramoption", "ram_option"</summary>
        RamOption,

        /// <summary>"release"</summary>
        Release,

        /// <summary>"release_details", "releasedetails"</summary>
        ReleaseDetails,

        /// <summary>"sample"</summary>
        Sample,

        /// <summary>"serials"</summary>
        Serials,

        /// <summary>"sharedfeat", "shared_feat", "sharedfeature", "shared_feature"</summary>
        SharedFeat,

        /// <summary>"slot"</summary>
        Slot,

        /// <summary>"slotoption", "slot_option"</summary>
        SlotOption,

        /// <summary>"softwarelist", "software_list"</summary>
        SoftwareList,

        /// <summary>"sound"</summary>
        Sound,

        /// <summary>"source_details", "sourcedetails"</summary>
        SourceDetails,

        /// <summary>"blank"</summary>
        Blank = 99, // This is not a real type, only used internally
    }

    /// <summary>
    /// Determine what type of machine it is
    /// </summary>
    [Flags]
    public enum MachineType
    {
        /// <summary>"none"</summary>
        None = 0,

        /// <summary>"bios"</summary>
        Bios = 1 << 0,

        /// <summary>"device", "dev"</summary>
        Device = 1 << 1,

        /// <summary>"mechanical", "mech"</summary>
        Mechanical = 1 << 2,
    }
}
