using System;
using System.IO;
using SabreTools.Models.PlayJ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PlayJPlaylist : IStreamSerializer<Playlist>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Playlist? obj)
        {
            var serializer = new PlayJPlaylist();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Playlist? obj) => throw new NotImplementedException();
    }
}