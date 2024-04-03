using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class VBSP : IStreamSerializer<Models.VBSP.File>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.VBSP.File? obj)
        {
            var serializer = new VBSP();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Models.VBSP.File? obj) => throw new NotImplementedException();
    }
}