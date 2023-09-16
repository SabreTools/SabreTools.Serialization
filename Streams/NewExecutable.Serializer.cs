using System;
using System.IO;
using SabreTools.Models.NewExecutable;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class NewExecutable : IStreamSerializer<Executable>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(Executable obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(Executable? obj) => throw new NotImplementedException();
#endif
    }
}