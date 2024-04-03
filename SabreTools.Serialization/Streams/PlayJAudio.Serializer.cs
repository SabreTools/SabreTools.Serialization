using System;
using System.IO;
using SabreTools.Models.PlayJ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PlayJAudio : IStreamSerializer<AudioFile>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(AudioFile? obj)
        {
            var serializer = new PlayJAudio();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(AudioFile? obj) => throw new NotImplementedException();
    }
}