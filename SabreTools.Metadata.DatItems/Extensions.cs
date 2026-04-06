namespace SabreTools.Metadata.DatItems
{
    public static class Extensions
    {
        #region String to Enum

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, default on error</returns>
        public static MachineType AsMachineType(this string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "none" => MachineType.None,
                "bios" => MachineType.Bios,
                "device" or "dev" => MachineType.Device,
                "mechanical" or "mech" => MachineType.Mechanical,
                _ => MachineType.None,
            };
        }

        #endregion
    }
}
