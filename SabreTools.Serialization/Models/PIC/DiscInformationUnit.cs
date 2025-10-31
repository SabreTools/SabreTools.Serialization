namespace SabreTools.Data.Models.PIC
{
    /// <see href="https://www.t10.org/ftp/t10/document.05/05-206r0.pdf"/>
    /// <see href="https://github.com/aaru-dps/Aaru.Decoders/blob/devel/Bluray/DI.cs"/>
    public class DiscInformationUnit
    {
        /// <summary>
        /// Unit header
        /// </summary>
        public DiscInformationUnitHeader Header { get; set; }

        /// <summary>
        /// Unit body
        /// </summary>
        public DiscInformationUnitBody Body { get; set; }

        /// <summary>
        /// Unit trailer (BD-R/RE only)
        /// </summary>
        public DiscInformationUnitTrailer? Trailer { get; set; }
    }
}
