using SabreTools.Numerics;

namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "PN" POSIX device number, the device number of a character or block device
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    public sealed class PosixDeviceNumber : BaseEntry
    {
        /// <summary>
        /// High order 32 bits of the 64-bit device number
        /// </summary>
        public BothUInt32 DeviceNumberHigh { get; set; } = 0;

        /// <summary>
        /// Low order 32 bits of the 64-bit device number
        /// </summary>
        public BothUInt32 DeviceNumberLow { get; set; } = 0;
    }
}
