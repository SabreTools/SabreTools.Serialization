using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("serials"), XmlRoot("serials")]
    public class Serials : DatItem, ICloneable, IEquatable<Serials>
    {
        #region Properties

        public string? BoxBarcode { get; set; }

        public string? BoxSerial { get; set; }

        public string? ChipSerial { get; set; }

        public string? DigitalSerial1 { get; set; }

        public string? DigitalSerial2 { get; set; }

        public string? LockoutSerial { get; set; }

        public string? MediaSerial1 { get; set; }

        public string? MediaSerial2 { get; set; }

        public string? MediaSerial3 { get; set; }

        public string? MediaStamp { get; set; }

        public string? PCBSerial { get; set; }

        public string? RomChipSerial1 { get; set; }

        public string? RomChipSerial2 { get; set; }

        public string? SaveChipSerial { get; set; }

        #endregion

        public Serials() => ItemType = ItemType.Serials;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Serials();

            obj.BoxBarcode = BoxBarcode;
            obj.BoxSerial = BoxSerial;
            obj.ChipSerial = ChipSerial;
            obj.DigitalSerial1 = DigitalSerial1;
            obj.DigitalSerial2 = DigitalSerial2;
            obj.LockoutSerial = LockoutSerial;
            obj.MediaSerial1 = MediaSerial1;
            obj.MediaSerial2 = MediaSerial2;
            obj.MediaSerial3 = MediaSerial3;
            obj.MediaStamp = MediaStamp;
            obj.PCBSerial = PCBSerial;
            obj.RomChipSerial1 = RomChipSerial1;
            obj.RomChipSerial2 = RomChipSerial2;
            obj.SaveChipSerial = SaveChipSerial;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Serials? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((BoxBarcode is null) ^ (other.BoxBarcode is null))
                return false;
            else if (BoxBarcode is not null && !BoxBarcode.Equals(other.BoxBarcode, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((BoxSerial is null) ^ (other.BoxSerial is null))
                return false;
            else if (BoxSerial is not null && !BoxSerial.Equals(other.BoxSerial, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((ChipSerial is null) ^ (other.ChipSerial is null))
                return false;
            else if (ChipSerial is not null && !ChipSerial.Equals(other.ChipSerial, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((DigitalSerial1 is null) ^ (other.DigitalSerial1 is null))
                return false;
            else if (DigitalSerial1 is not null && !DigitalSerial1.Equals(other.DigitalSerial1, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((DigitalSerial2 is null) ^ (other.DigitalSerial2 is null))
                return false;
            else if (DigitalSerial2 is not null && !DigitalSerial2.Equals(other.DigitalSerial2, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((LockoutSerial is null) ^ (other.LockoutSerial is null))
                return false;
            else if (LockoutSerial is not null && !LockoutSerial.Equals(other.LockoutSerial, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((MediaSerial1 is null) ^ (other.MediaSerial1 is null))
                return false;
            else if (MediaSerial1 is not null && !MediaSerial1.Equals(other.MediaSerial1, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((MediaSerial2 is null) ^ (other.MediaSerial2 is null))
                return false;
            else if (MediaSerial2 is not null && !MediaSerial2.Equals(other.MediaSerial2, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((MediaSerial3 is null) ^ (other.MediaSerial3 is null))
                return false;
            else if (MediaSerial3 is not null && !MediaSerial3.Equals(other.MediaSerial3, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((MediaStamp is null) ^ (other.MediaStamp is null))
                return false;
            else if (MediaStamp is not null && !MediaStamp.Equals(other.MediaStamp, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((PCBSerial is null) ^ (other.PCBSerial is null))
                return false;
            else if (PCBSerial is not null && !PCBSerial.Equals(other.PCBSerial, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((RomChipSerial1 is null) ^ (other.RomChipSerial1 is null))
                return false;
            else if (RomChipSerial1 is not null && !RomChipSerial1.Equals(other.RomChipSerial1, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((RomChipSerial2 is null) ^ (other.RomChipSerial2 is null))
                return false;
            else if (RomChipSerial2 is not null && !RomChipSerial2.Equals(other.RomChipSerial2, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((SaveChipSerial is null) ^ (other.SaveChipSerial is null))
                return false;
            else if (SaveChipSerial is not null && !SaveChipSerial.Equals(other.SaveChipSerial, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
