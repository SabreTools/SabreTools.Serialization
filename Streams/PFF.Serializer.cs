using System;
using System.IO;
using SabreTools.Models.PFF;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PFF : IStreamSerializer<Archive>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Archive? obj) => throw new NotImplementedException();
    }
}