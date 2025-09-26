using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Serializers
{
    /// <summary>
    /// Base class for all binary serializers
    /// </summary>
    /// <typeparam name="TModel">Type of the model to serialize</typeparam>
    /// <remarks>
    /// This class allows all inheriting types to only implement <see cref="IStreamSerializer<>"/>
    /// and still implicitly implement <see cref="IByteSerializer<>"/>  and <see cref="IFileSerializer<>"/> 
    /// </remarks>
    public abstract class BaseBinarySerializer<TModel> :
        IByteSerializer<TModel>,
        IFileSerializer<TModel>,
        IStreamSerializer<TModel>
    {
        #region IByteSerializer

        /// <inheritdoc/>
        public virtual byte[]? SerializeArray(TModel? obj)
        {
            using var stream = SerializeStream(obj);
            if (stream == null)
                return null;

            byte[] bytes = new byte[stream.Length];
            int read = stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        #endregion

        #region IFileSerializer

        /// <inheritdoc/>
        public virtual bool SerializeFile(TModel? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            stream.CopyTo(fs);
            fs.Flush();

            return true;
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc/>
        public abstract Stream? SerializeStream(TModel? obj);

        #endregion
    }
}
