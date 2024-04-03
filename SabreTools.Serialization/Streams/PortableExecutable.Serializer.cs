using System;
using System.IO;
using SabreTools.Models.PortableExecutable;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PortableExecutable : IStreamSerializer<Executable>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Executable? obj)
        {
            var serializer = new PortableExecutable();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Executable? obj) => throw new NotImplementedException();
    }
}