using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PAK : IStreamSerializer<Models.PAK.File>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.PAK.File? obj)
        {
            var serializer = new PAK();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Models.PAK.File? obj) => throw new NotImplementedException();
    }
}