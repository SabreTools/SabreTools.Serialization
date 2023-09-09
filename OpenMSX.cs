namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for OpenMSX software database files
    /// </summary>
    public static class OpenMSX
    {
        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string DocTypeName = "softwaredb";

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
        public const string DocTypeSysId = "softwaredb1.dtd";

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