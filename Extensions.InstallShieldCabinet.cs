using SabreTools.Models.InstallShieldCabinet;

namespace SabreTools.Serialization
{
    public static partial class Extensions
    {
        /// <summary>
        /// The major version of the cabinet
        /// </summary>
        /// <param name="cabinet">Cabinet model to derive from</param>
        /// <returns>Major version on success, -1 on error</returns>
        public static int MajorVersion(this Cabinet cabinet)
        {
            // If we have an invalid header
            if (cabinet.CommonHeader?.Version == null)
                return -1;

            uint majorVersion = cabinet.CommonHeader.Version;
            if (majorVersion >> 24 == 1)
            {
                majorVersion = (majorVersion >> 12) & 0x0F;
            }
            else if (majorVersion >> 24 == 2 || majorVersion >> 24 == 4)
            {
                majorVersion = majorVersion & 0xFFFF;
                if (majorVersion != 0)
                    majorVersion /= 100;
            }

            return (int)majorVersion;
        }
    }
}