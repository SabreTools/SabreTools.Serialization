using System;
using System.IO;
using SabreTools.Models.Nitro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Nitro : IStreamSerializer<Cart>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Cart? obj)
        {
            var serializer = new Nitro();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Cart? obj) => throw new NotImplementedException();
    }
}