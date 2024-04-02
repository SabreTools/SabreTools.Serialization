using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class XZP : IStreamSerializer<Models.XZP.File>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Models.XZP.File? obj) => throw new NotImplementedException();
    }
}