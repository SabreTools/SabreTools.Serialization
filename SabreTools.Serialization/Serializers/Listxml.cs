namespace SabreTools.Serialization.Serializers
{
    public class Listxml :
        XmlFile<Models.Listxml.Mame>
    {
        #region IFileSerializer

        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Listxml.Mame? obj, string? path)
        {
            var serializer = new Listxml();
            return serializer.Serialize(obj, path);
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="Interfaces.IStreamSerializer.Serialize(T?)"/>
        public static System.IO.Stream? SerializeStream(Models.Listxml.Mame? obj)
        {
            var serializer = new Listxml();
            return serializer.Serialize(obj);
        }

        #endregion
    }
}