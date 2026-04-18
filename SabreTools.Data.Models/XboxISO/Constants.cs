namespace SabreTools.Data.Models.XboxISO
{
    /// <see href="https://github.dev/Deterous/XboxKit/"/>
    public static class Constants
    {
        /// <summary>
        /// Known redump ISO lengths
        /// 0/1 = XGD1
        /// 2/3/4/5 = XGD2
        /// 6 = XGD2-Hybrid
        /// 7/8 = XGD3
        /// </summary>
        /// <see href="https://github.dev/Deterous/XboxKit/"/>
        public static readonly long[] RedumpIsoLengths = [
            0x1D330C000, 0x1D26A8000, 0x1D3301800, 0x1D2FEF800,
            0x1D3082000, 0x1D3390000, 0x1D31A0000, 0x208E05800, 0x208E03800
        ];

        /// <summary>
        /// Known XISO offsets into redump ISOs
        /// 0 = XGD1
        /// 1 = XGD2
        /// 2 = XGD2-Hybrid
        /// 3 = XGD3
        /// </summary>
        /// <see href="https://github.dev/Deterous/XboxKit/"/>
        public static readonly long[] XisoOffsets = [0x18300000, 0xFD90000, 0x89D80000, 0x2080000];

        /// <summary>
        /// Known XISO lengths from redump ISOs
        /// 0 = XGD1
        /// 1 = XGD2
        /// 2 = XGD2-Hybrid
        /// 3 = XGD3
        /// </summary>
        /// <see href="https://github.dev/Deterous/XboxKit/"/>
        public static readonly long[] XisoLengths = [0x1A2DB0000, 0x1B3880000, 0xBF8A0000, 0x204510000];
    }
}
