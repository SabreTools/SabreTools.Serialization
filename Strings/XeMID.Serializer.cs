using System;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Strings
{
    public partial class XeMID : IStringSerializer<Models.Xbox.XeMID>
    {
        /// <inheritdoc/>
#if NET48
        public string Serialize(Models.Xbox.XeMID obj) => throw new NotImplementedException();
#else
        public string? Serialize(Models.Xbox.XeMID? obj) => throw new NotImplementedException();
#endif
    }
}