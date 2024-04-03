namespace SabreTools.Serialization.Streams
{
    public partial class Listxml : XmlFile<Models.Listxml.Mame>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.SerializeImpl(T?)"/>
        public static System.IO.Stream? Serialize(Models.Listxml.Mame? obj)
        {
            var serializer = new Listxml();
            return serializer.SerializeImpl(obj);
        }
    }
}