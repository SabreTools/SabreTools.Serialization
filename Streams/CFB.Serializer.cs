using System;
using System.IO;
using SabreTools.Models.CFB;

namespace SabreTools.Serialization.Streams
{
    public partial class CFB : IStreamSerializer<Binary>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Binary obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Binary? obj) => throw new NotImplementedException();
#endif
    }
}