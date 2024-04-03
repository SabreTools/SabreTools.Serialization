using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PAK : IStreamSerializer<Models.PAK.File>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Models.PAK.File? obj)
        {
            var serializer = new PAK();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Models.PAK.File? obj) => throw new NotImplementedException();
    }
}