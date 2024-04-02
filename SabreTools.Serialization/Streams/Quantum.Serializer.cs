using System;
using System.IO;
using SabreTools.Models.Quantum;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Quantum : IStreamSerializer<Archive>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Archive? obj) => throw new NotImplementedException();
    }
}