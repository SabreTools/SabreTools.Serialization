using System;

namespace SabreTools.Serialization.Files
{
    public partial class XMID : IFileSerializer<Models.Xbox.XMID>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(Models.Xbox.XMID obj, string path) => throw new NotImplementedException();
#else
        public bool Serialize(Models.Xbox.XMID? obj, string? path) => throw new NotImplementedException();
#endif
    }
}