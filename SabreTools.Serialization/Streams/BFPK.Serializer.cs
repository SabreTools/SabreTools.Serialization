using System;
using System.IO;
using SabreTools.Models.BFPK;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class BFPK : IStreamSerializer<Archive>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Archive? obj)
        {
            var serializer = new BFPK();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Archive? obj) => throw new NotImplementedException();
    }
}