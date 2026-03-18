namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// An ISO-9660 data sector
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public abstract class DataSector : Sector
    {
        // Data sectors only contain user data
        // TODO: Create user data base class for all single-sector structures
        // TODO: Create generic data sector with byte array data
        // TODO: Update CD-ROM models to take advantage of this
    }
}
