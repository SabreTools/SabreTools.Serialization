namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The occlusion lump (Lump 9) contains the polygon geometry and some
    /// flags used by func_occluder entities. Unlike other brush entities,
    /// func_occluders don't use the 'model' key in the entity lump.
    /// Instead, the brushes are split from the entities during the compile
    /// process and numeric occluder keys are assigned as 'occludernum'.
    /// Brush sides textured with tools/toolsoccluder or tools/toolstrigger
    /// are then stored together with the occluder keys and some additional
    /// info in this lump.
    ///
    /// The lump is divided into three parts and begins with a integer value
    /// with the total number of occluders, followed by an array of
    /// doccluderdata_t fields of the same size. The next part begins with
    /// another integer value, this time for the total number of occluder
    /// polygons, as well as an array of doccluderpolydata_t fields of equal
    /// size. Part three begins with another integer value for the amount of
    /// occluder polygon vertices, followed by an array of integer values for
    /// the vertex indices, again of the same size.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class OcclusionLump : Lump
    {
        public int Count;

        /// <summary>
        /// <see cref="Count">
        /// </summary>
        public OccluderData[] Data { get; set; } = [];

        public int PolyDataCount;

        /// <summary>
        /// <see cref="PolyDataCount">
        /// </summary>
        public OccluderPolyData[] PolyData { get; set; } = [];

        public int VertexIndexCount;

        /// <summary>
        /// <see cref="VertexIndexCount">
        /// </summary>
        public int[] VertexIndicies { get; set; } = [];
    }
}
