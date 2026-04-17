namespace SabreTools.Data.Models.NintendoDisc
{
    /// <summary>
    /// Platform / console type for a Nintendo disc image
    /// </summary>
    public enum Platform
    {
        /// <summary>Platform could not be determined</summary>
        Unknown = 0,

        /// <summary>Nintendo GameCube</summary>
        GameCube = 1,

        /// <summary>Nintendo Wii</summary>
        Wii = 2,
    }

    /// <summary>
    /// Wii partition type
    /// </summary>
    public enum WiiPartitionType : uint
    {
        /// <summary>Game data partition (DATA)</summary>
        Data = 0,

        /// <summary>System update partition (UPDATE)</summary>
        Update = 1,

        /// <summary>Channel installer partition (CHANNEL)</summary>
        Channel = 2,
    }
}
