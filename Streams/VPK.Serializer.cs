using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class VPK : IStreamSerializer<Models.VPK.File>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Models.VPK.File? obj) => throw new NotImplementedException();
    }
}