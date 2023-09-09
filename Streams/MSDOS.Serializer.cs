using System;
using System.IO;
using SabreTools.Models.MSDOS;

namespace SabreTools.Serialization.Streams
{
    public partial class MSDOS : IStreamSerializer<Executable>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Executable obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Executable? obj) => throw new NotImplementedException();
#endif
    }
}