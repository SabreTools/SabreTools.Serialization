using System;
using System.IO;
using SabreTools.Models.AACS;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class AACS : IStreamSerializer<MediaKeyBlock>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(MediaKeyBlock? obj)
        {
            var serializer = new AACS();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(MediaKeyBlock? obj) => throw new NotImplementedException();
    }
}