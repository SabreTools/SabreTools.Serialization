using System;
using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class PAK : IStreamSerializer<Models.PAK.File>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Models.PAK.File obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Models.PAK.File? obj) => throw new NotImplementedException();
#endif
    }
}