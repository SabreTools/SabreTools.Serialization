using System;
using System.IO;
using SabreTools.Models.MoPaQ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class MoPaQ : IStreamSerializer<Archive>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Archive? obj)
        {
            var serializer = new MoPaQ();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Archive? obj) => throw new NotImplementedException();
    }
}