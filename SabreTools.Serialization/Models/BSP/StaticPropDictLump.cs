namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// Of interest is the gamelump which is used to store prop_static entities,
    /// which uses the gamelump ID of 'sprp' ASCII (1936749168 decimal). Unlike
    /// most other entities, static props are not stored in the entity lump. The
    /// gamelump formats used in Source are defined in the public/gamebspfile.h
    /// header file.
    ///
    /// The first element of the static prop game lump is the dictionary; this is
    /// an integer count followed by the list of model (prop) names used in the map
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class StaticPropDictLump
    {
        public int DictEntries;

        /// <summary>
        /// Model name
        /// </summary>
        /// <remarks>[dictEntries][128]</remarks>
        public char[][] Name;
    }
}
