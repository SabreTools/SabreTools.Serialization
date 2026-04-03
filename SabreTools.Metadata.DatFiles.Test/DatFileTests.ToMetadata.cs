using System;
using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using Xunit;

namespace SabreTools.Metadata.DatFiles.Test
{
    public partial class DatFileTests
    {
        #region ConvertToMetadata

        [Fact]
        public void ConvertToMetadata_Empty()
        {
            DatFile datFile = new Formats.Logiqx(null, useGame: false);

            Data.Models.Metadata.MetadataFile? actual = datFile.ConvertToMetadata();
            Assert.Null(actual);
        }

        [Fact]
        public void ConvertToMetadata_FilledHeader()
        {
            DatHeader header = CreateHeader();

            DatFile datFile = new Formats.Logiqx(null, useGame: false);
            datFile.SetHeader(header);
            datFile.AddItem(new Rom(), statsOnly: false);

            Data.Models.Metadata.MetadataFile? actual = datFile.ConvertToMetadata();
            Assert.NotNull(actual);

            Data.Models.Metadata.Header? actualHeader = actual.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            ValidateMetadataHeader(actualHeader);
        }

        [Fact]
        public void ConvertToMetadata_FilledMachine()
        {
            Machine machine = CreateMachine();

            List<DatItem> datItems =
            [
                CreateAdjuster(machine),
                CreateArchive(machine),
                CreateBiosSet(machine),
                CreateChip(machine),
                CreateConfiguration(machine),
                CreateDevice(machine),
                CreateDeviceRef(machine),
                CreateDipSwitch(machine),
                CreateDipSwitchWithPart(machine),
                CreateDisk(machine),
                CreateDiskWithDiskAreaPart(machine),
                CreateDisplay(machine),
                CreateDriver(machine),
                CreateFeature(machine),
                CreateInfo(machine),
                CreateInput(machine),
                CreateMedia(machine),
                CreatePartFeature(machine),
                CreatePort(machine),
                CreateRamOption(machine),
                CreateRelease(machine),
                CreateRom(machine),
                CreateRomWithDiskAreaPart(machine),
                CreateSample(machine),
                CreateSharedFeat(machine),
                CreateSlot(machine),
                CreateSoftwareList(machine),
                CreateSound(machine),
                CreateVideo(machine),
            ];

            DatFile datFile = new Formats.SabreJSON(null);
            datItems.ForEach(item => datFile.AddItem(item, statsOnly: false));

            Data.Models.Metadata.MetadataFile? actual = datFile.ConvertToMetadata();
            Assert.NotNull(actual);

            Data.Models.Metadata.Machine[]? machines = actual.ReadArray<Data.Models.Metadata.Machine>(Data.Models.Metadata.MetadataFile.MachineKey);
            Assert.NotNull(machines);
            Data.Models.Metadata.Machine actualMachine = Assert.Single(machines);
            ValidateMetadataMachine(actualMachine);
        }

        #endregion

        #region Creation Helpers

        private static DatHeader CreateHeader()
        {
            DatHeader item = new DatHeader(CreateMetadataHeader());

            item.Write<string[]>(Data.Models.Metadata.Header.CanOpenKey, ["ext"]);
            item.Write(Data.Models.Metadata.Header.ImagesKey,
                new Data.Models.OfflineList.Images() { Height = "height" });
            item.Write(Data.Models.Metadata.Header.InfosKey,
                new Data.Models.OfflineList.Infos() { Comment = new Data.Models.OfflineList.Comment() });
            item.Write(Data.Models.Metadata.Header.NewDatKey,
                new Data.Models.OfflineList.NewDat() { DatUrl = new Data.Models.OfflineList.DatUrl() });
            item.Write(Data.Models.Metadata.Header.SearchKey,
                new Data.Models.OfflineList.Search() { To = [] });

            return item;
        }

        private static Machine CreateMachine()
        {
            Machine item = new Machine(CreateMetadataMachine());
            item.Write(Data.Models.Metadata.Machine.TruripKey, CreateTrurip());
            return item;
        }

