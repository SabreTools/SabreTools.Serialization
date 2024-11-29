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
        public PlayJAudioFile(Models.PlayJ.AudioFile? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public PlayJAudioFile(Models.PlayJ.AudioFile? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

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
            if (data == null || data.Length == 0 || !data.CanRead)
                return null;

            try
            {
                var audioFile = Deserializers.PlayJAudio.DeserializeStream(data);
                if (audioFile == null)
                    return null;

                return new PlayJAudioFile(audioFile, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}