using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class NCF : IStreamSerializer<Models.NCF.File>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Models.NCF.File? obj) => throw new NotImplementedException();
    }
}