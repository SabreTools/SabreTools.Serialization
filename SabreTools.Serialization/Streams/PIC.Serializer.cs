using System;
using System.IO;
using SabreTools.Models.PIC;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class PIC : IStreamSerializer<DiscInformation>
    {
        /// <inheritdoc/>
        public Stream? Serialize(DiscInformation? obj) => throw new NotImplementedException();
    }
}