using System;
using System.Linq;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems.Formats;
using Xunit;

namespace SabreTools.Metadata.DatFiles.Test
{
    public partial class DatFileTests
    {
        #region ConvertFromMetadata

        [Fact]
        public void ConvertFromMetadata_Null()
        {
            Data.Models.Metadata.MetadataFile? item = null;

            DatFile datFile = new Formats.Logiqx(null, useGame: false);
            datFile.ConvertFromMetadata(item, "filename", indexId: 0, keep: true, statsOnly: false, filterRunner: null);

            Assert.Equal(0, datFile.Items.DatStatistics.TotalCount);
            Assert.Equal(0, datFile.ItemsDB.DatStatistics.TotalCount);
        }

        [Fact]
        public void ConvertFromMetadata_Empty()
        {
            Data.Models.Metadata.MetadataFile? item = new Data.Models.Metadata.MetadataFile();

            DatFile datFile = new Formats.Logiqx(null, useGame: false);
            datFile.ConvertFromMetadata(item, "filename", indexId: 0, keep: true, statsOnly: false, filterRunner: null);

            Assert.Equal(0, datFile.Items.DatStatistics.TotalCount);
            Assert.Equal(0, datFile.ItemsDB.DatStatistics.TotalCount);
        }

        [Fact]
        public void ConvertFromMetadata_FilledHeader()
        {
            Data.Models.Metadata.Header? header = CreateMetadataHeader();
            Data.Models.Metadata.Machine[]? machines = null;
            Data.Models.Metadata.MetadataFile? item = new Data.Models.Metadata.MetadataFile
            {
                Header = header,
                Machine = machines,
            };

            DatFile datFile = new Formats.Logiqx(null, useGame: false);
            datFile.ConvertFromMetadata(item, "filename", indexId: 0, keep: true, statsOnly: false, filterRunner: null);

            ValidateHeader(datFile.Header);
        }

        [Fact]
        public void ConvertFromMetadata_FilledMachine()
        {
            Data.Models.Metadata.Header? header = null;
            Data.Models.Metadata.Machine machine = CreateMetadataMachine();
            Data.Models.Metadata.Machine[]? machines = [machine];
            Data.Models.Metadata.MetadataFile? item = new Data.Models.Metadata.MetadataFile
            {
                Header = header,
                Machine = machines,
            };

            DatFile datFile = new Formats.Logiqx(null, useGame: false);
            datFile.ConvertFromMetadata(item, "filename", indexId: 0, keep: true, statsOnly: false, filterRunner: null);

            // TODO: Reenable when ItemsDB is used again
            // DatItems.Machine actualMachine = Assert.Single(datFile.ItemsDB.GetMachines()).Value;
            // ValidateMachine(actualMachine);

            // Aggregate for easier validation
            DatItems.DatItem[] datItems = [.. datFile.Items.SortedKeys
                .SelectMany(key => datFile.GetItemsForBucket(key))];

            Adjuster? adjuster = Array.Find(datItems, item => item is Adjuster) as Adjuster;
            ValidateAdjuster(adjuster);

            Archive? archive = Array.Find(datItems, item => item is Archive) as Archive;
            ValidateArchive(archive);

            BiosSet? biosSet = Array.Find(datItems, item => item is BiosSet) as BiosSet;
            ValidateBiosSet(biosSet);

            Chip? chip = Array.Find(datItems, item => item is Chip) as Chip;
            ValidateChip(chip);

            Configuration? configuration = Array.Find(datItems, item => item is Configuration) as Configuration;
            ValidateConfiguration(configuration);

            Device? device = Array.Find(datItems, item => item is Device) as Device;
            ValidateDevice(device);

            DeviceRef? deviceRef = Array.Find(datItems, item => item is DeviceRef) as DeviceRef;
            ValidateDeviceRef(deviceRef);

            DipSwitch? dipSwitch = Array.Find(datItems, item => item is DipSwitch dipSwitch && dipSwitch.PartInterface is null) as DipSwitch;
            ValidateDipSwitch(dipSwitch);

            Disk? disk = Array.Find(datItems, item => item is Disk disk && disk.DiskAreaName is null && disk.PartInterface is null) as Disk;
            ValidateDisk(disk);

            Display? display = Array.Find(datItems, item => item is Display display && display.AspectX is null) as Display;
            ValidateDisplay(display);

            Driver? driver = Array.Find(datItems, item => item is Driver) as Driver;
            ValidateDriver(driver);

            // All other fields are tested separately
            Rom? dump = Array.Find(datItems, item => item is Rom rom && rom.OpenMSXMediaType is not null) as Rom;
            Assert.NotNull(dump);
            Assert.Equal(Data.Models.Metadata.OpenMSXSubType.Rom, dump.OpenMSXMediaType);

            Feature? feature = Array.Find(datItems, item => item is Feature) as Feature;
            ValidateFeature(feature);

            Info? info = Array.Find(datItems, item => item is Info) as Info;
            ValidateInfo(info);

            Input? input = Array.Find(datItems, item => item is Input) as Input;
            ValidateInput(input);

            Media? media = Array.Find(datItems, item => item is Media) as Media;
            ValidateMedia(media);

            // All other fields are tested separately
            DipSwitch? partDipSwitch = Array.Find(datItems, item => item is DipSwitch dipSwitch && dipSwitch.PartInterface is not null) as DipSwitch;
            Assert.NotNull(partDipSwitch);
            Assert.Equal("interface", partDipSwitch.PartInterface);
            Assert.Equal("name", partDipSwitch.PartName);

            // All other fields are tested separately
            Disk? partDisk = Array.Find(datItems, item => item is Disk disk && disk.DiskAreaName is not null && disk.PartInterface is not null) as Disk;
            Assert.NotNull(partDisk);
            Assert.Equal("name", partDisk.DiskAreaName);
            Assert.Equal("interface", partDisk.PartInterface);
            Assert.Equal("name", partDisk.PartName);

            PartFeature? partFeature = Array.Find(datItems, item => item is PartFeature) as PartFeature;
            ValidatePartFeature(partFeature);

            // All other fields are tested separately
            Rom? partRom = Array.Find(datItems, item => item is Rom rom && rom.DataAreaSpecified && rom.PartInterface is not null) as Rom;
            Assert.NotNull(partRom);
            DataArea? romDataArea = partRom.DataArea;
            ValidateDataArea(romDataArea);
            Assert.Equal("interface", partRom.PartInterface);
            Assert.Equal("name", partRom.PartName);

            Port? port = Array.Find(datItems, item => item is Port) as Port;
            ValidatePort(port);

            RamOption? ramOption = Array.Find(datItems, item => item is RamOption) as RamOption;
            ValidateRamOption(ramOption);

            Release? release = Array.Find(datItems, item => item is Release) as Release;
            ValidateRelease(release);

            Rom? rom = Array.Find(datItems, item => item is Rom rom && !rom.DataAreaSpecified && rom.PartInterface is null && rom.OpenMSXMediaType is null) as Rom;
            ValidateRom(rom);

            Sample? sample = Array.Find(datItems, item => item is Sample) as Sample;
            ValidateSample(sample);

            SharedFeat? sharedFeat = Array.Find(datItems, item => item is SharedFeat) as SharedFeat;
            ValidateSharedFeat(sharedFeat);

            Slot? slot = Array.Find(datItems, item => item is Slot) as Slot;
            ValidateSlot(slot);

            SoftwareList? softwareList = Array.Find(datItems, item => item is SoftwareList) as SoftwareList;
            ValidateSoftwareList(softwareList);

            Sound? sound = Array.Find(datItems, item => item is Sound) as Sound;
            ValidateSound(sound);

            Display? video = Array.Find(datItems, item => item is Display display && display.AspectX is not null) as Display;
            ValidateVideo(video);
        }

        #endregion

        #region Creation Helpers

