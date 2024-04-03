using System;
using System.IO;
using SabreTools.Models.BFPK;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class BFPK : IStreamSerializer<Archive>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Archive? obj)
        {
            var serializer = new BFPK();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Archive? obj) => throw new NotImplementedException();
    }
}