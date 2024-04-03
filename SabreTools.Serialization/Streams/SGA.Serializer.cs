using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class SGA : IStreamSerializer<Models.SGA.File>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.SGA.File? obj)
        {
            var serializer = new SGA();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Models.SGA.File? obj) => throw new NotImplementedException();
    }
}