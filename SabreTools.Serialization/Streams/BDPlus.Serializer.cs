using System;
using System.IO;
using SabreTools.Models.BDPlus;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class BDPlus : IStreamSerializer<SVM>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(SVM? obj)
        {
            var serializer = new BDPlus();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(SVM? obj) => throw new NotImplementedException();
    }
}