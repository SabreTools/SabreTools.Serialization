using System;
using System.IO;
using SabreTools.Models.LinearExecutable;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class LinearExecutable : IStreamSerializer<Executable>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Executable? obj)
        {
            var serializer = new LinearExecutable();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Executable? obj) => throw new NotImplementedException();
    }
}