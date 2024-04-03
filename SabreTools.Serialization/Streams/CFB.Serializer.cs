using System;
using System.IO;
using SabreTools.Models.CFB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class CFB : IStreamSerializer<Binary>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Binary? obj)
        {
            var serializer = new CFB();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Binary? obj) => throw new NotImplementedException();
    }
}