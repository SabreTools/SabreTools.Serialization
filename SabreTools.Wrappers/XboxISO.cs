using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.XboxISO;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class XboxISO : WrapperBase<DiscImage>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xbox / Xbox 360 Disc Image";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="DiscImage.VideoPartition"/>
        public SabreTools.Data.Models.ISO9660.Volume VideoPartition => Model.VideoPartition;

        /// <inheritdoc cref="DiscImage.XGDType"/>
        public int XGDType => Model.XGDType;

        /// <inheritdoc cref="DiscImage.GamePartition"/>
        public SabreTools.Data.Models.XDVDFS.Volume GamePartition => Model.GamePartition;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XboxISO(DiscImage model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public XboxISO(DiscImage model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XboxISO(DiscImage model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public XboxISO(DiscImage model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public XboxISO(DiscImage model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XboxISO(DiscImage model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an XboxISO DiscImage from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the XboxISO DiscImage</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An XboxISO DiscImage on success, null on failure</returns>
        public static XboxISO? Create(byte[]? data, int offset)
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
        /// Create an XboxISO DiscImage from a Stream
        /// </summary>
        /// <param name="data">Stream representing the XboxISO DiscImage</param>
        /// <returns>An XboxISO DiscImage DiscImage on success, null on failure</returns>
        public static XboxISO? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                // Create new model to fill in
                var model = new DiscImage();
            
                // Try to detect XDVDFS partition
                int redumpType = Array.IndexOf(Constants.RedumpIsoLengths, data.Length);
                if (redumpType < 0)
                    return null;
                
                model.XGDType = redumpType switch
                {
                    0 => 0, // XGD1
                    1 or 2 or 3 or 4 => 1, // XGD2
                    5 => 2, // XGD2 (Hybrid)
                    6 or 7 => 3, // XGD3
                    _ => -1,
                };

                // Validate XGDType
                if (model.XGDType < 0)
                    return null;

                long magicOffset = Data.Models.XDVDFS.Constants.SectorSize * Data.Models.XDVDFS.Constants.ReservedSectors;
                data.SeekIfPossible(currentOffset + magicOffset, SeekOrigin.Begin);
                var magic = data.ReadBytes(20);

                if (magic is null)
                    return null;
                if (!magic.StartsWith(Data.Models.XDVDFS.Constants.VolumeDescriptorMagic))
                    return null;

                data.SeekIfPossible(currentOffset + Constants.XisoOffsets[model.XGDType], SeekOrigin.Begin);
                var gamePartition = new Serialization.Readers.XDVDFS().Deserialize(data);
                if (gamePartition is null)
                    return null;

                model.GamePartition = gamePartition;

                // Parse the Video partition last
                var videoPartition = new Serialization.Readers.ISO9660().Deserialize(data);
                if (videoPartition is null)
                    return null;

                model.VideoPartition = videoPartition;

                var wrapper = new XboxISO(model, data, currentOffset);

                return wrapper;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
