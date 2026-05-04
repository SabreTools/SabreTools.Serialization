using System.IO;
using SabreTools.Data.Models.PCEngineCDROM;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class PCEngineDiscImage : WrapperBase<Header>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PC Engine CD-ROM² / TurboGrafx-CD Disc Image";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Header.BootSector"/>
        public BootSector BootSector => Model.BootSector;

        /// <inheritdoc cref="Header.IPL"/>
        public IPL IPL => Model.IPL;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PCEngineDiscImage(Header model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public PCEngineDiscImage(Header model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PCEngineDiscImage(Header model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public PCEngineDiscImage(Header model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public PCEngineDiscImage(Header model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PCEngineDiscImage(Header model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a PCEngineCDROM Header from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the PCEngineCDROM Header</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PCEngineCDROM Header wrapper on success, null on failure</returns>
        public static PCEngineCDROM? Create(byte[]? data, int offset)
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
        /// Create a PCEngineCDROM Header from a Stream
        /// </summary>
        /// <param name="data">Stream representing the PCEngineCDROM Header</param>
        /// <returns>A PCEngineCDROM Header wrapper on success, null on failure</returns>
        public static PCEngineCDROM? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Create user data sub-stream
                var userData = new Data.Extensions.CDROMExtensions.UserDataStream(data);

                // Quit early if no data in stream
                if (userData.Length - userData.Position < 2 * Constants.SectorSize)
                    return null;

                // PC Engine CD-ROM header can exist after some amount of pre-gap (check 250 sectors)
                for (int i = 0; i < 250; i++)
                {
                    byte[] startBytes = userData.PeekBytes(16);
                    if (startBytes.EqualsExactly(Constants.MagicBytes))
                        break;
                    else if (startBytes.EqualsExactly(Constants.PregapBytes) && userData.Length - userData.Position >= 3 * Constants.SectorSize)
                        userData.SeekIfPossible(Constants.SectorSize, SeekOrigin.Current);
                    else
                        return null;
                }

                // Cache the current offset (after the pregap)
                long currentOffset = userData.Position;

                var model = new Serialization.Readers.PCEngineCDROM().Deserialize(userData);
                if (model is null)
                    return null;

                // Reset stream
                userData.Seek(currentOffset, SeekOrigin.Begin);

                return new PCEngineCDROM(model, userData, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
