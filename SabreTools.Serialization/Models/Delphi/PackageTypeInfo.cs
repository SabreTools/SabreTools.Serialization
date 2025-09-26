/// <remarks>
/// Information sourced from https://docwiki.embarcadero.com/Libraries/Sydney/en/System.TPackageTypeInfo
/// </remarks>
namespace SabreTools.Serialization.Models.Delphi
{
    public class PackageTypeInfo
    {
        public int TypeCount { get; set; }

        /// <remarks>
        /// System-dependent pointer type, assumed to be encoded for x86
        /// </remarks>
        public uint[]? TypeTable { get; set; }

        public int UnitCount { get; set; }

        public string[]? UnitNames { get; set; }
    }
}