using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class NCF : IStreamSerializer<Models.NCF.File>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Models.NCF.File? obj)
        {
            var serializer = new NCF();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Models.NCF.File? obj) => throw new NotImplementedException();
    }
}