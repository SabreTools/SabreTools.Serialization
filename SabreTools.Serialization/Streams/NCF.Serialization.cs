using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class NCF : IStreamSerializer<Models.NCF.File>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.NCF.File? obj)
        {
            var serializer = new NCF();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Models.NCF.File? obj) => throw new NotImplementedException();
    }
}