using System;
using System.IO;
using SabreTools.Models.LinearExecutable;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class LinearExecutable : IStreamSerializer<Executable>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Executable? obj) => throw new NotImplementedException();
    }
}