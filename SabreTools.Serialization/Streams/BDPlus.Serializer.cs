using System;
using System.IO;
using SabreTools.Models.BDPlus;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class BDPlus : IStreamSerializer<SVM>
    {
        /// <inheritdoc/>
        public Stream? Serialize(SVM? obj) => throw new NotImplementedException();
    }
}