using System.Collections.Generic;

namespace SabreTools.Data.Models.XenonExecutable
{
    /// <see href="https://free60.org/System-Software/Formats/XEX/"/>
    public static class Constants
    {
        /// <summary>
        /// Xenon (Xbox 360) Executable magic number ("XEX2")
        /// </summary>
        public static readonly byte[] MagicBytes = [0x58, 0x45, 0x58, 0x32];

        /// <summary>
        /// Xenon (Xbox 360) Executable magic number ("XEX2")
        /// </summary>
        public const string MagicString = "XEX2";

        /// <summary>
        /// Xenon (Xbox 360) Optional Header types
        /// </summary>
        public static readonly Dictionary<uint, string> OptionalHeaderTypes = new()
        {
            [0x0002FF] = "Resource Info",
            [0x0003FF] = "Base File Format",
            [0x000405] = "Base Reference",
            [0x0005FF] = "Delta Patch Descriptor",
            [0x0080FF] = "Bounding Path",
            [0x008105] = "Device ID",
            [0x010001] = "Original Base Address",
            [0x010100] = "Entry Point",
            [0x010201] = "Image Base Address",
            [0x0103FF] = "Import Libraries",
            [0x018002] = "Checksum Timestamp",
            [0x018102] = "Enabled For Callcap",
            [0x018200] = "Enabled For Fastcap",
            [0x0183FF] = "Original PE Name",
            [0x0200FF] = "Static Libraries",
            [0x020104] = "TLS Info",
            [0x020200] = "Default Stack Size",
            [0x020301] = "Default Filesystem Cache Size",
            [0x020401] = "Default Heap Size",
            [0x028002] = "Page Heap Size and Flags",
            [0x030000] = "System Flags",
            [0x040006] = "Execution ID",
            [0x0401FF] = "Service ID List",
            [0x040201] = "Title Workspace Size",
            [0x040310] = "Game Ratings",
            [0x040404] = "LAN Key",
            [0x0405FF] = "Xbox 360 Logo",
            [0x0406FF] = "Multidisc Media IDs",
            [0x0407FF] = "Alternate Title IDs",
            [0x040801] = "Additional Title Memory",
            [0xE10402] = "Exports by Name"
        };
    }
}
