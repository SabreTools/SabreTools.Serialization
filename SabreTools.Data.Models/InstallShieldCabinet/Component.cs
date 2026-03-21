using System;

namespace SabreTools.Data.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/libunshield.h"/>
    public sealed class Component
    {
        /// <summary>
        /// Offset to the component identifier
        /// </summary>
        public uint IdentifierOffset { get; set; }

        /// <summary>
        /// Component identifier
        /// </summary>
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// Offset to the component descriptor
        /// </summary>
        public uint DescriptorOffset { get; set; }

        /// <summary>
        /// Offset to the display name
        /// </summary>
        public uint DisplayNameOffset { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Component status
        /// </summary>
        public ComponentStatus Status { get; set; }

        /// <summary>
        /// Offset to the password
        /// </summary>
        public uint PasswordOffset { get; set; }

        /// <summary>
        /// Misc offset
        /// </summary>
        public uint MiscOffset { get; set; }

        /// <summary>
        /// Component index
        /// </summary>
        public ushort ComponentIndex { get; set; }

        /// <summary>
        /// Offset to the component name
        /// </summary>
        public uint NameOffset { get; set; }

        /// <summary>
        /// Component name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Offset to the CD-ROM folder
        /// </summary>
        public uint CDRomFolderOffset { get; set; }

        /// <summary>
        /// Offset to the HTTP location
        /// </summary>
        public uint HTTPLocationOffset { get; set; }

        /// <summary>
        /// Offset to the FTP location
        /// </summary>
        public uint FTPLocationOffset { get; set; }

        /// <summary>
        /// Unknown GUIDs
        /// </summary>
        public Guid[] Guid { get; set; } = new Guid[2];

        /// <summary>
        /// Offset to the component CLSID
        /// </summary>
        public uint CLSIDOffset { get; set; }

        /// <summary>
        /// Component CLSID
        /// </summary>
        public Guid CLSID { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>28 bytes, see CompAttrs</remarks>
        public byte[] Reserved2 { get; set; } = new byte[28];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>2 bytes (<= v5), 1 byte (> v5)</remarks>
        public byte[] Reserved3 { get; set; } = [];

        /// <summary>
        /// Number of depends(?)
        /// </summary>
        public ushort DependsCount { get; set; }

        /// <summary>
        /// Offset to depends(?)
        /// </summary>
        public uint DependsOffset { get; set; }

        /// <summary>
        /// Number of file groups
        /// </summary>
        public uint FileGroupCount { get; set; }

        /// <summary>
        /// Offset to the file group names
        /// </summary>
        public uint FileGroupNamesOffset { get; set; }

        /// <summary>
        /// File group names
        /// </summary>
        public string[] FileGroupNames { get; set; } = [];

        /// <summary>
        /// Number of X3(?)
        /// </summary>
        public ushort X3Count { get; set; }

        /// <summary>
        /// Offset to X3(?)
        /// </summary>
        public uint X3Offset { get; set; }

        /// <summary>
        /// Number of sub-components
        /// </summary>
        public ushort SubComponentsCount { get; set; }

        /// <summary>
        /// Offset to the sub-components
        /// </summary>
        public uint SubComponentsOffset { get; set; }

        /// <summary>
        /// Offset to the next component
        /// </summary>
        public uint NextComponentOffset { get; set; }

        /// <summary>
        /// Offset to on installing text
        /// </summary>
        public uint OnInstallingOffset { get; set; }

        /// <summary>
        /// Offset to on installed text
        /// </summary>
        public uint OnInstalledOffset { get; set; }

        /// <summary>
        /// Offset to on uninstalling text
        /// </summary>
        public uint OnUninstallingOffset { get; set; }

        /// <summary>
        /// Offset to on uninstalled text
        /// </summary>
        public uint OnUninstalledOffset { get; set; }
    }
}
