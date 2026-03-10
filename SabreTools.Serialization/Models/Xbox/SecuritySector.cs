namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// XGD1 security sector data
    /// </summary>
    /// <see href="https://github.com/Deterous/ParseXboxMetadata/blob/main/docs/Structure%20of%20XGD1%20SS%20and%20DMI.pdf"/>
    public abstract class SecuritySector
    {
        #region PFI Common Data

        /// <summary>
        /// Version Number (bits 0-3) and Book Type (bits 4-7)
        /// </summary>
        /// <remarks>Should be 0xD1 (XGD1) or 0xE1 (XGD2/3)</remarks>
        /// TODO: Split into separate properties
        public byte VersionNumberAndBookType { get; set; }

        /// <summary>
        /// Maximum Rate (bits 0-3) and Disc Size (bits 4-7)
        /// </summary>
        /// <remarks>Should be 0x0F</remarks>
        /// TODO: Split into separate properties
        public byte MaximumRateAndDiscSize { get; set; }

        /// <summary>
        /// Layer Type (bits 0-3), Path (bit 4), Layer Count (bits 5-6),
        /// and Reserved (bit 7)
        /// </summary>
        /// <remarks>Should be 0xA1</remarks>
        /// TODO: Split into separate properties
        public byte LayerTypePathLayerCount { get; set; }

        /// <summary>
        /// Track Density (bits 0-3) and Linear Density (bits 4-7)
        /// </summary>
        /// <remarks>Should be 0x10</remarks>
        /// TODO: Split into separate properties
        public byte TrackDensityAndLinearDensity { get; set; }

        /// <summary>
        /// Data start physical sector
        /// </summary>
        /// <remarks>Big-endian(?)</remarks>
        public uint DataStartPhysicalSector { get; set; }

        /// <summary>
        /// Data end physical sector
        /// </summary>
        /// <remarks>Big-endian(?)</remarks>
        public uint DataEndPhysicalSector { get; set; }

        /// <summary>
        /// Layer 0 end physical sector
        /// </summary>
        /// <remarks>Big-endian(?)</remarks>
        public uint Layer0EndPhysicalSector { get; set; }

        /// <summary>
        /// If 0x10000000, then BCA is present.
        /// </summary>
        /// <remarks>Bits 0-6 are reserved</remarks>
        public byte BCAFlag { get; set; }

        #endregion
    }
}
