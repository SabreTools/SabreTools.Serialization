using System;
using System.IO;
using SabreTools.Models.PIC;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PIC : IStreamSerializer<DiscInformation>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(DiscInformation? obj)
        {
            var serializer = new PIC();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(DiscInformation? obj) => throw new NotImplementedException();
    }
}