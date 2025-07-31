using SabreTools.Models.InstallShieldCabinet;

namespace SabreTools.Serialization
{
    public static partial class Extensions
    {
        /// <summary>
        /// Get the major version of an InstallShield Cabinet
        /// </summary>
        /// <param name="cabinet">Cabinet to derive the version from</param>
        /// <returns>Major version of the cabinet, -1 on error</returns>
        public static int GetMajorVersion(this Cabinet? cabinet)
        {
            // Ignore invalid cabinets
            if (cabinet == null)
                return -1;

            return cabinet.CommonHeader.GetMajorVersion();
        }

        /// <summary>
        /// Get the major version of an InstallShield Cabinet
        /// </summary>
        /// <param name="commonHeader">CommonHeader to derive the version from</param>
        /// <returns>Major version of the cabinet, -1 on error</returns>
        public static int GetMajorVersion(this CommonHeader? commonHeader)
        {
            // Ignore invalid headers
            if (commonHeader == null)
                return -1;

            uint majorVersion = commonHeader.Version;
            if (majorVersion >> 24 == 1)
            {
                majorVersion = (majorVersion >> 12) & 0x0F;
            }
            else if (majorVersion >> 24 == 2 || majorVersion >> 24 == 4)
            {
                majorVersion &= 0xFFFF;
                if (majorVersion != 0)
                    majorVersion /= 100;
            }

            return (int)majorVersion;
        }
    }
}