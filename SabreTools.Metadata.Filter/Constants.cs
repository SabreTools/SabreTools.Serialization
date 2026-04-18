namespace SabreTools.Metadata.Filter
{
    public static class Constants
    {
        #region Per-Type Accepted Keys

        /// <summary>
        /// Known keys for Adjuster
        /// </summary>
        public static readonly string[] AdjusterKeys =
        [
            "condition.mask",
            "condition.relation",
            "condition.tag",
            "condition.value",
            "default",
            "name"
        ];

        /// <summary>
        /// Known keys for Archive
        /// </summary>
        public static readonly string[] ArchiveKeys =
        [
            "additional",
            "adult",
            "alt",
            "bios",
            "categories",
            "clone",
            "clonetag",
            "complete",
            "dat",
            "datternote",
            "description",
            "devstatus",
            "gameid1",
            "gameid2",
            "langchecked",
            "languages",
            "licensed",
            "listed",
            "mergeof",
            "mergename",
            "name",
            "namealt",
            "number",
            "physical",
            "pirate",
            "private",
            "region",
            "regparent",
            "showlang",
            "special1",
            "special2",
            "stickynote",
            "version1",
            "version2",
        ];

        /// <summary>
        /// Known keys for BiosSet
        /// </summary>
        public static readonly string[] BiossetKeys =
        [
            "default",
            "description",
            "name",
        ];

        /// <summary>
        /// Known keys for Chip
        /// </summary>
        public static readonly string[] ChipKeys =
        [
            "chiptype",
            "clock",
            "flags",
            "name",
            "soundonly",
            "tag",
        ];

        /// <summary>
        /// Known keys for Configuration
        /// </summary>
        public static readonly string[] ConfigurationKeys =
        [
            "condition.mask",
            "condition.relation",
            "condition.tag",
            "condition.value",
            "mask",
            "name",
            "tag",
        ];

        /// <summary>
        /// Known keys for ConfLocation
        /// </summary>
        public static readonly string[] ConfLocationKeys =
        [
            "inverted",
            "name",
            "number",
        ];

        /// <summary>
        /// Known keys for ConfSetting
        /// </summary>
        public static readonly string[] ConfSettingKeys =
        [
            "condition.mask",
            "condition.relation",
            "condition.tag",
            "condition.value",
            "default",
            "name",
            "value",
        ];

        /// <summary>
        /// Known keys for Control
        /// </summary>
        public static readonly string[] ControlKeys =
        [
            "buttons",
            "controltype",
            "keydelta",
            "maximum",
            "minimum",
            "player",
            "reqbuttons",
            "reverse",
            "sensitivity",
            "ways",
            "ways2",
            "ways3",
        ];

        /// <summary>
        /// Known keys for DataArea
        /// </summary>
        public static readonly string[] DataAreaKeys =
        [
            "endianness",
            "name",
            "size",
            "width",
        ];

        /// <summary>
        /// Known keys for Device
        /// </summary>
        public static readonly string[] DeviceKeys =
        [
            "devicetype",
            "extension.name",
            "fixedimage",
            "instance.briefname",
            "instance.name",
            "interface",
            "mandatory",
            "tag",
        ];

        /// <summary>
        /// Known keys for DeviceRef
        /// </summary>
        public static readonly string[] DeviceRefKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for DipLocation
        /// </summary>
        public static readonly string[] DipLocationKeys =
        [
            "inverted",
            "name",
            "number",
        ];

        /// <summary>
        /// Known keys for DipSwitch
        /// </summary>
        public static readonly string[] DipSwitchKeys =
        [
            "condition.mask",
            "condition.relation",
            "condition.tag",
            "condition.value",
            "default",
            "mask",
            "name",
            "tag",
        ];

        /// <summary>
        /// Known keys for DipSwitch
        /// </summary>
        public static readonly string[] DipValueKeys =
        [
            "condition.mask",
            "condition.relation",
            "condition.tag",
            "condition.value",
            "default",
            "name",
            "value",
        ];

        /// <summary>
        /// Known keys for Disk
        /// </summary>
        public static readonly string[] DiskKeys =
        [
            "flags",
            "index",
            "md5",
            "merge",
            "name",
            "optional",
            "region",
            "sha1",
            "status",
            "writable",
        ];

        /// <summary>
        /// Known keys for DiskArea
        /// </summary>
        public static readonly string[] DiskAreaKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for Display
        /// </summary>
        public static readonly string[] DisplayKeys =
        [
            "aspectx",
            "aspecty",
            "displaytype",
            "flipx",
            "freq",
            "hbend",
            "hbstart",
            "height",
            "htotal",
            "orientation",
            "pixclock",
            "refresh",
            "rotate",
            "screen",
            "tag",
            "vbend",
            "vbstart",
            "vtotal",
            "width",
            "x",
            "y",
        ];

        /// <summary>
        /// Known keys for Driver
        /// </summary>
        public static readonly string[] DriverKeys =
        [
            "blit",
            "cocktail",
            "color",
            "emulation",
            "incomplete",
            "nosoundhardware",
            "palettesize",
            "requiresartwork",
            "savestate",
            "sound",
            "status",
            "unofficial",
        ];

        /// <summary>
        /// Known keys for Feature/PartFeature
        /// </summary>
        public static readonly string[] FeatureKeys =
        [
            "featuretype",
            "name",
            "overall",
            "status",
            "value",
        ];

        /// <summary>
        /// Known keys for Header
        /// </summary>
        public static readonly string[] HeaderKeys =
        [
            "author",
            "biosmode",
            "build",
            "category",
            "comment",
            "date",
            "datversion",
            "debug",
            "description",
            "email",
            "emulatorversion",
            "filename",
            "forcemerging",
            "forcenodump",
            "forcepacking",
            "forcezipping",
            "header",
            "headerskipper",
            "homepage",
            "id",
            "imfolder",
            "lockbiosmode",
            "lockrommode",
            "locksamplemode",
            "mameconfig",
            "name",
            "notes",
            "plugin",
            "refname",
            "rommode",
            "romtitle",
            "rootdir",
            "samplemode",
            "schemalocation",
            "screenshotsheight",
            "screenshotswidth",
            "skipper",
            "system",
            "timestamp",
            "type",
            "url",
            "version",
        ];

        /// <summary>
        /// Known keys for Info
        /// </summary>
        public static readonly string[] InfoKeys =
        [
            "name",
            "value",
        ];

        /// <summary>
        /// Known keys for Input
        /// </summary>
        public static readonly string[] InputKeys =
        [
            "buttons",
            "coins",
            "control",
            "controlattr",
            "players",
            "service",
            "tilt",
        ];

        /// <summary>
        /// Known keys for Machine
        /// </summary>
        public static readonly string[] MachineKeys =
        [
            "board",
            "buttons",
            "category",
            "cloneof",
            "comment",
            "company",
            "control",
            "crc",
            "country",
            "description",
            "developer",
            "dirname",
            "displaycount",
            "displaytype",
            "duplicateid",
            "emulator",
            "enabled",
            "extra",
            "favorite",
            "genmsxid",
            "genre",
            "hash",
            "history",
            "id",
            "im1crc",
            "im2crc",
            "imagenumber",
            "isbios",
            "isdevice",
            "ismechanical",
            "language",
            "location",
            "manufacturer",
            "name",
            "notes",
            "playedcount",
            "playedtime",
            "players",
            "publisher",
            "ratings",
            "rebuildto",
            "relatedto",
            "releasenumber",
            "romof",
            "rotation",
            "runnable",
            "sampleof",
            "savetype",
            "score",
            "source",
            "sourcefile",
            "sourcerom",
            "status",
            "subgenre",
            "supported",
            "system",
            "tags",
            "titleid",
            "type",
            "url",
            "year",
        ];

        /// <summary>
        /// Known keys for Media
        /// </summary>
        public static readonly string[] MediaKeys =
        [
            "md5",
            "name",
            "sha1",
            "sha256",
            "spamsum",
        ];

        /// <summary>
        /// Known keys for Original
        /// </summary>
        public static readonly string[] OriginalKeys =
        [
            "content",
            "value",
        ];

        /// <summary>
        /// Known keys for Part
        /// </summary>
        public static readonly string[] PartKeys =
        [
            "interface",
            "name",
        ];

        /// <summary>
        /// Known keys for Port
        /// </summary>
        public static readonly string[] PortKeys =
        [
            "analog.mask",
            "tag",
        ];

        /// <summary>
        /// Known keys for RamOption
        /// </summary>
        public static readonly string[] RamOptionKeys =
        [
            "content",
            "default",
            "name",
        ];

        /// <summary>
        /// Known keys for Release
        /// </summary>
        public static readonly string[] ReleaseKeys =
        [
            "date",
            "default",
            "language",
            "name",
            "region",
        ];

        /// <summary>
        /// Known keys for ReleaseDetails
        /// </summary>
        public static readonly string[] ReleaseDetailsKeys =
        [
            "appendtonumber",
            "archivename",
            "category",
            "comment",
            "date",
            "dirname",
            "group",
            "id",
            "nfocrc",
            "nfoname",
            "nfosize",
            "origin",
            "originalformat",
            "region",
            "rominfo",
            "tool",
        ];

        /// <summary>
        /// Known keys for Rom
        /// </summary>
        public static readonly string[] RomKeys =
        [
            "album",
            "alt_romname",
            "alt_title",
            "altromname",
            "alttitle",
            "artist",
            "asr_detected_lang",
            "asr_detected_lang_conf",
            "asr_transcribed_lang",
            "asrdetectedlang",
            "asrdetectedlangconf",
            "asrtranscribedlang",
            "bios",
            "bitrate",
            "bittorrentmagnethash",
            "btih",
            "cloth_cover_detection_module_version",
            "clothcoverdetectionmoduleversion",
            "collection-catalog-number",
            "collectioncatalognumber",
            "comment",
            "crc",
            "crc16",
            "crc32",
            "crc64",
            "creator",
            "date",
            "dispose",
            "extension",
            "filecount",
            "fileisavailable",
            "flags",
            "format",
            "header",
            "height",
            "hocr_char_to_word_hocr_version",
            "hocr_char_to_word_module_version",
            "hocr_fts_text_hocr_version",
            "hocr_fts_text_module_version",
            "hocr_pageindex_hocr_version",
            "hocr_pageindex_module_version",
            "hocrchartowordhocrversion",
            "hocrchartowordmoduleversion",
            "hocrftstexthocrversion",
            "hocrftstextmoduleversion",
            "hocrpageindexhocrversion",
            "hocrpageindexmoduleversion",
            "inverted",
            "lastmodifiedtime",
            "length",
            "loadflag",
            "matrix_number",
            "matrixnumber",
            "md2",
            "md4",
            "md5",
            "mediatype",
            "merge",
            "mia",
            "mtime",
            "name",
            "ocr",
            "ocr_converted",
            "ocr_detected_lang",
            "ocr_detected_lang_conf",
            "ocr_detected_script",
            "ocr_detected_script_conf",
            "ocr_module_version",
            "ocr_parameters",
            "offset",
            "openmsxmediatype",
            "openmsxtype",
            "optional",
            "original",
            "pdf_module_version",
            "pdfmoduleversion",
            "preview-image",
            "previewimage",
            "publisher",
            "region",
            "remark",
            "ripemd128",
            "ripemd160",
            "rotation",
            "serial",
            "sha1",
            "sha256",
            "sha384",
            "sha512",
            "size",
            "soundonly",
            "source",
            "spamsum",
            "start",
            "status",
            "summation",
            "tesseractocr",
            "tesseractocrconverted",
            "tesseractocrdetectedlang",
            "tesseractocrdetectedlangconf",
            "tesseractocrdetectedscript",
            "tesseractocrdetectedscriptconf",
            "tesseractocrmoduleversion",
            "tesseractocrparameters",
            "title",
            "track",
            "value",
            "whisper_asr_module_version",
            "whisper_model_hash",
            "whisper_model_name",
            "whisper_version",
            "whisperasrmoduleversion",
            "whispermodelhash",
            "whispermodelname",
            "whisperversion",
            "width",
            "word_conf_0_10",
            "word_conf_11_20",
            "word_conf_21_30",
            "word_conf_31_40",
            "word_conf_41_50",
            "word_conf_51_60",
            "word_conf_61_70",
            "word_conf_71_80",
            "word_conf_81_90",
            "word_conf_91_100",
            "wordconfidenceinterval0to10",
            "wordconfidenceinterval11to20",
            "wordconfidenceinterval21to30",
            "wordconfidenceinterval31to40",
            "wordconfidenceinterval41to50",
            "wordconfidenceinterval51to60",
            "wordconfidenceinterval61to70",
            "wordconfidenceinterval71to80",
            "wordconfidenceinterval81to90",
            "wordconfidenceinterval91to100",
            "xxhash3128",
            "xxhash364",
        ];

        /// <summary>
        /// Known keys for Sample
        /// </summary>
        public static readonly string[] SampleKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for Serials
        /// </summary>
        public static readonly string[] SerialsKeys =
        [
            "boxbarcode",
            "boxserial",
            "chipserial",
            "digitalserial1",
            "digitalserial2",
            "lockoutserial",
            "mediaserial1",
            "mediaserial2",
            "mediaserial3",
            "mediastamp",
            "pcbserial",
            "romchipserial1",
            "romchipserial2",
            "savechipserial",
        ];

        /// <summary>
        /// Known keys for SharedFeat
        /// </summary>
        public static readonly string[] SharedFeatKeys =
        [
            "name",
            "value",
        ];

        /// <summary>
        /// Known keys for Slot
        /// </summary>
        public static readonly string[] SlotKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for SlotOption
        /// </summary>
        public static readonly string[] SlotOptionKeys =
        [
            "default",
            "devname",
            "name",
        ];

        /// <summary>
        /// Known keys for SoftwareList
        /// </summary>
        public static readonly string[] SoftwareListKeys =
        [
            "filter",
            "name",
            "status",
            "tag",
        ];

        /// <summary>
        /// Known keys for Sound
        /// </summary>
        public static readonly string[] SoundKeys =
        [
            "channels",
        ];

        /// <summary>
        /// Known keys for SourceDetails
        /// </summary>
        public static readonly string[] SourceDetailsKeys =
        [
            "appendtonumber",
            "comment1",
            "comment2",
            "dumpdate",
            "dumpdateinfo",
            "dumper",
            "id",
            "link1",
            "link1public",
            "link2",
            "link2public",
            "link3",
            "link3public",
            "mediatitle",
            "nodump",
            "origin",
            "originalformat",
            "project",
            "region",
            "releasedate",
            "releasedateinfo",
            "rominfo",
            "section",
            "tool",
        ];

        /// <summary>
        /// Known keys for Video
        /// </summary>
        public static readonly string[] VideoKeys =
        [
            "aspectx",
            "aspecty",
            "displaytype",
            "freq",
            "height",
            "orientation",
            "refresh",
            "rotate",
            "screen",
            "width",
            "x",
            "y",
        ];

        #endregion
    }
}
