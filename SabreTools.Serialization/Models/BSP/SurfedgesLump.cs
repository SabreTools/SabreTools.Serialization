namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// This lump represents pretty much the same mechanism as the marksurfaces.
    /// A face can insert its surfedge indexes into this array to get the
    /// corresponding edges delimitting the face and further pointing to the
    /// vertexes, which are required for rendering. The index can be positive
    /// or negative. If the value of the surfedge is positive, the first vertex
    /// of the edge is used as vertex for rendering the face, otherwise, the
    /// value is multiplied by -1 and the second vertex of the indexed edge is
    /// used. 
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class SurfedgesLump : Lump
    {
        /// <summary>
        /// Surfedges
        /// </summary>
        public int[]? Surfedges { get; set; }
    }
}