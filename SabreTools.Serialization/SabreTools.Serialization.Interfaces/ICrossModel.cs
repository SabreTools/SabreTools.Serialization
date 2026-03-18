namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to convert between two model types
    /// </summary>
    public interface ICrossModel<TSource, TDest>
    {
        /// <summary>
        /// Enable outputting debug information
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Deserialize a <typeparamref name="TDest"/> into <typeparamref name="TSource"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <typeparam name="U">Type of object to deserialize from</typeparam>
        /// <param name="obj">Object to deserialize from</param>
        /// <returns>Filled object on success, null on error</returns>
        public TSource? Deserialize(TDest? obj);

        /// <summary>
        /// Serialize a <typeparamref name="TSource"/> into <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <typeparam name="U">Type of object to serialize to</typeparam>
        /// <param name="obj">Object to serialize from</param>
        /// <returns>Filled object on success, null on error</returns>
        public TDest? Serialize(TSource? obj);
    }
}
