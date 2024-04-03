using System;
using System.IO;
using SabreTools.Models.AACS;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class AACS : IStreamSerializer<MediaKeyBlock>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(MediaKeyBlock? obj)
        {
            var serializer = new AACS();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(MediaKeyBlock? obj) => throw new NotImplementedException();
    }
}