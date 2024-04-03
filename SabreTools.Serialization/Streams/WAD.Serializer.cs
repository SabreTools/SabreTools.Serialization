using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class WAD : IStreamSerializer<Models.WAD.File>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Models.WAD.File? obj)
        {
            var serializer = new WAD();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Models.WAD.File? obj) => throw new NotImplementedException();
    }
}