using System;
using System.IO;
using SabreTools.Models.CFB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class CFB : IStreamSerializer<Binary>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Binary? obj) => throw new NotImplementedException();
    }
}