        private static Adjuster CreateAdjuster(Machine machine)
        {
            Adjuster item = new Adjuster(CreateMetadataAdjuster());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Archive CreateArchive(Machine machine)
        {
            Archive item = new Archive(CreateMetadataArchive());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static BiosSet CreateBiosSet(Machine machine)
        {
            BiosSet item = new BiosSet(CreateMetadataBiosSet());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Chip CreateChip(Machine machine)
        {
            Chip item = new Chip(CreateMetadataChip());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Configuration CreateConfiguration(Machine machine)
        {
            Configuration item = new Configuration(CreateMetadataConfiguration());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Device CreateDevice(Machine machine)
        {
            Device item = new Device(CreateMetadataDevice());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static DataArea CreateDataArea(Machine machine)
        {
            DataArea item = new DataArea(CreateMetadataDataArea());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static DeviceRef CreateDeviceRef(Machine machine)
        {
            DeviceRef item = new DeviceRef(CreateMetadataDeviceRef());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static DipSwitch CreateDipSwitch(Machine machine)
        {
            DipSwitch item = new DipSwitch(CreateMetadataDipSwitch());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static DipSwitch CreateDipSwitchWithPart(Machine machine)
        {
            DipSwitch item = new DipSwitch(CreateMetadataDipSwitch());
            item.CopyMachineInformation(machine);
            item.Write(DipSwitch.PartKey, CreatePart(machine));
            return item;
        }

        private static Disk CreateDisk(Machine machine)
        {
            Disk item = new Disk(CreateMetadataDisk());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Disk CreateDiskWithDiskAreaPart(Machine machine)
        {
            Disk item = new Disk(CreateMetadataDisk());
            item.CopyMachineInformation(machine);
            item.Write(Disk.DiskAreaKey, CreateDiskArea(machine));
            item.Write(Disk.PartKey, CreatePart(machine));
            return item;
        }

        private static DiskArea CreateDiskArea(Machine machine)
        {
            DiskArea item = new DiskArea(CreateMetadataDiskArea());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Display CreateDisplay(Machine machine)
        {
            Display item = new Display(CreateMetadataDisplay());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Driver CreateDriver(Machine machine)
        {
            Driver item = new Driver(CreateMetadataDriver());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Feature CreateFeature(Machine machine)
        {
            Feature item = new Feature(CreateMetadataFeature());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Info CreateInfo(Machine machine)
        {
            Info item = new Info(CreateMetadataInfo());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Input CreateInput(Machine machine)
        {
            Input item = new Input(CreateMetadataInput());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Media CreateMedia(Machine machine)
        {
            Media item = new Media(CreateMetadataMedia());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Part CreatePart(Machine machine)
        {
            Part item = new Part(CreateMetadataPart());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static PartFeature CreatePartFeature(Machine machine)
        {
            PartFeature item = new PartFeature(CreateMetadataFeature());
            item.CopyMachineInformation(machine);
            item.Write(PartFeature.PartKey, CreatePart(machine));
            return item;
        }

        private static Port CreatePort(Machine machine)
        {
            Port item = new Port(CreateMetadataPort());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static RamOption CreateRamOption(Machine machine)
        {
            RamOption item = new RamOption(CreateMetadataRamOption());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Release CreateRelease(Machine machine)
        {
            Release item = new Release(CreateMetadataRelease());
            item.CopyMachineInformation(machine);
            return item;
        }

        // TODO: Create variant that results in a Dump
        private static Rom CreateRom(Machine machine)
        {
            Rom item = new Rom(CreateMetadataRom());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Rom CreateRomWithDiskAreaPart(Machine machine)
        {
            Rom item = new Rom(CreateMetadataRom());
            item.CopyMachineInformation(machine);
            item.Write(Rom.DataAreaKey, CreateDataArea(machine));
            item.Write(Rom.PartKey, CreatePart(machine));
            return item;
        }

        private static Sample CreateSample(Machine machine)
        {
            Sample item = new Sample(CreateMetadataSample());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static SharedFeat CreateSharedFeat(Machine machine)
        {
            SharedFeat item = new SharedFeat(CreateMetadataSharedFeat());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Slot CreateSlot(Machine machine)
        {
            Slot item = new Slot(CreateMetadataSlot());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static SoftwareList CreateSoftwareList(Machine machine)
        {
            SoftwareList item = new SoftwareList(CreateMetadataSoftwareList());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Sound CreateSound(Machine machine)
        {
            Sound item = new Sound(CreateMetadataSound());
            item.CopyMachineInformation(machine);
            return item;
        }

        private static Trurip CreateTrurip()
        {
            return new Trurip
            {
                TitleID = "titleid",
                // Publisher = "publisher",
                Developer = "developer",
                // Year = "year",
                Genre = "genre",
                Subgenre = "subgenre",
                Ratings = "ratings",
                Score = "score",
                // Players = "players",
                Enabled = "enabled",
                Crc = true,
                // Source = "source",
                // CloneOf = "cloneof",
                RelatedTo = "relatedto",
            };
        }

        private static Display CreateVideo(Machine machine)
        {
            Display item = new Display(CreateMetadataVideo());
            item.CopyMachineInformation(machine);
            return item;
        }

        #endregion

        #region Validation Helpers

        private static void ValidateMetadataHeader(Data.Models.Metadata.Header? header)
        {
            Assert.NotNull(header);
            Assert.Equal("author", header.Author);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, header.BiosMode);
            Assert.Equal("build", header.Build);
            Assert.NotNull(header.Read<Data.Models.OfflineList.CanOpen>(Data.Models.Metadata.Header.CanOpenKey));
            Assert.Equal("category", header.Category);
            Assert.Equal("comment", header.Comment);
            Assert.Equal("date", header.Date);
            Assert.Equal("datversion", header.DatVersion);
            Assert.True(header.Debug);
            Assert.Equal("description", header.Description);
            Assert.Equal("email", header.Email);
            Assert.Equal("emulatorversion", header.EmulatorVersion);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, header.ForceMerging);
            Assert.Equal(Data.Models.Metadata.NodumpFlag.Required, header.ForceNodump);
            Assert.Equal(Data.Models.Metadata.PackingFlag.Zip, header.ForcePacking);
            Assert.True(header.ForceZipping);
            Assert.Equal("header", header.ReadString(Data.Models.Metadata.Header.HeaderKey));
            Assert.Equal("homepage", header.Homepage);
            Assert.Equal("id", header.Id);
            Assert.NotNull(header.Read<Data.Models.OfflineList.Images>(Data.Models.Metadata.Header.ImagesKey));
            Assert.Equal("imfolder", header.ReadString(Data.Models.Metadata.Header.ImFolderKey));
            Assert.NotNull(header.Read<Data.Models.OfflineList.Infos>(Data.Models.Metadata.Header.InfosKey));
            Assert.True(header.LockBiosMode);
            Assert.True(header.LockRomMode);
            Assert.True(header.LockSampleMode);
            Assert.Equal("mameconfig", header.MameConfig);
            Assert.Equal("name", header.Name);
            Assert.NotNull(header.Read<Data.Models.OfflineList.NewDat>(Data.Models.Metadata.Header.NewDatKey));
            Assert.Equal("notes", header.Notes);
            Assert.Equal("plugin", header.Plugin);
            Assert.Equal("refname", header.RefName);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, header.RomMode);
            Assert.Equal("romtitle", header.RomTitle);
            Assert.Equal("rootdir", header.RootDir);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, header.SampleMode);
            Assert.Equal("schemalocation", header.ReadString(Data.Models.Metadata.Header.SchemaLocationKey));
            Assert.Equal("screenshotsheight", header.ReadString(Data.Models.Metadata.Header.ScreenshotsHeightKey));
            Assert.Equal("screenshotsWidth", header.ReadString(Data.Models.Metadata.Header.ScreenshotsWidthKey));
            Assert.NotNull(header.Read<Data.Models.OfflineList.Search>(Data.Models.Metadata.Header.SearchKey));
            Assert.Equal("system", header.System);
            Assert.Equal("timestamp", header.Timestamp);
            Assert.Equal("type", header.Type);
            Assert.Equal("url", header.Url);
            Assert.Equal("version", header.Version);
        }

        private static void ValidateMetadataMachine(Data.Models.Metadata.Machine machine)
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

            Data.Models.Metadata.Adjuster[]? adjusters = machine.ReadArray<Data.Models.Metadata.Adjuster>(Data.Models.Metadata.Machine.AdjusterKey);
            Assert.NotNull(adjusters);
            Data.Models.Metadata.Adjuster adjuster = Assert.Single(adjusters);
            ValidateMetadataAdjuster(adjuster);

            Data.Models.Metadata.Archive[]? archives = machine.ReadArray<Data.Models.Metadata.Archive>(Data.Models.Metadata.Machine.ArchiveKey);
            Assert.NotNull(archives);
            Data.Models.Metadata.Archive archive = Assert.Single(archives);
            ValidateMetadataArchive(archive);

            Data.Models.Metadata.BiosSet[]? biosSets = machine.ReadArray<Data.Models.Metadata.BiosSet>(Data.Models.Metadata.Machine.BiosSetKey);
            Assert.NotNull(biosSets);
            Data.Models.Metadata.BiosSet biosSet = Assert.Single(biosSets);
            ValidateMetadataBiosSet(biosSet);

            Data.Models.Metadata.Chip[]? chips = machine.ReadArray<Data.Models.Metadata.Chip>(Data.Models.Metadata.Machine.ChipKey);
            Assert.NotNull(chips);
            Data.Models.Metadata.Chip chip = Assert.Single(chips);
            ValidateMetadataChip(chip);

            Data.Models.Metadata.Configuration[]? configurations = machine.ReadArray<Data.Models.Metadata.Configuration>(Data.Models.Metadata.Machine.ConfigurationKey);
            Assert.NotNull(configurations);
            Data.Models.Metadata.Configuration configuration = Assert.Single(configurations);
            ValidateMetadataConfiguration(configuration);

            Data.Models.Metadata.Device[]? devices = machine.ReadArray<Data.Models.Metadata.Device>(Data.Models.Metadata.Machine.DeviceKey);
            Assert.NotNull(devices);
            Data.Models.Metadata.Device device = Assert.Single(devices);
            ValidateMetadataDevice(device);

            Data.Models.Metadata.DeviceRef[]? deviceRefs = machine.ReadArray<Data.Models.Metadata.DeviceRef>(Data.Models.Metadata.Machine.DeviceRefKey);
            Assert.NotNull(deviceRefs);
            Data.Models.Metadata.DeviceRef deviceRef = Assert.Single(deviceRefs);
            ValidateMetadataDeviceRef(deviceRef);

            Data.Models.Metadata.DipSwitch[]? dipSwitches = machine.ReadArray<Data.Models.Metadata.DipSwitch>(Data.Models.Metadata.Machine.DipSwitchKey);
            Assert.NotNull(dipSwitches);
            Assert.Equal(2, dipSwitches.Length);
            Data.Models.Metadata.DipSwitch dipSwitch = dipSwitches[0];
            ValidateMetadataDipSwitch(dipSwitch);

            Data.Models.Metadata.Disk[]? disks = machine.ReadArray<Data.Models.Metadata.Disk>(Data.Models.Metadata.Machine.DiskKey);
            Assert.NotNull(disks);
            Assert.Equal(2, disks.Length);
            Data.Models.Metadata.Disk disk = disks[0];
            ValidateMetadataDisk(disk);

            Data.Models.Metadata.Display[]? displays = machine.ReadArray<Data.Models.Metadata.Display>(Data.Models.Metadata.Machine.DisplayKey);
            Assert.NotNull(displays);
            Assert.Equal(2, displays.Length);
            Data.Models.Metadata.Display? display = Array.Find(displays, d => d.AspectX == null);
            ValidateMetadataDisplay(display);

            Data.Models.Metadata.Driver[]? drivers = machine.ReadArray<Data.Models.Metadata.Driver>(Data.Models.Metadata.Machine.DriverKey);
            Assert.NotNull(drivers);
            Data.Models.Metadata.Driver driver = Assert.Single(drivers);
            ValidateMetadataDriver(driver);

            // TODO: Implement this validation
            // Data.Models.Metadata.Dump[]? dumps = machine.ReadArray<Data.Models.Metadata.Dump>(Data.Models.Metadata.Machine.DumpKey);
            // Assert.NotNull(dumps);
            // Data.Models.Metadata.Dump dump = Assert.Single(dumps);
            // ValidateMetadataDump(dump);

            Data.Models.Metadata.Feature[]? features = machine.ReadArray<Data.Models.Metadata.Feature>(Data.Models.Metadata.Machine.FeatureKey);
            Assert.NotNull(features);
            Assert.Equal(2, features.Length);
            Data.Models.Metadata.Feature feature = features[0];
            ValidateMetadataFeature(feature);

            Data.Models.Metadata.Info[]? infos = machine.ReadArray<Data.Models.Metadata.Info>(Data.Models.Metadata.Machine.InfoKey);
            Assert.NotNull(infos);
            Data.Models.Metadata.Info info = Assert.Single(infos);
            ValidateMetadataInfo(info);

            Data.Models.Metadata.Input[]? inputs = machine.ReadArray<Data.Models.Metadata.Input>(Data.Models.Metadata.Machine.InputKey);
            Assert.NotNull(inputs);
            Data.Models.Metadata.Input input = Assert.Single(inputs);
            ValidateMetadataInput(input);

            Data.Models.Metadata.Media[]? media = machine.ReadArray<Data.Models.Metadata.Media>(Data.Models.Metadata.Machine.MediaKey);
            Assert.NotNull(media);
            Data.Models.Metadata.Media medium = Assert.Single(media);
            ValidateMetadataMedia(medium);

            Data.Models.Metadata.Part[]? parts = machine.ReadArray<Data.Models.Metadata.Part>(Data.Models.Metadata.Machine.PartKey);
            Assert.NotNull(parts);
            Data.Models.Metadata.Part part = Assert.Single(parts);
            ValidateMetadataPart(part);

            Data.Models.Metadata.Port[]? ports = machine.ReadArray<Data.Models.Metadata.Port>(Data.Models.Metadata.Machine.PortKey);
            Assert.NotNull(ports);
            Data.Models.Metadata.Port port = Assert.Single(ports);
            ValidateMetadataPort(port);

            Data.Models.Metadata.RamOption[]? ramOptions = machine.ReadArray<Data.Models.Metadata.RamOption>(Data.Models.Metadata.Machine.RamOptionKey);
            Assert.NotNull(ramOptions);
            Data.Models.Metadata.RamOption ramOption = Assert.Single(ramOptions);
            ValidateMetadataRamOption(ramOption);

            Data.Models.Metadata.Release[]? releases = machine.ReadArray<Data.Models.Metadata.Release>(Data.Models.Metadata.Machine.ReleaseKey);
            Assert.NotNull(releases);
            Data.Models.Metadata.Release release = Assert.Single(releases);
            ValidateMetadataRelease(release);

            Data.Models.Metadata.Rom[]? roms = machine.ReadArray<Data.Models.Metadata.Rom>(Data.Models.Metadata.Machine.RomKey);
            Assert.NotNull(roms);
            Assert.Equal(2, roms.Length);
            Data.Models.Metadata.Rom rom = roms[0];
            ValidateMetadataRom(rom);

            Data.Models.Metadata.Sample[]? samples = machine.ReadArray<Data.Models.Metadata.Sample>(Data.Models.Metadata.Machine.SampleKey);
            Assert.NotNull(samples);
            Data.Models.Metadata.Sample sample = Assert.Single(samples);
            ValidateMetadataSample(sample);

            Data.Models.Metadata.SharedFeat[]? sharedFeats = machine.ReadArray<Data.Models.Metadata.SharedFeat>(Data.Models.Metadata.Machine.SharedFeatKey);
            Assert.NotNull(sharedFeats);
            Data.Models.Metadata.SharedFeat sharedFeat = Assert.Single(sharedFeats);
            ValidateMetadataSharedFeat(sharedFeat);

            Data.Models.Metadata.Slot[]? slots = machine.ReadArray<Data.Models.Metadata.Slot>(Data.Models.Metadata.Machine.SlotKey);
            Assert.NotNull(slots);
            Data.Models.Metadata.Slot slot = Assert.Single(slots);
            ValidateMetadataSlot(slot);

            Data.Models.Metadata.SoftwareList[]? softwareLists = machine.ReadArray<Data.Models.Metadata.SoftwareList>(Data.Models.Metadata.Machine.SoftwareListKey);
            Assert.NotNull(softwareLists);
            Data.Models.Metadata.SoftwareList softwareList = Assert.Single(softwareLists);
            ValidateMetadataSoftwareList(softwareList);

            Data.Models.Metadata.Sound[]? sounds = machine.ReadArray<Data.Models.Metadata.Sound>(Data.Models.Metadata.Machine.SoundKey);
            Assert.NotNull(sounds);
            Data.Models.Metadata.Sound sound = Assert.Single(sounds);
            ValidateMetadataSound(sound);

            Data.Models.Logiqx.Trurip? trurip = machine.Read<Data.Models.Logiqx.Trurip>(Data.Models.Metadata.Machine.TruripKey);
            ValidateMetadataTrurip(trurip);

            Data.Models.Metadata.Video[]? videos = machine.ReadArray<Data.Models.Metadata.Video>(Data.Models.Metadata.Machine.VideoKey);
            Assert.NotNull(videos);
            Data.Models.Metadata.Video video = Assert.Single(videos);
            ValidateMetadataVideo(video);
        }

        private static void ValidateMetadataAdjuster(Data.Models.Metadata.Adjuster? adjuster)
        {
            Assert.NotNull(adjuster);
            Assert.True(adjuster.Default);
            Assert.Equal("name", adjuster.Name);

            Data.Models.Metadata.Condition? condition = adjuster.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.Adjuster.ConditionKey);
            ValidateMetadataCondition(condition);
        }

        private static void ValidateMetadataAnalog(Data.Models.Metadata.Analog? analog)
        {
            Assert.NotNull(analog);
            Assert.Equal("mask", analog.Mask);
        }

        private static void ValidateMetadataArchive(Data.Models.Metadata.Archive? archive)
        {
            Assert.NotNull(archive);
            Assert.Equal("number", archive.ReadString(Data.Models.Metadata.Archive.NumberKey));
            Assert.Equal("clone", archive.ReadString(Data.Models.Metadata.Archive.CloneKey));
            Assert.Equal("regparent", archive.ReadString(Data.Models.Metadata.Archive.RegParentKey));
            Assert.Equal("mergeof", archive.ReadString(Data.Models.Metadata.Archive.MergeOfKey));
            Assert.Equal("mergename", archive.ReadString(Data.Models.Metadata.Archive.MergeNameKey));
            Assert.Equal("name", archive.Name);
            Assert.Equal("name_alt", archive.ReadString(Data.Models.Metadata.Archive.NameAltKey));
            Assert.Equal("region", archive.ReadString(Data.Models.Metadata.Archive.RegionKey));
            Assert.Equal("languages", archive.ReadString(Data.Models.Metadata.Archive.LanguagesKey));
            Assert.Equal("showlang", archive.ReadString(Data.Models.Metadata.Archive.ShowLangKey));
            Assert.Equal("langchecked", archive.ReadString(Data.Models.Metadata.Archive.LangCheckedKey));
            Assert.Equal("version1", archive.ReadString(Data.Models.Metadata.Archive.Version1Key));
            Assert.Equal("version2", archive.ReadString(Data.Models.Metadata.Archive.Version2Key));
            Assert.Equal("devstatus", archive.ReadString(Data.Models.Metadata.Archive.DevStatusKey));
            Assert.Equal("additional", archive.ReadString(Data.Models.Metadata.Archive.AdditionalKey));
            Assert.Equal("special1", archive.ReadString(Data.Models.Metadata.Archive.Special1Key));
            Assert.Equal("special2", archive.ReadString(Data.Models.Metadata.Archive.Special2Key));
            Assert.Equal("alt", archive.ReadString(Data.Models.Metadata.Archive.AltKey));
            Assert.Equal("gameid1", archive.ReadString(Data.Models.Metadata.Archive.GameId1Key));
            Assert.Equal("gameid2", archive.ReadString(Data.Models.Metadata.Archive.GameId2Key));
            Assert.Equal("description", archive.Description);
            Assert.Equal("bios", archive.ReadString(Data.Models.Metadata.Archive.BiosKey));
            Assert.Equal("licensed", archive.ReadString(Data.Models.Metadata.Archive.LicensedKey));
            Assert.Equal("pirate", archive.ReadString(Data.Models.Metadata.Archive.PirateKey));
            Assert.Equal("physical", archive.ReadString(Data.Models.Metadata.Archive.PhysicalKey));
            Assert.Equal("complete", archive.ReadString(Data.Models.Metadata.Archive.CompleteKey));
            Assert.Equal("adult", archive.ReadString(Data.Models.Metadata.Archive.AdultKey));
            Assert.Equal("dat", archive.ReadString(Data.Models.Metadata.Archive.DatKey));
            Assert.Equal("listed", archive.ReadString(Data.Models.Metadata.Archive.ListedKey));
            Assert.Equal("private", archive.ReadString(Data.Models.Metadata.Archive.PrivateKey));
            Assert.Equal("stickynote", archive.ReadString(Data.Models.Metadata.Archive.StickyNoteKey));
            Assert.Equal("datternote", archive.ReadString(Data.Models.Metadata.Archive.DatterNoteKey));
            Assert.Equal("categories", archive.ReadString(Data.Models.Metadata.Archive.CategoriesKey));
        }

        private static void ValidateMetadataBiosSet(Data.Models.Metadata.BiosSet? biosSet)
        {
            Assert.NotNull(biosSet);
            Assert.True(biosSet.Default);
            Assert.Equal("description", biosSet.Description);
            Assert.Equal("name", biosSet.Name);
        }

        private static void ValidateMetadataChip(Data.Models.Metadata.Chip? chip)
        {
            Assert.NotNull(chip);
            Assert.Equal(12345, chip.Clock);
            Assert.Equal("flags", chip.Flags);
            Assert.Equal("name", chip.Name);
            Assert.True(chip.SoundOnly);
            Assert.Equal("tag", chip.Tag);
            Assert.Equal(Data.Models.Metadata.ChipType.CPU, chip.ChipType);
        }

        private static void ValidateMetadataCondition(Data.Models.Metadata.Condition? condition)
        {
            Assert.NotNull(condition);
            Assert.Equal("value", condition.Value);
            Assert.Equal("mask", condition.Mask);
            Assert.Equal(Data.Models.Metadata.Relation.Equal, condition.Relation);
            Assert.Equal("tag", condition.Tag);
        }

        private static void ValidateMetadataConfiguration(Data.Models.Metadata.Configuration? configuration)
        {
            Assert.NotNull(configuration);
            Assert.Equal("mask", configuration.Mask);
            Assert.Equal("name", configuration.Name);
            Assert.Equal("tag", configuration.Tag);

            Data.Models.Metadata.Condition? condition = configuration.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.Configuration.ConditionKey);
            ValidateMetadataCondition(condition);

            Data.Models.Metadata.ConfLocation[]? confLocations = configuration.ReadArray<Data.Models.Metadata.ConfLocation>(Data.Models.Metadata.Configuration.ConfLocationKey);
            Assert.NotNull(confLocations);
            Data.Models.Metadata.ConfLocation? confLocation = Assert.Single(confLocations);
            ValidateMetadataConfLocation(confLocation);

            Data.Models.Metadata.ConfSetting[]? confSettings = configuration.ReadArray<Data.Models.Metadata.ConfSetting>(Data.Models.Metadata.Configuration.ConfSettingKey);
            Assert.NotNull(confSettings);
            Data.Models.Metadata.ConfSetting? confSetting = Assert.Single(confSettings);
            ValidateMetadataConfSetting(confSetting);
        }

        private static void ValidateMetadataConfLocation(Data.Models.Metadata.ConfLocation? confLocation)
        {
            Assert.NotNull(confLocation);
            Assert.True(confLocation.Inverted);
            Assert.Equal("name", confLocation.Name);
            Assert.Equal(12345, confLocation.Number);
        }

        private static void ValidateMetadataConfSetting(Data.Models.Metadata.ConfSetting? confSetting)
        {
            Assert.NotNull(confSetting);
            Assert.True(confSetting.Default);
            Assert.Equal("name", confSetting.Name);
            Assert.Equal("value", confSetting.Value);

            Data.Models.Metadata.Condition? condition = confSetting.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.ConfSetting.ConditionKey);
            ValidateMetadataCondition(condition);
        }

        private static void ValidateMetadataControl(Data.Models.Metadata.Control? control)
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

        private static void ValidateMetadataDataArea(Data.Models.Metadata.DataArea? dataArea)
        {
            Assert.NotNull(dataArea);
            Assert.Equal(Data.Models.Metadata.Endianness.Big, dataArea.Endianness);
            Assert.Equal("name", dataArea.Name);
            Assert.Equal(12345, dataArea.Size);
            Assert.Equal(Data.Models.Metadata.Width.Long, dataArea.Width);

            Data.Models.Metadata.Rom[]? roms = dataArea.ReadArray<Data.Models.Metadata.Rom>(Data.Models.Metadata.DataArea.RomKey);
            Assert.NotNull(roms);
            Data.Models.Metadata.Rom? rom = Assert.Single(roms);
            ValidateMetadataRom(rom);
        }

        private static void ValidateMetadataDevice(Data.Models.Metadata.Device? device)
        {
            Assert.NotNull(device);
            Assert.Equal("fixedimage", device.FixedImage);
            Assert.Equal("interface", device.Interface);
            Assert.Equal(true, device.Mandatory);
            Assert.Equal("tag", device.Tag);
            Assert.Equal(Data.Models.Metadata.DeviceType.PunchTape, device.DeviceType);

            Data.Models.Metadata.Extension[]? extensions = device.ReadArray<Data.Models.Metadata.Extension>(Data.Models.Metadata.Device.ExtensionKey);
            Assert.NotNull(extensions);
            Data.Models.Metadata.Extension? extension = Assert.Single(extensions);
            ValidateMetadataExtension(extension);

            Data.Models.Metadata.Instance? instance = device.Read<Data.Models.Metadata.Instance>(Data.Models.Metadata.Device.InstanceKey);
            ValidateMetadataInstance(instance);
        }

        private static void ValidateMetadataDeviceRef(Data.Models.Metadata.DeviceRef? deviceRef)
        {
            Assert.NotNull(deviceRef);
            Assert.Equal("name", deviceRef.Name);
        }

        private static void ValidateMetadataDipLocation(Data.Models.Metadata.DipLocation? dipLocation)
        {
            Assert.NotNull(dipLocation);
            Assert.True(dipLocation.Inverted);
            Assert.Equal("name", dipLocation.Name);
            Assert.Equal(12345, dipLocation.Number);
        }

        private static void ValidateMetadataDipSwitch(Data.Models.Metadata.DipSwitch? dipSwitch)
        {
            Assert.NotNull(dipSwitch);
            Assert.True(dipSwitch.Default);
            Assert.Equal("mask", dipSwitch.Mask);
            Assert.Equal("name", dipSwitch.Name);
            Assert.Equal("tag", dipSwitch.Tag);

            Data.Models.Metadata.Condition? condition = dipSwitch.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.DipSwitch.ConditionKey);
            ValidateMetadataCondition(condition);

            Data.Models.Metadata.DipLocation[]? dipLocations = dipSwitch.ReadArray<Data.Models.Metadata.DipLocation>(Data.Models.Metadata.DipSwitch.DipLocationKey);
            Assert.NotNull(dipLocations);
            Data.Models.Metadata.DipLocation? dipLocation = Assert.Single(dipLocations);
            ValidateMetadataDipLocation(dipLocation);

            Data.Models.Metadata.DipValue[]? dipValues = dipSwitch.ReadArray<Data.Models.Metadata.DipValue>(Data.Models.Metadata.DipSwitch.DipValueKey);
            Assert.NotNull(dipValues);
            Data.Models.Metadata.DipValue? dipValue = Assert.Single(dipValues);
            ValidateMetadataDipValue(dipValue);

            string[]? entries = dipSwitch.ReadStringArray(Data.Models.Metadata.DipSwitch.EntryKey);
            Assert.NotNull(entries);
            string entry = Assert.Single(entries);
            Assert.Equal("entry", entry);
        }

        private static void ValidateMetadataDipValue(Data.Models.Metadata.DipValue? dipValue)
        {
            Assert.NotNull(dipValue);
            Assert.True(dipValue.Default);
            Assert.Equal("name", dipValue.Name);
            Assert.Equal("value", dipValue.Value);

            Data.Models.Metadata.Condition? condition = dipValue.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.DipValue.ConditionKey);
            ValidateMetadataCondition(condition);
        }

        private static void ValidateMetadataDisk(Data.Models.Metadata.Disk? disk)
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

        private static void ValidateMetadataDiskArea(Data.Models.Metadata.DiskArea? diskArea)
        {
            Assert.NotNull(diskArea);
            Assert.Equal("name", diskArea.Name);

            Data.Models.Metadata.Disk[]? disks = diskArea.ReadArray<Data.Models.Metadata.Disk>(Data.Models.Metadata.DiskArea.DiskKey);
            Assert.NotNull(disks);
            Data.Models.Metadata.Disk? disk = Assert.Single(disks);
            ValidateMetadataDisk(disk);
        }

        private static void ValidateMetadataDisplay(Data.Models.Metadata.Display? display)
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

        private static void ValidateMetadataDriver(Data.Models.Metadata.Driver? driver)
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

        private static void ValidateMetadataExtension(Data.Models.Metadata.Extension? extension)
        {
            Assert.NotNull(extension);
            Assert.Equal("name", extension.Name);
        }

        private static void ValidateMetadataFeature(Data.Models.Metadata.Feature? feature)
        {
            Assert.NotNull(feature);
            Assert.Equal("name", feature.Name);
            Assert.Equal(Data.Models.Metadata.FeatureStatus.Imperfect, feature.Overall);
            Assert.Equal(Data.Models.Metadata.FeatureStatus.Imperfect, feature.Status);
            Assert.Equal(Data.Models.Metadata.FeatureType.Protection, feature.FeatureType);
            Assert.Equal("value", feature.Value);
        }

        private static void ValidateMetadataInfo(Data.Models.Metadata.Info? info)
        {
            Assert.NotNull(info);
            Assert.Equal("name", info.Name);
            Assert.Equal("value", info.Value);
        }

        private static void ValidateMetadataInput(Data.Models.Metadata.Input? input)
        {
            Assert.NotNull(input);
            Assert.Equal(12345, input.Buttons);
            Assert.Equal(12345, input.Coins);
            Assert.Equal(12345, input.Players);
            Assert.True(input.Service);
            Assert.True(input.Tilt);

            Data.Models.Metadata.Control[]? controls = input.ReadArray<Data.Models.Metadata.Control>(Data.Models.Metadata.Input.ControlKey);
            Assert.NotNull(controls);
            Data.Models.Metadata.Control? control = Assert.Single(controls);
            ValidateMetadataControl(control);
        }

        private static void ValidateMetadataInstance(Data.Models.Metadata.Instance? instance)
        {
            Assert.NotNull(instance);
            Assert.Equal("briefname", instance.BriefName);
            Assert.Equal("name", instance.Name);
        }

        private static void ValidateMetadataMedia(Data.Models.Metadata.Media? media)
        {
            Assert.NotNull(media);
            Assert.Equal(HashType.MD5.ZeroString, media.ReadString(Data.Models.Metadata.Media.MD5Key));
            Assert.Equal("name", media.Name);
            Assert.Equal(HashType.SHA1.ZeroString, media.ReadString(Data.Models.Metadata.Media.SHA1Key));
            Assert.Equal(HashType.SHA256.ZeroString, media.ReadString(Data.Models.Metadata.Media.SHA256Key));
            Assert.Equal(HashType.SpamSum.ZeroString, media.ReadString(Data.Models.Metadata.Media.SpamSumKey));
        }

        private static void ValidateMetadataPart(Data.Models.Metadata.Part? part)
        {
            Assert.NotNull(part);
            Assert.Equal("interface", part.Interface);
            Assert.Equal("name", part.Name);

            Data.Models.Metadata.DataArea[]? dataAreas = part.ReadArray<Data.Models.Metadata.DataArea>(Data.Models.Metadata.Part.DataAreaKey);
            Assert.NotNull(dataAreas);
            Data.Models.Metadata.DataArea? dataArea = Assert.Single(dataAreas);
            ValidateMetadataDataArea(dataArea);

            Data.Models.Metadata.DiskArea[]? diskAreas = part.ReadArray<Data.Models.Metadata.DiskArea>(Data.Models.Metadata.Part.DiskAreaKey);
            Assert.NotNull(diskAreas);
            Data.Models.Metadata.DiskArea? diskArea = Assert.Single(diskAreas);
            ValidateMetadataDiskArea(diskArea);

            Data.Models.Metadata.DipSwitch[]? dipSwitches = part.ReadArray<Data.Models.Metadata.DipSwitch>(Data.Models.Metadata.Part.DipSwitchKey);
            Assert.NotNull(dipSwitches);
            Data.Models.Metadata.DipSwitch? dipSwitch = Assert.Single(dipSwitches);
            ValidateMetadataDipSwitch(dipSwitch);

            Data.Models.Metadata.Feature[]? features = part.ReadArray<Data.Models.Metadata.Feature>(Data.Models.Metadata.Part.FeatureKey);
            Assert.NotNull(features);
            Data.Models.Metadata.Feature? feature = Assert.Single(features);
            ValidateMetadataFeature(feature);
        }

        private static void ValidateMetadataPort(Data.Models.Metadata.Port? port)
        {
            Assert.NotNull(port);
            Assert.Equal("tag", port.Tag);

            Data.Models.Metadata.Analog[]? dipValues = port.ReadArray<Data.Models.Metadata.Analog>(Data.Models.Metadata.Port.AnalogKey);
            Assert.NotNull(dipValues);
            Data.Models.Metadata.Analog? dipValue = Assert.Single(dipValues);
            ValidateMetadataAnalog(dipValue);
        }

        private static void ValidateMetadataRamOption(Data.Models.Metadata.RamOption? ramOption)
        {
            Assert.NotNull(ramOption);
            Assert.Equal("content", ramOption.Content);
            Assert.True(ramOption.Default);
            Assert.Equal("name", ramOption.Name);
        }

        private static void ValidateMetadataRelease(Data.Models.Metadata.Release? release)
        {
            Assert.NotNull(release);
            Assert.Equal("date", release.Date);
            Assert.True(release.Default);
            Assert.Equal("language", release.Language);
            Assert.Equal("name", release.Name);
            Assert.Equal("region", release.Region);
        }

        private static void ValidateMetadataReleaseDetails(Data.Models.Metadata.ReleaseDetails? releaseDetails)
        {
            Assert.NotNull(releaseDetails);
            Assert.Equal("id", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.IdKey));
            Assert.Equal("appendtonumber", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.AppendToNumberKey));
            Assert.Equal("date", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.DateKey));
            Assert.Equal("originalformat", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.OriginalFormatKey));
            Assert.Equal("group", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.GroupKey));
            Assert.Equal("dirname", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.DirNameKey));
            Assert.Equal("nfoname", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.NfoNameKey));
            Assert.Equal("nfosize", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.NfoSizeKey));
            Assert.Equal("nfocrc", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.NfoCRCKey));
            Assert.Equal("archivename", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.ArchiveNameKey));
            Assert.Equal("rominfo", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.RomInfoKey));
            Assert.Equal("category", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.CategoryKey));
            Assert.Equal("comment", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.CommentKey));
            Assert.Equal("tool", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.ToolKey));
            Assert.Equal("region", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.RegionKey));
            Assert.Equal("origin", releaseDetails.ReadString(Data.Models.Metadata.ReleaseDetails.OriginKey));
        }

