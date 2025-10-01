namespace SabreTools.Data.Models.BZip2
{
    public class Header
    {
        /// <summary>
        /// "BZ"
        /// </summary>
        public string? Signature { get; set; }

        /// <summary>
        /// Version byte
        /// </summary>
        /// <remarks>
        /// '0' indicates a BZ1 file
        /// 'h' indicates a BZ2 file
        /// </remarks>
        public byte Version { get; set; }

        /// <summary>
        /// ASCII value of the compression level
        /// </summary>
        /// <remarks>Valid values between '1' and '9'</remarks>
        public byte Level { get; set; }
    }
}
