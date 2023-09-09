using System;
using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class XZP : IStreamSerializer<Models.XZP.File>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Models.XZP.File obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Models.XZP.File? obj) => throw new NotImplementedException();
#endif
    }
}