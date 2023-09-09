using System;
using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class CIA : IStreamSerializer<Models.N3DS.CIA>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Models.N3DS.CIA obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Models.N3DS.CIA? obj) => throw new NotImplementedException();
#endif
    }
}