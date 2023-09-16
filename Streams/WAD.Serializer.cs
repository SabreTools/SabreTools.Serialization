using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class WAD : IStreamSerializer<Models.WAD.File>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Models.WAD.File obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Models.WAD.File? obj) => throw new NotImplementedException();
#endif
    }
}