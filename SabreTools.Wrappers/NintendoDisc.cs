using System.IO;
using SabreTools.Data.Models.NintendoDisc;

namespace SabreTools.Wrappers
{
    public partial class NintendoDisc : WrapperBase<Disc>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo GameCube / Wii Disc Image";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Disc.Header"/>
        public DiscHeader Header => Model.Header;

        /// <inheritdoc cref="Disc.Platform"/>
        public Platform Platform => Model.Platform;

        /// <inheritdoc cref="DiscHeader.GameId"/>
        public string GameId => Model.Header.GameId;

        /// <inheritdoc cref="DiscHeader.MakerCode"/>
        public string MakerCode => Model.Header.MakerCode;

        /// <inheritdoc cref="DiscHeader.GameTitle"/>
        public string GameTitle => Model.Header.GameTitle;

        /// <inheritdoc cref="DiscHeader.DiscNumber"/>
        public byte DiscNumber => Model.Header.DiscNumber;

        /// <inheritdoc cref="DiscHeader.DiscVersion"/>
        public byte DiscVersion => Model.Header.DiscVersion;

        /// <inheritdoc cref="Disc.PartitionTableEntries"/>
        public WiiPartitionTableEntry[]? PartitionTableEntries => Model.PartitionTableEntries;

        /// <inheritdoc cref="Disc.RegionData"/>
        public WiiRegionData? RegionData => Model.RegionData;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public NintendoDisc(Disc model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public NintendoDisc(Disc model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public NintendoDisc(Disc model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public NintendoDisc(Disc model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public NintendoDisc(Disc model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public NintendoDisc(Disc model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a Nintendo disc image wrapper from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the disc image</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A NintendoDisc wrapper on success, null on failure</returns>
        public static NintendoDisc? Create(byte[]? data, int offset)
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
        /// Create a Nintendo disc image wrapper from a Stream
        /// </summary>
        /// <param name="data">Stream representing the disc image</param>
        /// <returns>A NintendoDisc wrapper on success, null on failure</returns>
        public static NintendoDisc? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                long currentOffset = data.Position;

                var model = new Serialization.Readers.NintendoDisc().Deserialize(data);
                if (model is null)
                    return null;

                return new NintendoDisc(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
