namespace SabreTools.Serialization.Models.PKZIP
{
    public static class Constants
    {
        #region Archive Extra Data Record

        public const uint ArchiveExtraDataRecordSignature = 0x08064B50;

        public static readonly byte[] ArchiveExtraDataRecordSignatureBytes = [0x50, 0x4B, 0x06, 0x08];

        public static readonly string ArchiveExtraDataRecordSignatureString = "PK" + (char)0x06 + (char)0x08;

        #endregion

        #region Central Directory File Header

        public const uint CentralDirectoryFileHeaderSignature = 0x02014B50;

        public static readonly byte[] CentralDirectoryFileHeaderSignatureBytes = [0x50, 0x4B, 0x01, 0x02];

        public static readonly string CentralDirectoryFileHeaderSignatureString = "PK" + (char)0x01 + (char)0x02;

        #endregion

        #region Data Descriptor

        public const uint DataDescriptorSignature = 0x08074B50;

        public static readonly byte[] DataDescriptorSignatureBytes = [0x50, 0x4B, 0x07, 0x08];

        public static readonly string DataDescriptorSignatureString = "PK" + (char)0x07 + (char)0x08;

        #endregion

        #region Digital Signature

        public const uint DigitalSignatureSignature = 0x05054B50;

        public static readonly byte[] DigitalSignatureSignatureBytes = [0x50, 0x4B, 0x05, 0x05];

        public static readonly string DigitalSignatureSignatureString = "PK" + (char)0x05 + (char)0x05;

        #endregion

        #region End of Central Directory Locator (ZIP64)

        public const uint EndOfCentralDirectoryLocator64Signature = 0x07064B50;

        public static readonly byte[] EndOfCentralDirectoryLocator64SignatureBytes = [0x50, 0x4B, 0x06, 0x07];

        public static readonly string EndOfCentralDirectoryLocator64SignatureString = "PK" + (char)0x06 + (char)0x07;

        #endregion

        #region End of Central Directory Record

        public const uint EndOfCentralDirectoryRecordSignature = 0x06054B50;

        public static readonly byte[] EndOfCentralDirectoryRecordSignatureBytes = [0x50, 0x4B, 0x05, 0x06];

        public static readonly string EndOfCentralDirectoryRecordSignatureString = "PK" + (char)0x05 + (char)0x06;

        #endregion

        #region End of Central Directory Record (ZIP64)

        public const uint EndOfCentralDirectoryRecord64Signature = 0x06064B50;

        public static readonly byte[] EndOfCentralDirectoryRecord64SignatureBytes = [0x50, 0x4B, 0x06, 0x06];

        public static readonly string EndOfCentralDirectoryRecord64SignatureString = "PK" + (char)0x06 + (char)0x06;

        #endregion

        #region Local File Header

        public const uint LocalFileHeaderSignature = 0x04034B50;

        public static readonly byte[] LocalFileHeaderSignatureBytes = [0x50, 0x4B, 0x03, 0x04];

        public static readonly string LocalFileHeaderSignatureString = "PK" + (char)0x03 + (char)0x04;

        #endregion
    }
}
