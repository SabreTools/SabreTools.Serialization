using System;
using System.IO;
using SabreTools.Models.AACS;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class AACS : IStreamSerializer<MediaKeyBlock>
    {
        /// <inheritdoc/>
        public Stream? Serialize(MediaKeyBlock? obj) => throw new NotImplementedException();
    }
}