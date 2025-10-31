namespace SabreTools.Data.Models.OLE
{
    public static class Constants
    {
        #region Format IDs

        public const string SummaryInformationFMTIDString = "F29F85E0-4FF9-1068-AB91-08002B27B3D9";
        public static readonly GUID SummaryInformationFMTIDGUID = new()
        {
            Data1 = 0xF29F85E0,
            Data2 = 0x4FF9,
            Data3 = 0x1068,
            Data4 = [0xAB, 0x91, 0x08, 0x00, 0x2B, 0x27, 0xB3, 0xD9],
        };

        public const string DocSummaryInformationFMTIDString = "D5CDD502-2E9C-101B-9397-08002B2CF9AE";
        public static readonly GUID DocSummaryInformationFMTIDGUID = new()
        {
            Data1 = 0xD5CDD502,
            Data2 = 0x2E9C,
            Data3 = 0x101B,
            Data4 = [0x93, 0x97, 0x08, 0x00, 0x2B, 0x2C, 0xF9, 0xAE],
        };

        public const string UserDefinedPropertiesFMTIDString = "D5CDD502-2E9C-101B-9397-08002B2CF9AE";
        public static readonly GUID UserDefinedPropertiesFMTIDGUID = new()
        {
            Data1 = 0xD5CDD502,
            Data2 = 0x2E9C,
            Data3 = 0x101B,
            Data4 = [0x93, 0x97, 0x08, 0x00, 0x2B, 0x2C, 0xF9, 0xAE],
        };

        public const string GlobalInfoFMTIDString = "56616F00-C154-11CE-8553-00AA00A1F95B";
        public static readonly GUID GlobalInfoFMTIDGUID = new()
        {
            Data1 = 0x56616F00,
            Data2 = 0xC154,
            Data3 = 0x11CE,
            Data4 = [0x85, 0x53, 0x00, 0xAA, 0x00, 0xA1, 0xF9, 0x5B],
        };

        public const string ImageContentsFMTIDString = "556616400-C154-11CE-8553-00AA00A1F95B";
        public static readonly GUID ImageContentsFMTIDGUID = new()
        {
            Data1 = 0x56616F00,
            Data2 = 0xC154,
            Data3 = 0x11CE,
            Data4 = [0x85, 0x53, 0x00, 0xAA, 0x00, 0xA1, 0xF9, 0x5B],
        };

        public const string ImageInfoFMTIDString = "556616400-C154-11CE-8553-00AA00A1F95B";
        public static readonly GUID ImageInfoFMTIDGUID = new()
        {
            Data1 = 0x56616F00,
            Data2 = 0xC154,
            Data3 = 0x11CE,
            Data4 = [0x85, 0x53, 0x00, 0xAA, 0x00, 0xA1, 0xF9, 0x5B],
        };

        public const string PropertyBagFMTIDString = "20001801-5DE6-11D1-8E38-00C04FB9386D";
        public static readonly GUID PropertyBagFMTIDGUID = new()
        {
            Data1 = 0x20001801,
            Data2 = 0x5DE6,
            Data3 = 0x11D1,
            Data4 = [0x8E, 0x38, 0x00, 0xC0, 0x4F, 0xB9, 0x38, 0x6D],
        };

        #endregion

        #region Stream or Storage Names

        public static readonly string SummaryInformationName = (byte)0x05 + "SummaryInformation";

        public static readonly string DocSummaryInformationName = (byte)0x05 + "DocumentSummaryInformation";

        public static readonly string UserDefinedPropertiesName = (byte)0x05 + "DocumentSummaryInformation";

        public static readonly string GlobalInfoName = (byte)0x05 + "GlobalInfo";

        public static readonly string ImageContentsName = (byte)0x05 + "ImageContents";

        public static readonly string ImageInfoName = (byte)0x05 + "ImageInfo";

        public const string ControlStreamName = "{4c8cc155-6c1e-11d1-8e41-00c04fb9386d}";

        #endregion
    }
}
