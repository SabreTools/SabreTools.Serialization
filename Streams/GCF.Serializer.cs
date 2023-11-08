using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class GCF : IStreamSerializer<Models.GCF.File>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Models.GCF.File? obj) => throw new NotImplementedException();
    }
}