using System;
using System.IO;
using SabreTools.Models.CFB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class CFB : IStreamSerializer<Binary>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Binary? obj)
        {
            var serializer = new CFB();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Binary? obj) => throw new NotImplementedException();
    }
}