using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class BSP : IStreamSerializer<Models.BSP.File>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Models.BSP.File? obj) => throw new NotImplementedException();
    }
}