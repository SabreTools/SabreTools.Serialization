using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Readers
{
    /// <summary>
    /// Base class for all binary deserializers
    /// </summary>
    /// <typeparam name="TModel">Type of the model to deserialize</typeparam>
    /// <remarks>
    /// This class allows all inheriting types to only implement <see cref="IStreamReader<>"/>
    /// and still implicitly implement <see cref="IByteReader<>"/>  and <see cref="IFileReader<>"/> 
    /// </remarks>
    public abstract class BaseBinaryReader<TModel> :
        IByteReader<TModel>,
        IFileReader<TModel>,
        IStreamReader<TModel>
    {
        #region IByteReader

        /// <inheritdoc/>
        public virtual TModel? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return default;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return default;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Deserialize(dataStream);
        }

        #endregion

        #region IFileReader

        /// <inheritdoc/>
        public virtual TModel? Deserialize(string? path)
        {
            try
            {
                // If we don't have a file
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return default;

                // Open the file for deserialization
                using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return Deserialize(stream);
            }
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
        }

        #endregion

        #region IStreamReader

        /// <inheritdoc/>
        public abstract TModel? Deserialize(Stream? data);

        #endregion
    }
}
