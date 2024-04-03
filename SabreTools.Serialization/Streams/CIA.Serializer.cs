using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class CIA : IStreamSerializer<Models.N3DS.CIA>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.N3DS.CIA? obj)
        {
            var serializer = new CIA();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Models.N3DS.CIA? obj) => throw new NotImplementedException();
    }
}