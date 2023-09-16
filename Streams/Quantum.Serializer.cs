using System;
using System.IO;
using SabreTools.Models.Quantum;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Quantum : IStreamSerializer<Archive>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Archive obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Archive? obj) => throw new NotImplementedException();
#endif
    }
}