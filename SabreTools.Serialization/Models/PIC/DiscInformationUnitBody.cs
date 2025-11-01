namespace SabreTools.Data.Models.PIC
{
    /// <see href="https://www.t10.org/ftp/t10/document.05/05-206r0.pdf"/>
    /// <see href="https://github.com/aaru-dps/Aaru.Decoders/blob/devel/Bluray/DI.cs"/>
    /// TODO: Write models for the dependent contents, if possible
    public class DiscInformationUnitBody
    {
        /// <summary>
        /// Disc Type Identifier
        /// </summary>
        public string DiscTypeIdentifier { get; set; } = string.Empty;

        /// <summary>
        /// Disc Size/Class/Version
        /// </summary>
        public byte DiscSizeClassVersion { get; set; }

        /// <summary>
        /// DI Unit Format dependent contents
        /// </summary>
        /// <remarks>52 bytes for BD-ROM, 100 bytes for BD-R/RE</remarks>
        public byte[] FormatDependentContents { get; set; } = [];
    }
}
