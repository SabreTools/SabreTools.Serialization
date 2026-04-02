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

            Display? display = Array.Find(datItems, item => item is Display display && display.ReadLong(Data.Models.Metadata.Video.AspectXKey) is null) as Display;
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
            Part? dipSwitchPart = partDipSwitch.Read<Part>(DipSwitch.PartKey);
            ValidatePart(dipSwitchPart);

            // All other fields are tested separately
            Disk? partDisk = Array.Find(datItems, item => item is Disk disk && disk.DiskAreaSpecified && disk.PartSpecified) as Disk;
            Assert.NotNull(partDisk);
            DiskArea? diskDiskArea = partDisk.Read<DiskArea>(Disk.DiskAreaKey);
            ValidateDiskArea(diskDiskArea);
            Part? diskPart = partDisk.Read<Part>(Disk.PartKey);
            ValidatePart(diskPart);

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

            Display? video = Array.Find(datItems, item => item is Display display && display.ReadLong(Data.Models.Metadata.Video.AspectXKey) is not null) as Display;
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
                [Data.Models.Metadata.Header.AuthorKey] = "author",
                BiosMode = Data.Models.Metadata.MergingFlag.Merged,
                [Data.Models.Metadata.Header.BuildKey] = "build",
                [Data.Models.Metadata.Header.CanOpenKey] = canOpen,
                [Data.Models.Metadata.Header.CategoryKey] = "category",
                [Data.Models.Metadata.Header.CommentKey] = "comment",
                [Data.Models.Metadata.Header.DateKey] = "date",
                [Data.Models.Metadata.Header.DatVersionKey] = "datversion",
                Debug = true,
                Description = "description",
                [Data.Models.Metadata.Header.EmailKey] = "email",
                [Data.Models.Metadata.Header.EmulatorVersionKey] = "emulatorversion",
                ForceMerging = Data.Models.Metadata.MergingFlag.Merged,
                ForceNodump = Data.Models.Metadata.NodumpFlag.Required,
                ForcePacking = Data.Models.Metadata.PackingFlag.Zip,
                ForceZipping = true,
                [Data.Models.Metadata.Header.HeaderKey] = "header",
                [Data.Models.Metadata.Header.HomepageKey] = "homepage",
                [Data.Models.Metadata.Header.IdKey] = "id",
                [Data.Models.Metadata.Header.ImagesKey] = images,
                [Data.Models.Metadata.Header.ImFolderKey] = "imfolder",
                [Data.Models.Metadata.Header.InfosKey] = infos,
                LockBiosMode = true,
                LockRomMode = true,
                LockSampleMode = true,
                [Data.Models.Metadata.Header.MameConfigKey] = "mameconfig",
                Name = "name",
                [Data.Models.Metadata.Header.NewDatKey] = newDat,
                [Data.Models.Metadata.Header.NotesKey] = "notes",
                [Data.Models.Metadata.Header.PluginKey] = "plugin",
                [Data.Models.Metadata.Header.RefNameKey] = "refname",
                RomMode = Data.Models.Metadata.MergingFlag.Merged,
                [Data.Models.Metadata.Header.RomTitleKey] = "romtitle",
                [Data.Models.Metadata.Header.RootDirKey] = "rootdir",
                SampleMode = Data.Models.Metadata.MergingFlag.Merged,
                [Data.Models.Metadata.Header.SchemaLocationKey] = "schemalocation",
                [Data.Models.Metadata.Header.ScreenshotsHeightKey] = "screenshotsheight",
                [Data.Models.Metadata.Header.ScreenshotsWidthKey] = "screenshotsWidth",
                [Data.Models.Metadata.Header.SearchKey] = search,
                [Data.Models.Metadata.Header.SystemKey] = "system",
                [Data.Models.Metadata.Header.TimestampKey] = "timestamp",
                [Data.Models.Metadata.Header.TypeKey] = "type",
                [Data.Models.Metadata.Header.UrlKey] = "url",
                [Data.Models.Metadata.Header.VersionKey] = "version",
            };
        }

        private static Data.Models.Metadata.Machine CreateMetadataMachine()
        {
            return new Data.Models.Metadata.Machine
            {
                [Data.Models.Metadata.Machine.AdjusterKey] = new Data.Models.Metadata.Adjuster[] { CreateMetadataAdjuster() },
                [Data.Models.Metadata.Machine.ArchiveKey] = new Data.Models.Metadata.Archive[] { CreateMetadataArchive() },
                [Data.Models.Metadata.Machine.BiosSetKey] = new Data.Models.Metadata.BiosSet[] { CreateMetadataBiosSet() },
                [Data.Models.Metadata.Machine.BoardKey] = "board",
                [Data.Models.Metadata.Machine.ButtonsKey] = "buttons",
                [Data.Models.Metadata.Machine.CategoryKey] = "category",
                [Data.Models.Metadata.Machine.ChipKey] = new Data.Models.Metadata.Chip[] { CreateMetadataChip() },
                [Data.Models.Metadata.Machine.CloneOfKey] = "cloneof",
                [Data.Models.Metadata.Machine.CloneOfIdKey] = "cloneofid",
                [Data.Models.Metadata.Machine.CommentKey] = "comment",
                [Data.Models.Metadata.Machine.CompanyKey] = "company",
                [Data.Models.Metadata.Machine.ConfigurationKey] = new Data.Models.Metadata.Configuration[] { CreateMetadataConfiguration() },
                [Data.Models.Metadata.Machine.ControlKey] = "control",
                [Data.Models.Metadata.Machine.CountryKey] = "country",
                Description = "description",
                [Data.Models.Metadata.Machine.DeviceKey] = new Data.Models.Metadata.Device[] { CreateMetadataDevice() },
                [Data.Models.Metadata.Machine.DeviceRefKey] = new Data.Models.Metadata.DeviceRef[] { CreateMetadataDeviceRef() },
                [Data.Models.Metadata.Machine.DipSwitchKey] = new Data.Models.Metadata.DipSwitch[] { CreateMetadataDipSwitch() },
                [Data.Models.Metadata.Machine.DirNameKey] = "dirname",
                [Data.Models.Metadata.Machine.DiskKey] = new Data.Models.Metadata.Disk[] { CreateMetadataDisk() },
                [Data.Models.Metadata.Machine.DisplayCountKey] = "displaycount",
                [Data.Models.Metadata.Machine.DisplayKey] = new Data.Models.Metadata.Display[] { CreateMetadataDisplay() },
                [Data.Models.Metadata.Machine.DisplayTypeKey] = "displaytype",
                [Data.Models.Metadata.Machine.DriverKey] = CreateMetadataDriver(),
                [Data.Models.Metadata.Machine.DumpKey] = new Data.Models.Metadata.Dump[] { CreateMetadataDump() },
                [Data.Models.Metadata.Machine.DuplicateIDKey] = "duplicateid",
                [Data.Models.Metadata.Machine.EmulatorKey] = "emulator",
                [Data.Models.Metadata.Machine.ExtraKey] = "extra",
                [Data.Models.Metadata.Machine.FavoriteKey] = "favorite",
                [Data.Models.Metadata.Machine.FeatureKey] = new Data.Models.Metadata.Feature[] { CreateMetadataFeature() },
                [Data.Models.Metadata.Machine.GenMSXIDKey] = "genmsxid",
                [Data.Models.Metadata.Machine.HistoryKey] = "history",
                [Data.Models.Metadata.Machine.IdKey] = "id",
                [Data.Models.Metadata.Machine.Im1CRCKey] = HashType.CRC32.ZeroString,
                [Data.Models.Metadata.Machine.Im2CRCKey] = HashType.CRC32.ZeroString,
                [Data.Models.Metadata.Machine.ImageNumberKey] = "imagenumber",
                [Data.Models.Metadata.Machine.InfoKey] = new Data.Models.Metadata.Info[] { CreateMetadataInfo() },
                [Data.Models.Metadata.Machine.InputKey] = CreateMetadataInput(),
                IsBios = true,
                IsDevice = true,
                IsMechanical = true,
                [Data.Models.Metadata.Machine.LanguageKey] = "language",
                [Data.Models.Metadata.Machine.LocationKey] = "location",
                [Data.Models.Metadata.Machine.ManufacturerKey] = "manufacturer",
                [Data.Models.Metadata.Machine.MediaKey] = new Data.Models.Metadata.Media[] { CreateMetadataMedia() },
                Name = "name",
                [Data.Models.Metadata.Machine.NotesKey] = "notes",
                [Data.Models.Metadata.Machine.PartKey] = new Data.Models.Metadata.Part[] { CreateMetadataPart() },
                [Data.Models.Metadata.Machine.PlayedCountKey] = "playedcount",
                [Data.Models.Metadata.Machine.PlayedTimeKey] = "playedtime",
                [Data.Models.Metadata.Machine.PlayersKey] = "players",
                [Data.Models.Metadata.Machine.PortKey] = new Data.Models.Metadata.Port[] { CreateMetadataPort() },
                [Data.Models.Metadata.Machine.PublisherKey] = "publisher",
                [Data.Models.Metadata.Machine.RamOptionKey] = new Data.Models.Metadata.RamOption[] { CreateMetadataRamOption() },
                [Data.Models.Metadata.Machine.RebuildToKey] = "rebuildto",
                [Data.Models.Metadata.Machine.ReleaseKey] = new Data.Models.Metadata.Release[] { CreateMetadataRelease() },
                [Data.Models.Metadata.Machine.ReleaseNumberKey] = "releasenumber",
                [Data.Models.Metadata.Machine.RomKey] = new Data.Models.Metadata.Rom[] { CreateMetadataRom() },
                [Data.Models.Metadata.Machine.RomOfKey] = "romof",
                [Data.Models.Metadata.Machine.RotationKey] = "rotation",
                Runnable = Data.Models.Metadata.Runnable.Yes,
                [Data.Models.Metadata.Machine.SampleKey] = new Data.Models.Metadata.Sample[] { CreateMetadataSample() },
                [Data.Models.Metadata.Machine.SampleOfKey] = "sampleof",
                [Data.Models.Metadata.Machine.SaveTypeKey] = "savetype",
                [Data.Models.Metadata.Machine.SharedFeatKey] = new Data.Models.Metadata.SharedFeat[] { CreateMetadataSharedFeat() },
                [Data.Models.Metadata.Machine.SlotKey] = new Data.Models.Metadata.Slot[] { CreateMetadataSlot() },
                [Data.Models.Metadata.Machine.SoftwareListKey] = new Data.Models.Metadata.SoftwareList[] { CreateMetadataSoftwareList() },
                [Data.Models.Metadata.Machine.SoundKey] = CreateMetadataSound(),
                [Data.Models.Metadata.Machine.SourceFileKey] = "sourcefile",
                [Data.Models.Metadata.Machine.SourceRomKey] = "sourcerom",
                [Data.Models.Metadata.Machine.StatusKey] = "status",
                Supported = Data.Models.Metadata.Supported.Yes,
                [Data.Models.Metadata.Machine.SystemKey] = "system",
                [Data.Models.Metadata.Machine.TagsKey] = "tags",
                [Data.Models.Metadata.Machine.TruripKey] = CreateMetadataTrurip(),
                [Data.Models.Metadata.Machine.VideoKey] = new Data.Models.Metadata.Video[] { CreateMetadataVideo() },
                [Data.Models.Metadata.Machine.YearKey] = "year",
            };
        }

        private static Data.Models.Metadata.Adjuster CreateMetadataAdjuster()
        {
            return new Data.Models.Metadata.Adjuster
            {
                [Data.Models.Metadata.Adjuster.ConditionKey] = CreateMetadataCondition(),
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
                [Data.Models.Metadata.Chip.ClockKey] = 12345L,
                [Data.Models.Metadata.Chip.FlagsKey] = "flags",
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
                [Data.Models.Metadata.Configuration.ConditionKey] = CreateMetadataCondition(),
                [Data.Models.Metadata.Configuration.ConfLocationKey] = new Data.Models.Metadata.ConfLocation[] { CreateMetadataConfLocation() },
                [Data.Models.Metadata.Configuration.ConfSettingKey] = new Data.Models.Metadata.ConfSetting[] { CreateMetadataConfSetting() },
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
                [Data.Models.Metadata.ConfLocation.NumberKey] = "number",
            };
        }

        private static Data.Models.Metadata.ConfSetting CreateMetadataConfSetting()
        {
            return new Data.Models.Metadata.ConfSetting
            {
                [Data.Models.Metadata.ConfSetting.ConditionKey] = CreateMetadataCondition(),
                Default = true,
                Name = "name",
                Value = "value",
            };
        }

        private static Data.Models.Metadata.Control CreateMetadataControl()
        {
            return new Data.Models.Metadata.Control
            {
                [Data.Models.Metadata.Control.ButtonsKey] = 12345L,
                [Data.Models.Metadata.Control.KeyDeltaKey] = 12345L,
                [Data.Models.Metadata.Control.MaximumKey] = 12345L,
                [Data.Models.Metadata.Control.MinimumKey] = 12345L,
                [Data.Models.Metadata.Control.PlayerKey] = 12345L,
                [Data.Models.Metadata.Control.ReqButtonsKey] = 12345L,
                Reverse = true,
                [Data.Models.Metadata.Control.SensitivityKey] = 12345L,
                ControlType = Data.Models.Metadata.ControlType.Lightgun,
                [Data.Models.Metadata.Control.WaysKey] = "ways",
                [Data.Models.Metadata.Control.Ways2Key] = "ways2",
                [Data.Models.Metadata.Control.Ways3Key] = "ways3",
            };
        }

        private static Data.Models.Metadata.Device CreateMetadataDevice()
        {
            return new Data.Models.Metadata.Device
            {
                [Data.Models.Metadata.Device.ExtensionKey] = new Data.Models.Metadata.Extension[] { CreateMetadataExtension() },
                [Data.Models.Metadata.Device.FixedImageKey] = "fixedimage",
                [Data.Models.Metadata.Device.InstanceKey] = CreateMetadataInstance(),
                [Data.Models.Metadata.Device.InterfaceKey] = "interface",
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
                [Data.Models.Metadata.DipLocation.NumberKey] = "number",
            };
        }

        private static Data.Models.Metadata.DipSwitch CreateMetadataDipSwitch()
        {
            return new Data.Models.Metadata.DipSwitch
            {
                [Data.Models.Metadata.DipSwitch.ConditionKey] = CreateMetadataCondition(),
                Default = true,
                [Data.Models.Metadata.DipSwitch.DipLocationKey] = new Data.Models.Metadata.DipLocation[] { CreateMetadataDipLocation() },
                [Data.Models.Metadata.DipSwitch.DipValueKey] = new Data.Models.Metadata.DipValue[] { CreateMetadataDipValue() },
                [Data.Models.Metadata.DipSwitch.EntryKey] = new string[] { "entry" },
                Mask = "mask",
                Name = "name",
                Tag = "tag",
            };
        }

        private static Data.Models.Metadata.DipValue CreateMetadataDipValue()
        {
            return new Data.Models.Metadata.DipValue
            {
                [Data.Models.Metadata.DipValue.ConditionKey] = CreateMetadataCondition(),
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
                [Data.Models.Metadata.DataArea.SizeKey] = 12345L,
                [Data.Models.Metadata.DataArea.WidthKey] = 64,
            };
        }

        private static Data.Models.Metadata.Disk CreateMetadataDisk()
        {
            return new Data.Models.Metadata.Disk
            {
                [Data.Models.Metadata.Disk.FlagsKey] = "flags",
                [Data.Models.Metadata.Disk.IndexKey] = "index",
                [Data.Models.Metadata.Disk.MD5Key] = HashType.MD5.ZeroString,
                [Data.Models.Metadata.Disk.MergeKey] = "merge",
                Name = "name",
                Optional = true,
                [Data.Models.Metadata.Disk.RegionKey] = "region",
                [Data.Models.Metadata.Disk.SHA1Key] = HashType.SHA1.ZeroString,
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
                FlipX = true,
                [Data.Models.Metadata.Display.HBEndKey] = 12345L,
                [Data.Models.Metadata.Display.HBStartKey] = 12345L,
                [Data.Models.Metadata.Display.HeightKey] = 12345L,
                [Data.Models.Metadata.Display.HTotalKey] = 12345L,
                [Data.Models.Metadata.Display.PixClockKey] = 12345L,
                [Data.Models.Metadata.Display.RefreshKey] = 12345L,
                [Data.Models.Metadata.Display.RotateKey] = 90,
                Tag = "tag",
                DisplayType = Data.Models.Metadata.DisplayType.Vector,
                [Data.Models.Metadata.Display.VBEndKey] = 12345L,
                [Data.Models.Metadata.Display.VBStartKey] = 12345L,
                [Data.Models.Metadata.Display.VTotalKey] = 12345L,
                [Data.Models.Metadata.Display.WidthKey] = 12345L,
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
                [Data.Models.Metadata.Driver.PaletteSizeKey] = "pallettesize",
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
                [Data.Models.Metadata.Input.ButtonsKey] = 12345L,
                [Data.Models.Metadata.Input.CoinsKey] = 12345L,
                [Data.Models.Metadata.Input.ControlKey] = new Data.Models.Metadata.Control[] { CreateMetadataControl() },
                [Data.Models.Metadata.Input.PlayersKey] = 12345L,
                Service = true,
                Tilt = true,
            };
        }

        private static Data.Models.Metadata.Instance CreateMetadataInstance()
        {
            return new Data.Models.Metadata.Instance
            {
                [Data.Models.Metadata.Instance.BriefNameKey] = "briefname",
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Media CreateMetadataMedia()
        {
            return new Data.Models.Metadata.Media
            {
                [Data.Models.Metadata.Media.MD5Key] = HashType.MD5.ZeroString,
                Name = "name",
                [Data.Models.Metadata.Media.SHA1Key] = HashType.SHA1.ZeroString,
                [Data.Models.Metadata.Media.SHA256Key] = HashType.SHA256.ZeroString,
                [Data.Models.Metadata.Media.SpamSumKey] = HashType.SpamSum.ZeroString,
            };
        }

        private static Data.Models.Metadata.Original CreateMetadataOriginal()
        {
            return new Data.Models.Metadata.Original
            {
                [Data.Models.Metadata.Original.ContentKey] = "content",
                [Data.Models.Metadata.Original.ValueKey] = true,
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
                [Data.Models.Metadata.Part.InterfaceKey] = "interface",
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
                [Data.Models.Metadata.RamOption.ContentKey] = "content",
                Default = true,
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Release CreateMetadataRelease()
        {
            return new Data.Models.Metadata.Release
            {
                [Data.Models.Metadata.Release.DateKey] = "date",
                Default = true,
                [Data.Models.Metadata.Release.LanguageKey] = "language",
                Name = "name",
                [Data.Models.Metadata.Release.RegionKey] = "region",
            };
        }

        private static Data.Models.Metadata.ReleaseDetails CreateMetadataReleaseDetails()
        {
            return new Data.Models.Metadata.ReleaseDetails
            {
                [Data.Models.Metadata.ReleaseDetails.IdKey] = "id",
                [Data.Models.Metadata.ReleaseDetails.AppendToNumberKey] = "appendtonumber",
                [Data.Models.Metadata.ReleaseDetails.DateKey] = "date",
                [Data.Models.Metadata.ReleaseDetails.OriginalFormatKey] = "originalformat",
                [Data.Models.Metadata.ReleaseDetails.GroupKey] = "group",
                [Data.Models.Metadata.ReleaseDetails.DirNameKey] = "dirname",
                [Data.Models.Metadata.ReleaseDetails.NfoNameKey] = "nfoname",
                [Data.Models.Metadata.ReleaseDetails.NfoSizeKey] = "nfosize",
                [Data.Models.Metadata.ReleaseDetails.NfoCRCKey] = "nfocrc",
                [Data.Models.Metadata.ReleaseDetails.ArchiveNameKey] = "archivename",
                [Data.Models.Metadata.ReleaseDetails.RomInfoKey] = "rominfo",
                [Data.Models.Metadata.ReleaseDetails.CategoryKey] = "category",
                [Data.Models.Metadata.ReleaseDetails.CommentKey] = "comment",
                [Data.Models.Metadata.ReleaseDetails.ToolKey] = "tool",
                [Data.Models.Metadata.ReleaseDetails.RegionKey] = "region",
                [Data.Models.Metadata.ReleaseDetails.OriginKey] = "origin",
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
                [Data.Models.Metadata.Rom.FileCountKey] = 12345L,
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
                [Data.Models.Metadata.Rom.SizeKey] = 12345L,
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
                [Data.Models.Metadata.Serials.MediaSerial1Key] = "mediaserial1",
                [Data.Models.Metadata.Serials.MediaSerial2Key] = "mediaserial2",
                [Data.Models.Metadata.Serials.MediaSerial3Key] = "mediaserial3",
                [Data.Models.Metadata.Serials.PCBSerialKey] = "pcbserial",
                [Data.Models.Metadata.Serials.RomChipSerial1Key] = "romchipserial1",
                [Data.Models.Metadata.Serials.RomChipSerial2Key] = "romchipserial2",
                [Data.Models.Metadata.Serials.LockoutSerialKey] = "lockoutserial",
                [Data.Models.Metadata.Serials.SaveChipSerialKey] = "savechipserial",
                [Data.Models.Metadata.Serials.ChipSerialKey] = "chipserial",
                [Data.Models.Metadata.Serials.BoxSerialKey] = "boxserial",
                [Data.Models.Metadata.Serials.MediaStampKey] = "mediastamp",
                [Data.Models.Metadata.Serials.BoxBarcodeKey] = "boxbarcode",
                [Data.Models.Metadata.Serials.DigitalSerial1Key] = "digitalserial1",
                [Data.Models.Metadata.Serials.DigitalSerial2Key] = "digitalserial2",
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
                [Data.Models.Metadata.SlotOption.DevNameKey] = "devname",
                Name = "name",
            };
        }

        private static Data.Models.Metadata.Software CreateMetadataSoftware()
        {
            return new Data.Models.Metadata.Software
            {
                [Data.Models.Metadata.Software.CloneOfKey] = "cloneof",
                Description = "description",
                [Data.Models.Metadata.Software.InfoKey] = new Data.Models.Metadata.Info[] { CreateMetadataInfo() },
                Name = "name",
                [Data.Models.Metadata.Software.NotesKey] = "notes",
                [Data.Models.Metadata.Software.PartKey] = new Data.Models.Metadata.Part[] { CreateMetadataPart() },
                [Data.Models.Metadata.Software.PublisherKey] = "publisher",
                [Data.Models.Metadata.Software.SharedFeatKey] = new Data.Models.Metadata.SharedFeat[] { CreateMetadataSharedFeat() },
                Supported = Data.Models.Metadata.Supported.Yes,
                [Data.Models.Metadata.Software.YearKey] = "year",
            };
        }

        private static Data.Models.Metadata.SoftwareList CreateMetadataSoftwareList()
        {
            return new Data.Models.Metadata.SoftwareList
            {
                Description = "description",
                [Data.Models.Metadata.SoftwareList.FilterKey] = "filter",
                Name = "name",
                [Data.Models.Metadata.SoftwareList.NotesKey] = "notes",
                [Data.Models.Metadata.SoftwareList.SoftwareKey] = new Data.Models.Metadata.Software[] { CreateMetadataSoftware() },
                Status = Data.Models.Metadata.SoftwareListStatus.Original,
                Tag = "tag",
            };
        }

        private static Data.Models.Metadata.Sound CreateMetadataSound()
        {
            return new Data.Models.Metadata.Sound
            {
                [Data.Models.Metadata.Sound.ChannelsKey] = 12345L,
            };
        }

        private static Data.Models.Metadata.SourceDetails CreateMetadataSourceDetails()
        {
            return new Data.Models.Metadata.SourceDetails
            {
                [Data.Models.Metadata.SourceDetails.IdKey] = "id",
                [Data.Models.Metadata.SourceDetails.AppendToNumberKey] = "appendtonumber",
                [Data.Models.Metadata.SourceDetails.SectionKey] = "section",
                [Data.Models.Metadata.SourceDetails.RomInfoKey] = "rominfo",
                [Data.Models.Metadata.SourceDetails.DumpDateKey] = "dumpdate",
                [Data.Models.Metadata.SourceDetails.DumpDateInfoKey] = "dumpdateinfo",
                [Data.Models.Metadata.SourceDetails.ReleaseDateKey] = "releasedate",
                [Data.Models.Metadata.SourceDetails.ReleaseDateInfoKey] = "releasedateinfo",
                [Data.Models.Metadata.SourceDetails.DumperKey] = "dumper",
                [Data.Models.Metadata.SourceDetails.ProjectKey] = "project",
                [Data.Models.Metadata.SourceDetails.OriginalFormatKey] = "originalformat",
                [Data.Models.Metadata.SourceDetails.NodumpKey] = "nodump",
                [Data.Models.Metadata.SourceDetails.ToolKey] = "tool",
                [Data.Models.Metadata.SourceDetails.OriginKey] = "origin",
                [Data.Models.Metadata.SourceDetails.Comment1Key] = "comment1",
                [Data.Models.Metadata.SourceDetails.Comment2Key] = "comment2",
                [Data.Models.Metadata.SourceDetails.Link1Key] = "link1",
                [Data.Models.Metadata.SourceDetails.Link1PublicKey] = "link1public",
                [Data.Models.Metadata.SourceDetails.Link2Key] = "link2",
                [Data.Models.Metadata.SourceDetails.Link2PublicKey] = "link2public",
                [Data.Models.Metadata.SourceDetails.Link3Key] = "link3",
                [Data.Models.Metadata.SourceDetails.Link3PublicKey] = "link3public",
                [Data.Models.Metadata.SourceDetails.RegionKey] = "region",
                [Data.Models.Metadata.SourceDetails.MediaTitleKey] = "mediatitle",
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
                [Data.Models.Metadata.Video.AspectXKey] = 12345L,
                [Data.Models.Metadata.Video.AspectYKey] = 12345L,
                [Data.Models.Metadata.Video.HeightKey] = 12345L,
                [Data.Models.Metadata.Video.OrientationKey] = "vertical",
                [Data.Models.Metadata.Video.RefreshKey] = 12345L,
                Screen = Data.Models.Metadata.DisplayType.Vector,
                [Data.Models.Metadata.Video.WidthKey] = 12345L,
            };
        }

        #endregion

        #region Validation Helpers

        private static void ValidateHeader(DatHeader datHeader)
        {
            Assert.Equal("author", datHeader.ReadString(Data.Models.Metadata.Header.AuthorKey));
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, datHeader.BiosMode);
            Assert.Equal("build", datHeader.ReadString(Data.Models.Metadata.Header.BuildKey));
            Assert.Equal("ext", datHeader.ReadString(Data.Models.Metadata.Header.CanOpenKey));
            Assert.Equal("category", datHeader.ReadString(Data.Models.Metadata.Header.CategoryKey));
            Assert.Equal("comment", datHeader.ReadString(Data.Models.Metadata.Header.CommentKey));
            Assert.Equal("date", datHeader.ReadString(Data.Models.Metadata.Header.DateKey));
            Assert.Equal("datversion", datHeader.ReadString(Data.Models.Metadata.Header.DatVersionKey));
            Assert.True(datHeader.Debug);
            Assert.Equal("description", datHeader.Description);
            Assert.Equal("email", datHeader.ReadString(Data.Models.Metadata.Header.EmailKey));
            Assert.Equal("emulatorversion", datHeader.ReadString(Data.Models.Metadata.Header.EmulatorVersionKey));
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, datHeader.ForceMerging);
            Assert.Equal(Data.Models.Metadata.NodumpFlag.Required, datHeader.ForceNodump);
            Assert.Equal(Data.Models.Metadata.PackingFlag.Zip, datHeader.ForcePacking);
            Assert.True(datHeader.ForceZipping);
            Assert.Equal("header", datHeader.ReadString(Data.Models.Metadata.Header.HeaderKey));
            Assert.Equal("homepage", datHeader.ReadString(Data.Models.Metadata.Header.HomepageKey));
            Assert.Equal("id", datHeader.ReadString(Data.Models.Metadata.Header.IdKey));
            Assert.NotNull(datHeader.ReadString(Data.Models.Metadata.Header.ImagesKey));
            Assert.Equal("imfolder", datHeader.ReadString(Data.Models.Metadata.Header.ImFolderKey));
            Assert.NotNull(datHeader.ReadString(Data.Models.Metadata.Header.InfosKey));
            Assert.True(datHeader.LockBiosMode);
            Assert.True(datHeader.LockRomMode);
            Assert.True(datHeader.LockSampleMode);
            Assert.Equal("mameconfig", datHeader.ReadString(Data.Models.Metadata.Header.MameConfigKey));
            Assert.Equal("name", datHeader.Name);
            Assert.NotNull(datHeader.ReadString(Data.Models.Metadata.Header.NewDatKey));
            Assert.Equal("notes", datHeader.ReadString(Data.Models.Metadata.Header.NotesKey));
            Assert.Equal("plugin", datHeader.ReadString(Data.Models.Metadata.Header.PluginKey));
            Assert.Equal("refname", datHeader.ReadString(Data.Models.Metadata.Header.RefNameKey));
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, datHeader.RomMode);
            Assert.Equal("romtitle", datHeader.ReadString(Data.Models.Metadata.Header.RomTitleKey));
            Assert.Equal("rootdir", datHeader.ReadString(Data.Models.Metadata.Header.RootDirKey));
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, datHeader.SampleMode);
            Assert.Equal("schemalocation", datHeader.ReadString(Data.Models.Metadata.Header.SchemaLocationKey));
            Assert.Equal("screenshotsheight", datHeader.ReadString(Data.Models.Metadata.Header.ScreenshotsHeightKey));
            Assert.Equal("screenshotsWidth", datHeader.ReadString(Data.Models.Metadata.Header.ScreenshotsWidthKey));
            Assert.NotNull(datHeader.ReadString(Data.Models.Metadata.Header.SearchKey));
            Assert.Equal("system", datHeader.ReadString(Data.Models.Metadata.Header.SystemKey));
            Assert.Equal("timestamp", datHeader.ReadString(Data.Models.Metadata.Header.TimestampKey));
            Assert.Equal("type", datHeader.ReadString(Data.Models.Metadata.Header.TypeKey));
            Assert.Equal("url", datHeader.ReadString(Data.Models.Metadata.Header.UrlKey));
            Assert.Equal("version", datHeader.ReadString(Data.Models.Metadata.Header.VersionKey));
        }