        private static Data.Models.Metadata.Header CreateMetadataHeader()
        {
            Data.Models.OfflineList.CanOpen canOpen = new Data.Models.OfflineList.CanOpen
            {
                Extension = ["ext"],
            };

            Data.Models.OfflineList.Images images = new Data.Models.OfflineList.Images();

            Data.Models.OfflineList.Infos infos = new Data.Models.OfflineList.Infos();

            Data.Models.OfflineList.NewDat newDat = new Data.Models.OfflineList.NewDat();

            Data.Models.OfflineList.Search search = new Data.Models.OfflineList.Search();

            return new Data.Models.Metadata.Header
            {
                Author = "author",
                BiosMode = Data.Models.Metadata.MergingFlag.Merged,
                Build = "build",
                CanOpen = canOpen,
                Category = "category",
                Comment = "comment",
                Date = "date",
                DatVersion = "datversion",
                Debug = true,
                Description = "description",
                Email = "email",
                EmulatorVersion = "emulatorversion",
                FileName = "filename",
                ForceMerging = Data.Models.Metadata.MergingFlag.Merged,
                ForceNodump = Data.Models.Metadata.NodumpFlag.Required,
                ForcePacking = Data.Models.Metadata.PackingFlag.Zip,
                ForceZipping = true,
                HeaderRow = ["header"],
                HeaderSkipper = "header",
                Homepage = "homepage",
                Id = "id",
                Images = images,
                ImFolder = "imfolder",
                Infos = infos,
                LockBiosMode = true,
                LockRomMode = true,
                LockSampleMode = true,
                MameConfig = "mameconfig",
                Name = "name",
                NewDat = newDat,
                Notes = "notes",
                Plugin = "plugin",
                RefName = "refname",
                RomMode = Data.Models.Metadata.MergingFlag.Merged,
                RomTitle = "romtitle",
                RootDir = "rootdir",
                SampleMode = Data.Models.Metadata.MergingFlag.Merged,
                SchemaLocation = "schemalocation",
                ScreenshotsHeight = "screenshotsheight",
                ScreenshotsWidth = "screenshotsWidth",
                Search = search,
                System = "system",
                Timestamp = "timestamp",
                Type = "type",
                Url = "url",
                Version = "version",
            };
        }

        private static Data.Models.Metadata.Machine CreateMetadataMachine()
        {
            return new Data.Models.Metadata.Machine
            {
                Adjuster = [CreateMetadataAdjuster()],
                Archive = [CreateMetadataArchive()],
                BiosSet = [CreateMetadataBiosSet()],
                Board = "board",
                Buttons = "buttons",
                Category = ["category"],
                Chip = [CreateMetadataChip()],
                CloneOf = "cloneof",
                CloneOfId = "cloneofid",
                Comment = ["comment"],
                Company = "company",
                Configuration = [CreateMetadataConfiguration()],
                Control = "control",
                CRC = "crc32",
                Country = "country",
                Description = "description",
                Developer = "developer",
                Device = [CreateMetadataDevice()],
                DeviceRef = [CreateMetadataDeviceRef()],
                DipSwitch = [CreateMetadataDipSwitch()],
                DirName = "dirname",
                Disk = [CreateMetadataDisk()],
                DisplayCount = "displaycount",
                Display = [CreateMetadataDisplay()],
                DisplayType = "displaytype",
                Driver = CreateMetadataDriver(),
                Dump = [CreateMetadataDump()],
                DuplicateID = "duplicateid",
                Emulator = "emulator",
                Enabled = "enabled",
                Extra = "extra",
                Favorite = "favorite",
                Feature = [CreateMetadataFeature()],
                GenMSXID = "genmsxid",
                Genre = "genre",
                History = "history",
                Id = "id",
                Im1CRC = HashType.CRC32.ZeroString,
                Im2CRC = HashType.CRC32.ZeroString,
                ImageNumber = "imagenumber",
                Info = [CreateMetadataInfo()],
                Input = CreateMetadataInput(),
                IsBios = true,
                IsDevice = true,
                IsMechanical = true,
                Language = "language",
                Location = "location",
                Manufacturer = "manufacturer",
                Media = [CreateMetadataMedia()],
                Name = "name",
                Notes = "notes",
                Part = [CreateMetadataPart()],
                PlayedCount = "playedcount",
                PlayedTime = "playedtime",
                Players = "players",
                Port = [CreateMetadataPort()],
                Publisher = "publisher",
                RamOption = [CreateMetadataRamOption()],
                Ratings = "ratings",
                RebuildTo = "rebuildto",
                RelatedTo = "relatedto",
                Release = [CreateMetadataRelease()],
                ReleaseNumber = "releasenumber",
                Rom = [CreateMetadataRom()],
                RomOf = "romof",
                Rotation = "rotation",
                Runnable = Data.Models.Metadata.Runnable.Yes,
                Sample = [CreateMetadataSample()],
                SampleOf = "sampleof",
                SaveType = "savetype",
                Score = "score",
                SharedFeat = [CreateMetadataSharedFeat()],
                Slot = [CreateMetadataSlot()],
                SoftwareList = [CreateMetadataSoftwareList()],
                Sound = CreateMetadataSound(),
                Source = "source",
                SourceFile = "sourcefile",
                SourceRom = "sourcerom",
                Status = "status",
                Subgenre = "subgenre",
                Supported = Data.Models.Metadata.Supported.Yes,
                System = "system",
                Tags = "tags",
                TitleID = "titleid",
                Url = "url",
                Video = [CreateMetadataVideo()],
                Year = "year",
            };
        }

