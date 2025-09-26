/// <remarks>
/// Information sourced from https://docwiki.embarcadero.com/Libraries/Alexandria/en/System.PackageInfoTable
/// </remarks>
namespace SabreTools.Serialization.Models.Delphi
{
    public class PackageInfoTable
    {
        public int UnitCount { get; set; }

        public PackageUnitEntry[]? UnitInfo { get; set; }

        public PackageTypeInfo? TypeInfo { get; set; }
    }
}