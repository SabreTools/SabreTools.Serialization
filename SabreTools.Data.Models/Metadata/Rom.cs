using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    // TODO: IEquatable<Rom>
    [JsonObject("rom"), XmlRoot("rom")]
    public class Rom : DatItem, ICloneable
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Dispose { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public long? FileCount { get; set; }

        /// <remarks>bool; AttractMode.Row</remarks>
        public bool? FileIsAvailable { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Inverted { get; set; }

        /// <remarks>(load16_byte|load16_word|load16_word_swap|load32_byte|load32_word|load32_word_swap|load32_dword|load64_word|load64_word_swap|reload|fill|continue|reload_plain|ignore)</remarks>
        public LoadFlag? LoadFlag { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? MIA { get; set; }

        public string? Name { get; set; }

        /// <remarks>OpenMSX.RomBase</remarks>
        public OpenMSXSubType? OpenMSXMediaType { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Optional { get; set; }

        public long? Size { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? SoundOnly { get; set; }

        /// <remarks>(baddump|nodump|good|verified) "good"</remarks>
        public ItemStatus? Status { get; set; }

        public string? Value { get; set; }

        #endregion

        #region Keys

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Album { get; set; }

        /// <remarks>AttractMode.Row</remarks>
        public string? AltRomname { get; set; }

        /// <remarks>AttractMode.Row</remarks>
        public string? AltTitle { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Artist { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? ASRDetectedLang { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? ASRDetectedLangConf { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? ASRTranscribedLang { get; set; }

        public string? Bios { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Bitrate { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? BitTorrentMagnetHash { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? ClothCoverDetectionModuleVersion { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? CollectionCatalogNumber { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Comment { get; set; }

        /// <remarks>Also "crc32" in ArchiveDotOrg.File</remarks>
        public string? CRC { get; set; }

        public string? CRC16 { get; set; }

        public string? CRC64 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Creator { get; set; }

        public string? Date { get; set; }

        /// <remarks>OfflineList.FileRomCRC</remarks>
        public string? Extension { get; set; }

        public string? Flags { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Format { get; set; }

        public string? Header { get; set; }

        /// <remarks>Possibly long; ArchiveDotOrg.File</remarks>
        public string? Height { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? hOCRCharToWordhOCRVersion { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? hOCRCharToWordModuleVersion { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? hOCRFtsTexthOCRVersion { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? hOCRFtsTextModuleVersion { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? hOCRPageIndexhOCRVersion { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? hOCRPageIndexModuleVersion { get; set; }

        /// <remarks>Possibly long; ArchiveDotOrg.File</remarks>
        public string? LastModifiedTime { get; set; }

        /// <remarks>Possibly long; Also in ArchiveDotOrg.File</remarks>
        public string? Length { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? MatrixNumber { get; set; }

        public string? MD2 { get; set; }

        public string? MD4 { get; set; }

        public string? MD5 { get; set; }

        public string? Merge { get; set; }

        /// <remarks>Possibly long; Originally "offs"</remarks>
        public string? Offset { get; set; }

        /// <remarks>OpenMSX.RomBase</remarks>
        public string? OpenMSXType { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Original { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? PDFModuleVersion { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? PreviewImage { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Publisher { get; set; }

        public string? Region { get; set; }

        /// <remarks>OpenMSX.RomBase</remarks>
        public string? Remark { get; set; }

        public string? RIPEMD128 { get; set; }

        public string? RIPEMD160 { get; set; }

        /// <remarks>Possibly long; ArchiveDotOrg.File</remarks>
        public string? Rotation { get; set; }

        public string? Serial { get; set; }

        public string? SHA1 { get; set; }

        public string? SHA256 { get; set; }

        public string? SHA384 { get; set; }

        public string? SHA512 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Source { get; set; }

        public string? SpamSum { get; set; }

        /// <remarks>Possibly long; OpenMSX.RomBase</remarks>
        public string? Start { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Summation { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? TesseractOCR { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? TesseractOCRConverted { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? TesseractOCRDetectedLang { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? TesseractOCRDetectedLangConf { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? TesseractOCRDetectedScript { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? TesseractOCRDetectedScriptConf { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? TesseractOCRModuleVersion { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? TesseractOCRParameters { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? Title { get; set; }

        /// <remarks>Possibly long; ArchiveDotOrg.File</remarks>
        public string? Track { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WhisperASRModuleVersion { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WhisperModelHash { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WhisperModelName { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WhisperVersion { get; set; }

        /// <remarks>Possibly long; ArchiveDotOrg.File</remarks>
        public string? Width { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WordConfidenceInterval0To10 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WordConfidenceInterval11To20 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WordConfidenceInterval21To30 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WordConfidenceInterval31To40 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WordConfidenceInterval41To50 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WordConfidenceInterval51To60 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WordConfidenceInterval61To70 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WordConfidenceInterval71To80 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WordConfidenceInterval81To90 { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public string? WordConfidenceInterval91To100 { get; set; }

        public string? xxHash364 { get; set; }

        public string? xxHash3128 { get; set; }

        #endregion

        public Rom() => ItemType = ItemType.Rom;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Rom();

            obj.Album = Album;
            obj.AltRomname = AltRomname;
            obj.AltTitle = AltTitle;
            obj.Artist = Artist;
            obj.ASRDetectedLang = ASRDetectedLang;
            obj.ASRDetectedLangConf = ASRDetectedLangConf;
            obj.ASRTranscribedLang = ASRTranscribedLang;
            obj.Bios = Bios;
            obj.Bitrate = Bitrate;
            obj.BitTorrentMagnetHash = BitTorrentMagnetHash;
            obj.ClothCoverDetectionModuleVersion = ClothCoverDetectionModuleVersion;
            obj.CollectionCatalogNumber = CollectionCatalogNumber;
            obj.Comment = Comment;
            obj.CRC = CRC;
            obj.CRC16 = CRC16;
            obj.CRC64 = CRC64;
            obj.Creator = Creator;
            obj.Date = Date;
            obj.Dispose = Dispose;
            obj.Extension = Extension;
            obj.FileCount = FileCount;
            obj.FileIsAvailable = FileIsAvailable;
            obj.Flags = Flags;
            obj.Format = Format;
            obj.Header = Header;
            obj.Height = Height;
            obj.hOCRCharToWordhOCRVersion = hOCRCharToWordhOCRVersion;
            obj.hOCRCharToWordModuleVersion = hOCRCharToWordModuleVersion;
            obj.hOCRFtsTexthOCRVersion = hOCRFtsTexthOCRVersion;
            obj.hOCRFtsTextModuleVersion = hOCRFtsTextModuleVersion;
            obj.hOCRPageIndexhOCRVersion = hOCRPageIndexhOCRVersion;
            obj.hOCRPageIndexModuleVersion = hOCRPageIndexModuleVersion;
            obj.Inverted = Inverted;
            obj.LastModifiedTime = LastModifiedTime;
            obj.Length = Length;
            obj.LoadFlag = LoadFlag;
            obj.MatrixNumber = MatrixNumber;
            obj.MD2 = MD2;
            obj.MD4 = MD4;
            obj.MD5 = MD5;
            obj.Merge = Merge;
            obj.MIA = MIA;
            obj.Name = Name;
            obj.Offset = Offset;
            obj.OpenMSXType = OpenMSXType;
            obj.OpenMSXMediaType = OpenMSXMediaType;
            obj.Optional = Optional;
            obj.Original = Original;
            obj.PDFModuleVersion = PDFModuleVersion;
            obj.PreviewImage = PreviewImage;
            obj.Publisher = Publisher;
            obj.Region = Region;
            obj.Remark = Remark;
            obj.RIPEMD128 = RIPEMD128;
            obj.RIPEMD160 = RIPEMD160;
            obj.Rotation = Rotation;
            obj.Serial = Serial;
            obj.SHA1 = SHA1;
            obj.SHA256 = SHA256;
            obj.SHA384 = SHA384;
            obj.SHA512 = SHA512;
            obj.Size = Size;
            obj.SoundOnly = SoundOnly;
            obj.Source = Source;
            obj.SpamSum = SpamSum;
            obj.Start = Start;
            obj.Status = Status;
            obj.Summation = Summation;
            obj.TesseractOCR = TesseractOCR;
            obj.TesseractOCRConverted = TesseractOCRConverted;
            obj.TesseractOCRDetectedLang = TesseractOCRDetectedLang;
            obj.TesseractOCRDetectedLangConf = TesseractOCRDetectedLangConf;
            obj.TesseractOCRDetectedScript = TesseractOCRDetectedScript;
            obj.TesseractOCRDetectedScriptConf = TesseractOCRDetectedScriptConf;
            obj.TesseractOCRModuleVersion = TesseractOCRModuleVersion;
            obj.TesseractOCRParameters = TesseractOCRParameters;
            obj.Title = Title;
            obj.Track = Track;
            obj.Value = Value;
            obj.WhisperASRModuleVersion = WhisperASRModuleVersion;
            obj.WhisperModelHash = WhisperModelHash;
            obj.WhisperModelName = WhisperModelName;
            obj.WhisperVersion = WhisperVersion;
            obj.Width = Width;
            obj.WordConfidenceInterval0To10 = WordConfidenceInterval0To10;
            obj.WordConfidenceInterval11To20 = WordConfidenceInterval11To20;
            obj.WordConfidenceInterval21To30 = WordConfidenceInterval21To30;
            obj.WordConfidenceInterval31To40 = WordConfidenceInterval31To40;
            obj.WordConfidenceInterval41To50 = WordConfidenceInterval41To50;
            obj.WordConfidenceInterval51To60 = WordConfidenceInterval51To60;
            obj.WordConfidenceInterval61To70 = WordConfidenceInterval61To70;
            obj.WordConfidenceInterval71To80 = WordConfidenceInterval71To80;
            obj.WordConfidenceInterval81To90 = WordConfidenceInterval81To90;
            obj.WordConfidenceInterval91To100 = WordConfidenceInterval91To100;
            obj.xxHash364 = xxHash364;
            obj.xxHash3128 = xxHash3128;

            return obj;
        }
    }
}
