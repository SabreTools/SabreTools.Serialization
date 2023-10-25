using System;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Strings
{
    public partial class XMID : IStringSerializer<Models.Xbox.XMID>
    {
        /// <inheritdoc/>
#if NET48
        public string Serialize(Models.Xbox.XMID obj) => throw new NotImplementedException();
#else
        public string? Serialize(Models.Xbox.XMID? obj) => throw new NotImplementedException();
#endif
    }
}