        private static void ValidateMetadataRom(Data.Models.Metadata.Rom? rom)
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

        private static void ValidateMetadataSample(Data.Models.Metadata.Sample? sample)
        {
            Assert.NotNull(sample);
            Assert.Equal("name", sample.Name);
        }

        private static void ValidateMetadataSerials(Data.Models.Metadata.Serials? serials)
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

        private static void ValidateMetadataSharedFeat(Data.Models.Metadata.SharedFeat? sharedFeat)
        {
            Assert.NotNull(sharedFeat);
            Assert.Equal("name", sharedFeat.Name);
            Assert.Equal("value", sharedFeat.Value);
        }

        private static void ValidateMetadataSlot(Data.Models.Metadata.Slot? slot)
        {
            Assert.NotNull(slot);
            Assert.Equal("name", slot.Name);

            Data.Models.Metadata.SlotOption[]? slotOptions = slot.ReadArray<Data.Models.Metadata.SlotOption>(Data.Models.Metadata.Slot.SlotOptionKey);
            Assert.NotNull(slotOptions);
            Data.Models.Metadata.SlotOption? slotOption = Assert.Single(slotOptions);
            ValidateMetadataSlotOption(slotOption);
        }

        private static void ValidateMetadataSlotOption(Data.Models.Metadata.SlotOption? slotOption)
        {
            Assert.NotNull(slotOption);
            Assert.True(slotOption.Default);
            Assert.Equal("devname", slotOption.DevName);
            Assert.Equal("name", slotOption.Name);
        }

