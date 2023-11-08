using System;
using System.IO;
using SabreTools.Models.MSDOS;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class MSDOS : IStreamSerializer<Executable>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Executable? obj) => throw new NotImplementedException();
    }
}