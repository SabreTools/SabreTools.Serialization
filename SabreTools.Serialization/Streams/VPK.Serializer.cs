using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class VPK : IStreamSerializer<Models.VPK.File>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Models.VPK.File? obj)
        {
            var serializer = new VPK();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Models.VPK.File? obj) => throw new NotImplementedException();
    }
}