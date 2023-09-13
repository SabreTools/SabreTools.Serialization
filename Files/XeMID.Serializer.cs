using System;

namespace SabreTools.Serialization.Files
{
    public partial class XeMID : IFileSerializer<Models.Xbox.XeMID>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(Models.Xbox.XeMID obj, string path) => throw new NotImplementedException();
#else
        public bool Serialize(Models.Xbox.XeMID? obj, string? path) => throw new NotImplementedException();
#endif
    }
}