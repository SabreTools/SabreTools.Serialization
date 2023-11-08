using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class VBSP : IStreamSerializer<Models.VBSP.File>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Models.VBSP.File? obj) => throw new NotImplementedException();
    }
}