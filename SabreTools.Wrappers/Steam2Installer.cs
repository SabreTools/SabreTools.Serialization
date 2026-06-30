using System.IO;
using SabreTools.Data.Models.Steam2Installer;

namespace SabreTools.Wrappers
{
    public partial class Steam2Installer : WrapperBase<Steam2InstallerSet>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Steam2 Installer Set";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Steam2InstallerSet.Sim"/>
        public SimFile Sim => Model.Sim;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public Steam2Installer(Steam2InstallerSet model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public Steam2Installer(Steam2InstallerSet model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Steam2Installer(Steam2InstallerSet model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public Steam2Installer(Steam2InstallerSet model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public Steam2Installer(Steam2InstallerSet model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Steam2Installer(Steam2InstallerSet model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a Steam2 Installer Set from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the sim file</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Steam2 Installer Set wrapper on success, null on failure</returns>
        public static Steam2Installer? Create(byte[]? data, int offset)
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
        /// Create a Steam2 Installer Set from a Stream
        /// </summary>
        /// <param name="data">Stream representing the sim file</param>
        /// <returns>A Steam2 Installer Set wrapper on success, null on failure</returns>
        public static Steam2Installer? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Serialization.Readers.Steam2Installer().Deserialize(data);
                if (model is null)
                    return null;

                return new Steam2Installer(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
