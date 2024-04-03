using System;
using System.IO;
using SabreTools.Models.MSDOS;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class MSDOS : IStreamSerializer<Executable>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Executable? obj)
        {
            var serializer = new MSDOS();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Executable? obj) => throw new NotImplementedException();
    }
}