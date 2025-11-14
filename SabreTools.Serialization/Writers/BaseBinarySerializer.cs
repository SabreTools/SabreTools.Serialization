using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Writers
{
    /// <summary>
    /// Base class for all binary serializers
    /// </summary>
    /// <typeparam name="TModel">Type of the model to serialize</typeparam>
    /// <remarks>
    /// This class allows all inheriting types to only implement <see cref="IStreamWriter<>"/>
    /// and still implicitly implement <see cref="IByteWriter<>"/>  and <see cref="IFileWriter<>"/>
    /// </remarks>
    public abstract class BaseBinaryWriter<TModel> :
        IByteWriter<TModel>,
        IFileWriter<TModel>,
        IStreamWriter<TModel>
    {
        #region IByteWriter

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

        #region IFileWriter

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

        #region IStreamWriter

        /// <inheritdoc/>
        public abstract Stream? SerializeStream(TModel? obj);

        #endregion
    }
}
