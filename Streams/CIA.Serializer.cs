using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class CIA : IStreamSerializer<Models.N3DS.CIA>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Models.N3DS.CIA? obj) => throw new NotImplementedException();
    }
}