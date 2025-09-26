namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// The Game lump (Lump 35) seems to be intended to be used for
    /// map data that is specific to a particular game using the Source
    /// engine, so that the file format can be extended without altering
    /// the previously defined format.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class GameLump
    {
        /// <summary>
        /// Number of game lumps
        /// </summary>
        public int LumpCount;

        /// <summary>
        /// <see cref="LumpCount"/> 
        /// </summary>
        public GameLumpDirectory[]? Directories;
    }
}