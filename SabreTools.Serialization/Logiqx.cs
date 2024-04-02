namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for Logiqx-derived metadata files
    /// </summary>
    public static class Logiqx
    {
        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string DocTypeName = "datafile";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
        public const string DocTypePubId = "-//Logiqx//DTD ROM Management Datafile//EN";

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string DocTypeSysId = "http://www.logiqx.com/Dats/datafile.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
        public const string? DocTypeSubset = null;
    }
}