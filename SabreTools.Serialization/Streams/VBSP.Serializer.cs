using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class VBSP : IStreamSerializer<Models.VBSP.File>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Models.VBSP.File? obj)
        {
            var serializer = new VBSP();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Models.VBSP.File? obj) => throw new NotImplementedException();
    }
}