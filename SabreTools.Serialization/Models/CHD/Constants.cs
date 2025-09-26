namespace SabreTools.Serialization.Models.CHD
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = [0x4D, 0x43, 0x6F, 0x6D, 0x70, 0x72, 0x48, 0x44];

        public const string SignatureString = "MComprHD";

        #region Header Sizes

        public const int HeaderV1Size = 76;
        public const int HeaderV2Size = 80;
        public const int HeaderV3Size = 120;
        public const int HeaderV4Size = 108;
        public const int HeaderV5Size = 124;

        #endregion

        #region Metadata Parameters

        public const uint CHDMETAINDEX_APPEND = uint.MaxValue;

        #endregion
    }
}