using System;
using System.IO;
using SabreTools.Models.PIC;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PIC : IStreamSerializer<DiscInformation>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(DiscInformation? obj)
        {
            var serializer = new PIC();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(DiscInformation? obj) => throw new NotImplementedException();
    }
}