#pragma warning disable IDE0051
        private static void ValidateMachine(DatItems.Machine machine)
        {
            Assert.Equal("board", machine.ReadString(Data.Models.Metadata.Machine.BoardKey));
            Assert.Equal("buttons", machine.ReadString(Data.Models.Metadata.Machine.ButtonsKey));
            Assert.Equal("category", machine.ReadString(Data.Models.Metadata.Machine.CategoryKey));
            Assert.Equal("cloneof", machine.ReadString(Data.Models.Metadata.Machine.CloneOfKey));
            Assert.Equal("cloneofid", machine.ReadString(Data.Models.Metadata.Machine.CloneOfIdKey));
            Assert.Equal("comment", machine.ReadString(Data.Models.Metadata.Machine.CommentKey));
            Assert.Equal("company", machine.ReadString(Data.Models.Metadata.Machine.CompanyKey));
            Assert.Equal("control", machine.ReadString(Data.Models.Metadata.Machine.ControlKey));
            Assert.Equal("country", machine.ReadString(Data.Models.Metadata.Machine.CountryKey));
            Assert.Equal("description", machine.Description);
            Assert.Equal("dirname", machine.ReadString(Data.Models.Metadata.Machine.DirNameKey));
            Assert.Equal("displaycount", machine.ReadString(Data.Models.Metadata.Machine.DisplayCountKey));
            Assert.Equal("displaytype", machine.ReadString(Data.Models.Metadata.Machine.DisplayTypeKey));
            Assert.Equal("duplicateid", machine.ReadString(Data.Models.Metadata.Machine.DuplicateIDKey));
            Assert.Equal("emulator", machine.ReadString(Data.Models.Metadata.Machine.EmulatorKey));
            Assert.Equal("extra", machine.ReadString(Data.Models.Metadata.Machine.ExtraKey));
            Assert.Equal("favorite", machine.ReadString(Data.Models.Metadata.Machine.FavoriteKey));
            Assert.Equal("genmsxid", machine.ReadString(Data.Models.Metadata.Machine.GenMSXIDKey));
            Assert.Equal("history", machine.ReadString(Data.Models.Metadata.Machine.HistoryKey));
            Assert.Equal("id", machine.ReadString(Data.Models.Metadata.Machine.IdKey));
            Assert.Equal(HashType.CRC32.ZeroString, machine.ReadString(Data.Models.Metadata.Machine.Im1CRCKey));
            Assert.Equal(HashType.CRC32.ZeroString, machine.ReadString(Data.Models.Metadata.Machine.Im2CRCKey));
            Assert.Equal("imagenumber", machine.ReadString(Data.Models.Metadata.Machine.ImageNumberKey));
            Assert.Equal(true, machine.IsBios);
            Assert.Equal(true, machine.IsDevice);
            Assert.Equal(true, machine.IsMechanical);
            Assert.Equal("language", machine.ReadString(Data.Models.Metadata.Machine.LanguageKey));
            Assert.Equal("location", machine.ReadString(Data.Models.Metadata.Machine.LocationKey));
            Assert.Equal("manufacturer", machine.ReadString(Data.Models.Metadata.Machine.ManufacturerKey));
            Assert.Equal("name", machine.Name);
            Assert.Equal("notes", machine.ReadString(Data.Models.Metadata.Machine.NotesKey));
            Assert.Equal("playedcount", machine.ReadString(Data.Models.Metadata.Machine.PlayedCountKey));
            Assert.Equal("playedtime", machine.ReadString(Data.Models.Metadata.Machine.PlayedTimeKey));
            Assert.Equal("players", machine.ReadString(Data.Models.Metadata.Machine.PlayersKey));
            Assert.Equal("publisher", machine.ReadString(Data.Models.Metadata.Machine.PublisherKey));
            Assert.Equal("rebuildto", machine.ReadString(Data.Models.Metadata.Machine.RebuildToKey));
            Assert.Equal("releasenumber", machine.ReadString(Data.Models.Metadata.Machine.ReleaseNumberKey));
            Assert.Equal("romof", machine.ReadString(Data.Models.Metadata.Machine.RomOfKey));
            Assert.Equal("rotation", machine.ReadString(Data.Models.Metadata.Machine.RotationKey));
            Assert.Equal(Data.Models.Metadata.Runnable.Yes, machine.Runnable);
            Assert.Equal("sampleof", machine.ReadString(Data.Models.Metadata.Machine.SampleOfKey));
            Assert.Equal("savetype", machine.ReadString(Data.Models.Metadata.Machine.SaveTypeKey));
            Assert.Equal("sourcefile", machine.ReadString(Data.Models.Metadata.Machine.SourceFileKey));
            Assert.Equal("sourcerom", machine.ReadString(Data.Models.Metadata.Machine.SourceRomKey));
            Assert.Equal("status", machine.ReadString(Data.Models.Metadata.Machine.StatusKey));
            Assert.Equal(Data.Models.Metadata.Supported.Yes, machine.Supported);
            Assert.Equal("system", machine.ReadString(Data.Models.Metadata.Machine.SystemKey));
            Assert.Equal("tags", machine.ReadString(Data.Models.Metadata.Machine.TagsKey));
            Assert.Equal("year", machine.ReadString(Data.Models.Metadata.Machine.YearKey));

            DatItems.Trurip? trurip = machine.Read<DatItems.Trurip>(Data.Models.Metadata.Machine.TruripKey);
            ValidateTrurip(trurip);
        }
