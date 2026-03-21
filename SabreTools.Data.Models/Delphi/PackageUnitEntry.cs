using System.Runtime.InteropServices;

/// <remarks>
/// Information sourced from https://docwiki.embarcadero.com/Libraries/Alexandria/en/System.PackageUnitEntry
/// </remarks>
namespace SabreTools.Data.Models.Delphi
{
    [StructLayout(LayoutKind.Sequential)]
    public class PackageUnitEntry
    {
        /// <remarks>
        /// System-dependent pointer type, assumed to be encoded for x86
        /// </remarks>
        public uint Init;

        /// <remarks>
        /// System-dependent pointer type, assumed to be encoded for x86
        /// </remarks>
        public uint FInit;
    }
}
