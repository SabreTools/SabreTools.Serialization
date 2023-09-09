using System;
using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class SGA : IStreamSerializer<Models.SGA.File>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Models.SGA.File obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Models.SGA.File? obj) => throw new NotImplementedException();
#endif
    }
}