#pragma warning restore IDE0051

        private static void ValidateAdjuster(Adjuster? adjuster)
        {
            Assert.NotNull(adjuster);
            Assert.True(adjuster.Default);
            Assert.Equal("name", adjuster.Name);

            Condition? condition = adjuster.Read<Condition>(Data.Models.Metadata.Adjuster.ConditionKey);
            ValidateCondition(condition);
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
            Assert.Equal(12345L, chip.ReadLong(Data.Models.Metadata.Chip.ClockKey));
            Assert.Equal("flags", chip.ReadString(Data.Models.Metadata.Chip.FlagsKey));
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

            Condition? condition = configuration.Read<Condition>(Data.Models.Metadata.Configuration.ConditionKey);
            ValidateCondition(condition);

            ConfLocation[]? confLocations = configuration.Read<ConfLocation[]>(Data.Models.Metadata.Configuration.ConfLocationKey);
            Assert.NotNull(confLocations);
            ConfLocation? confLocation = Assert.Single(confLocations);
            ValidateConfLocation(confLocation);

            ConfSetting[]? confSettings = configuration.Read<ConfSetting[]>(Data.Models.Metadata.Configuration.ConfSettingKey);
            Assert.NotNull(confSettings);
            ConfSetting? confSetting = Assert.Single(confSettings);
            ValidateConfSetting(confSetting);
        }

        private static void ValidateConfLocation(ConfLocation? confLocation)
        {
            Assert.NotNull(confLocation);
            Assert.True(confLocation.Inverted);
            Assert.Equal("name", confLocation.Name);
            Assert.Equal("number", confLocation.ReadString(Data.Models.Metadata.ConfLocation.NumberKey));
        }

        private static void ValidateConfSetting(ConfSetting? confSetting)
        {
            Assert.NotNull(confSetting);
            Assert.True(confSetting.Default);
            Assert.Equal("name", confSetting.Name);
            Assert.Equal("value", confSetting.Value);

            Condition? condition = confSetting.Read<Condition>(Data.Models.Metadata.ConfSetting.ConditionKey);
            ValidateCondition(condition);
        }

        private static void ValidateControl(Control? control)
        {
            Assert.NotNull(control);
            Assert.Equal(12345L, control.ReadLong(Data.Models.Metadata.Control.ButtonsKey));
            Assert.Equal(12345L, control.ReadLong(Data.Models.Metadata.Control.KeyDeltaKey));
            Assert.Equal(12345L, control.ReadLong(Data.Models.Metadata.Control.MaximumKey));
            Assert.Equal(12345L, control.ReadLong(Data.Models.Metadata.Control.MinimumKey));
            Assert.Equal(12345L, control.ReadLong(Data.Models.Metadata.Control.PlayerKey));
            Assert.Equal(12345L, control.ReadLong(Data.Models.Metadata.Control.ReqButtonsKey));
            Assert.True(control.Reverse);
            Assert.Equal(12345L, control.ReadLong(Data.Models.Metadata.Control.SensitivityKey));
            Assert.Equal(Data.Models.Metadata.ControlType.Lightgun, control.ControlType);
            Assert.Equal("ways", control.ReadString(Data.Models.Metadata.Control.WaysKey));
            Assert.Equal("ways2", control.ReadString(Data.Models.Metadata.Control.Ways2Key));
            Assert.Equal("ways3", control.ReadString(Data.Models.Metadata.Control.Ways3Key));
        }

        private static void ValidateDataArea(DataArea? dataArea)
        {
            Assert.NotNull(dataArea);
            Assert.Equal(Data.Models.Metadata.Endianness.Big, dataArea.Endianness);
            Assert.Equal("name", dataArea.Name);
            Assert.Equal(12345L, dataArea.ReadLong(Data.Models.Metadata.DataArea.SizeKey));
            Assert.Equal(64, dataArea.ReadLong(Data.Models.Metadata.DataArea.WidthKey));
        }

        private static void ValidateDevice(Device? device)
        {
            Assert.NotNull(device);
            Assert.Equal("fixedimage", device.ReadString(Data.Models.Metadata.Device.FixedImageKey));
            Assert.Equal("interface", device.ReadString(Data.Models.Metadata.Device.InterfaceKey));
            Assert.Equal(true, device.Mandatory);
            Assert.Equal("tag", device.Tag);
            Assert.Equal(Data.Models.Metadata.DeviceType.PunchTape, device.DeviceType);

            Extension[]? extensions = device.Read<Extension[]>(Data.Models.Metadata.Device.ExtensionKey);
            Assert.NotNull(extensions);
            Extension? extension = Assert.Single(extensions);
            ValidateExtension(extension);

            Instance? instance = device.Read<Instance>(Data.Models.Metadata.Device.InstanceKey);
            ValidateInstance(instance);
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
            Assert.Equal("number", dipLocation.ReadString(Data.Models.Metadata.DipLocation.NumberKey));
        }

        private static void ValidateDipSwitch(DipSwitch? dipSwitch)
        {
            Assert.NotNull(dipSwitch);
            Assert.True(dipSwitch.Default);
            Assert.Equal("mask", dipSwitch.Mask);
            Assert.Equal("name", dipSwitch.Name);
            Assert.Equal("tag", dipSwitch.Tag);

            Condition? condition = dipSwitch.Read<Condition>(Data.Models.Metadata.DipSwitch.ConditionKey);
            ValidateCondition(condition);

            DipLocation[]? dipLocations = dipSwitch.Read<DipLocation[]>(Data.Models.Metadata.DipSwitch.DipLocationKey);
            Assert.NotNull(dipLocations);
            DipLocation? dipLocation = Assert.Single(dipLocations);
            ValidateDipLocation(dipLocation);

            DipValue[]? dipValues = dipSwitch.Read<DipValue[]>(Data.Models.Metadata.DipSwitch.DipValueKey);
            Assert.NotNull(dipValues);
            DipValue? dipValue = Assert.Single(dipValues);
            ValidateDipValue(dipValue);

            string[]? entries = dipSwitch.ReadStringArray(Data.Models.Metadata.DipSwitch.EntryKey);
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

            Condition? condition = dipValue.Read<Condition>(Data.Models.Metadata.DipValue.ConditionKey);
            ValidateCondition(condition);
        }

        private static void ValidateDisk(Disk? disk)
        {
            Assert.NotNull(disk);
            Assert.Equal("flags", disk.ReadString(Data.Models.Metadata.Disk.FlagsKey));
            Assert.Equal("index", disk.ReadString(Data.Models.Metadata.Disk.IndexKey));
            Assert.Equal(HashType.MD5.ZeroString, disk.ReadString(Data.Models.Metadata.Disk.MD5Key));
            Assert.Equal("merge", disk.ReadString(Data.Models.Metadata.Disk.MergeKey));
            Assert.Equal("name", disk.Name);
            Assert.True(disk.Optional);
            Assert.Equal("region", disk.ReadString(Data.Models.Metadata.Disk.RegionKey));
            Assert.Equal(HashType.SHA1.ZeroString, disk.ReadString(Data.Models.Metadata.Disk.SHA1Key));
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
            Assert.True(display.FlipX);
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.HBEndKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.HBStartKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.HeightKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.HTotalKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.PixClockKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.RefreshKey));
            Assert.Equal(90, display.ReadLong(Data.Models.Metadata.Display.RotateKey));
            Assert.Equal("tag", display.Tag);
            Assert.Equal(Data.Models.Metadata.DisplayType.Vector, display.DisplayType);
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.VBEndKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.VBStartKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.VTotalKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.WidthKey));
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
            Assert.Equal("pallettesize", driver.ReadString(Data.Models.Metadata.Driver.PaletteSizeKey));
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
            Assert.Equal(12345L, input.ReadLong(Data.Models.Metadata.Input.ButtonsKey));
            Assert.Equal(12345L, input.ReadLong(Data.Models.Metadata.Input.CoinsKey));
            Assert.Equal(12345L, input.ReadLong(Data.Models.Metadata.Input.PlayersKey));
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
            Assert.Equal("briefname", instance.ReadString(Data.Models.Metadata.Instance.BriefNameKey));
            Assert.Equal("name", instance.Name);
        }

        private static void ValidateMedia(Media? media)
        {
            Assert.NotNull(media);
            Assert.Equal(HashType.MD5.ZeroString, media.ReadString(Data.Models.Metadata.Media.MD5Key));
            Assert.Equal("name", media.Name);
            Assert.Equal(HashType.SHA1.ZeroString, media.ReadString(Data.Models.Metadata.Media.SHA1Key));
            Assert.Equal(HashType.SHA256.ZeroString, media.ReadString(Data.Models.Metadata.Media.SHA256Key));
            Assert.Equal(HashType.SpamSum.ZeroString, media.ReadString(Data.Models.Metadata.Media.SpamSumKey));
        }

        private static void ValidatePart(Part? part)
        {
            Assert.NotNull(part);
            Assert.Equal("interface", part.ReadString(Data.Models.Metadata.Part.InterfaceKey));
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
            Assert.Equal("content", ramOption.ReadString(Data.Models.Metadata.RamOption.ContentKey));
            Assert.True(ramOption.Default);
            Assert.Equal("name", ramOption.Name);
        }

        private static void ValidateRelease(Release? release)
        {
            Assert.NotNull(release);
            Assert.Equal("date", release.ReadString(Data.Models.Metadata.Release.DateKey));
            Assert.True(release.Default);
            Assert.Equal("language", release.ReadString(Data.Models.Metadata.Release.LanguageKey));
            Assert.Equal("name", release.Name);
            Assert.Equal("region", release.ReadString(Data.Models.Metadata.Release.RegionKey));
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
            Assert.Equal(12345L, rom.ReadLong(Data.Models.Metadata.Rom.FileCountKey));
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
            Assert.Equal(12345L, rom.ReadLong(Data.Models.Metadata.Rom.SizeKey));
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
            Assert.Equal("devname", slotOption.ReadString(Data.Models.Metadata.SlotOption.DevNameKey));
            Assert.Equal("name", slotOption.Name);
        }

        private static void ValidateSoftwareList(SoftwareList? softwareList)
        {
            Assert.NotNull(softwareList);
            Assert.Equal("description", softwareList.Description);
            Assert.Equal("filter", softwareList.ReadString(Data.Models.Metadata.SoftwareList.FilterKey));
            Assert.Equal("name", softwareList.Name);
            Assert.Equal("notes", softwareList.ReadString(Data.Models.Metadata.SoftwareList.NotesKey));
            // TODO: Figure out why Data.Models.Metadata.SoftwareList.SoftwareKey doesn't get processed
            Assert.Equal(Data.Models.Metadata.SoftwareListStatus.Original, softwareList.Status);
            Assert.Equal("tag", softwareList.Tag);
        }

        private static void ValidateSound(Sound? sound)
        {
            Assert.NotNull(sound);
            Assert.Equal(12345L, sound.ReadLong(Data.Models.Metadata.Sound.ChannelsKey));
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
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Video.AspectXKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Video.AspectYKey));
            Assert.Equal(Data.Models.Metadata.DisplayType.Vector, display.DisplayType);
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.HeightKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.RefreshKey));
            Assert.Equal(12345L, display.ReadLong(Data.Models.Metadata.Display.WidthKey));
            Assert.Equal(90, display.ReadLong(Data.Models.Metadata.Display.RotateKey));
        }

        #endregion
    }
}
