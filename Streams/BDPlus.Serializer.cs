using System;
using System.IO;
using SabreTools.Models.BDPlus;

namespace SabreTools.Serialization.Streams
{
    public partial class BDPlus : IStreamSerializer<SVM>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(SVM obj) => throw new NotImplementedException();
#else
        public Stream? Serialize(SVM? obj) => throw new NotImplementedException();
#endif
    }
}