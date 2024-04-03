using System;
using System.IO;
using SabreTools.Models.N3DS;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class N3DS : IStreamSerializer<Cart>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Cart? obj)
        {
            var serializer = new N3DS();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Cart? obj) => throw new NotImplementedException();
    }
}