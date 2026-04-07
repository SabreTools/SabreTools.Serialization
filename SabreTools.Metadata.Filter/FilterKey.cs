using System;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Metadata.Filter
{
    /// <summary>
    /// Represents a single filter key
    /// </summary>
    public class FilterKey
    {
        #region Properties

        /// <summary>
        /// Item name associated with the filter
        /// </summary>
        public readonly string ItemName;

        /// <summary>
        /// Field name associated with the filter
        /// </summary>
        public readonly string FieldName;

        #endregion

        #region Constants

        /// <summary>
        /// Cached item type names for filter selection
        /// </summary>
#if NET5_0_OR_GREATER
        private static readonly string[] _datItemTypeNames = Enum.GetNames<ItemType>();
#else
        private static readonly string[] _datItemTypeNames = Enum.GetNames(typeof(ItemType));
#endif

        /// <summary>
        /// Known keys for Adjuster
        /// </summary>
        private static readonly string[] _adjusterKeys =
        [
            "default",
            "name"
        ];

        /// <summary>
        /// Known keys for Analog
        /// </summary>
        private static readonly string[] _analogKeys =
        [
            "mask"
        ];

        /// <summary>
        /// Known keys for Archive
        /// </summary>
        private static readonly string[] _archiveKeys =
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
        private static readonly string[] _biossetKeys =
        [
            "default",
            "description",
            "name",
        ];

        /// <summary>
        /// Known keys for Chip
        /// </summary>
        private static readonly string[] _chipKeys =
        [
            "chiptype",
            "clock",
            "flags",
            "name",
            "soundonly",
            "tag",
        ];

        /// <summary>
        /// Known keys for Condition
        /// </summary>
        private static readonly string[] _conditionKeys =
        [
            "mask",
            "relation",
            "tag",
            "value",
        ];

        /// <summary>
        /// Known keys for Configuration
        /// </summary>
        private static readonly string[] _configurationKeys =
        [
            "mask",
            "name",
            "tag",
        ];

        /// <summary>
        /// Known keys for ConfLocation
        /// </summary>
        private static readonly string[] _confLocationKeys =
        [
            "inverted",
            "name",
            "number",
        ];

        /// <summary>
        /// Known keys for ConfSetting
        /// </summary>
        private static readonly string[] _confSettingKeys =
        [
            "default",
            "name",
            "value",
        ];

        /// <summary>
        /// Known keys for Control
        /// </summary>
        private static readonly string[] _controlKeys =
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
        private static readonly string[] _dataAreaKeys =
        [
            "endianness",
            "name",
            "size",
            "width",
        ];

        /// <summary>
        /// Known keys for Device
        /// </summary>
        private static readonly string[] _deviceKeys =
        [
            "devicetype",
            "fixedimage",
            "interface",
            "mandatory",
            "tag",
        ];

        /// <summary>
        /// Known keys for DeviceRef
        /// </summary>
        private static readonly string[] _deviceRefKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for DipLocation
        /// </summary>
        private static readonly string[] _dipLocationKeys =
        [
            "inverted",
            "name",
            "number",
        ];

        /// <summary>
        /// Known keys for DipSwitch
        /// </summary>
        private static readonly string[] _dipSwitchKeys =
        [
            "default",
            "mask",
            "name",
            "tag",
        ];

        /// <summary>
        /// Known keys for DipSwitch
        /// </summary>
        private static readonly string[] _dipValueKeys =
        [
            "default",
            "name",
            "value",
        ];

        /// <summary>
        /// Known keys for Disk
        /// </summary>
        private static readonly string[] _diskKeys =
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
        private static readonly string[] _diskAreaKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for Display
        /// </summary>
        private static readonly string[] _displayKeys =
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
        private static readonly string[] _driverKeys =
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
        /// Known keys for Extension
        /// </summary>
        private static readonly string[] _extensionKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for Feature/PartFeature
        /// </summary>
        private static readonly string[] _featureKeys =
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
        private static readonly string[] _headerKeys =
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
        private static readonly string[] _infoKeys =
        [
            "name",
            "value",
        ];

        /// <summary>
        /// Known keys for Input
        /// </summary>
        private static readonly string[] _inputKeys =
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
        /// Known keys for Instance
        /// </summary>
        private static readonly string[] _instanceKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for Machine
        /// </summary>
        private static readonly string[] _machineKeys =
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
        private static readonly string[] _mediaKeys =
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
        private static readonly string[] _originalKeys =
        [
            "content",
            "value",
        ];

        /// <summary>
        /// Known keys for Part
        /// </summary>
        private static readonly string[] _partKeys =
        [
            "interface",
            "name",
        ];

        /// <summary>
        /// Known keys for Port
        /// </summary>
        private static readonly string[] _portKeys =
        [
            "tag",
        ];

        /// <summary>
        /// Known keys for RamOption
        /// </summary>
        private static readonly string[] _ramOptionKeys =
        [
            "content",
            "default",
            "name",
        ];

        /// <summary>
        /// Known keys for Release
        /// </summary>
        private static readonly string[] _releaseKeys =
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
        private static readonly string[] _releaseDetailsKeys =
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
        private static readonly string[] _romKeys =
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
        private static readonly string[] _sampleKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for Serials
        /// </summary>
        private static readonly string[] _serialsKeys =
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
        private static readonly string[] _sharedFeatKeys =
        [
            "name",
            "value",
        ];

        /// <summary>
        /// Known keys for Slot
        /// </summary>
        private static readonly string[] _slotKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for SlotOption
        /// </summary>
        private static readonly string[] _slotOptionKeys =
        [
            "default",
            "devname",
            "name",
        ];

        /// <summary>
        /// Known keys for SoftwareList
        /// </summary>
        private static readonly string[] _softwareListKeys =
        [
            "filter",
            "name",
            "status",
            "tag",
        ];

        /// <summary>
        /// Known keys for Sound
        /// </summary>
        private static readonly string[] _soundKeys =
        [
            "channels",
        ];

        /// <summary>
        /// Known keys for SourceDetails
        /// </summary>
        private static readonly string[] _sourceDetailsKeys =
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
        private static readonly string[] _videoKeys =
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

        /// <summary>
        /// Validating combined key constructor
        /// </summary>
        public FilterKey(string? key)
        {
            if (!ParseFilterId(key, out string itemName, out string fieldName))
                throw new ArgumentException($"{nameof(key)} could not be parsed", nameof(key));

            ItemName = itemName;
            FieldName = fieldName;
        }

        /// <summary>
        /// Validating discrete value constructor
        /// </summary>
        public FilterKey(string itemName, string fieldName)
        {
            if (!ParseFilterId(ref itemName, ref fieldName))
                throw new ArgumentException($"{nameof(itemName)} was not recognized", nameof(itemName));

            ItemName = itemName;
            FieldName = fieldName;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{ItemName}.{FieldName}";

        /// <summary>
        /// Parse a filter ID string into the item name and field name, if possible
        /// </summary>
        private static bool ParseFilterId(string? itemFieldString, out string itemName, out string fieldName)
        {
            // Set default values
            itemName = string.Empty; fieldName = string.Empty;

            // If we don't have a filter ID, we can't do anything
            if (string.IsNullOrEmpty(itemFieldString))
                return false;

            // If we only have one part, we can't do anything
            string[] splitFilter = itemFieldString!.Split('.');
            if (splitFilter.Length != 2)
                return false;

            // Set and sanitize the filter ID
            itemName = splitFilter[0];
            fieldName = splitFilter[1];
            return ParseFilterId(ref itemName, ref fieldName);
        }

        /// <summary>
        /// Parse a filter ID string into the item name and field name, if possible
        /// </summary>
        private static bool ParseFilterId(ref string itemName, ref string fieldName)
        {
            // If we don't have a filter ID, we can't do anything
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(fieldName))
                return false;

            // Return santized values based on the split ID
            return itemName.ToLowerInvariant() switch
            {
                // Header
                "header" => ParseHeaderFilterId(ref itemName, ref fieldName),

                // Machine
                "game" => ParseMachineFilterId(ref itemName, ref fieldName),
                "machine" => ParseMachineFilterId(ref itemName, ref fieldName),
                "resource" => ParseMachineFilterId(ref itemName, ref fieldName),
                "set" => ParseMachineFilterId(ref itemName, ref fieldName),

                // DatItem
                "datitem" => ParseDatItemFilterId(ref itemName, ref fieldName),
                "item" => ParseDatItemFilterId(ref itemName, ref fieldName),
                _ => ParseDatItemFilterId(ref itemName, ref fieldName),
            };
        }

        /// <summary>
        /// Parse and validate header fields
        /// </summary>
        private static bool ParseHeaderFilterId(ref string itemName, ref string fieldName)
        {
            // Get if there's a match to a property
            string localFieldName = fieldName;
            string? propertyMatch = Array.Find(_headerKeys, c => string.Equals(c, localFieldName, StringComparison.OrdinalIgnoreCase));
            if (propertyMatch is null)
                return false;

            // Return the sanitized ID
            itemName = "header";
            fieldName = propertyMatch.ToLowerInvariant();
            return true;
        }

        /// <summary>
        /// Parse and validate machine/game fields
        /// </summary>
        private static bool ParseMachineFilterId(ref string itemName, ref string fieldName)
        {
            // Get if there's a match to a property
            string localFieldName = fieldName;
            string? propertyMatch = Array.Find(_machineKeys, c => string.Equals(c, localFieldName, StringComparison.OrdinalIgnoreCase));
            if (propertyMatch is null)
                return false;

            // Return the sanitized ID
            itemName = "machine";
            fieldName = propertyMatch.ToLowerInvariant();
            return true;
        }

        /// <summary>
        /// Parse and validate item fields
        /// </summary>
        private static bool ParseDatItemFilterId(ref string itemName, ref string fieldName)
        {
            // Special case if the item name is reserved
            if (string.Equals(itemName, "datitem", StringComparison.OrdinalIgnoreCase)
                || string.Equals(itemName, "item", StringComparison.OrdinalIgnoreCase))
            {
                // Handle item type
                if (string.Equals(fieldName, "type", StringComparison.OrdinalIgnoreCase))
                {
                    itemName = "item";
                    fieldName = "type";
                    return true;
                }

                // If we get any matches
                string localFieldName = fieldName;
                string? matchedType = Array.Find(_datItemTypeNames, t => DatItemContainsField(t, localFieldName));
                if (matchedType is not null)
                {
                    // Check for a matching field
                    string? matchedField = GetMatchingField(matchedType, fieldName);
                    if (matchedField is null)
                        return false;

                    itemName = "item";
                    fieldName = matchedField;
                    return true;
                }
            }
            else
            {
                // Check for a matching field
                string? matchedField = GetMatchingField(itemName, fieldName);
                if (matchedField is null)
                    return false;

                itemName = itemName.ToLowerInvariant();
                fieldName = matchedField;
                return true;
            }

            // Nothing was found
            return false;
        }

        /// <summary>
        /// Determine if an item type contains a field
        /// </summary>
        private static bool DatItemContainsField(string itemName, string fieldName)
            => GetMatchingField(itemName, fieldName) is not null;

        /// <summary>
        /// Determine if an item type contains a field
        /// </summary>
        private static string? GetMatchingField(string itemName, string fieldName)
        {
            // Get the set of properties
            string[]? properties = itemName.ToLowerInvariant() switch
            {
                "adjuster" => _adjusterKeys,
                "analog" => _analogKeys,
                "archive" => _archiveKeys,
                "biosset" => _biossetKeys,
                "chip" => _chipKeys,
                "condition" => _conditionKeys,
                "configuration" => _configurationKeys,
                "conflocation" => _confLocationKeys,
                "confsetting" => _confSettingKeys,
                "control" => _controlKeys,
                "dataarea" => _dataAreaKeys,
                "device" => _deviceKeys,
                "deviceref" => _deviceRefKeys,
                "diplocation" => _dipLocationKeys,
                "dipswitch" => _dipSwitchKeys,
                "dipvalue" => _dipValueKeys,
                "disk" => _diskKeys,
                "diskarea" => _diskAreaKeys,
                "display" => _displayKeys,
                "driver" => _driverKeys,
                "extension" => _extensionKeys,
                "feature" or "partfeature" => _featureKeys,
                "game" or "machine" or "resource" or "set" => _machineKeys,
                "header" => _headerKeys,
                "info" => _infoKeys,
                "input" => _inputKeys,
                "instance" => _instanceKeys,
                "media" => _mediaKeys,
                "original" => _originalKeys,
                "part" => _partKeys,
                "port" => _portKeys,
                "ramoption" => _ramOptionKeys,
                "release" => _releaseKeys,
                "releasedetails" => _releaseDetailsKeys,
                "rom" => _romKeys,
                "sample" => _sampleKeys,
                "serials" => _serialsKeys,
                "sharedfeat" => _sharedFeatKeys,
                "slot" => _slotKeys,
                "slotoption" => _slotOptionKeys,
                "softwarelist" => _softwareListKeys,
                "sound" => _soundKeys,
                "sourcedetails" => _sourceDetailsKeys,
                "video" => _videoKeys,
                _ => null,
            };
            if (properties is null)
                return null;

            // Get if there's a match to a property
            string? propertyMatch = Array.Find(properties, c => string.Equals(c, fieldName, StringComparison.OrdinalIgnoreCase));
            return propertyMatch?.ToLowerInvariant();
        }
    }
}
