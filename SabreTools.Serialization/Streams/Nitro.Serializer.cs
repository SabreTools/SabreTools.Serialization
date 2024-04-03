using System;
using System.IO;
using SabreTools.Models.Nitro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Nitro : IStreamSerializer<Cart>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Cart? obj)
        {
            var serializer = new Nitro();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Cart? obj) => throw new NotImplementedException();
    }
}