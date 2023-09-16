using System;
using System.IO;
using SabreTools.Models.N3DS;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class N3DS : IStreamSerializer<Cart>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Cart obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Cart? obj) => throw new NotImplementedException();
#endif
    }
}