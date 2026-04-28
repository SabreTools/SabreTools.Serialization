using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.PCEngineCDROM;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class PCEngineCDROM : WrapperBase<Header>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PC Engine CD-ROM² / TurboGrafx-CD Header";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Header.BootSector"/>
        public BootSector BootSector => Model.BootSector;

        /// <inheritdoc cref="Header.IPL"/>
        public IPL IPL => Model.IPL;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PCEngineCDROM(Header model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public PCEngineCDROM(Header model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PCEngineCDROM(Header model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public PCEngineCDROM(Header model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public PCEngineCDROM(Header model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PCEngineCDROM(Header model, Stream data, long offset, long length) : base(model, data, offset, length) { }

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
                // Quit early if no data in stream
                if (data.Length - data.Position < 2 * Constants.SectorSize)
                    return null;

                // PC Engine CD-ROM header can exist after some amount of pre-gap (check 250 sectors)
                for (int i = 0; i < 250; i++)
                {
                    byte[] startBytes = data.PeekBytes(16);
                    if (startBytes.EqualsExactly(Constants.MagicBytes))
                        break;
                    else if (startBytes.EqualsExactly(Constants.PregapBytes) && data.Length - data.Position >= 3 * Constants.SectorSize)
                        data.SeekIfPossible(Constants.SectorSize, SeekOrigin.Current);
                    else
                        return null;
                }

                // Cache the current offset (after the pregap)
                long currentOffset = data.Position;

                var model = new Serialization.Readers.PCEngineCDROM().Deserialize(data);
                if (model is null)
                    return null;

                // Reset stream
                data.Seek(currentOffset, SeekOrigin.Begin);

                return new PCEngineCDROM(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
