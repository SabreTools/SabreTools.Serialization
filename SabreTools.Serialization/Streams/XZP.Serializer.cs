using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class XZP : IStreamSerializer<Models.XZP.File>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.XZP.File? obj)
        {
            var serializer = new XZP();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Models.XZP.File? obj) => throw new NotImplementedException();
    }
}