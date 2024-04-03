using System;
using System.IO;
using SabreTools.Models.MoPaQ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class MoPaQ : IStreamSerializer<Archive>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Archive? obj)
        {
            var serializer = new MoPaQ();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Archive? obj) => throw new NotImplementedException();
    }
}