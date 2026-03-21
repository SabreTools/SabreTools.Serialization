namespace SabreTools.Data.Models.VDF
{
    public static class Constants
    {
        /// <summary>
        /// Top-level item (and thus also first 5 bytes) of Steam2 (sis/sim/sid) retail installers
        /// </summary>
        public static readonly byte[] SteamSimSidSisSignatureBytes = [0x22, 0x53, 0x4B, 0x55, 0x22]; // "SKU"

        public static readonly string SteamSimSidSisSignatureString = "\"SKU\"";

        /// <summary>
        /// Top-level item (and thus also first 5 bytes) of Steam3 (sis/csm/csd) retail installers
        /// </summary>
        public static readonly byte[] SteamCsmCsdSisSignatureBytes = [0x22, 0x73, 0x6B, 0x75, 0x22]; // "sku"

        public static readonly string SteamCsmCsdSisSignatureString = "\"sku\"";
    }
}
