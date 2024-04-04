namespace SabreTools.Serialization.Serializers
{
    public class M1 :
        XmlFile<Models.Listxml.M1>
    {
        #region IFileSerializer

        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Listxml.M1? obj, string? path)
        {
            var serializer = new M1();
            return serializer.Serialize(obj, path);
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="Interfaces.IStreamSerializer.Serialize(T?)"/>
        public static System.IO.Stream? SerializeStream(Models.Listxml.M1? obj)
        {
            var serializer = new M1();
            return serializer.Serialize(obj);
        }

        #endregion
    }
}