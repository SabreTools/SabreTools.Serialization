using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class CueSheet : IStreamSerializer<Models.CueSheets.CueSheet>
    {
        /// <inheritdoc/>
#if NET48
        public Models.CueSheets.CueSheet Deserialize(Stream data) => throw new NotImplementedException();
#else
        public Models.CueSheets.CueSheet? Deserialize(Stream? data) => throw new NotImplementedException();
#endif
    }
}