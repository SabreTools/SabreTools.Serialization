using System;
using System.IO;
using SabreTools.Models.PlayJ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PlayJPlaylist : IStreamSerializer<Playlist>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Playlist? obj)
        {
            var serializer = new PlayJPlaylist();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Playlist? obj) => throw new NotImplementedException();
    }
}