using System;
using System.IO;
using SabreTools.Models.PortableExecutable;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PortableExecutable : IStreamSerializer<Executable>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Executable? obj)
        {
            var serializer = new PortableExecutable();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Executable? obj) => throw new NotImplementedException();
    }
}