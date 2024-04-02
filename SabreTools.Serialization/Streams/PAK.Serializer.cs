using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PAK : IStreamSerializer<Models.PAK.File>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Models.PAK.File? obj) => throw new NotImplementedException();
    }
}