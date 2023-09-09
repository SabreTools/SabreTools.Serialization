using System;
using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class NCF : IStreamSerializer<Models.NCF.File>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Models.NCF.File obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Models.NCF.File? obj) => throw new NotImplementedException();
#endif
    }
}