        private static Data.Models.Metadata.Adjuster CreateMetadataAdjuster()
        {
            return new Data.Models.Metadata.Adjuster
            {
                ConditionValue = "value",
                ConditionMask = "mask",
                ConditionRelation = Data.Models.Metadata.Relation.Equal,
                ConditionTag = "tag",
                Default = true,
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Archive CreateMetadataArchive()
        {
            return new Data.Models.Metadata.Archive
            {
                Number = "number",
                CloneTag = "clone",
                RegParent = "regparent",
                MergeOf = "mergeof",
                MergeName = "mergename",
                Name = "name",
                NameAlt = "name_alt",
                Region = "region",
                Languages = "languages",
                ShowLang = true,
                LangChecked = "langchecked",
                Version1 = "version1",
                Version2 = "version2",
                DevStatus = "devstatus",
                Additional = "additional",
                Special1 = "special1",
                Special2 = "special2",
                Alt = true,
                GameId1 = "gameid1",
                GameId2 = "gameid2",
                Description = "description",
                Bios = true,
                Licensed = true,
                Pirate = true,
                Physical = true,
                Complete = true,
                Adult = true,
                Dat = true,
                Listed = true,
                Private = true,
                StickyNote = "stickynote",
                DatterNote = "datternote",
                Categories = "categories",
            };
        }

        private static Data.Models.Metadata.BiosSet CreateMetadataBiosSet()
        {
            return new Data.Models.Metadata.BiosSet
            {
                Default = true,
                Description = "description",
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Chip CreateMetadataChip()
        {
            return new Data.Models.Metadata.Chip
            {
                Clock = 12345,
                Flags = "flags",
                Name = "name",
                SoundOnly = true,
                Tag = "tag",
                ChipType = Data.Models.Metadata.ChipType.CPU,
            };
        }

        private static Data.Models.Metadata.Configuration CreateMetadataConfiguration()
        {
            return new Data.Models.Metadata.Configuration
            {
                ConditionValue = "value",
                ConditionMask = "mask",
                ConditionRelation = Data.Models.Metadata.Relation.Equal,
                ConditionTag = "tag",
                ConfLocation = [CreateMetadataConfLocation()],
                ConfSetting = [CreateMetadataConfSetting()],
                Mask = "mask",
                Name = "name",
                Tag = "tag",
            };
        }

        private static Data.Models.Metadata.ConfLocation CreateMetadataConfLocation()
        {
            return new Data.Models.Metadata.ConfLocation
            {
                Inverted = true,
                Name = "name",
                Number = 12345,
            };
        }

        private static Data.Models.Metadata.ConfSetting CreateMetadataConfSetting()
        {
            return new Data.Models.Metadata.ConfSetting
            {
                ConditionValue = "value",
                ConditionMask = "mask",
                ConditionRelation = Data.Models.Metadata.Relation.Equal,
                ConditionTag = "tag",
                Default = true,
                Name = "name",
                Value = "value",
            };
        }

        private static Data.Models.Metadata.Control CreateMetadataControl()
        {
            return new Data.Models.Metadata.Control
            {
                Buttons = 12345,
                KeyDelta = 12345,
                Maximum = 12345,
                Minimum = 12345,
                Player = 12345,
                ReqButtons = 12345,
                Reverse = true,
                Sensitivity = 12345,
                ControlType = Data.Models.Metadata.ControlType.Lightgun,
                Ways = "ways",
                Ways2 = "ways2",
                Ways3 = "ways3",
            };
        }

        private static Data.Models.Metadata.Device CreateMetadataDevice()
        {
            return new Data.Models.Metadata.Device
            {
                ExtensionName = ["name"],
                FixedImage = "fixedimage",
                InstanceBriefName = "briefname",
                InstanceName = "name",
                Interface = "interface",
                Mandatory = true,
                Tag = "tag",
                DeviceType = Data.Models.Metadata.DeviceType.PunchTape,
            };
        }

        private static Data.Models.Metadata.DeviceRef CreateMetadataDeviceRef()
        {
            return new Data.Models.Metadata.DeviceRef
            {
                Name = "name",
            };
        }

        private static Data.Models.Metadata.DipLocation CreateMetadataDipLocation()
        {
            return new Data.Models.Metadata.DipLocation
            {
                Inverted = true,
                Name = "name",
                Number = 12345,
            };
        }

        private static Data.Models.Metadata.DipSwitch CreateMetadataDipSwitch()
        {
            return new Data.Models.Metadata.DipSwitch
            {
                ConditionValue = "value",
                ConditionMask = "mask",
                ConditionRelation = Data.Models.Metadata.Relation.Equal,
                ConditionTag = "tag",
                Default = true,
                DipLocation = [CreateMetadataDipLocation()],
                DipValue = [CreateMetadataDipValue()],
                Entry = ["entry"],
                Mask = "mask",
                Name = "name",
                Tag = "tag",
            };
        }

        private static Data.Models.Metadata.DipValue CreateMetadataDipValue()
        {
            return new Data.Models.Metadata.DipValue
            {
                ConditionValue = "value",
                ConditionMask = "mask",
                ConditionRelation = Data.Models.Metadata.Relation.Equal,
                ConditionTag = "tag",
                Default = true,
                Name = "name",
                Value = "value",
            };
        }

        private static Data.Models.Metadata.DataArea CreateMetadataDataArea()
        {
            return new Data.Models.Metadata.DataArea
            {
                Endianness = Data.Models.Metadata.Endianness.Big,
                Name = "name",
                Rom = [new Data.Models.Metadata.Rom()],
                Size = 12345,
                Width = Data.Models.Metadata.Width.Long,
            };
        }

        private static Data.Models.Metadata.Disk CreateMetadataDisk()
        {
            return new Data.Models.Metadata.Disk
            {
                Flags = "flags",
                Index = 12345,
                MD5 = HashType.MD5.ZeroString,
                Merge = "merge",
                Name = "name",
                Optional = true,
                Region = "region",
                SHA1 = HashType.SHA1.ZeroString,
                Writable = true,
            };
        }

        private static Data.Models.Metadata.DiskArea CreateMetadataDiskArea()
        {
            return new Data.Models.Metadata.DiskArea
            {
                Disk = [new Data.Models.Metadata.Disk()],
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Display CreateMetadataDisplay()
        {
            return new Data.Models.Metadata.Display
            {
                AspectX = null, // Null to ensure it is not Video
                AspectY = null, // Null to ensure it is not Video
                FlipX = true,
                HBEnd = 12345,
                HBStart = 12345,
                Height = 12345,
                HTotal = 12345,
                PixClock = 12345,
                Refresh = 123.45,
                Rotate = Data.Models.Metadata.Rotation.East,
                Tag = "tag",
                DisplayType = Data.Models.Metadata.DisplayType.Vector,
                VBEnd = 12345,
                VBStart = 12345,
                VTotal = 12345,
                Width = 12345,
            };
        }

        private static Data.Models.Metadata.Driver CreateMetadataDriver()
        {
            return new Data.Models.Metadata.Driver
            {
                Blit = Data.Models.Metadata.Blit.Plain,
                Cocktail = Data.Models.Metadata.SupportStatus.Good,
                Color = Data.Models.Metadata.SupportStatus.Good,
                Emulation = Data.Models.Metadata.SupportStatus.Good,
                Incomplete = true,
                NoSoundHardware = true,
                PaletteSize = "palettesize",
                RequiresArtwork = true,
                SaveState = Data.Models.Metadata.Supported.Yes,
                Sound = Data.Models.Metadata.SupportStatus.Good,
                Status = Data.Models.Metadata.SupportStatus.Good,
                Unofficial = true,
            };
        }

        private static Data.Models.Metadata.Dump CreateMetadataDump()
        {
            return new Data.Models.Metadata.Dump
            {
                Original = CreateMetadataOriginal(),

                // The following are searched for in order
                // For the purposes of this test, only RomKey will be populated
                // The only difference is what OpenMSXSubType value is applied
                Rom = new Data.Models.Metadata.Rom(),
                MegaRom = null,
                SCCPlusCart = null,
            };
        }

        private static Data.Models.Metadata.Feature CreateMetadataFeature()
        {
            return new Data.Models.Metadata.Feature
            {
                Name = "name",
                Overall = Data.Models.Metadata.FeatureStatus.Imperfect,
                Status = Data.Models.Metadata.FeatureStatus.Imperfect,
                FeatureType = Data.Models.Metadata.FeatureType.Protection,
                Value = "value",
            };
        }

        private static Data.Models.Metadata.Info CreateMetadataInfo()
        {
            return new Data.Models.Metadata.Info
            {
                Name = "name",
                Value = "value",
            };
        }

        private static Data.Models.Metadata.Input CreateMetadataInput()
        {
            return new Data.Models.Metadata.Input
            {
                Buttons = 12345,
                Coins = 12345,
                Control = [CreateMetadataControl()],
                ControlAttr = "controlattr",
                Players = 12345,
                Service = true,
                Tilt = true,
            };
        }

        private static Data.Models.Metadata.Media CreateMetadataMedia()
        {
            return new Data.Models.Metadata.Media
            {
                MD5 = HashType.MD5.ZeroString,
                Name = "name",
                SHA1 = HashType.SHA1.ZeroString,
                SHA256 = HashType.SHA256.ZeroString,
                SpamSum = HashType.SpamSum.ZeroString,
            };
        }

        private static Data.Models.Metadata.Original CreateMetadataOriginal()
        {
            return new Data.Models.Metadata.Original
            {
                Content = "content",
                Value = true,
            };
        }

        private static Data.Models.Metadata.Part CreateMetadataPart()
        {
            return new Data.Models.Metadata.Part
            {
                DataArea = [CreateMetadataDataArea()],
                DiskArea = [CreateMetadataDiskArea()],
                DipSwitch = [new Data.Models.Metadata.DipSwitch()],
                Feature = [CreateMetadataFeature()],
                Interface = "interface",
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Port CreateMetadataPort()
        {
            return new Data.Models.Metadata.Port
            {
                AnalogMask = ["mask"],
                Tag = "tag",
            };
        }

        private static Data.Models.Metadata.RamOption CreateMetadataRamOption()
        {
            return new Data.Models.Metadata.RamOption
            {
                Content = "content",
                Default = true,
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Release CreateMetadataRelease()
        {
            return new Data.Models.Metadata.Release
            {
                Date = "date",
                Default = true,
                Language = "language",
                Name = "name",
                Region = "region",
            };
        }

        private static Data.Models.Metadata.ReleaseDetails CreateMetadataReleaseDetails()
        {
            return new Data.Models.Metadata.ReleaseDetails
            {
                AppendToNumber = "appendtonumber",
                ArchiveName = "archivename",
                Category = "category",
                Comment = "comment",
                Date = "date",
                DirName = "dirname",
                Group = "group",
                Id = "id",
                Origin = "origin",
                OriginalFormat = "originalformat",
                NfoCRC = "nfocrc",
                NfoName = "nfoname",
                NfoSize = "nfosize",
                Region = "region",
                RomInfo = "rominfo",
                Tool = "tool",
            };
        }

        private static Data.Models.Metadata.Rom CreateMetadataRom()
        {
            return new Data.Models.Metadata.Rom
            {
                Album = "album",
                AltRomname = "alt_romname",
                AltTitle = "alt_title",
                Artist = "artist",
                ASRDetectedLang = "asr_detected_lang",
                ASRDetectedLangConf = "asr_detected_lang_conf",
                ASRTranscribedLang = "asr_transcribed_lang",
                Bios = "bios",
                Bitrate = "bitrate",
                BitTorrentMagnetHash = "btih",
                ClothCoverDetectionModuleVersion = "cloth_cover_detection_module_version",
                CollectionCatalogNumber = "collection-catalog-number",
                Comment = "comment",
                CRC16 = HashType.CRC16.ZeroString,
                CRC32 = HashType.CRC32.ZeroString,
                CRC64 = HashType.CRC64.ZeroString,
                Creator = "creator",
                Date = "date",
                Dispose = true,
                Extension = "extension",
                FileCount = 12345,
                FileIsAvailable = true,
                Flags = "flags",
                Format = "format",
                Header = "header",
                Height = "height",
                hOCRCharToWordhOCRVersion = "hocr_char_to_word_hocr_version",
                hOCRCharToWordModuleVersion = "hocr_char_to_word_module_version",
                hOCRFtsTexthOCRVersion = "hocr_fts_text_hocr_version",
                hOCRFtsTextModuleVersion = "hocr_fts_text_module_version",
                hOCRPageIndexhOCRVersion = "hocr_pageindex_hocr_version",
                hOCRPageIndexModuleVersion = "hocr_pageindex_module_version",
                Inverted = true,
                LastModifiedTime = "mtime",
                Length = "length",
                LoadFlag = Data.Models.Metadata.LoadFlag.Load16Byte,
                MatrixNumber = "matrix_number",
                MD2 = HashType.MD2.ZeroString,
                MD4 = HashType.MD4.ZeroString,
                MD5 = HashType.MD5.ZeroString,
                // [Data.Models.Metadata.Rom.OpenMSXMediaType] = null, // Omit due to other test
                OpenMSXType = "type",
                Merge = "merge",
                MIA = true,
                Name = "name",
                Offset = "offset",
                Optional = true,
                Original = "original",
                PDFModuleVersion = "pdf_module_version",
                PreviewImage = "preview-image",
                Publisher = "publisher",
                Region = "region",
                Remark = "remark",
                RIPEMD128 = HashType.RIPEMD128.ZeroString,
                RIPEMD160 = HashType.RIPEMD160.ZeroString,
                Rotation = "rotation",
                Serial = "serial",
                SHA1 = HashType.SHA1.ZeroString,
                SHA256 = HashType.SHA256.ZeroString,
                SHA384 = HashType.SHA384.ZeroString,
                SHA512 = HashType.SHA512.ZeroString,
                Size = 12345,
                SoundOnly = true,
                Source = "source",
                SpamSum = HashType.SpamSum.ZeroString,
                Start = "start",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Summation = "summation",
                TesseractOCR = "ocr",
                TesseractOCRConverted = "ocr_converted",
                TesseractOCRDetectedLang = "ocr_detected_lang",
                TesseractOCRDetectedLangConf = "ocr_detected_lang_conf",
                TesseractOCRDetectedScript = "ocr_detected_script",
                TesseractOCRDetectedScriptConf = "ocr_detected_script_conf",
                TesseractOCRModuleVersion = "ocr_module_version",
                TesseractOCRParameters = "ocr_parameters",
                Title = "title",
                Track = "track",
                Value = "value",
                WhisperASRModuleVersion = "whisper_asr_module_version",
                WhisperModelHash = "whisper_model_hash",
                WhisperModelName = "whisper_model_name",
                WhisperVersion = "whisper_version",
                Width = "width",
                WordConfidenceInterval0To10 = "word_conf_0_10",
                WordConfidenceInterval11To20 = "word_conf_11_20",
                WordConfidenceInterval21To30 = "word_conf_21_30",
                WordConfidenceInterval31To40 = "word_conf_31_40",
                WordConfidenceInterval41To50 = "word_conf_41_50",
                WordConfidenceInterval51To60 = "word_conf_51_60",
                WordConfidenceInterval61To70 = "word_conf_61_70",
                WordConfidenceInterval71To80 = "word_conf_71_80",
                WordConfidenceInterval81To90 = "word_conf_81_90",
                WordConfidenceInterval91To100 = "word_conf_91_100",
                xxHash364 = HashType.XxHash3.ZeroString,
                xxHash3128 = HashType.XxHash128.ZeroString,
            };
        }

        private static Data.Models.Metadata.Sample CreateMetadataSample()
        {
            return new Data.Models.Metadata.Sample
            {
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Serials CreateMetadataSerials()
        {
            return new Data.Models.Metadata.Serials
            {
                BoxBarcode = "boxbarcode",
                BoxSerial = "boxserial",
                ChipSerial = "chipserial",
                DigitalSerial1 = "digitalserial1",
                DigitalSerial2 = "digitalserial2",
                LockoutSerial = "lockoutserial",
                MediaSerial1 = "mediaserial1",
                MediaSerial2 = "mediaserial2",
                MediaSerial3 = "mediaserial3",
                MediaStamp = "mediastamp",
                PCBSerial = "pcbserial",
                RomChipSerial1 = "romchipserial1",
                RomChipSerial2 = "romchipserial2",
                SaveChipSerial = "savechipserial",
            };
        }

        private static Data.Models.Metadata.SharedFeat CreateMetadataSharedFeat()
        {
            return new Data.Models.Metadata.SharedFeat
            {
                Name = "name",
                Value = "value",
            };
        }

        private static Data.Models.Metadata.Slot CreateMetadataSlot()
        {
            return new Data.Models.Metadata.Slot
            {
                Name = "name",
                SlotOption = [CreateMetadataSlotOption()],
            };
        }

        private static Data.Models.Metadata.SlotOption CreateMetadataSlotOption()
        {
            return new Data.Models.Metadata.SlotOption
            {
                Default = true,
                DevName = "devname",
                Name = "name",
            };
        }

        private static Data.Models.Metadata.SoftwareList CreateMetadataSoftwareList()
        {
            return new Data.Models.Metadata.SoftwareList
            {
                Filter = "filter",
                Name = "name",
                Status = Data.Models.Metadata.SoftwareListStatus.Original,
                Tag = "tag",
            };
        }

        private static Data.Models.Metadata.Sound CreateMetadataSound()
        {
            return new Data.Models.Metadata.Sound
            {
                Channels = 12345,
            };
        }

        private static Data.Models.Metadata.SourceDetails CreateMetadataSourceDetails()
        {
            return new Data.Models.Metadata.SourceDetails
            {
                AppendToNumber = "appendtonumber",
                Comment1 = "comment1",
                Comment2 = "comment2",
                DumpDate = "dumpdate",
                DumpDateInfo = true,
                Dumper = "dumper",
                Id = "id",
                Link1 = "link1",
                Link1Public = true,
                Link2 = "link2",
                Link2Public = true,
                Link3 = "link3",
                Link3Public = true,
                MediaTitle = "mediatitle",
                Nodump = true,
                Origin = "origin",
                OriginalFormat = "originalformat",
                Project = "project",
                Region = "region",
                ReleaseDate = "releasedate",
                ReleaseDateInfo = true,
                RomInfo = "rominfo",
                Section = "section",
                Tool = "tool",
            };
        }

        private static Data.Models.Metadata.Video CreateMetadataVideo()
        {
            return new Data.Models.Metadata.Video
            {
                AspectX = 12345,
                AspectY = 12345,
                Height = 12345,
                Orientation = Data.Models.Metadata.Rotation.East,
                Refresh = 123.45,
                Screen = Data.Models.Metadata.DisplayType.Vector,
                Width = 12345,
            };
        }

        #endregion

        #region Validation Helpers

        private static void ValidateHeader(DatHeader datHeader)
        {
            Assert.Equal("author", datHeader.Author);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, datHeader.BiosMode);
            Assert.Equal("build", datHeader.Build);
            Assert.NotNull(datHeader.CanOpen);
            Assert.Equal("category", datHeader.Category);
            Assert.Equal("comment", datHeader.Comment);
            Assert.Equal("date", datHeader.Date);
            Assert.Equal("datversion", datHeader.DatVersion);
            Assert.True(datHeader.Debug);
            Assert.Equal("description", datHeader.Description);
            Assert.Equal("email", datHeader.Email);
            Assert.Equal("emulatorversion", datHeader.EmulatorVersion);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, datHeader.ForceMerging);
            Assert.Equal(Data.Models.Metadata.NodumpFlag.Required, datHeader.ForceNodump);
            Assert.Equal(Data.Models.Metadata.PackingFlag.Zip, datHeader.ForcePacking);
            Assert.True(datHeader.ForceZipping);
            Assert.Equal("header", datHeader.HeaderSkipper);
            Assert.Equal("homepage", datHeader.Homepage);
            Assert.Equal("id", datHeader.Id);
            Assert.NotNull(datHeader.Images);
            Assert.Equal("imfolder", datHeader.ImFolder);
            Assert.NotNull(datHeader.Infos);
            Assert.True(datHeader.LockBiosMode);
            Assert.True(datHeader.LockRomMode);
            Assert.True(datHeader.LockSampleMode);
            Assert.Equal("mameconfig", datHeader.MameConfig);
            Assert.Equal("name", datHeader.Name);
            Assert.NotNull(datHeader.NewDat);
            Assert.Equal("notes", datHeader.Notes);
            Assert.Equal("plugin", datHeader.Plugin);
            Assert.Equal("refname", datHeader.RefName);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, datHeader.RomMode);
            Assert.Equal("romtitle", datHeader.RomTitle);
            Assert.Equal("rootdir", datHeader.RootDir);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, datHeader.SampleMode);
            Assert.Equal("schemalocation", datHeader.SchemaLocation);
            Assert.Equal("screenshotsheight", datHeader.ScreenshotsHeight);
            Assert.Equal("screenshotsWidth", datHeader.ScreenshotsWidth);
            Assert.NotNull(datHeader.Search);
            Assert.Equal("system", datHeader.System);
            Assert.Equal("timestamp", datHeader.Timestamp);
            Assert.Equal("type", datHeader.Type);
            Assert.Equal("url", datHeader.Url);
            Assert.Equal("version", datHeader.Version);
        }

#pragma warning disable IDE0051
        private static void ValidateMachine(DatItems.Machine machine)
        {
            Assert.Equal("board", machine.Board);
            Assert.Equal("buttons", machine.Buttons);
            Assert.Equal("cloneof", machine.CloneOf);
            Assert.Equal("cloneofid", machine.CloneOfId);
            Assert.Equal("company", machine.Company);
            Assert.Equal("control", machine.Control);
            Assert.Equal("country", machine.Country);
            Assert.Equal("crc32", machine.CRC);
            Assert.Equal("description", machine.Description);
            Assert.Equal("developer", machine.Developer);
            Assert.Equal("dirname", machine.DirName);
            Assert.Equal("displaycount", machine.DisplayCount);
            Assert.Equal("displaytype", machine.DisplayType);
            Assert.Equal("duplicateid", machine.DuplicateID);
            Assert.Equal("emulator", machine.Emulator);
            Assert.Equal("enabled", machine.Enabled);
            Assert.Equal("extra", machine.Extra);
            Assert.Equal("favorite", machine.Favorite);
            Assert.Equal("genmsxid", machine.GenMSXID);
            Assert.Equal("genre", machine.Genre);
            Assert.Equal("history", machine.History);
            Assert.Equal("id", machine.Id);
            Assert.Equal(HashType.CRC32.ZeroString, machine.Im1CRC);
            Assert.Equal(HashType.CRC32.ZeroString, machine.Im2CRC);
            Assert.Equal("imagenumber", machine.ImageNumber);
            Assert.Equal(true, machine.IsBios);
            Assert.Equal(true, machine.IsDevice);
            Assert.Equal(true, machine.IsMechanical);
            Assert.Equal("language", machine.Language);
            Assert.Equal("location", machine.Location);
            Assert.Equal("manufacturer", machine.Manufacturer);
            Assert.Equal("name", machine.Name);
            Assert.Equal("notes", machine.Notes);
            Assert.Equal("playedcount", machine.PlayedCount);
            Assert.Equal("playedtime", machine.PlayedTime);
            Assert.Equal("players", machine.Players);
            Assert.Equal("publisher", machine.Publisher);
            Assert.Equal("ratings", machine.Ratings);
            Assert.Equal("rebuildto", machine.RebuildTo);
            Assert.Equal("relatedto", machine.RelatedTo);
            Assert.Equal("releasenumber", machine.ReleaseNumber);
            Assert.Equal("romof", machine.RomOf);
            Assert.Equal("rotation", machine.Rotation);
            Assert.Equal(Data.Models.Metadata.Runnable.Yes, machine.Runnable);
            Assert.Equal("sampleof", machine.SampleOf);
            Assert.Equal("savetype", machine.SaveType);
            Assert.Equal("score", machine.Score);
            Assert.Equal("source", machine.Source);
            Assert.Equal("sourcefile", machine.SourceFile);
            Assert.Equal("sourcerom", machine.SourceRom);
            Assert.Equal("status", machine.Status);
            Assert.Equal("subgenre", machine.Subgenre);
            Assert.Equal(Data.Models.Metadata.Supported.Yes, machine.Supported);
            Assert.Equal("system", machine.System);
            Assert.Equal("tags", machine.Tags);
            Assert.Equal("titleid", machine.TitleID);
            Assert.Equal("url", machine.Url);
            Assert.Equal("year", machine.Year);

            string[]? categories = machine.Category;
            Assert.NotNull(categories);
            string? category = Assert.Single(categories);
            Assert.Equal("category", category);

            string[]? comments = machine.Comment;
            Assert.NotNull(comments);
            string? comment = Assert.Single(comments);
            Assert.Equal("comment", comment);
        }
#pragma warning restore IDE0051

        private static void ValidateAdjuster(Adjuster? adjuster)
        {
            Assert.NotNull(adjuster);
            Assert.Equal("value", adjuster.ConditionValue);
            Assert.Equal("mask", adjuster.ConditionMask);
            Assert.Equal(Data.Models.Metadata.Relation.Equal, adjuster.ConditionRelation);
            Assert.Equal("tag", adjuster.ConditionTag);
            Assert.True(adjuster.Default);
            Assert.Equal("name", adjuster.Name);
        }

        private static void ValidateArchive(Archive? archive)
        {
            Assert.NotNull(archive);
            Assert.Equal("name", archive.Name);
        }

        private static void ValidateBiosSet(BiosSet? biosSet)
        {
            Assert.NotNull(biosSet);
            Assert.True(biosSet.Default);
            Assert.Equal("description", biosSet.Description);
            Assert.Equal("name", biosSet.Name);
        }

        private static void ValidateChip(Chip? chip)
        {
            Assert.NotNull(chip);
            Assert.Equal(12345, chip.Clock);
            Assert.Equal("flags", chip.Flags);
            Assert.Equal("name", chip.Name);
            Assert.True(chip.SoundOnly);
            Assert.Equal("tag", chip.Tag);
            Assert.Equal(Data.Models.Metadata.ChipType.CPU, chip.ChipType);
        }

        private static void ValidateConfiguration(Configuration? configuration)
        {
            Assert.NotNull(configuration);
            Assert.Equal("value", configuration.ConditionValue);
            Assert.Equal("mask", configuration.ConditionMask);
            Assert.Equal(Data.Models.Metadata.Relation.Equal, configuration.ConditionRelation);
            Assert.Equal("tag", configuration.ConditionTag);
            Assert.Equal("mask", configuration.Mask);
            Assert.Equal("name", configuration.Name);
            Assert.Equal("tag", configuration.Tag);

            ConfLocation[]? confLocations = configuration.ConfLocation;
            Assert.NotNull(confLocations);
            ConfLocation? confLocation = Assert.Single(confLocations);
            ValidateConfLocation(confLocation);

            ConfSetting[]? confSettings = configuration.ConfSetting;
            Assert.NotNull(confSettings);
            ConfSetting? confSetting = Assert.Single(confSettings);
            ValidateConfSetting(confSetting);
        }

        private static void ValidateConfLocation(ConfLocation? confLocation)
        {
            Assert.NotNull(confLocation);
            Assert.True(confLocation.Inverted);
            Assert.Equal("name", confLocation.Name);
            Assert.Equal(12345, confLocation.Number);
        }

        private static void ValidateConfSetting(ConfSetting? confSetting)
        {
            Assert.NotNull(confSetting);
            Assert.Equal("value", confSetting.ConditionValue);
            Assert.Equal("mask", confSetting.ConditionMask);
            Assert.Equal(Data.Models.Metadata.Relation.Equal, confSetting.ConditionRelation);
            Assert.Equal("tag", confSetting.ConditionTag);
            Assert.True(confSetting.Default);
            Assert.Equal("name", confSetting.Name);
            Assert.Equal("value", confSetting.Value);
        }

        private static void ValidateControl(Control? control)
        {
            Assert.NotNull(control);
            Assert.Equal(12345, control.Buttons);
            Assert.Equal(12345, control.KeyDelta);
            Assert.Equal(12345, control.Maximum);
            Assert.Equal(12345, control.Minimum);
            Assert.Equal(12345, control.Player);
            Assert.Equal(12345, control.ReqButtons);
            Assert.True(control.Reverse);
            Assert.Equal(12345, control.Sensitivity);
            Assert.Equal(Data.Models.Metadata.ControlType.Lightgun, control.ControlType);
            Assert.Equal("ways", control.Ways);
            Assert.Equal("ways2", control.Ways2);
            Assert.Equal("ways3", control.Ways3);
        }

        private static void ValidateDataArea(DataArea? dataArea)
        {
            Assert.NotNull(dataArea);
            Assert.Equal(Data.Models.Metadata.Endianness.Big, dataArea.Endianness);
            Assert.Equal("name", dataArea.Name);
            Assert.Equal(12345, dataArea.Size);
            Assert.Equal(Data.Models.Metadata.Width.Long, dataArea.Width);
        }

        private static void ValidateDevice(Device? device)
        {
            Assert.NotNull(device);
            Assert.Equal("fixedimage", device.FixedImage);
            Assert.Equal("briefname", device.InstanceBriefName);
            Assert.Equal("name", device.InstanceName);
            Assert.Equal("interface", device.Interface);
            Assert.Equal(true, device.Mandatory);
            Assert.Equal("tag", device.Tag);
            Assert.Equal(Data.Models.Metadata.DeviceType.PunchTape, device.DeviceType);

            string[]? extensionNames = device.ExtensionName;
            Assert.NotNull(extensionNames);
            string? extensionName = Assert.Single(extensionNames);
            Assert.Equal("name", extensionName);
        }

        private static void ValidateDeviceRef(DeviceRef? deviceRef)
        {
            Assert.NotNull(deviceRef);
            Assert.Equal("name", deviceRef.Name);
        }

        private static void ValidateDipLocation(DipLocation? dipLocation)
        {
            Assert.NotNull(dipLocation);
            Assert.True(dipLocation.Inverted);
            Assert.Equal("name", dipLocation.Name);
            Assert.Equal(12345, dipLocation.Number);
        }

        private static void ValidateDipSwitch(DipSwitch? dipSwitch)
        {
            Assert.NotNull(dipSwitch);
            Assert.Equal("value", dipSwitch.ConditionValue);
            Assert.Equal("mask", dipSwitch.ConditionMask);
            Assert.Equal(Data.Models.Metadata.Relation.Equal, dipSwitch.ConditionRelation);
            Assert.Equal("tag", dipSwitch.ConditionTag);
            Assert.True(dipSwitch.Default);
            Assert.Equal("mask", dipSwitch.Mask);
            Assert.Equal("name", dipSwitch.Name);
            Assert.Equal("tag", dipSwitch.Tag);

            DipLocation[]? dipLocations = dipSwitch.DipLocation;
            Assert.NotNull(dipLocations);
            DipLocation? dipLocation = Assert.Single(dipLocations);
            ValidateDipLocation(dipLocation);

            DipValue[]? dipValues = dipSwitch.DipValue;
            Assert.NotNull(dipValues);
            DipValue? dipValue = Assert.Single(dipValues);
            ValidateDipValue(dipValue);

            string[]? entries = dipSwitch.Entry;
            Assert.NotNull(entries);
            string entry = Assert.Single(entries);
            Assert.Equal("entry", entry);
        }

        private static void ValidateDipValue(DipValue? dipValue)
        {
            Assert.NotNull(dipValue);
            Assert.Equal("value", dipValue.ConditionValue);
            Assert.Equal("mask", dipValue.ConditionMask);
            Assert.Equal(Data.Models.Metadata.Relation.Equal, dipValue.ConditionRelation);
            Assert.Equal("tag", dipValue.ConditionTag);
            Assert.True(dipValue.Default);
            Assert.Equal("name", dipValue.Name);
            Assert.Equal("value", dipValue.Value);
        }

        private static void ValidateDisk(Disk? disk)
        {
            Assert.NotNull(disk);
            Assert.Equal("flags", disk.Flags);
            Assert.Equal(12345, disk.Index);
            Assert.Equal(HashType.MD5.ZeroString, disk.MD5);
            Assert.Equal("merge", disk.Merge);
            Assert.Equal("name", disk.Name);
            Assert.True(disk.Optional);
            Assert.Equal("region", disk.Region);
            Assert.Equal(HashType.SHA1.ZeroString, disk.SHA1);
            Assert.True(disk.Writable);
        }

        private static void ValidateDiskArea(DiskArea? diskArea)
        {
            Assert.NotNull(diskArea);
            Assert.Equal("name", diskArea.Name);
        }

        private static void ValidateDisplay(Display? display)
        {
            Assert.NotNull(display);
            Assert.Null(display.AspectX);
            Assert.Null(display.AspectY);
            Assert.True(display.FlipX);
            Assert.Equal(12345, display.HBEnd);
            Assert.Equal(12345, display.HBStart);
            Assert.Equal(12345, display.Height);
            Assert.Equal(12345, display.HTotal);
            Assert.Equal(12345, display.PixClock);
            Assert.Equal(123.45, display.Refresh);
            Assert.Equal(Data.Models.Metadata.Rotation.East, display.Rotate);
            Assert.Equal("tag", display.Tag);
            Assert.Equal(Data.Models.Metadata.DisplayType.Vector, display.DisplayType);
            Assert.Equal(12345, display.VBEnd);
            Assert.Equal(12345, display.VBStart);
            Assert.Equal(12345, display.VTotal);
            Assert.Equal(12345, display.Width);
        }

        private static void ValidateDriver(Driver? driver)
        {
            Assert.NotNull(driver);
            Assert.Equal(Data.Models.Metadata.Blit.Plain, driver.Blit);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Cocktail);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Color);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Emulation);
            Assert.True(driver.Incomplete);
            Assert.True(driver.NoSoundHardware);
            Assert.Equal("palettesize", driver.PaletteSize);
            Assert.True(driver.RequiresArtwork);
            Assert.Equal(Data.Models.Metadata.Supported.Yes, driver.SaveState);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Sound);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Status);
            Assert.True(driver.Unofficial);
        }

        private static void ValidateFeature(Feature? feature)
        {
            Assert.NotNull(feature);
            Assert.Equal("name", feature.Name);
            Assert.Equal(Data.Models.Metadata.FeatureStatus.Imperfect, feature.Overall);
            Assert.Equal(Data.Models.Metadata.FeatureStatus.Imperfect, feature.Status);
            Assert.Equal(Data.Models.Metadata.FeatureType.Protection, feature.FeatureType);
            Assert.Equal("value", feature.Value);
        }

        private static void ValidateInfo(Info? info)
        {
            Assert.NotNull(info);
            Assert.Equal("name", info.Name);
            Assert.Equal("value", info.Value);
        }

        private static void ValidateInput(Input? input)
        {
            Assert.NotNull(input);
            Assert.Equal(12345, input.Buttons);
            Assert.Equal(12345, input.Coins);
            Assert.Equal("controlattr", input.ControlAttr);
            Assert.Equal(12345, input.Players);
            Assert.True(input.Service);
            Assert.True(input.Tilt);

            Control[]? controls = input.Control;
            Assert.NotNull(controls);
            Control? control = Assert.Single(controls);
            ValidateControl(control);
        }

        private static void ValidateMedia(Media? media)
        {
            Assert.NotNull(media);
            Assert.Equal(HashType.MD5.ZeroString, media.MD5);
            Assert.Equal("name", media.Name);
            Assert.Equal(HashType.SHA1.ZeroString, media.SHA1);
            Assert.Equal(HashType.SHA256.ZeroString, media.SHA256);
            Assert.Equal(HashType.SpamSum.ZeroString, media.SpamSum);
        }

        private static void ValidatePartFeature(PartFeature? partFeature)
        {
            Assert.NotNull(partFeature);
            Assert.Equal("name", partFeature.Name);
            Assert.Equal(Data.Models.Metadata.FeatureStatus.Imperfect, partFeature.Overall);
            Assert.Equal("interface", partFeature.PartInterface);
            Assert.Equal("name", partFeature.PartName);
            Assert.Equal(Data.Models.Metadata.FeatureStatus.Imperfect, partFeature.Status);
            Assert.Equal(Data.Models.Metadata.FeatureType.Protection, partFeature.FeatureType);
            Assert.Equal("value", partFeature.Value);
        }

        private static void ValidatePort(Port? port)
        {
            Assert.NotNull(port);
            Assert.Equal("tag", port.Tag);

            string[]? analogMasks = port.AnalogMask;
            Assert.NotNull(analogMasks);
            string? analogMask = Assert.Single(analogMasks);
            Assert.Equal("mask", analogMask);
        }

        private static void ValidateRamOption(RamOption? ramOption)
        {
            Assert.NotNull(ramOption);
            Assert.Equal("content", ramOption.Content);
            Assert.True(ramOption.Default);
            Assert.Equal("name", ramOption.Name);
        }

        private static void ValidateRelease(Release? release)
        {
            Assert.NotNull(release);
            Assert.Equal("date", release.Date);
            Assert.True(release.Default);
            Assert.Equal("language", release.Language);
            Assert.Equal("name", release.Name);
            Assert.Equal("region", release.Region);
        }

        private static void ValidateReleaseDetails(ReleaseDetails? releaseDetails)
        {
            Assert.NotNull(releaseDetails);
            Assert.Equal("appendtonumber", releaseDetails.AppendToNumber);
            Assert.Equal("archivename", releaseDetails.ArchiveName);
            Assert.Equal("category", releaseDetails.Category);
            Assert.Equal("comment", releaseDetails.Comment);
            Assert.Equal("date", releaseDetails.Date);
            Assert.Equal("dirname", releaseDetails.DirName);
            Assert.Equal("group", releaseDetails.Group);
            Assert.Equal("id", releaseDetails.Id);
            Assert.Equal("nfocrc", releaseDetails.NfoCRC);
            Assert.Equal("nfoname", releaseDetails.NfoName);
            Assert.Equal("nfosize", releaseDetails.NfoSize);
            Assert.Equal("origin", releaseDetails.Origin);
            Assert.Equal("originalformat", releaseDetails.OriginalFormat);
            Assert.Equal("region", releaseDetails.Region);
            Assert.Equal("rominfo", releaseDetails.RomInfo);
            Assert.Equal("tool", releaseDetails.Tool);
        }

        private static void ValidateRom(Rom? rom)
        {
            Assert.NotNull(rom);
            Assert.Equal("album", rom.Album);
            Assert.Equal("alt_romname", rom.AltRomname);
            Assert.Equal("alt_title", rom.AltTitle);
            Assert.Equal("artist", rom.Artist);
            Assert.Equal("asr_detected_lang", rom.ASRDetectedLang);
            Assert.Equal("asr_detected_lang_conf", rom.ASRDetectedLangConf);
            Assert.Equal("asr_transcribed_lang", rom.ASRTranscribedLang);
            Assert.Equal("bios", rom.Bios);
            Assert.Equal("bitrate", rom.Bitrate);
            Assert.Equal("btih", rom.BitTorrentMagnetHash);
            Assert.Equal("cloth_cover_detection_module_version", rom.ClothCoverDetectionModuleVersion);
            Assert.Equal("collection-catalog-number", rom.CollectionCatalogNumber);
            Assert.Equal("comment", rom.Comment);
            Assert.Equal(HashType.CRC16.ZeroString, rom.CRC16);
            Assert.Equal(HashType.CRC32.ZeroString, rom.CRC32);
            Assert.Equal(HashType.CRC64.ZeroString, rom.CRC64);
            Assert.Equal("creator", rom.Creator);
            Assert.Equal("date", rom.Date);
            Assert.True(rom.Dispose);
            Assert.Equal("extension", rom.Extension);
            Assert.Equal(12345, rom.FileCount);
            Assert.True(rom.FileIsAvailable);
            Assert.Equal("flags", rom.Flags);
            Assert.Equal("format", rom.Format);
            Assert.Equal("header", rom.Header);
            Assert.Equal("height", rom.Height);
            Assert.Equal("hocr_char_to_word_hocr_version", rom.hOCRCharToWordhOCRVersion);
            Assert.Equal("hocr_char_to_word_module_version", rom.hOCRCharToWordModuleVersion);
            Assert.Equal("hocr_fts_text_hocr_version", rom.hOCRFtsTexthOCRVersion);
            Assert.Equal("hocr_fts_text_module_version", rom.hOCRFtsTextModuleVersion);
            Assert.Equal("hocr_pageindex_hocr_version", rom.hOCRPageIndexhOCRVersion);
            Assert.Equal("hocr_pageindex_module_version", rom.hOCRPageIndexModuleVersion);
            Assert.True(rom.Inverted);
            Assert.Equal("mtime", rom.LastModifiedTime);
            Assert.Equal("length", rom.Length);
            Assert.Equal(Data.Models.Metadata.LoadFlag.Load16Byte, rom.LoadFlag);
            Assert.Equal("matrix_number", rom.MatrixNumber);
            Assert.Equal(HashType.MD2.ZeroString, rom.MD2);
            Assert.Equal(HashType.MD4.ZeroString, rom.MD4);
            Assert.Equal(HashType.MD5.ZeroString, rom.MD5);
            Assert.Null(rom.OpenMSXMediaType); // Omit due to other test
            Assert.Equal("type", rom.OpenMSXType);
            Assert.Equal("merge", rom.Merge);
            Assert.True(rom.MIA);
            Assert.Equal("name", rom.Name);
            Assert.Equal("offset", rom.Offset);
            Assert.True(rom.Optional);
            Assert.Equal("original", rom.OriginalProperty);
            Assert.Equal("pdf_module_version", rom.PDFModuleVersion);
            Assert.Equal("preview-image", rom.PreviewImage);
            Assert.Equal("publisher", rom.Publisher);
            Assert.Equal("region", rom.Region);
            Assert.Equal("remark", rom.Remark);
            Assert.Equal(HashType.RIPEMD128.ZeroString, rom.RIPEMD128);
            Assert.Equal(HashType.RIPEMD160.ZeroString, rom.RIPEMD160);
            Assert.Equal("rotation", rom.Rotation);
            Assert.Equal("serial", rom.Serial);
            Assert.Equal(HashType.SHA1.ZeroString, rom.SHA1);
            Assert.Equal(HashType.SHA256.ZeroString, rom.SHA256);
            Assert.Equal(HashType.SHA384.ZeroString, rom.SHA384);
            Assert.Equal(HashType.SHA512.ZeroString, rom.SHA512);
            Assert.Equal(12345, rom.Size);
            Assert.True(rom.SoundOnly);
            Assert.Equal("source", rom.SourceProperty);
            Assert.Equal(HashType.SpamSum.ZeroString, rom.SpamSum);
            Assert.Equal("start", rom.Start);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, rom.Status);
            Assert.Equal("summation", rom.Summation);
            Assert.Equal("ocr", rom.TesseractOCR);
            Assert.Equal("ocr_converted", rom.TesseractOCRConverted);
            Assert.Equal("ocr_detected_lang", rom.TesseractOCRDetectedLang);
            Assert.Equal("ocr_detected_lang_conf", rom.TesseractOCRDetectedLangConf);
            Assert.Equal("ocr_detected_script", rom.TesseractOCRDetectedScript);
            Assert.Equal("ocr_detected_script_conf", rom.TesseractOCRDetectedScriptConf);
            Assert.Equal("ocr_module_version", rom.TesseractOCRModuleVersion);
            Assert.Equal("ocr_parameters", rom.TesseractOCRParameters);
            Assert.Equal("title", rom.Title);
            Assert.Equal("track", rom.Track);
            Assert.Equal("value", rom.Value);
            Assert.Equal("whisper_asr_module_version", rom.WhisperASRModuleVersion);
            Assert.Equal("whisper_model_hash", rom.WhisperModelHash);
            Assert.Equal("whisper_model_name", rom.WhisperModelName);
            Assert.Equal("whisper_version", rom.WhisperVersion);
            Assert.Equal("width", rom.Width);
            Assert.Equal("word_conf_0_10", rom.WordConfidenceInterval0To10);
            Assert.Equal("word_conf_11_20", rom.WordConfidenceInterval11To20);
            Assert.Equal("word_conf_21_30", rom.WordConfidenceInterval21To30);
            Assert.Equal("word_conf_31_40", rom.WordConfidenceInterval31To40);
            Assert.Equal("word_conf_41_50", rom.WordConfidenceInterval41To50);
            Assert.Equal("word_conf_51_60", rom.WordConfidenceInterval51To60);
            Assert.Equal("word_conf_61_70", rom.WordConfidenceInterval61To70);
            Assert.Equal("word_conf_71_80", rom.WordConfidenceInterval71To80);
            Assert.Equal("word_conf_81_90", rom.WordConfidenceInterval81To90);
            Assert.Equal("word_conf_91_100", rom.WordConfidenceInterval91To100);
            Assert.Equal(HashType.XxHash3.ZeroString, rom.xxHash364);
            Assert.Equal(HashType.XxHash128.ZeroString, rom.xxHash3128);
        }

        private static void ValidateSample(Sample? sample)
        {
            Assert.NotNull(sample);
            Assert.Equal("name", sample.Name);
        }

        private static void ValidateSharedFeat(SharedFeat? sharedFeat)
        {
            Assert.NotNull(sharedFeat);
            Assert.Equal("name", sharedFeat.Name);
            Assert.Equal("value", sharedFeat.Value);
        }

        private static void ValidateSerials(Serials? serials)
        {
            Assert.NotNull(serials);
            Assert.Equal("boxbarcode", serials.BoxBarcode);
            Assert.Equal("boxserial", serials.BoxSerial);
            Assert.Equal("chipserial", serials.ChipSerial);
            Assert.Equal("digitalserial1", serials.DigitalSerial1);
            Assert.Equal("digitalserial2", serials.DigitalSerial2);
            Assert.Equal("lockoutserial", serials.LockoutSerial);
            Assert.Equal("mediaserial1", serials.MediaSerial1);
            Assert.Equal("mediaserial2", serials.MediaSerial2);
            Assert.Equal("mediaserial3", serials.MediaSerial3);
            Assert.Equal("mediastamp", serials.MediaStamp);
            Assert.Equal("pcbserial", serials.PCBSerial);
            Assert.Equal("romchipserial1", serials.RomChipSerial1);
            Assert.Equal("romchipserial2", serials.RomChipSerial2);
            Assert.Equal("savechipserial", serials.SaveChipSerial);
        }

        private static void ValidateSlot(Slot? slot)
        {
            Assert.NotNull(slot);
            Assert.Equal("name", slot.Name);

            SlotOption[]? slotOptions = slot.SlotOption;
            Assert.NotNull(slotOptions);
            SlotOption? slotOption = Assert.Single(slotOptions);
            ValidateSlotOption(slotOption);
        }

        private static void ValidateSlotOption(SlotOption? slotOption)
        {
            Assert.NotNull(slotOption);
            Assert.True(slotOption.Default);
            Assert.Equal("devname", slotOption.DevName);
            Assert.Equal("name", slotOption.Name);
        }

        private static void ValidateSoftwareList(SoftwareList? softwareList)
        {
            Assert.NotNull(softwareList);
            Assert.Equal("filter", softwareList.Filter);
            Assert.Equal("name", softwareList.Name);
            Assert.Equal(Data.Models.Metadata.SoftwareListStatus.Original, softwareList.Status);
            Assert.Equal("tag", softwareList.Tag);
        }

        private static void ValidateSound(Sound? sound)
        {
            Assert.NotNull(sound);
            Assert.Equal(12345, sound.Channels);
        }

        private static void ValidateVideo(Display? display)
        {
            Assert.NotNull(display);
            Assert.Equal(12345, display.AspectX);
            Assert.Equal(12345, display.AspectY);
            Assert.Equal(Data.Models.Metadata.DisplayType.Vector, display.DisplayType);
            Assert.Equal(12345, display.Height);
            Assert.Equal(123.45, display.Refresh);
            Assert.Equal(12345, display.Width);
            Assert.Equal(Data.Models.Metadata.Rotation.East, display.Rotate);
        }

        #endregion
    }
}
