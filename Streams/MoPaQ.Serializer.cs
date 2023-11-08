using System;
using System.IO;
using SabreTools.Models.MoPaQ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class MoPaQ : IStreamSerializer<Archive>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Archive? obj) => throw new NotImplementedException();
    }
}