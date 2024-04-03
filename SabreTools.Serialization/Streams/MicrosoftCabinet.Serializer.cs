using System;
using System.IO;
using SabreTools.Models.MicrosoftCabinet;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class MicrosoftCabinet : IStreamSerializer<Cabinet>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Cabinet? obj)
        {
            var serializer = new MicrosoftCabinet();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Cabinet? obj) => throw new NotImplementedException();
    }
}