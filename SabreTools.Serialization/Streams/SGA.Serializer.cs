using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class SGA : IStreamSerializer<Models.SGA.File>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Models.SGA.File? obj)
        {
            var serializer = new SGA();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Models.SGA.File? obj) => throw new NotImplementedException();
    }
}