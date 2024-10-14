using System;
using System.IO;
using SabreTools.Models.N3DS;

namespace SabreTools.Serialization.Wrappers
{
    public class N3DS : WrapperBase<Models.N3DS.Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo 3DS Cart Image";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public N3DS(Models.N3DS.Cart? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public N3DS(Models.N3DS.Cart? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a 3DS cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
        public static N3DS? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a 3DS cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
        public static N3DS? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = Deserializers.N3DS.DeserializeStream(data);
            if (archive == null)
                return null;

            try
            {
                return new N3DS(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        // TODO: Hook these up for external use
        #region Currently Unused Extensions

        #region ExeFSFileHeader

        /// <summary>
        /// Determines if a file header represents a CODE block
        /// </summary>
        public static bool IsCodeBinary(ExeFSFileHeader? header)
        {
            if (header == null)
                return false;

            return header.FileName == ".code\0\0\0";
        }

        #endregion

        #region NCCHHeaderFlags

        /// <summary>
        /// Get if the NoCrypto bit is set
        /// </summary>
        public static bool PossblyDecrypted(NCCHHeaderFlags flags)
        {
            if (flags == null)
                return false;

            return flags.BitMasks.HasFlag(BitMasks.NoCrypto);
        }

        #endregion

        #region NCSDHeader

        /// <summary>
        /// Partition table entry for Executable Content (CXI)
        /// </summary>
        public static PartitionTableEntry? ExecutableContent(NCSDHeader? header)
        {
            if (header?.PartitionsTable == null)
                return null;

            return header.PartitionsTable[0];
        }

        /// <summary>
        /// Partition table entry for E-Manual (CFA)
        /// </summary>
        public static PartitionTableEntry? EManual(NCSDHeader? header)
        {
            if (header?.PartitionsTable == null)
                return null;

            return header.PartitionsTable[1];
        }

        /// <summary>
        /// Partition table entry for Download Play Child container (CFA)
        /// </summary>
        public static PartitionTableEntry? DownloadPlayChildContainer(NCSDHeader? header)
        {
            if (header?.PartitionsTable == null)
                return null;

            return header.PartitionsTable[2];
        }

        /// <summary>
        /// Partition table entry for New3DS Update Data (CFA)
        /// </summary>
        public static PartitionTableEntry? New3DSUpdateData(NCSDHeader? header)
        {
            if (header?.PartitionsTable == null)
                return null;

            return header.PartitionsTable[6];
        }

        /// <summary>
        /// Partition table entry for Update Data (CFA)
        /// </summary>
        public static PartitionTableEntry? UpdateData(NCSDHeader? header)
        {
            if (header?.PartitionsTable == null)
                return null;

            return header.PartitionsTable[7];
        }

        /// <summary>
        /// Backup Write Wait Time (The time to wait to write save to backup after the card is recognized (0-255
        /// seconds)).NATIVE_FIRM loads this flag from the gamecard NCSD header starting with 6.0.0-11.
        /// </summary>
        public static byte BackupWriteWaitTime(NCSDHeader? header)
        {
            if (header?.PartitionFlags == null)
                return default;

            return header.PartitionFlags[(int)NCSDFlags.BackupWriteWaitTime];
        }

        /// <summary>
        /// Media Card Device (1 = NOR Flash, 2 = None, 3 = BT) (SDK 3.X+)
        /// </summary>
        public static MediaCardDeviceType MediaCardDevice3X(NCSDHeader? header)
        {
            if (header?.PartitionFlags == null)
                return default;

            return (MediaCardDeviceType)header.PartitionFlags[(int)NCSDFlags.MediaCardDevice3X];
        }

        /// <summary>
        /// Media Platform Index (1 = CTR)
        /// </summary>
        public static MediaPlatformIndex MediaPlatformIndex(NCSDHeader? header)
        {
            if (header?.PartitionFlags == null)
                return default;

            return (MediaPlatformIndex)header.PartitionFlags[(int)NCSDFlags.MediaPlatformIndex];
        }

        /// <summary>
        /// Media Type Index (0 = Inner Device, 1 = Card1, 2 = Card2, 3 = Extended Device)
        /// </summary>
        public static MediaTypeIndex MediaTypeIndex(NCSDHeader? header)
        {
            if (header?.PartitionFlags == null)
                return default;

            return (MediaTypeIndex)header.PartitionFlags[(int)NCSDFlags.MediaTypeIndex];
        }

        /// <summary>
        /// Media Unit Size i.e. u32 MediaUnitSize = 0x200*2^flags[6];
        /// </summary>
        public static uint MediaUnitSize(Cart cart)
        {
            return MediaUnitSize(cart.Header);
        }

        /// <summary>
        /// Media Unit Size i.e. u32 MediaUnitSize = 0x200*2^flags[6];
        /// </summary>
        public static uint MediaUnitSize(NCSDHeader? header)
        {
            if (header?.PartitionFlags == null)
                return default;

            return (uint)(0x200 * Math.Pow(2, header.PartitionFlags[(int)NCSDFlags.MediaUnitSize]));
        }

        /// <summary>
        /// Media Card Device (1 = NOR Flash, 2 = None, 3 = BT) (Only SDK 2.X)
        /// </summary>
        public static MediaCardDeviceType MediaCardDevice2X(NCSDHeader? header)
        {
            if (header?.PartitionFlags == null)
                return default;

            return (MediaCardDeviceType)header.PartitionFlags[(int)NCSDFlags.MediaCardDevice2X];
        }

        #endregion

        #endregion
    }
}