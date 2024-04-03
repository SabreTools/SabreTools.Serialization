using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class CIA : IStreamSerializer<Models.N3DS.CIA>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Models.N3DS.CIA? obj)
        {
            var serializer = new CIA();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Models.N3DS.CIA? obj) => throw new NotImplementedException();
    }
}