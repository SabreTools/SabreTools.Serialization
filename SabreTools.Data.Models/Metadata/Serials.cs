using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("serials"), XmlRoot("serials")]
    public class Serials : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string MediaSerial1Key = "mediaserial1";

        /// <remarks>string</remarks>
        public const string MediaSerial2Key = "mediaserial2";

        /// <remarks>string</remarks>
        public const string MediaSerial3Key = "mediaserial3";

        /// <remarks>string</remarks>
        public const string PCBSerialKey = "pcbserial";

        /// <remarks>string</remarks>
        public const string RomChipSerial1Key = "romchipserial1";

        /// <remarks>string</remarks>
        public const string RomChipSerial2Key = "romchipserial2";

        /// <remarks>string</remarks>
        public const string LockoutSerialKey = "lockoutserial";

        /// <remarks>string</remarks>
        public const string SaveChipSerialKey = "savechipserial";

        /// <remarks>string</remarks>
        public const string ChipSerialKey = "chipserial";

        /// <remarks>string</remarks>
        public const string BoxSerialKey = "boxserial";

        /// <remarks>string</remarks>
        public const string MediaStampKey = "mediastamp";

        /// <remarks>string</remarks>
        public const string BoxBarcodeKey = "boxbarcode";

        /// <remarks>string</remarks>
        public const string DigitalSerial1Key = "digitalserial1";

        /// <remarks>string</remarks>
        public const string DigitalSerial2Key = "digitalserial2";

        #endregion

        public Serials() => Type = ItemType.Serials;
    }
}
