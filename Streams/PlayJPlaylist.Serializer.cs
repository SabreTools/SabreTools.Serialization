using System;
using System.IO;
using SabreTools.Models.PlayJ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PlayJPlaylist : IStreamSerializer<Playlist>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Playlist obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Playlist? obj) => throw new NotImplementedException();
#endif
    }
}