        private static void ValidateMetadataSoftwareList(Data.Models.Metadata.SoftwareList? softwareList)
        {
            Assert.NotNull(softwareList);
            Assert.Equal("filter", softwareList.Filter);
            Assert.Equal("name", softwareList.Name);
            Assert.Equal(Data.Models.Metadata.SoftwareListStatus.Original, softwareList.Status);
            Assert.Equal("tag", softwareList.Tag);
        }

        private static void ValidateMetadataSound(Data.Models.Metadata.Sound? sound)
        {
            Assert.NotNull(sound);
            Assert.Equal(12345, sound.Channels);
        }

        private static void ValidateMetadataSourceDetails(Data.Models.Metadata.SourceDetails? sourceDetails)
        {
            Assert.NotNull(sourceDetails);
            Assert.Equal("appendtonumber", sourceDetails.AppendToNumber);
            Assert.Equal("comment1", sourceDetails.Comment1);
            Assert.Equal("comment2", sourceDetails.Comment2);
            Assert.Equal("dumpdate", sourceDetails.DumpDate);
            Assert.Equal(true, sourceDetails.DumpDateInfo);
            Assert.Equal("dumper", sourceDetails.Dumper);
            Assert.Equal("id", sourceDetails.Id);
            Assert.Equal("link1", sourceDetails.Link1);
            Assert.Equal(true, sourceDetails.Link1Public);
            Assert.Equal("link2", sourceDetails.Link2);
            Assert.Equal(true, sourceDetails.Link2Public);
            Assert.Equal("link3", sourceDetails.Link3);
            Assert.Equal(true, sourceDetails.Link3Public);
            Assert.Equal("mediatitle", sourceDetails.MediaTitle);
            Assert.Equal(true, sourceDetails.Nodump);
            Assert.Equal("origin", sourceDetails.Origin);
            Assert.Equal("originalformat", sourceDetails.OriginalFormat);
            Assert.Equal("project", sourceDetails.Project);
            Assert.Equal("region", sourceDetails.Region);
            Assert.Equal("releasedate", sourceDetails.ReleaseDate);
            Assert.Equal(true, sourceDetails.ReleaseDateInfo);
            Assert.Equal("rominfo", sourceDetails.RomInfo);
            Assert.Equal("section", sourceDetails.Section);
            Assert.Equal("tool", sourceDetails.Tool);
        }

