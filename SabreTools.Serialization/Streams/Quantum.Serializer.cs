using System;
using System.IO;
using SabreTools.Models.Quantum;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Quantum : IStreamSerializer<Archive>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Archive? obj)
        {
            var serializer = new Quantum();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Archive? obj) => throw new NotImplementedException();
    }
}