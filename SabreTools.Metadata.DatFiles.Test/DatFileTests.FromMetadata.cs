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
            Data.Models.Metadata.MetadataFile? item = [];

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
                [Data.Models.Metadata.MetadataFile.HeaderKey] = header,
                [Data.Models.Metadata.MetadataFile.MachineKey] = machines,
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
                [Data.Models.Metadata.MetadataFile.HeaderKey] = header,
                [Data.Models.Metadata.MetadataFile.MachineKey] = machines,
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

            DipSwitch? dipSwitch = Array.Find(datItems, item => item is DipSwitch dipSwitch && !dipSwitch.PartSpecified) as DipSwitch;
            ValidateDipSwitch(dipSwitch);

            Disk? disk = Array.Find(datItems, item => item is Disk disk && !disk.DiskAreaSpecified && !disk.PartSpecified) as Disk;
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
            DipSwitch? partDipSwitch = Array.Find(datItems, item => item is DipSwitch dipSwitch && dipSwitch.PartSpecified) as DipSwitch;
            Assert.NotNull(partDipSwitch);
            Part? dipSwitchPart = partDipSwitch.Part;
            ValidatePart(dipSwitchPart);

            // All other fields are tested separately
            Disk? partDisk = Array.Find(datItems, item => item is Disk disk && disk.DiskAreaSpecified && disk.PartSpecified) as Disk;
            Assert.NotNull(partDisk);
            ValidateDiskArea(partDisk.DiskArea);
            ValidatePart(partDisk.Part);

            PartFeature? partFeature = Array.Find(datItems, item => item is PartFeature) as PartFeature;
            ValidatePartFeature(partFeature);

            // All other fields are tested separately
            Rom? partRom = Array.Find(datItems, item => item is Rom rom && rom.DataAreaSpecified && rom.PartSpecified) as Rom;
            Assert.NotNull(partRom);
            DataArea? romDataArea = partRom.Read<DataArea>(Rom.DataAreaKey);
            ValidateDataArea(romDataArea);
            Part? romPart = partRom.Read<Part>(Rom.PartKey);
            ValidatePart(romPart);

            Port? port = Array.Find(datItems, item => item is Port) as Port;
            ValidatePort(port);

            RamOption? ramOption = Array.Find(datItems, item => item is RamOption) as RamOption;
            ValidateRamOption(ramOption);

            Release? release = Array.Find(datItems, item => item is Release) as Release;
            ValidateRelease(release);

            Rom? rom = Array.Find(datItems, item => item is Rom rom && !rom.DataAreaSpecified && !rom.PartSpecified && rom.OpenMSXMediaType is null) as Rom;
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
                [Data.Models.Metadata.Header.CanOpenKey] = canOpen,
                Category = "category",
                Comment = "comment",
                Date = "date",
                DatVersion = "datversion",
                Debug = true,
                Description = "description",
                Email = "email",
                EmulatorVersion = "emulatorversion",
                ForceMerging = Data.Models.Metadata.MergingFlag.Merged,
                ForceNodump = Data.Models.Metadata.NodumpFlag.Required,
                ForcePacking = Data.Models.Metadata.PackingFlag.Zip,
                ForceZipping = true,
                [Data.Models.Metadata.Header.HeaderKey] = "header",
                Homepage = "homepage",
                Id = "id",
                [Data.Models.Metadata.Header.ImagesKey] = images,
                [Data.Models.Metadata.Header.ImFolderKey] = "imfolder",
                [Data.Models.Metadata.Header.InfosKey] = infos,
                LockBiosMode = true,
                LockRomMode = true,
                LockSampleMode = true,
                MameConfig = "mameconfig",
                Name = "name",
                [Data.Models.Metadata.Header.NewDatKey] = newDat,
                Notes = "notes",
                Plugin = "plugin",
                RefName = "refname",
                RomMode = Data.Models.Metadata.MergingFlag.Merged,
                RomTitle = "romtitle",
                RootDir = "rootdir",
                SampleMode = Data.Models.Metadata.MergingFlag.Merged,
                [Data.Models.Metadata.Header.SchemaLocationKey] = "schemalocation",
                [Data.Models.Metadata.Header.ScreenshotsHeightKey] = "screenshotsheight",
                [Data.Models.Metadata.Header.ScreenshotsWidthKey] = "screenshotsWidth",
                [Data.Models.Metadata.Header.SearchKey] = search,
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
                [Data.Models.Metadata.Machine.AdjusterKey] = new Data.Models.Metadata.Adjuster[] { CreateMetadataAdjuster() },
                [Data.Models.Metadata.Machine.ArchiveKey] = new Data.Models.Metadata.Archive[] { CreateMetadataArchive() },
                [Data.Models.Metadata.Machine.BiosSetKey] = new Data.Models.Metadata.BiosSet[] { CreateMetadataBiosSet() },
                Board = "board",
                Buttons = "buttons",
                [Data.Models.Metadata.Machine.CategoryKey] = "category",
                [Data.Models.Metadata.Machine.ChipKey] = new Data.Models.Metadata.Chip[] { CreateMetadataChip() },
                CloneOf = "cloneof",
                CloneOfId = "cloneofid",
                [Data.Models.Metadata.Machine.CommentKey] = "comment",
                Company = "company",
                [Data.Models.Metadata.Machine.ConfigurationKey] = new Data.Models.Metadata.Configuration[] { CreateMetadataConfiguration() },
                Control = "control",
                Country = "country",
                Description = "description",
                [Data.Models.Metadata.Machine.DeviceKey] = new Data.Models.Metadata.Device[] { CreateMetadataDevice() },
                [Data.Models.Metadata.Machine.DeviceRefKey] = new Data.Models.Metadata.DeviceRef[] { CreateMetadataDeviceRef() },
                [Data.Models.Metadata.Machine.DipSwitchKey] = new Data.Models.Metadata.DipSwitch[] { CreateMetadataDipSwitch() },
                DirName = "dirname",
                [Data.Models.Metadata.Machine.DiskKey] = new Data.Models.Metadata.Disk[] { CreateMetadataDisk() },
                DisplayCount = "displaycount",
                [Data.Models.Metadata.Machine.DisplayKey] = new Data.Models.Metadata.Display[] { CreateMetadataDisplay() },
                DisplayType = "displaytype",
                [Data.Models.Metadata.Machine.DriverKey] = CreateMetadataDriver(),
                [Data.Models.Metadata.Machine.DumpKey] = new Data.Models.Metadata.Dump[] { CreateMetadataDump() },
                DuplicateID = "duplicateid",
                Emulator = "emulator",
                Extra = "extra",
                Favorite = "favorite",
                [Data.Models.Metadata.Machine.FeatureKey] = new Data.Models.Metadata.Feature[] { CreateMetadataFeature() },
                GenMSXID = "genmsxid",
                History = "history",
                Id = "id",
                Im1CRC = HashType.CRC32.ZeroString,
                Im2CRC = HashType.CRC32.ZeroString,
                ImageNumber = "imagenumber",
                [Data.Models.Metadata.Machine.InfoKey] = new Data.Models.Metadata.Info[] { CreateMetadataInfo() },
                [Data.Models.Metadata.Machine.InputKey] = CreateMetadataInput(),
                IsBios = true,
                IsDevice = true,
                IsMechanical = true,
                Language = "language",
                Location = "location",
                Manufacturer = "manufacturer",
                [Data.Models.Metadata.Machine.MediaKey] = new Data.Models.Metadata.Media[] { CreateMetadataMedia() },
                Name = "name",
                Notes = "notes",
                [Data.Models.Metadata.Machine.PartKey] = new Data.Models.Metadata.Part[] { CreateMetadataPart() },
                PlayedCount = "playedcount",
                PlayedTime = "playedtime",
                Players = "players",
                [Data.Models.Metadata.Machine.PortKey] = new Data.Models.Metadata.Port[] { CreateMetadataPort() },
                Publisher = "publisher",
                [Data.Models.Metadata.Machine.RamOptionKey] = new Data.Models.Metadata.RamOption[] { CreateMetadataRamOption() },
                RebuildTo = "rebuildto",
                [Data.Models.Metadata.Machine.ReleaseKey] = new Data.Models.Metadata.Release[] { CreateMetadataRelease() },
                ReleaseNumber = "releasenumber",
                [Data.Models.Metadata.Machine.RomKey] = new Data.Models.Metadata.Rom[] { CreateMetadataRom() },
                RomOf = "romof",
                Rotation = "rotation",
                Runnable = Data.Models.Metadata.Runnable.Yes,
                [Data.Models.Metadata.Machine.SampleKey] = new Data.Models.Metadata.Sample[] { CreateMetadataSample() },
                SampleOf = "sampleof",
                SaveType = "savetype",
                [Data.Models.Metadata.Machine.SharedFeatKey] = new Data.Models.Metadata.SharedFeat[] { CreateMetadataSharedFeat() },
                [Data.Models.Metadata.Machine.SlotKey] = new Data.Models.Metadata.Slot[] { CreateMetadataSlot() },
                [Data.Models.Metadata.Machine.SoftwareListKey] = new Data.Models.Metadata.SoftwareList[] { CreateMetadataSoftwareList() },
                [Data.Models.Metadata.Machine.SoundKey] = CreateMetadataSound(),
                SourceFile = "sourcefile",
                SourceRom = "sourcerom",
                Status = "status",
                Supported = Data.Models.Metadata.Supported.Yes,
                System = "system",
                Tags = "tags",
                [Data.Models.Metadata.Machine.TruripKey] = CreateMetadataTrurip(),
                [Data.Models.Metadata.Machine.VideoKey] = new Data.Models.Metadata.Video[] { CreateMetadataVideo() },
                Year = "year",
            };
        }

        private static Data.Models.Metadata.Adjuster CreateMetadataAdjuster()
        {
            return new Data.Models.Metadata.Adjuster
            {
                Condition = CreateMetadataCondition(),
                Default = true,
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Analog CreateMetadataAnalog()
        {
            return new Data.Models.Metadata.Analog
            {
                Mask = "mask",
            };
        }

        private static Data.Models.Metadata.Archive CreateMetadataArchive()
        {
            return new Data.Models.Metadata.Archive
            {
                [Data.Models.Metadata.Archive.NumberKey] = "number",
                [Data.Models.Metadata.Archive.CloneKey] = "clone",
                [Data.Models.Metadata.Archive.RegParentKey] = "regparent",
                [Data.Models.Metadata.Archive.MergeOfKey] = "mergeof",
                [Data.Models.Metadata.Archive.MergeNameKey] = "mergename",
                Name = "name",
                [Data.Models.Metadata.Archive.NameAltKey] = "name_alt",
                [Data.Models.Metadata.Archive.RegionKey] = "region",
                [Data.Models.Metadata.Archive.LanguagesKey] = "languages",
                [Data.Models.Metadata.Archive.ShowLangKey] = "showlang",
                [Data.Models.Metadata.Archive.LangCheckedKey] = "langchecked",
                [Data.Models.Metadata.Archive.Version1Key] = "version1",
                [Data.Models.Metadata.Archive.Version2Key] = "version2",
                [Data.Models.Metadata.Archive.DevStatusKey] = "devstatus",
                [Data.Models.Metadata.Archive.AdditionalKey] = "additional",
                [Data.Models.Metadata.Archive.Special1Key] = "special1",
                [Data.Models.Metadata.Archive.Special2Key] = "special2",
                [Data.Models.Metadata.Archive.AltKey] = "alt",
                [Data.Models.Metadata.Archive.GameId1Key] = "gameid1",
                [Data.Models.Metadata.Archive.GameId2Key] = "gameid2",
                Description = "description",
                [Data.Models.Metadata.Archive.BiosKey] = "bios",
                [Data.Models.Metadata.Archive.LicensedKey] = "licensed",
                [Data.Models.Metadata.Archive.PirateKey] = "pirate",
                [Data.Models.Metadata.Archive.PhysicalKey] = "physical",
                [Data.Models.Metadata.Archive.CompleteKey] = "complete",
                [Data.Models.Metadata.Archive.AdultKey] = "adult",
                [Data.Models.Metadata.Archive.DatKey] = "dat",
                [Data.Models.Metadata.Archive.ListedKey] = "listed",
                [Data.Models.Metadata.Archive.PrivateKey] = "private",
                [Data.Models.Metadata.Archive.StickyNoteKey] = "stickynote",
                [Data.Models.Metadata.Archive.DatterNoteKey] = "datternote",
                [Data.Models.Metadata.Archive.CategoriesKey] = "categories",
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
                Condition = CreateMetadataCondition(),
                ConfLocation = [CreateMetadataConfLocation()],
                ConfSetting = [CreateMetadataConfSetting()],
                Mask = "mask",
                Name = "name",
                Tag = "tag",
            };
        }

        private static Data.Models.Metadata.Condition CreateMetadataCondition()
        {
            return new Data.Models.Metadata.Condition
            {
                Value = "value",
                Mask = "mask",
                Relation = Data.Models.Metadata.Relation.Equal,
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
                Condition = CreateMetadataCondition(),
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
                Extension = [CreateMetadataExtension()],
                FixedImage = "fixedimage",
                Instance = CreateMetadataInstance(),
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
                Condition = CreateMetadataCondition(),
                Default = true,
                DipLocation = [CreateMetadataDipLocation()],
                DipValue = [CreateMetadataDipValue()],
                Entry = new string[] { "entry" },
                Mask = "mask",
                Name = "name",
                Tag = "tag",
            };
        }

        private static Data.Models.Metadata.DipValue CreateMetadataDipValue()
        {
            return new Data.Models.Metadata.DipValue
            {
                Condition = CreateMetadataCondition(),
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
                [Data.Models.Metadata.DataArea.RomKey] = new Data.Models.Metadata.Rom[] { [] },
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
                [Data.Models.Metadata.DiskArea.DiskKey] = new Data.Models.Metadata.Disk[] { [] },
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
                [Data.Models.Metadata.Dump.OriginalKey] = CreateMetadataOriginal(),

                // The following are searched for in order
                // For the purposes of this test, only RomKey will be populated
                // The only difference is what OpenMSXSubType value is applied
                [Data.Models.Metadata.Dump.RomKey] = new Data.Models.Metadata.Rom(),
                [Data.Models.Metadata.Dump.MegaRomKey] = null,
                [Data.Models.Metadata.Dump.SCCPlusCartKey] = null,
            };
        }

        private static Data.Models.Metadata.Extension CreateMetadataExtension()
        {
            return new Data.Models.Metadata.Extension
            {
                Name = "name",
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
                [Data.Models.Metadata.Input.ControlKey] = new Data.Models.Metadata.Control[] { CreateMetadataControl() },
                ControlAttr = "controlattr",
                Players = 12345,
                Service = true,
                Tilt = true,
            };
        }

        private static Data.Models.Metadata.Instance CreateMetadataInstance()
        {
            return new Data.Models.Metadata.Instance
            {
                BriefName = "briefname",
                Name = "name",
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
                [Data.Models.Metadata.Part.DataAreaKey] = new Data.Models.Metadata.DataArea[] { CreateMetadataDataArea() },
                [Data.Models.Metadata.Part.DiskAreaKey] = new Data.Models.Metadata.DiskArea[] { CreateMetadataDiskArea() },
                [Data.Models.Metadata.Part.DipSwitchKey] = new Data.Models.Metadata.DipSwitch[] { [] },
                [Data.Models.Metadata.Part.FeatureKey] = new Data.Models.Metadata.Feature[] { CreateMetadataFeature() },
                Interface = "interface",
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Port CreateMetadataPort()
        {
            return new Data.Models.Metadata.Port
            {
                [Data.Models.Metadata.Port.AnalogKey] = new Data.Models.Metadata.Analog[] { CreateMetadataAnalog() },
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
                [Data.Models.Metadata.Rom.AlbumKey] = "album",
                [Data.Models.Metadata.Rom.AltRomnameKey] = "alt_romname",
                [Data.Models.Metadata.Rom.AltTitleKey] = "alt_title",
                [Data.Models.Metadata.Rom.ArtistKey] = "artist",
                [Data.Models.Metadata.Rom.ASRDetectedLangKey] = "asr_detected_lang",
                [Data.Models.Metadata.Rom.ASRDetectedLangConfKey] = "asr_detected_lang_conf",
                [Data.Models.Metadata.Rom.ASRTranscribedLangKey] = "asr_transcribed_lang",
                [Data.Models.Metadata.Rom.BiosKey] = "bios",
                [Data.Models.Metadata.Rom.BitrateKey] = "bitrate",
                [Data.Models.Metadata.Rom.BitTorrentMagnetHashKey] = "btih",
                [Data.Models.Metadata.Rom.ClothCoverDetectionModuleVersionKey] = "cloth_cover_detection_module_version",
                [Data.Models.Metadata.Rom.CollectionCatalogNumberKey] = "collection-catalog-number",
                [Data.Models.Metadata.Rom.CommentKey] = "comment",
                [Data.Models.Metadata.Rom.CRCKey] = HashType.CRC32.ZeroString,
                [Data.Models.Metadata.Rom.CRC16Key] = HashType.CRC16.ZeroString,
                [Data.Models.Metadata.Rom.CRC64Key] = HashType.CRC64.ZeroString,
                [Data.Models.Metadata.Rom.CreatorKey] = "creator",
                [Data.Models.Metadata.Rom.DateKey] = "date",
                Dispose = true,
                [Data.Models.Metadata.Rom.ExtensionKey] = "extension",
                FileCount = 12345,
                FileIsAvailable = true,
                [Data.Models.Metadata.Rom.FlagsKey] = "flags",
                [Data.Models.Metadata.Rom.FormatKey] = "format",
                [Data.Models.Metadata.Rom.HeaderKey] = "header",
                [Data.Models.Metadata.Rom.HeightKey] = "height",
                [Data.Models.Metadata.Rom.hOCRCharToWordhOCRVersionKey] = "hocr_char_to_word_hocr_version",
                [Data.Models.Metadata.Rom.hOCRCharToWordModuleVersionKey] = "hocr_char_to_word_module_version",
                [Data.Models.Metadata.Rom.hOCRFtsTexthOCRVersionKey] = "hocr_fts_text_hocr_version",
                [Data.Models.Metadata.Rom.hOCRFtsTextModuleVersionKey] = "hocr_fts_text_module_version",
                [Data.Models.Metadata.Rom.hOCRPageIndexhOCRVersionKey] = "hocr_pageindex_hocr_version",
                [Data.Models.Metadata.Rom.hOCRPageIndexModuleVersionKey] = "hocr_pageindex_module_version",
                Inverted = true,
                [Data.Models.Metadata.Rom.LastModifiedTimeKey] = "mtime",
                [Data.Models.Metadata.Rom.LengthKey] = "length",
                LoadFlag = Data.Models.Metadata.LoadFlag.Load16Byte,
                [Data.Models.Metadata.Rom.MatrixNumberKey] = "matrix_number",
                [Data.Models.Metadata.Rom.MD2Key] = HashType.MD2.ZeroString,
                [Data.Models.Metadata.Rom.MD4Key] = HashType.MD4.ZeroString,
                [Data.Models.Metadata.Rom.MD5Key] = HashType.MD5.ZeroString,
                // [Data.Models.Metadata.Rom.OpenMSXMediaType] = null, // Omit due to other test
                [Data.Models.Metadata.Rom.MergeKey] = "merge",
                MIA = true,
                Name = "name",
                [Data.Models.Metadata.Rom.TesseractOCRKey] = "ocr",
                [Data.Models.Metadata.Rom.TesseractOCRConvertedKey] = "ocr_converted",
                [Data.Models.Metadata.Rom.TesseractOCRDetectedLangKey] = "ocr_detected_lang",
                [Data.Models.Metadata.Rom.TesseractOCRDetectedLangConfKey] = "ocr_detected_lang_conf",
                [Data.Models.Metadata.Rom.TesseractOCRDetectedScriptKey] = "ocr_detected_script",
                [Data.Models.Metadata.Rom.TesseractOCRDetectedScriptConfKey] = "ocr_detected_script_conf",
                [Data.Models.Metadata.Rom.TesseractOCRModuleVersionKey] = "ocr_module_version",
                [Data.Models.Metadata.Rom.TesseractOCRParametersKey] = "ocr_parameters",
                [Data.Models.Metadata.Rom.OffsetKey] = "offset",
                Optional = true,
                [Data.Models.Metadata.Rom.OriginalKey] = "original",
                [Data.Models.Metadata.Rom.PDFModuleVersionKey] = "pdf_module_version",
                [Data.Models.Metadata.Rom.PreviewImageKey] = "preview-image",
                [Data.Models.Metadata.Rom.PublisherKey] = "publisher",
                [Data.Models.Metadata.Rom.RegionKey] = "region",
                [Data.Models.Metadata.Rom.RemarkKey] = "remark",
                [Data.Models.Metadata.Rom.RIPEMD128Key] = HashType.RIPEMD128.ZeroString,
                [Data.Models.Metadata.Rom.RIPEMD160Key] = HashType.RIPEMD160.ZeroString,
                [Data.Models.Metadata.Rom.RotationKey] = "rotation",
                [Data.Models.Metadata.Rom.SerialKey] = "serial",
                [Data.Models.Metadata.Rom.SHA1Key] = HashType.SHA1.ZeroString,
                [Data.Models.Metadata.Rom.SHA256Key] = HashType.SHA256.ZeroString,
                [Data.Models.Metadata.Rom.SHA384Key] = HashType.SHA384.ZeroString,
                [Data.Models.Metadata.Rom.SHA512Key] = HashType.SHA512.ZeroString,
                Size = 12345,
                SoundOnly = true,
                [Data.Models.Metadata.Rom.SourceKey] = "source",
                [Data.Models.Metadata.Rom.SpamSumKey] = HashType.SpamSum.ZeroString,
                [Data.Models.Metadata.Rom.StartKey] = "start",
                Status = Data.Models.Metadata.ItemStatus.Good,
                [Data.Models.Metadata.Rom.SummationKey] = "summation",
                [Data.Models.Metadata.Rom.TitleKey] = "title",
                [Data.Models.Metadata.Rom.TrackKey] = "track",
                [Data.Models.Metadata.Rom.OpenMSXType] = "type",
                Value = "value",
                [Data.Models.Metadata.Rom.WhisperASRModuleVersionKey] = "whisper_asr_module_version",
                [Data.Models.Metadata.Rom.WhisperModelHashKey] = "whisper_model_hash",
                [Data.Models.Metadata.Rom.WhisperModelNameKey] = "whisper_model_name",
                [Data.Models.Metadata.Rom.WhisperVersionKey] = "whisper_version",
                [Data.Models.Metadata.Rom.WidthKey] = "width",
                [Data.Models.Metadata.Rom.WordConfidenceInterval0To10Key] = "word_conf_0_10",
                [Data.Models.Metadata.Rom.WordConfidenceInterval11To20Key] = "word_conf_11_20",
                [Data.Models.Metadata.Rom.WordConfidenceInterval21To30Key] = "word_conf_21_30",
                [Data.Models.Metadata.Rom.WordConfidenceInterval31To40Key] = "word_conf_31_40",
                [Data.Models.Metadata.Rom.WordConfidenceInterval41To50Key] = "word_conf_41_50",
                [Data.Models.Metadata.Rom.WordConfidenceInterval51To60Key] = "word_conf_51_60",
                [Data.Models.Metadata.Rom.WordConfidenceInterval61To70Key] = "word_conf_61_70",
                [Data.Models.Metadata.Rom.WordConfidenceInterval71To80Key] = "word_conf_71_80",
                [Data.Models.Metadata.Rom.WordConfidenceInterval81To90Key] = "word_conf_81_90",
                [Data.Models.Metadata.Rom.WordConfidenceInterval91To100Key] = "word_conf_91_100",
                [Data.Models.Metadata.Rom.xxHash364Key] = HashType.XxHash3.ZeroString,
                [Data.Models.Metadata.Rom.xxHash3128Key] = HashType.XxHash128.ZeroString,
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
                [Data.Models.Metadata.Slot.SlotOptionKey] = new Data.Models.Metadata.SlotOption[] { CreateMetadataSlotOption() },
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

        private static Data.Models.Logiqx.Trurip CreateMetadataTrurip()
        {
            return new Data.Models.Logiqx.Trurip
            {
                TitleID = "titleid",
                Publisher = "publisher",
                Developer = "developer",
                Year = "year",
                Genre = "genre",
                Subgenre = "subgenre",
                Ratings = "ratings",
                Score = "score",
                Players = "players",
                Enabled = "enabled",
                CRC = "yes",
                Source = "source",
                CloneOf = "cloneof",
                RelatedTo = "relatedto",
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
            Assert.Equal("ext", datHeader.ReadString(Data.Models.Metadata.Header.CanOpenKey));
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
            Assert.Equal("header", datHeader.ReadString(Data.Models.Metadata.Header.HeaderKey));
            Assert.Equal("homepage", datHeader.Homepage);
            Assert.Equal("id", datHeader.Id);
            Assert.NotNull(datHeader.ReadString(Data.Models.Metadata.Header.ImagesKey));
            Assert.Equal("imfolder", datHeader.ReadString(Data.Models.Metadata.Header.ImFolderKey));
            Assert.NotNull(datHeader.ReadString(Data.Models.Metadata.Header.InfosKey));
            Assert.True(datHeader.LockBiosMode);
            Assert.True(datHeader.LockRomMode);
            Assert.True(datHeader.LockSampleMode);
            Assert.Equal("mameconfig", datHeader.MameConfig);
            Assert.Equal("name", datHeader.Name);
            Assert.NotNull(datHeader.ReadString(Data.Models.Metadata.Header.NewDatKey));
            Assert.Equal("notes", datHeader.Notes);
            Assert.Equal("plugin", datHeader.Plugin);
            Assert.Equal("refname", datHeader.RefName);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, datHeader.RomMode);
            Assert.Equal("romtitle", datHeader.RomTitle);
            Assert.Equal("rootdir", datHeader.RootDir);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, datHeader.SampleMode);
            Assert.Equal("schemalocation", datHeader.ReadString(Data.Models.Metadata.Header.SchemaLocationKey));
            Assert.Equal("screenshotsheight", datHeader.ReadString(Data.Models.Metadata.Header.ScreenshotsHeightKey));
            Assert.Equal("screenshotsWidth", datHeader.ReadString(Data.Models.Metadata.Header.ScreenshotsWidthKey));
            Assert.NotNull(datHeader.ReadString(Data.Models.Metadata.Header.SearchKey));
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
            Assert.Equal("category", machine.ReadString(Data.Models.Metadata.Machine.CategoryKey));
            Assert.Equal("cloneof", machine.CloneOf);
            Assert.Equal("cloneofid", machine.CloneOfId);
            Assert.Equal("comment", machine.ReadString(Data.Models.Metadata.Machine.CommentKey));
            Assert.Equal("company", machine.Company);
            Assert.Equal("control", machine.Control);
            Assert.Equal("country", machine.Country);
            Assert.Equal("description", machine.Description);
            Assert.Equal("dirname", machine.DirName);
            Assert.Equal("displaycount", machine.DisplayCount);
            Assert.Equal("displaytype", machine.DisplayType);
            Assert.Equal("duplicateid", machine.DuplicateID);
            Assert.Equal("emulator", machine.Emulator);
            Assert.Equal("extra", machine.Extra);
            Assert.Equal("favorite", machine.Favorite);
            Assert.Equal("genmsxid", machine.GenMSXID);
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
            Assert.Equal("rebuildto", machine.RebuildTo);
            Assert.Equal("releasenumber", machine.ReleaseNumber);
            Assert.Equal("romof", machine.RomOf);
            Assert.Equal("rotation", machine.Rotation);
            Assert.Equal(Data.Models.Metadata.Runnable.Yes, machine.Runnable);
            Assert.Equal("sampleof", machine.SampleOf);
            Assert.Equal("savetype", machine.SaveType);
            Assert.Equal("sourcefile", machine.SourceFile);
            Assert.Equal("sourcerom", machine.SourceRom);
            Assert.Equal("status", machine.Status);
            Assert.Equal(Data.Models.Metadata.Supported.Yes, machine.Supported);
            Assert.Equal("system", machine.System);
            Assert.Equal("tags", machine.Tags);
            Assert.Equal("year", machine.Year);

            DatItems.Trurip? trurip = machine.Read<DatItems.Trurip>(Data.Models.Metadata.Machine.TruripKey);
            ValidateTrurip(trurip);
        }
#pragma warning restore IDE0051

        private static void ValidateAdjuster(Adjuster? adjuster)
        {
            Assert.NotNull(adjuster);
            Assert.True(adjuster.Default);
            Assert.Equal("name", adjuster.Name);

            ValidateCondition(adjuster.Condition);
        }

        private static void ValidateAnalog(Analog? analog)
        {
            Assert.NotNull(analog);
            Assert.Equal("mask", analog.Mask);
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

        private static void ValidateCondition(Condition? condition)
        {
            Assert.NotNull(condition);
            Assert.Equal("value", condition.Value);
            Assert.Equal("mask", condition.Mask);
            Assert.Equal(Data.Models.Metadata.Relation.Equal, condition.Relation);
            Assert.Equal("tag", condition.Tag);
        }

        private static void ValidateConfiguration(Configuration? configuration)
        {
            Assert.NotNull(configuration);
            Assert.Equal("mask", configuration.Mask);
            Assert.Equal("name", configuration.Name);
            Assert.Equal("tag", configuration.Tag);

            ValidateCondition(configuration.Condition);

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
            Assert.True(confSetting.Default);
            Assert.Equal("name", confSetting.Name);
            Assert.Equal("value", confSetting.Value);

            ValidateCondition(confSetting.Condition);
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
            Assert.Equal("interface", device.Interface);
            Assert.Equal(true, device.Mandatory);
            Assert.Equal("tag", device.Tag);
            Assert.Equal(Data.Models.Metadata.DeviceType.PunchTape, device.DeviceType);

            Extension[]? extensions = device.Extension;
            Assert.NotNull(extensions);
            Extension? extension = Assert.Single(extensions);
            ValidateExtension(extension);

            ValidateInstance(device.Instance);
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
            Assert.True(dipSwitch.Default);
            Assert.Equal("mask", dipSwitch.Mask);
            Assert.Equal("name", dipSwitch.Name);
            Assert.Equal("tag", dipSwitch.Tag);

            ValidateCondition(dipSwitch.Condition);

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
            Assert.True(dipValue.Default);
            Assert.Equal("name", dipValue.Name);
            Assert.Equal("value", dipValue.Value);

            ValidateCondition(dipValue.Condition);
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

        private static void ValidateExtension(Extension? extension)
        {
            Assert.NotNull(extension);
            Assert.Equal("name", extension.Name);
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

            Control[]? controls = input.Read<Control[]>(Data.Models.Metadata.Input.ControlKey);
            Assert.NotNull(controls);
            Control? control = Assert.Single(controls);
            ValidateControl(control);
        }

        private static void ValidateInstance(Instance? instance)
        {
            Assert.NotNull(instance);
            Assert.Equal("briefname", instance.BriefName);
            Assert.Equal("name", instance.Name);
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

        private static void ValidatePart(Part? part)
        {
            Assert.NotNull(part);
            Assert.Equal("interface", part.Interface);
            Assert.Equal("name", part.Name);
        }

        private static void ValidatePartFeature(PartFeature? partFeature)
        {
            Assert.NotNull(partFeature);
            Assert.Equal("name", partFeature.Name);
            Assert.Equal(Data.Models.Metadata.FeatureStatus.Imperfect, partFeature.Overall);
            Assert.Equal(Data.Models.Metadata.FeatureStatus.Imperfect, partFeature.Status);
            Assert.Equal(Data.Models.Metadata.FeatureType.Protection, partFeature.FeatureType);
            Assert.Equal("value", partFeature.Value);

            Part? part = partFeature.Read<Part>(PartFeature.PartKey);
            ValidatePart(part);
        }

        private static void ValidatePort(Port? port)
        {
            Assert.NotNull(port);
            Assert.Equal("tag", port.Tag);

            Analog[]? dipValues = port.Read<Analog[]>(Data.Models.Metadata.Port.AnalogKey);
            Assert.NotNull(dipValues);
            Analog? dipValue = Assert.Single(dipValues);
            ValidateAnalog(dipValue);
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
            Assert.Equal("album", rom.ReadString(Data.Models.Metadata.Rom.AlbumKey));
            Assert.Equal("alt_romname", rom.ReadString(Data.Models.Metadata.Rom.AltRomnameKey));
            Assert.Equal("alt_title", rom.ReadString(Data.Models.Metadata.Rom.AltTitleKey));
            Assert.Equal("artist", rom.ReadString(Data.Models.Metadata.Rom.ArtistKey));
            Assert.Equal("asr_detected_lang", rom.ReadString(Data.Models.Metadata.Rom.ASRDetectedLangKey));
            Assert.Equal("asr_detected_lang_conf", rom.ReadString(Data.Models.Metadata.Rom.ASRDetectedLangConfKey));
            Assert.Equal("asr_transcribed_lang", rom.ReadString(Data.Models.Metadata.Rom.ASRTranscribedLangKey));
            Assert.Equal("bios", rom.ReadString(Data.Models.Metadata.Rom.BiosKey));
            Assert.Equal("bitrate", rom.ReadString(Data.Models.Metadata.Rom.BitrateKey));
            Assert.Equal("btih", rom.ReadString(Data.Models.Metadata.Rom.BitTorrentMagnetHashKey));
            Assert.Equal("cloth_cover_detection_module_version", rom.ReadString(Data.Models.Metadata.Rom.ClothCoverDetectionModuleVersionKey));
            Assert.Equal("collection-catalog-number", rom.ReadString(Data.Models.Metadata.Rom.CollectionCatalogNumberKey));
            Assert.Equal("comment", rom.ReadString(Data.Models.Metadata.Rom.CommentKey));
            Assert.Equal(HashType.CRC32.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.CRCKey));
            Assert.Equal(HashType.CRC16.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.CRC16Key));
            Assert.Equal(HashType.CRC64.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.CRC64Key));
            Assert.Equal("creator", rom.ReadString(Data.Models.Metadata.Rom.CreatorKey));
            Assert.Equal("date", rom.ReadString(Data.Models.Metadata.Rom.DateKey));
            Assert.True(rom.Dispose);
            Assert.Equal("extension", rom.ReadString(Data.Models.Metadata.Rom.ExtensionKey));
            Assert.Equal(12345, rom.FileCount);
            Assert.True(rom.FileIsAvailable);
            Assert.Equal("flags", rom.ReadString(Data.Models.Metadata.Rom.FlagsKey));
            Assert.Equal("format", rom.ReadString(Data.Models.Metadata.Rom.FormatKey));
            Assert.Equal("header", rom.ReadString(Data.Models.Metadata.Rom.HeaderKey));
            Assert.Equal("height", rom.ReadString(Data.Models.Metadata.Rom.HeightKey));
            Assert.Equal("hocr_char_to_word_hocr_version", rom.ReadString(Data.Models.Metadata.Rom.hOCRCharToWordhOCRVersionKey));
            Assert.Equal("hocr_char_to_word_module_version", rom.ReadString(Data.Models.Metadata.Rom.hOCRCharToWordModuleVersionKey));
            Assert.Equal("hocr_fts_text_hocr_version", rom.ReadString(Data.Models.Metadata.Rom.hOCRFtsTexthOCRVersionKey));
            Assert.Equal("hocr_fts_text_module_version", rom.ReadString(Data.Models.Metadata.Rom.hOCRFtsTextModuleVersionKey));
            Assert.Equal("hocr_pageindex_hocr_version", rom.ReadString(Data.Models.Metadata.Rom.hOCRPageIndexhOCRVersionKey));
            Assert.Equal("hocr_pageindex_module_version", rom.ReadString(Data.Models.Metadata.Rom.hOCRPageIndexModuleVersionKey));
            Assert.True(rom.Inverted);
            Assert.Equal("mtime", rom.ReadString(Data.Models.Metadata.Rom.LastModifiedTimeKey));
            Assert.Equal("length", rom.ReadString(Data.Models.Metadata.Rom.LengthKey));
            Assert.Equal(Data.Models.Metadata.LoadFlag.Load16Byte, rom.LoadFlag);
            Assert.Equal("matrix_number", rom.ReadString(Data.Models.Metadata.Rom.MatrixNumberKey));
            Assert.Equal(HashType.MD2.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.MD2Key));
            Assert.Equal(HashType.MD4.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.MD4Key));
            Assert.Equal(HashType.MD5.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.MD5Key));
            Assert.Null(rom.OpenMSXMediaType); // Omit due to other test
            Assert.Equal("merge", rom.ReadString(Data.Models.Metadata.Rom.MergeKey));
            Assert.True(rom.MIA);
            Assert.Equal("name", rom.Name);
            Assert.Equal("ocr", rom.ReadString(Data.Models.Metadata.Rom.TesseractOCRKey));
            Assert.Equal("ocr_converted", rom.ReadString(Data.Models.Metadata.Rom.TesseractOCRConvertedKey));
            Assert.Equal("ocr_detected_lang", rom.ReadString(Data.Models.Metadata.Rom.TesseractOCRDetectedLangKey));
            Assert.Equal("ocr_detected_lang_conf", rom.ReadString(Data.Models.Metadata.Rom.TesseractOCRDetectedLangConfKey));
            Assert.Equal("ocr_detected_script", rom.ReadString(Data.Models.Metadata.Rom.TesseractOCRDetectedScriptKey));
            Assert.Equal("ocr_detected_script_conf", rom.ReadString(Data.Models.Metadata.Rom.TesseractOCRDetectedScriptConfKey));
            Assert.Equal("ocr_module_version", rom.ReadString(Data.Models.Metadata.Rom.TesseractOCRModuleVersionKey));
            Assert.Equal("ocr_parameters", rom.ReadString(Data.Models.Metadata.Rom.TesseractOCRParametersKey));
            Assert.Equal("offset", rom.ReadString(Data.Models.Metadata.Rom.OffsetKey));
            Assert.True(rom.Optional);
            Assert.Equal("original", rom.ReadString(Data.Models.Metadata.Rom.OriginalKey));
            Assert.Equal("pdf_module_version", rom.ReadString(Data.Models.Metadata.Rom.PDFModuleVersionKey));
            Assert.Equal("preview-image", rom.ReadString(Data.Models.Metadata.Rom.PreviewImageKey));
            Assert.Equal("publisher", rom.ReadString(Data.Models.Metadata.Rom.PublisherKey));
            Assert.Equal("region", rom.ReadString(Data.Models.Metadata.Rom.RegionKey));
            Assert.Equal("remark", rom.ReadString(Data.Models.Metadata.Rom.RemarkKey));
            Assert.Equal(HashType.RIPEMD128.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.RIPEMD128Key));
            Assert.Equal(HashType.RIPEMD160.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.RIPEMD160Key));
            Assert.Equal("rotation", rom.ReadString(Data.Models.Metadata.Rom.RotationKey));
            Assert.Equal("serial", rom.ReadString(Data.Models.Metadata.Rom.SerialKey));
            Assert.Equal(HashType.SHA1.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.SHA1Key));
            Assert.Equal(HashType.SHA256.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.SHA256Key));
            Assert.Equal(HashType.SHA384.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.SHA384Key));
            Assert.Equal(HashType.SHA512.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.SHA512Key));
            Assert.Equal(12345, rom.Size);
            Assert.True(rom.SoundOnly);
            Assert.Equal("source", rom.ReadString(Data.Models.Metadata.Rom.SourceKey));
            Assert.Equal(HashType.SpamSum.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.SpamSumKey));
            Assert.Equal("start", rom.ReadString(Data.Models.Metadata.Rom.StartKey));
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, rom.Status);
            Assert.Equal("summation", rom.ReadString(Data.Models.Metadata.Rom.SummationKey));
            Assert.Equal("title", rom.ReadString(Data.Models.Metadata.Rom.TitleKey));
            Assert.Equal("track", rom.ReadString(Data.Models.Metadata.Rom.TrackKey));
            Assert.Equal("type", rom.ReadString(Data.Models.Metadata.Rom.OpenMSXType));
            Assert.Equal("value", rom.Value);
            Assert.Equal("whisper_asr_module_version", rom.ReadString(Data.Models.Metadata.Rom.WhisperASRModuleVersionKey));
            Assert.Equal("whisper_model_hash", rom.ReadString(Data.Models.Metadata.Rom.WhisperModelHashKey));
            Assert.Equal("whisper_model_name", rom.ReadString(Data.Models.Metadata.Rom.WhisperModelNameKey));
            Assert.Equal("whisper_version", rom.ReadString(Data.Models.Metadata.Rom.WhisperVersionKey));
            Assert.Equal("width", rom.ReadString(Data.Models.Metadata.Rom.WidthKey));
            Assert.Equal("word_conf_0_10", rom.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval0To10Key));
            Assert.Equal("word_conf_11_20", rom.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval11To20Key));
            Assert.Equal("word_conf_21_30", rom.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval21To30Key));
            Assert.Equal("word_conf_31_40", rom.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval31To40Key));
            Assert.Equal("word_conf_41_50", rom.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval41To50Key));
            Assert.Equal("word_conf_51_60", rom.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval51To60Key));
            Assert.Equal("word_conf_61_70", rom.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval61To70Key));
            Assert.Equal("word_conf_71_80", rom.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval71To80Key));
            Assert.Equal("word_conf_81_90", rom.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval81To90Key));
            Assert.Equal("word_conf_91_100", rom.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval91To100Key));
            Assert.Equal(HashType.XxHash3.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.xxHash364Key));
            Assert.Equal(HashType.XxHash128.ZeroString, rom.ReadString(Data.Models.Metadata.Rom.xxHash3128Key));
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

            SlotOption[]? slotOptions = slot.Read<SlotOption[]>(Data.Models.Metadata.Slot.SlotOptionKey);
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

        // TODO: Figure out why so many fields are omitted
        private static void ValidateTrurip(DatItems.Trurip? trurip)
        {
            Assert.NotNull(trurip);
            Assert.Equal("titleid", trurip.TitleID);
            //Assert.Equal("publisher", trurip.Publisher); // Omitted from conversion
            Assert.Equal("developer", trurip.Developer);
            // Assert.Equal("year", trurip.Year); // Omitted from conversion
            Assert.Equal("genre", trurip.Genre);
            Assert.Equal("subgenre", trurip.Subgenre);
            Assert.Equal("ratings", trurip.Ratings);
            Assert.Equal("score", trurip.Score);
            // Assert.Equal("players", trurip.Players); // Omitted from conversion
            Assert.Equal("enabled", trurip.Enabled);
            Assert.True(trurip.Crc);
            // Assert.Equal("source", trurip.Source); // Omitted from conversion
            // Assert.Equal("cloneof", trurip.CloneOf); // Omitted from conversion
            Assert.Equal("relatedto", trurip.RelatedTo);
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
