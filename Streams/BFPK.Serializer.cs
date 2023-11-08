using System;
using System.IO;
using SabreTools.Models.BFPK;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class BFPK : IStreamSerializer<Archive>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Archive? obj) => throw new NotImplementedException();
    }
}