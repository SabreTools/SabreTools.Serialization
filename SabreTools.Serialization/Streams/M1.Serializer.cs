namespace SabreTools.Serialization.Streams
{
    public partial class M1 : XmlFile<Models.Listxml.M1>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.SerializeImpl(T?)"/>
        public static System.IO.Stream? Serialize(Models.Listxml.M1? obj)
        {
            var serializer = new M1();
            return serializer.SerializeImpl(obj);
        }
    }
}