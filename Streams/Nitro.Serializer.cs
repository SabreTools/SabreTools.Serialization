using System;
using System.IO;
using SabreTools.Models.Nitro;

namespace SabreTools.Serialization.Streams
{
    public partial class Nitro : IStreamSerializer<Cart>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Cart obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Cart? obj) => throw new NotImplementedException();
#endif
    }
}