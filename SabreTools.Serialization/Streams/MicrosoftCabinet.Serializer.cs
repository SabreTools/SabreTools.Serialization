using System;
using System.IO;
using SabreTools.Models.MicrosoftCabinet;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class MicrosoftCabinet : IStreamSerializer<Cabinet>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Cabinet? obj) => throw new NotImplementedException();
    }
}