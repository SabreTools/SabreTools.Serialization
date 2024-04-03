using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class GCF : IStreamSerializer<Models.GCF.File>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.GCF.File? obj)
        {
            var serializer = new GCF();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Models.GCF.File? obj) => throw new NotImplementedException();
    }
}