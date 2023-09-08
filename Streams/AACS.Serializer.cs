using System;
using System.IO;
using SabreTools.Models.AACS;

namespace SabreTools.Serialization.Streams
{
    public partial class AACS : IStreamSerializer<MediaKeyBlock>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(MediaKeyBlock obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(MediaKeyBlock? obj) => throw new NotImplementedException();
#endif
    }
}