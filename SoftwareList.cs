namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for MAME softwarelist files
    /// </summary>
    public static class SoftawreList
    {
        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string DocTypeName = "softwarelist";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
#if NET48
        public const string DocTypePubId = null;
#else
        public const string? DocTypePubId = null;
#endif

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string DocTypeSysId = "softwarelist.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
#if NET48
        public const string DocTypeSubset = null;
#else
        public const string? DocTypeSubset = null;
#endif
    }
}