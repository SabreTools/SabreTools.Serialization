using System;

namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    public enum BspContents : int
    {
        CONTENTS_EMPTY = -1,
        CONTENTS_SOLID = -2,
        CONTENTS_WATER = -3,
        CONTENTS_SLIME = -4,
        CONTENTS_LAVA = -5,
        CONTENTS_SKY = -6,
        CONTENTS_ORIGIN = -7,
        CONTENTS_CLIP = -8,
        CONTENTS_CURRENT_0 = -9,
        CONTENTS_CURRENT_90 = -10,
        CONTENTS_CURRENT_180 = -11,
        CONTENTS_CURRENT_270 = -12,
        CONTENTS_CURRENT_UP = -13,
        CONTENTS_CURRENT_DOWN = -14,
        CONTENTS_TRANSLUCENT = -15,
    }

    /// <see href="https://developer.valvesoftware.com/wiki/BSP_flags_(Source)"/>
    [Flags]
    public enum VbspContents : uint
    {
        /// <summary>
        /// No contents
        /// </summary>
        CONTENTS_EMPTY = 0x00000000,

        /// <summary>
        /// An eye is never valid in a solid
        /// </summary>
        CONTENTS_SOLID = 0x00000001,

        /// <summary>
        /// Translucent, but not watery (glass)
        /// </summary>
        CONTENTS_WINDOW = 0x00000002,

        /// <summary>
        /// Unused
        /// </summary>
        CONTENTS_AUX = 0x00000004,

        /// <summary>
        /// Alpha-tested "grate" textures. Bullets/sight pass through,
        /// but solids don't
        /// </summary>
        CONTENTS_GRATE = 0x00000008,

        /// <summary>
        /// Set via %CompileSlime. Unlike Quake II, slime does not do
        /// damage; a separate trigger_hurt should be used for this.
        /// </summary>
        CONTENTS_SLIME = 0x00000010,

        /// <summary>
        /// Set via %CompileWater.
        /// </summary>
        CONTENTS_WATER = 0x00000020,

        /// <summary>
        /// Unknown purpose; only set by %CompilePlayerControlClip.
        /// </summary>
        CONTENTS_MIST = 0x00000040,

        /// <summary>
        /// Block AI line of sight
        /// </summary>
        CONTENTS_BLOCKLOS = 0x00000040,

        /// <summary>
        /// Things that cannot be seen through (may be non-solid though)
        /// </summary>
        CONTENTS_OPAQUE = 0x00000080,

        /// <summary>
        /// Unknown
        /// </summary>
        CONTENTS_TESTFOGVOLUME = 0x00000100,

        /// <summary>
        /// Unused
        /// </summary>
        CONTENTS_UNUSED = 0x00000200,

        /// <summary>
        /// Unused
        /// </summary>
        CONTENTS_UNUSED6 = 0x00000400,

        /// <summary>
        /// If it's visible, grab from the top + update LAST_VISIBLE_CONTENTS
        /// if not visible, then grab from the bottom.
        /// </summary>
        CONTENTS_BLOCKLIGHT = 0x00000400,

        /// <summary>
        /// Per team contents used to differentiate collisions
        /// between players and objects on different teams
        /// </summary>
        CONTENTS_TEAM1 = 0x00000800,

        /// <summary>
        /// Per team contents used to differentiate collisions
        /// between players and objects on different teams
        /// </summary>
        CONTENTS_TEAM2 = 0x00001000,

        /// <summary>
        /// Ignore CONTENTS_OPAQUE on surfaces that have SURF_NODRAW
        /// </summary>
        CONTENTS_IGNORE_NODRAW_OPAQUE = 0x00002000,

        /// <summary>
        /// Hits entities which are MOVETYPE_PUSH (doors, plats, etc.)
        /// </summary>
        CONTENTS_MOVEABLE = 0x00004000,

        /// <summary>
        /// Is an areaportal.
        /// </summary>
        CONTENTS_AREAPORTAL = 0x00008000,

        /// <summary>
        /// Solid to players, including bots.
        /// </summary>
        CONTENTS_PLAYERCLIP = 0x00010000,

        /// <summary>
        /// Solid to monsters, better known in Source as NPCs. Also solid
        /// to bots in CSGO, even though they are players.
        /// </summary>
        CONTENTS_MONSTERCLIP = 0x00020000,

        /// <summary>
        /// Currents can be added to any other contents, and may be mixed
        /// </summary>
        CONTENTS_CURRENT_0 = 0x00040000,

        /// <summary>
        /// Currents can be added to any other contents, and may be mixed
        /// </summary>
        CONTENTS_CURRENT_90 = 0x00080000,

        /// <summary>
        /// Currents can be added to any other contents, and may be mixed
        /// </summary>
        CONTENTS_CURRENT_180 = 0x00100000,

        /// <summary>
        /// Currents can be added to any other contents, and may be mixed
        /// </summary>
        CONTENTS_CURRENT_270 = 0x00200000,

        /// <summary>
        /// Currents can be added to any other contents, and may be mixed
        /// </summary>
        CONTENTS_CURRENT_UP = 0x00400000,

        /// <summary>
        /// Currents can be added to any other contents, and may be mixed
        /// </summary>
        CONTENTS_CURRENT_DOWN = 0x00800000,

        /// <summary>
        /// Unknown
        /// </summary>
        CONTENTS_BRUSH_PAINT = 0x00040000,

        /// <summary>
        /// Unknown
        /// </summary>
        CONTENTS_GRENADECLIP = 0x00080000,

        /// <summary>
        /// Unknown
        /// </summary>
        CONTENTS_DRONECLIP = 0x00100000,

        /// <summary>
        /// Removed before bsping an entity
        /// </summary>
        CONTENTS_ORIGIN = 0x01000000,

        /// <summary>
        /// Should never be on a brush, only in game
        /// </summary>
        CONTENTS_MONSTER = 0x02000000,

        /// <summary>
        /// Solid to point traces (ex hitscan weapons) and non-debris
        /// physics objects[confirm]. Non-solid to QPhysics entities,
        /// such as players.
        /// </summary>
        CONTENTS_DEBRIS = 0x04000000,

        /// <summary>
        /// Brushes to be added after vis leafs
        /// </summary>
        CONTENTS_DETAIL = 0x08000000,

        /// <summary>
        /// Auto set if any surface has trans
        /// </summary>
        CONTENTS_TRANSLUCENT = 0x10000000,

        /// <summary>
        /// Is a ladder
        /// </summary>
        CONTENTS_LADDER = 0x20000000,

        /// <summary>
        /// Use accurate hitboxes on trace
        /// </summary>
        CONTENTS_HITBOX = 0x40000000,
    }

    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [Flags]
    public enum DispTriTag : ushort
    {
        DISPTRI_TAG_SURFACE = 0x01,
        DISPTRI_TAG_WALKABLE = 0x02,
        DISPTRI_TAG_BUILDABLE = 0x04,
        DISPTRI_FLAG_SURFPROP1 = 0x08,
        DISPTRI_FLAG_SURFPROP2 = 0x10,
    }

    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public enum EmitType
    {
        /// <summary>
        /// 90 degree spotlight
        /// </summary>
        EMIT_SURFACE,

        /// <summary>
        /// Simple point light source
        /// </summary>
        EMIT_POINT,

        /// <summary>
        /// Spotlight with penumbra
        /// </summary>
        EMIT_SPOTLIGHT,

        /// <summary>
        /// Directional light with no falloff
        /// (surface must trace to SKY texture)
        /// </summary>
        EMIT_SKYLIGHT,

        /// <summary>
        /// Linear falloff, non-lambertian
        /// </summary>
        EMIT_QUAKELIGHT,

        /// <summary>
        /// Spherical light source with no falloff
        /// (surface must trace to SKY texture)
        /// </summary>
        EMIT_SKYAMBIENT,
    }

    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public enum LumpType : int
    {
        #region BSP and VBSP

        /// <summary>
        /// The entity lump is basically a pure ASCII text section.
        /// It consists of the string representations of all entities,
        /// which are copied directly from the input file to the output
        /// BSP file by the compiler.
        /// </summary>
        LUMP_ENTITIES = 0,

        /// <summary>
        /// This lump is a simple array of binary data structures.
        /// Each of this structures defines a plane in 3-dimensional
        /// space by using the Hesse normal form
        /// </summary>
        LUMP_PLANES = 1,

        /// <summary>
        /// The texture lump is somehow a bit more complex then the
        /// other lumps, because it is possible to save textures
        /// directly within the BSP file instead of storing them in
        /// external WAD files.
        /// </summary>
        /// <remarks>LUMP_TEXDATA in VBSP</remarks>
        LUMP_TEXTURES = 2,

        /// <summary>
        /// This lump simply consists of all vertices of the BSP tree.
        /// They are stored as a primitve array of triples of floats.
        /// </summary>
        /// <remarks>LUMP_VERTEXES in VBSP</remarks>
        LUMP_VERTICES = 3,

        /// <summary>
        /// The VIS lump contains data, which is irrelevant to the actual
        /// BSP tree, but offers a way to boost up the speed of the
        /// renderer significantly. Especially complex maps profit from
        /// the use if this data. This lump contains the so-called
        /// Potentially Visible Sets (PVS) (also called VIS lists) in the
        /// same amout of leaves of the tree, the user can enter (often
        /// referred to as VisLeaves). The visiblilty lists are stored as
        /// sequences of bitfields, which are run-length encoded.
        /// </summary>
        LUMP_VISIBILITY = 4,

        /// <summary>
        /// This lump is simple again and contains an array of binary
        /// structures, the nodes, which are a major part of the BSP tree.
        /// </summary>
        LUMP_NODES = 5,

        /// <summary>
        /// The texinfo lump contains informations about how textures are
        /// applied to surfaces. The lump itself is an array of binary data
        /// structures.
        /// </summary>
        LUMP_TEXINFO = 6,

        /// <summary>
        /// The face lump contains the surfaces of the scene.
        /// </summary>
        LUMP_FACES = 7,

        /// <summary>
        /// This is one of the largest lumps in the BSP file. The lightmap
        /// lump stores all lightmaps used in the entire map. The lightmaps
        /// are arrays of triples of bytes (3 channel color, RGB) and stored
        /// continuously.
        /// </summary>
        LUMP_LIGHTING = 8,

        /// <summary>
        /// This lump contains the so-called clipnodes, which build a second
        /// BSP tree used only for collision detection.
        /// </summary>
        /// <remarks>LUMP_OCCLUSION in VBSP</remarks>
        LUMP_CLIPNODES = 9,

        /// <summary>
        /// The leaves lump contains the leaves of the BSP tree.
        /// </summary>
        /// <remarks>LUMP_LEAFS in VBSP</remarks>
        LUMP_LEAVES = 10,

        /// <summary>
        /// The marksurfaces lump is a simple array of short integers.
        /// </summary>
        /// <remarks>LUMP_FACEIDS in VBSP</remarks>
        LUMP_MARKSURFACES = 11,

        /// <summary>
        /// The edges delimit the face and further refer to the vertices of the
        /// face. Each edge is pointing to the start and end vertex of the edge.
        /// </summary>
        LUMP_EDGES = 12,

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
        LUMP_SURFEDGES = 13,

        /// <summary>
        /// A model is kind of a mini BSP tree. Its size is determinded by the
        /// bounding box spaned by the first to members of this struct.
        /// </summary>
        LUMP_MODELS = 14,

        #endregion

        #region VBSP Only

        /// <summary>
        /// Internal world lights converted from the entity lump
        /// </summary>
        LUMP_WORLDLIGHTS = 15,

        /// <summary>
        /// Index to faces in each leaf
        /// </summary>
        LUMP_LEAFFACES = 16,

        /// <summary>
        /// Index to brushes in each leaf
        /// </summary>
        LUMP_LEAFBRUSHES = 17,

        /// <summary>
        /// Brush array
        /// </summary>
        LUMP_BRUSHES = 18,

        /// <summary>
        /// Brushside array
        /// </summary>
        LUMP_BRUSHSIDES = 19,

        /// <summary>
        /// Area array
        /// </summary>
        LUMP_AREAS = 20,

        /// <summary>
        /// Portals between areas
        /// </summary>
        LUMP_AREAPORTALS = 21,

        /// <remarks>Source 2004</remarks>
        LUMP_PORTALS = 22,

        /// <summary>
        /// Unused
        /// </summary>
        /// <remarks>Source 2007/2009</remarks>
        LUMP_UNUSED0 = 22,

        /// <summary>
        /// Static props convex hull lists
        /// </summary>
        /// <remarks>Source (L4D2 Branch)</remarks>
        LUMP_PROPCOLLISION = 22,

        /// <summary>
        /// Leaves that are enterable by the player
        /// </summary>
        /// <remarks>Source 2004</remarks>
        LUMP_CLUSTERS = 23,

        /// <summary>
        /// Unused
        /// </summary>
        /// <remarks>Source 2007/2009</remarks>
        LUMP_UNUSED1 = 23,

        /// <summary>
        /// Static prop convex hulls
        /// </summary>
        /// <remarks>Source (L4D2 Branch)</remarks>
        LUMP_PROPHULLS = 23,

        /// <summary>
        /// Vertices of portal polygons
        /// </summary>
        /// <remarks>Source 2004</remarks>
        LUMP_PORTALVERTS = 24,

        /// <summary>
        /// Unused
        /// </summary>
        /// <remarks>Source 2007/2009</remarks>
        LUMP_UNUSED2 = 24,

        /// <summary>
        /// Used to store client side entities (Similar to Lump #0)
        /// </summary>
        /// <remarks>Source (TacInt branch)</remarks>
        LUMP_FAKEENTITIES = 24,

        /// <summary>
        /// Static prop collision vertices
        /// </summary>
        /// <remarks>Source (L4D2 Branch)</remarks>
        LUMP_PROPHULLVERTS = 24,

        /// <remarks>Source 2004</remarks>
        LUMP_CLUSTERPORTALS = 25,

        /// <summary>
        /// Unused
        /// </summary>
        /// <remarks>Source 2007/2009</remarks>
        LUMP_UNUSED3 = 25,

        /// <summary>
        /// Static prop per hull triangle index start/count
        /// </summary>
        /// <remarks>Source (L4D2 Branch)</remarks>
        LUMP_PROPTRIS = 25,

        /// <summary>
        /// Displacement surface array
        /// </summary>
        LUMP_DISPINFO = 26,

        /// <summary>
        /// Brush faces array before splitting
        /// </summary>
        LUMP_ORIGINALFACES = 27,

        /// <summary>
        /// Displacement physics collision data
        /// </summary>
        LUMP_PHYSDISP = 28,

        /// <summary>
        /// Physics collision data
        /// </summary>
        LUMP_PHYSCOLLIDE = 29,

        /// <summary>
        /// Face plane normals
        /// </summary>
        LUMP_VERTNORMALS = 30,

        /// <summary>
        /// Face plane normal index array
        /// </summary>
        LUMP_VERTNORMALINDICES = 31,

        /// <summary>
        /// Displacement lightmap alphas (unused/empty since Source 2006)
        /// </summary>
        LUMP_DISP_LIGHTMAP_ALPHAS = 32,

        /// <summary>
        /// Vertices of displacement surface meshes
        /// </summary>
        LUMP_DISP_VERTS = 33,

        /// <summary>
        /// Displacement lightmap sample positions
        /// </summary>
        LUMP_DISP_LIGHTMAP_SAMPLE_POSITIONS = 34,

        /// <summary>
        /// Game-specific data lump
        /// </summary>
        LUMP_GAME_LUMP = 35,

        /// <summary>
        /// Data for leaf nodes that are inside water
        /// </summary>
        LUMP_LEAFWATERDATA = 36,

        /// <summary>
        /// Water polygon data
        /// </summary>
        LUMP_PRIMITIVES = 37,

        /// <summary>
        /// Water polygon vertices
        /// </summary>
        LUMP_PRIMVERTS = 38,

        /// <summary>
        /// Water polygon vertex index array
        /// </summary>
        LUMP_PRIMINDICES = 39,

        /// <summary>
        /// Embedded uncompressed or LZMA-compressed Zip-format file
        /// </summary>
        LUMP_PAKFILE = 40,

        /// <summary>
        /// Clipped portal polygon vertices
        /// </summary>
        LUMP_CLIPPORTALVERTS = 41,

        /// <summary>
        /// env_cubemap location array
        /// </summary>
        LUMP_CUBEMAPS = 42,

        /// <summary>
        /// Texture name data
        /// </summary>
        LUMP_TEXDATA_STRING_DATA = 43,

        /// <summary>
        /// Index array into texdata string data
        /// </summary>
        LUMP_TEXDATA_STRING_TABLE = 44,

        /// <summary>
        /// info_overlay data array
        /// </summary>
        LUMP_OVERLAYS = 45,

        /// <summary>
        /// Distance from leaves to water
        /// </summary>
        LUMP_LEAFMINDISTTOWATER = 46,

        /// <summary>
        /// Macro texture info for faces
        /// </summary>
        LUMP_FACE_MACRO_TEXTURE_INFO = 47,

        /// <summary>
        /// Displacement surface triangles
        /// </summary>
        LUMP_DISP_TRIS = 48,

        /// <summary>
        /// Compressed win32-specific Havok terrain surface collision data.
        /// Deprecated and no longer used.
        /// </summary>
        /// <remarks>Source 2004</remarks>
        LUMP_PHYSCOLLIDESURFACE = 49,

        /// <summary>
        /// Static prop triangle and string data
        /// </summary>
        /// <remarks>Source (L4D2 Branch)</remarks>
        LUMP_PROP_BLOB = 49,

        /// <summary>
        /// Tied to any entity that uses the overlay_transition helper in FGD
        /// </summary>
        LUMP_WATEROVERLAYS = 50,

        /// <summary>
        /// Alternate lightdata implementation for Xbox
        /// </summary>
        /// <remarks>Source 2006</remarks>
        LUMP_LIGHTMAPPAGES = 51,

        /// <summary>
        /// Index of LUMP_LEAF_AMBIENT_LIGHTING_HDR
        /// </summary>
        /// <remarks>Source 2007/2009</remarks>
        LUMP_LEAF_AMBIENT_INDEX_HDR = 51,

        /// <summary>
        /// Alternate lightdata indices for Xbox
        /// </summary>
        /// <remarks>Source 2006</remarks>
        LUMP_LIGHTMAPPAGEINFOS = 52,

        /// <summary>
        /// Index of LUMP_LEAF_AMBIENT_LIGHTING
        /// </summary>
        /// <remarks>Source 2007/2009</remarks>
        LUMP_LEAF_AMBIENT_INDEX = 52,

        /// <summary>
        /// HDR lightmap samples
        /// </summary>
        LUMP_LIGHTING_HDR = 53,

        /// <summary>
        /// Internal HDR world lights converted from the entity lump
        /// </summary>
        LUMP_WORLDLIGHTS_HDR = 54,

        /// <summary>
        /// Per-leaf ambient light samples (HDR)
        /// </summary>
        LUMP_LEAF_AMBIENT_LIGHTING_HDR = 55,

        /// <summary>
        /// Per-leaf ambient light samples (LDR)
        /// </summary>
        LUMP_LEAF_AMBIENT_LIGHTING = 56,

        /// <summary>
        /// XZip version of pak file for Xbox. Deprecated.
        /// </summary>
        LUMP_XZIPPAKFILE = 57,

        /// <summary>
        /// HDR maps may have different face data
        /// </summary>
        LUMP_FACES_HDR = 58,

        /// <summary>
        /// Extended level-wide flags. Not present in all levels.
        /// </summary>
        LUMP_MAP_FLAGS = 59,

        /// <summary>
        /// Fade distances for overlays
        /// </summary>
        LUMP_OVERLAY_FADES = 60,

        /// <summary>
        /// System level settings (min/max CPU & GPU to render this overlay)
        /// </summary>
        LUMP_OVERLAY_SYSTEM_LEVELS = 61,

        /// <summary>
        /// PhysX model of the World Brush.
        /// </summary>
        LUMP_PHYSLEVEL = 62,

        /// <summary>
        /// Displacement multiblend info
        /// </summary>
        LUMP_DISP_MULTIBLEND = 63,

        #endregion
    }

    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    public enum PlaneType : int
    {
        // Plane is perpendicular to given axis
        PLANE_X = 0,
        PLANE_Y = 1,
        PLANE_Z = 2,

        // Non-axial plane is snapped to the nearest
        PLANE_ANYX = 3,
        PLANE_ANYY = 4,
        PLANE_ANYZ = 5,
    }

    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)/Static_prop_flags"/>
    [Flags]
    public enum StaticPropFlags : uint
    {
        /// <summary>
        /// Set by engine at runtime if the model fades out at a distance.
        /// </summary>
        STATIC_PROP_FLAG_FADES = 0x01,

        /// <summary>
        /// Set by engine at runtime if the model's lighting origin is
        /// different from its position in the world.
        /// </summary>
        STATIC_PROP_USE_LIGHTING_ORIGIN = 0x02,

        /// <summary>
        /// Computed at run time based on dx level
        /// </summary>
        STATIC_PROP_NO_DRAW = 0x04,

        /// <summary>
        /// Set if disableflashlight is enabled.
        /// </summary>
        STATIC_PROP_NO_FLASHLIGHT = 0x04,

        /// <summary>
        /// Set if ignorenormals is enabled.
        /// </summary>
        STATIC_PROP_IGNORE_NORMALS = 0x08,

        /// <summary>
        /// Set if disableshadows is enabled.
        /// </summary>
        STATIC_PROP_NO_SHADOW = 0x10,

        /// <summary>
        /// Set if disableshadows is enabled.
        /// </summary>
        STATIC_PROP_SCREEN_SPACE_FADE = 0x20,

        /// <summary>
        /// Set if drawinfastreflection is enabled.
        /// </summary>
        STATIC_PROP_MARKED_FOR_FAST_REFLECTION = 0x20,

        /// <summary>
        /// In vrad, compute lighting at lighting origin,
        /// not for each vertex
        /// </summary>
        STATIC_PROP_NO_PER_VERTEX_LIGHTING = 0x40,

        /// <summary>
        /// Disable self shadowing in vrad
        /// </summary>
        STATIC_PROP_NO_SELF_SHADOWING = 0x80,

        /// <summary>
        /// Whether we should do per-texel lightmaps in vrad
        /// </summary>
        STATIC_PROP_NO_PER_TEXEL_LIGHTING = 0x100,
    }

    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)/Static_prop_flags"/>
    [Flags]
    public enum StaticPropFlagsEx : uint
    {
        /// <summary>
        /// Set if disableshadowdepth is enabled.
        /// </summary>
        STATIC_PROP_FLAGS_EX_DISABLE_SHADOW_DEPTH = 0x00000001,

        /// <summary>
        /// Automatically set at runtime
        /// </summary>
        STATIC_PROP_FLAGS_EX_DISABLE_CSM = 0x00000002,

        /// <summary>
        /// Set if enablelightbounce is enabled.
        /// </summary>
        STATIC_PROP_FLAGS_EX_ENABLE_LIGHT_BOUNCE = 0x00000004,
    }

    /// <see href="https://developer.valvesoftware.com/wiki/BSP_flags_(Source)"/>
    [Flags]
    public enum SurfaceFlag : uint
    {
        /// <summary>
        /// Normally set on any surface that matches a RAD file entry;
        /// not actually written to the BSP, unlike Quake II.
        /// </summary>
        SURF_LIGHT = 0x0001,

        /// <summary>
        /// Deprecated: Legacy Quake II flag; deprecated in favor of
        /// surface properties.
        /// </summary>
        SURF_SLICK = 0x0002,

        /// <summary>
        /// Shows only the 2D skybox. Set via $Compile2DSky
        /// </summary>
        SURF_SKY2D = 0x0002,

        /// <summary>
        /// Shows both the 2D and 3D skybox. Set via $CompileSky
        /// </summary>
        SURF_SKY = 0x0004,

        /// <summary>
        /// Tells VVIS and the engine renderer that the surface is water.
        /// Set via %CompileWater, but not %CompileSlime.
        /// </summary>
        SURF_WARP = 0x0008,

        /// <summary>
        /// Surface is translucent, either via $translucent or $alpha.
        /// </summary>
        SURF_TRANS = 0x0010,

        /// <summary>
        /// Deprecated: Legacy Quake II flag; deprecated in favor of
        /// surface properties.
        /// </summary>
        SURF_WET = 0x0020,

        /// <summary>
        /// Set via %NoPortal
        /// </summary>
        SURF_NOPORTAL = 0x0020,

        /// <summary>
        /// Deprecated: Legacy Quake II flag; deprecated in favor of
        /// material proxies.
        /// </summary>
        SURF_FLOWING = 0x0040,

        /// <summary>
        /// Set via %CompileTrigger. Doesn't do anything in the PC versions.
        /// </summary>
        SURF_TRIGGER = 0x0040,

        /// <summary>
        /// Set via %CompileNoDraw
        /// </summary>
        SURF_NODRAW = 0x0080,

        /// <summary>
        /// Set via %CompileHint
        /// </summary>
        SURF_HINT = 0x0100,

        /// <summary>
        /// Set via %CompileSkip. Should never be used on anything except
        /// a hint brush.
        /// </summary>
        SURF_SKIP = 0x0200,

        /// <summary>
        /// Don't calculate light
        /// </summary>
        SURF_NOLIGHT = 0x0400,

        /// <summary>
        /// Calculate three lightmaps for the surface for bumpmapping
        /// </summary>
        SURF_BUMPLIGHT = 0x0800,

        /// <summary>
        /// Don't receive shadows
        /// </summary>
        SURF_NOSHADOWS = 0x1000,

        /// <summary>
        /// Don't receive decals
        /// </summary>
        SURF_NODECALS = 0x2000,

        /// <summary>
        /// Don't subdivide patches on this surface
        /// </summary>
        SURF_NOCHOP = 0x4000,

        /// <summary>
        /// Surface is part of a hitbox
        /// </summary>
        SURF_HITBOX = 0x8000,
    }

    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    [Flags]
    public enum TextureFlag : uint
    {
        /// <summary>
        /// Disable lightmaps and subdivision for the surface
        /// </summary>
        /// <remarks>Used by sky and liquids</remarks>
        DisableLightmaps = 0x01,
    }
}
