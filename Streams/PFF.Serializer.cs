using System;
using System.IO;
using SabreTools.Models.PFF;

namespace SabreTools.Serialization.Streams
{
    public partial class PFF : IStreamSerializer<Archive>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Archive obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Archive? obj) => throw new NotImplementedException();
#endif
    }
}