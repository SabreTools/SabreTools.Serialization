using System;
using System.IO;
using SabreTools.Models.Nitro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Nitro : IStreamSerializer<Cart>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Cart? obj) => throw new NotImplementedException();
    }
}