        private static void ValidateMetadataTrurip(Data.Models.Logiqx.Trurip? trurip)
        {
            Assert.NotNull(trurip);
            Assert.Equal("titleid", trurip.TitleID);
            Assert.Equal("publisher", trurip.Publisher);
            Assert.Equal("developer", trurip.Developer);
            Assert.Equal("year", trurip.Year);
            Assert.Equal("genre", trurip.Genre);
            Assert.Equal("subgenre", trurip.Subgenre);
            Assert.Equal("ratings", trurip.Ratings);
            Assert.Equal("score", trurip.Score);
            Assert.Equal("players", trurip.Players);
            Assert.Equal(expected: "enabled", trurip.Enabled);
            Assert.Equal("yes", trurip.CRC);
            Assert.Equal("sourcefile", trurip.Source);
            Assert.Equal("cloneof", trurip.CloneOf);
            Assert.Equal("relatedto", trurip.RelatedTo);
        }

        private static void ValidateMetadataVideo(Data.Models.Metadata.Video? video)
        {
            Assert.NotNull(video);
            Assert.Equal(12345, video.AspectX);
            Assert.Equal(12345, video.AspectY);
            Assert.Equal(12345, video.Height);
            Assert.Equal(Data.Models.Metadata.Rotation.East, video.Orientation);
            Assert.Equal(123.45, video.Refresh);
            Assert.Equal(Data.Models.Metadata.DisplayType.Vector, video.Screen);
            Assert.Equal(12345, video.Width);
        }

        #endregion
    }
}
