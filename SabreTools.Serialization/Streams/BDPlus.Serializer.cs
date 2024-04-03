using System;
using System.IO;
using SabreTools.Models.BDPlus;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class BDPlus : IStreamSerializer<SVM>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(SVM? obj)
        {
            var serializer = new BDPlus();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(SVM? obj) => throw new NotImplementedException();
    }
}