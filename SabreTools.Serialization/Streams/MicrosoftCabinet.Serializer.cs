using System;
using System.IO;
using SabreTools.Models.MicrosoftCabinet;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class MicrosoftCabinet : IStreamSerializer<Cabinet>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Cabinet? obj)
        {
            var serializer = new MicrosoftCabinet();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Cabinet? obj) => throw new NotImplementedException();
    }
}