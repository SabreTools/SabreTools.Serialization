using System.IO;
using SabreTools.Data.Models.InstallShieldExecutable;

namespace SabreTools.Serialization.Wrappers
{
    public partial class InstallShieldExecutable : WrapperBase<SFX>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "InstallShield Executable";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="SFX.FileEntry"/>
        public FileEntry[] Entries => Model.Entries;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public InstallShieldExecutable(SFX model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public InstallShieldExecutable(SFX model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public InstallShieldExecutable(SFX model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public InstallShieldExecutable(SFX model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public InstallShieldExecutable(SFX model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public InstallShieldExecutable(SFX model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an InstallShield Executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An executable wrapper on success, null on failure</returns>
        public static InstallShieldExecutable? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data is null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create an InstallShield Executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An executable wrapper on success, null on failure</returns>
        public static InstallShieldExecutable? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.InstallShieldExecutable().Deserialize(data);
                if (model is null)
                    return null;

                return new InstallShieldExecutable(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
