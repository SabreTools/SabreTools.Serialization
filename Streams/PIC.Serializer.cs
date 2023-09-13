using System;
using System.IO;
using SabreTools.Models.PIC;

namespace SabreTools.Serialization.Streams
{
    public partial class PIC : IStreamSerializer<DiscInformation>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(DiscInformation obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(DiscInformation? obj) => throw new NotImplementedException();
#endif
    }
}