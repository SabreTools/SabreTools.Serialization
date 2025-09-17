using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class PlayJAudioFile : WrapperBase<Models.PlayJ.AudioFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PlayJ Audio File (PLJ)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PlayJAudioFile(Models.PlayJ.AudioFile model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public PlayJAudioFile(Models.PlayJ.AudioFile model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PlayJAudioFile(Models.PlayJ.AudioFile model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public PlayJAudioFile(Models.PlayJ.AudioFile model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public PlayJAudioFile(Models.PlayJ.AudioFile model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PlayJAudioFile(Models.PlayJ.AudioFile model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a PlayJ audio file from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the audio file</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PlayJ audio file wrapper on success, null on failure</returns>
        public static PlayJAudioFile? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a PlayJ audio file from a Stream
        /// </summary>
        /// <param name="data">Stream representing the audio file</param>
        /// <returns>A PlayJ audio file wrapper on success, null on failure</returns>
        public static PlayJAudioFile? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.PlayJAudio.DeserializeStream(data);
                if (model == null)
                    return null;

                return new PlayJAudioFile(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
