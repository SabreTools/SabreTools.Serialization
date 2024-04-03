namespace SabreTools.Serialization.Streams
{
    public partial class Listxml : XmlFile<Models.Listxml.Mame>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.Serialize(T?)"/>
        public static System.IO.Stream? SerializeStream(Models.Listxml.Mame? obj)
        {
            var serializer = new Listxml();
            return serializer.Serialize(obj);
        }
    }
}