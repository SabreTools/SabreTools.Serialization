using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class BDPlus :
        IByteDeserializer<Models.BDPlus.SVM>,
        IFileDeserializer<Models.BDPlus.SVM>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.BDPlus.SVM? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new BDPlus();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public Models.BDPlus.SVM? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Streams.BDPlus.DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.BDPlus.SVM? DeserializeFile(string? path)
        {
            var deserializer = new BDPlus();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.BDPlus.SVM? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.BDPlus.DeserializeStream(stream);
        }

        #endregion
    }
}