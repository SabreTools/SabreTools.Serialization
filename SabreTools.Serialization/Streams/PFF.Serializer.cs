using System;
using System.IO;
using SabreTools.Models.PFF;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PFF : IStreamSerializer<Archive>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Archive? obj)
        {
            var serializer = new PFF();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Archive? obj) => throw new NotImplementedException();
    }
}