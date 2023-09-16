using System;
using System.IO;
using SabreTools.Models.MoPaQ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class MoPaQ : IStreamSerializer<Archive>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Archive obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Archive? obj) => throw new NotImplementedException();
#endif
    }
}