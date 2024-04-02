using System;
using System.IO;
using SabreTools.Models.PlayJ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PlayJPlaylist : IStreamSerializer<Playlist>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Playlist? obj) => throw new NotImplementedException();
    }
}