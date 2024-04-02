using System;
using System.IO;
using SabreTools.Models.PlayJ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PlayJAudio : IStreamSerializer<AudioFile>
    {
        /// <inheritdoc/>
        public Stream? Serialize(AudioFile? obj) => throw new NotImplementedException();
    }
}