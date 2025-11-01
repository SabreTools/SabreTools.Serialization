using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#Access_Control_Info"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class AccessControlInfo
    {
        /// <summary>
        /// ARM11 local system capabilities
        /// </summary>
        public ARM11LocalSystemCapabilities ARM11LocalSystemCapabilities = new();

        /// <summary>
        /// ARM11 kernel capabilities
        /// </summary>
        public ARM11KernelCapabilities ARM11KernelCapabilities = new();

        /// <summary>
        /// ARM9 access control
        /// </summary>
        /// TODO: Fix serialization issue with this type
        public ARM9AccessControl ARM9AccessControl = new();
    }
}
