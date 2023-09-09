using System;
using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class VBSP : IStreamSerializer<Models.VBSP.File>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Models.VBSP.File obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Models.VBSP.File? obj) => throw new NotImplementedException();
#endif
    }
}