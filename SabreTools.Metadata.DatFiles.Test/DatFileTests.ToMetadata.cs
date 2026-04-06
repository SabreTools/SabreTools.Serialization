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

            Data.Models.Metadata.Header? actualHeader = actual.Header;
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

            Data.Models.Metadata.Machine[]? machines = actual.Machine;
            Assert.NotNull(machines);
            Data.Models.Metadata.Machine actualMachine = Assert.Single(machines);
            ValidateMetadataMachine(actualMachine);
        }

        #endregion

        #region Creation Helpers

        private static DatHeader CreateHeader()
        {
            DatHeader item = new DatHeader(CreateMetadataHeader())
            {
                CanOpen = new Data.Models.OfflineList.CanOpen { Extension = ["ext"] },
                Images = new Data.Models.OfflineList.Images() { Height = "height" },
                Infos = new Data.Models.OfflineList.Infos() { Comment = new Data.Models.OfflineList.Comment() },
                NewDat = new Data.Models.OfflineList.NewDat() { DatUrl = new Data.Models.OfflineList.DatUrl() },
                Search = new Data.Models.OfflineList.Search() { To = [] },
            };

            return item;
        }

        private static Machine CreateMachine()
        {
            Machine item = new Machine(CreateMetadataMachine());
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
            item.Part = CreatePart(machine);
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
            item.DiskArea = CreateDiskArea(machine);
            item.Part = CreatePart(machine);
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
            item.Part = CreatePart(machine);
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
            item.DataArea = CreateDataArea(machine);
            item.Part = CreatePart(machine);
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
            Assert.NotNull(header.CanOpen);
            Assert.Equal("category", header.Category);
            Assert.Equal("comment", header.Comment);
            Assert.Equal("date", header.Date);
            Assert.Equal("datversion", header.DatVersion);
            Assert.True(header.Debug);
            Assert.Equal("description", header.Description);
            Assert.Equal("email", header.Email);
            Assert.Equal("emulatorversion", header.EmulatorVersion);
            Assert.Equal("filename", header.FileName);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, header.ForceMerging);
            Assert.Equal(Data.Models.Metadata.NodumpFlag.Required, header.ForceNodump);
            Assert.Equal(Data.Models.Metadata.PackingFlag.Zip, header.ForcePacking);
            Assert.True(header.ForceZipping);
            Assert.Equal("header", header.HeaderSkipper);
            Assert.Equal("homepage", header.Homepage);
            Assert.Equal("id", header.Id);
            Assert.NotNull(header.Images);
            Assert.Equal("imfolder", header.ImFolder);
            Assert.NotNull(header.Infos);
            Assert.True(header.LockBiosMode);
            Assert.True(header.LockRomMode);
            Assert.True(header.LockSampleMode);
            Assert.Equal("mameconfig", header.MameConfig);
            Assert.Equal("name", header.Name);
            Assert.NotNull(header.NewDat);
            Assert.Equal("notes", header.Notes);
            Assert.Equal("plugin", header.Plugin);
            Assert.Equal("refname", header.RefName);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, header.RomMode);
            Assert.Equal("romtitle", header.RomTitle);
            Assert.Equal("rootdir", header.RootDir);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, header.SampleMode);
            Assert.Equal("schemalocation", header.SchemaLocation);
            Assert.Equal("screenshotsheight", header.ScreenshotsHeight);
            Assert.Equal("screenshotsWidth", header.ScreenshotsWidth);
            Assert.NotNull(header.Search);
            Assert.Equal("system", header.System);
            Assert.Equal("timestamp", header.Timestamp);
            Assert.Equal("type", header.Type);
            Assert.Equal("url", header.Url);
            Assert.Equal("version", header.Version);

            string[]? headerRowTitles = header.HeaderRow;
            Assert.NotNull(headerRowTitles);
            string? headerRowTitle = Assert.Single(headerRowTitles);
            Assert.Equal("header", headerRowTitle);
        }

        private static void ValidateMetadataMachine(Data.Models.Metadata.Machine machine)
        {
            Assert.Equal("board", machine.Board);
            Assert.Equal("buttons", machine.Buttons);
            Assert.Equal("cloneof", machine.CloneOf);
            Assert.Equal("cloneofid", machine.CloneOfId);
            Assert.Equal("company", machine.Company);
            Assert.Equal("control", machine.Control);
            Assert.Equal("country", machine.Country);
            Assert.Equal("crc", machine.CRC);
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
            Assert.Equal("year", machine.Year);

            Data.Models.Metadata.Adjuster[]? adjusters = machine.Adjuster;
            Assert.NotNull(adjusters);
            Data.Models.Metadata.Adjuster adjuster = Assert.Single(adjusters);
            ValidateMetadataAdjuster(adjuster);

            Data.Models.Metadata.Archive[]? archives = machine.Archive;
            Assert.NotNull(archives);
            Data.Models.Metadata.Archive archive = Assert.Single(archives);
            ValidateMetadataArchive(archive);

            Data.Models.Metadata.BiosSet[]? biosSets = machine.BiosSet;
            Assert.NotNull(biosSets);
            Data.Models.Metadata.BiosSet biosSet = Assert.Single(biosSets);
            ValidateMetadataBiosSet(biosSet);

            string[]? categories = machine.Category;
            Assert.NotNull(categories);
            string? category = Assert.Single(categories);
            Assert.Equal("category", category);

            Data.Models.Metadata.Chip[]? chips = machine.Chip;
            Assert.NotNull(chips);
            Data.Models.Metadata.Chip chip = Assert.Single(chips);
            ValidateMetadataChip(chip);

            string[]? comments = machine.Comment;
            Assert.NotNull(comments);
            string? comment = Assert.Single(comments);
            Assert.Equal("comment", comment);

            Data.Models.Metadata.Configuration[]? configurations = machine.Configuration;
            Assert.NotNull(configurations);
            Data.Models.Metadata.Configuration configuration = Assert.Single(configurations);
            ValidateMetadataConfiguration(configuration);

            Data.Models.Metadata.Device[]? devices = machine.Device;
            Assert.NotNull(devices);
            Data.Models.Metadata.Device device = Assert.Single(devices);
            ValidateMetadataDevice(device);

            Data.Models.Metadata.DeviceRef[]? deviceRefs = machine.DeviceRef;
            Assert.NotNull(deviceRefs);
            Data.Models.Metadata.DeviceRef deviceRef = Assert.Single(deviceRefs);
            ValidateMetadataDeviceRef(deviceRef);

            Data.Models.Metadata.DipSwitch[]? dipSwitches = machine.DipSwitch;
            Assert.NotNull(dipSwitches);
            Assert.Equal(2, dipSwitches.Length);
            Data.Models.Metadata.DipSwitch dipSwitch = dipSwitches[0];
            ValidateMetadataDipSwitch(dipSwitch);

            Data.Models.Metadata.Disk[]? disks = machine.Disk;
            Assert.NotNull(disks);
            Assert.Equal(2, disks.Length);
            Data.Models.Metadata.Disk disk = disks[0];
            ValidateMetadataDisk(disk);

            Data.Models.Metadata.Display[]? displays = machine.Display;
            Assert.NotNull(displays);
            Assert.Equal(2, displays.Length);
            Data.Models.Metadata.Display? display = Array.Find(displays, d => d.AspectX == null);
            ValidateMetadataDisplay(display);

            Data.Models.Metadata.Driver? driver = machine.Driver;
            Assert.NotNull(driver);
            ValidateMetadataDriver(driver);

            // TODO: Implement this validation
            // Data.Models.Metadata.Dump[]? dumps = machine.Dump;
            // Assert.NotNull(dumps);
            // Data.Models.Metadata.Dump dump = Assert.Single(dumps);
            // ValidateMetadataDump(dump);

            Data.Models.Metadata.Feature[]? features = machine.Feature;
            Assert.NotNull(features);
            Assert.Equal(2, features.Length);
            Data.Models.Metadata.Feature feature = features[0];
            ValidateMetadataFeature(feature);

            Data.Models.Metadata.Info[]? infos = machine.Info;
            Assert.NotNull(infos);
            Data.Models.Metadata.Info info = Assert.Single(infos);
            ValidateMetadataInfo(info);

            Data.Models.Metadata.Input? input = machine.Input;
            Assert.NotNull(input);
            ValidateMetadataInput(input);

            Data.Models.Metadata.Media[]? media = machine.Media;
            Assert.NotNull(media);
            Data.Models.Metadata.Media medium = Assert.Single(media);
            ValidateMetadataMedia(medium);

            Data.Models.Metadata.Part[]? parts = machine.Part;
            Assert.NotNull(parts);
            Data.Models.Metadata.Part part = Assert.Single(parts);
            ValidateMetadataPart(part);

            Data.Models.Metadata.Port[]? ports = machine.Port;
            Assert.NotNull(ports);
            Data.Models.Metadata.Port port = Assert.Single(ports);
            ValidateMetadataPort(port);

            Data.Models.Metadata.RamOption[]? ramOptions = machine.RamOption;
            Assert.NotNull(ramOptions);
            Data.Models.Metadata.RamOption ramOption = Assert.Single(ramOptions);
            ValidateMetadataRamOption(ramOption);

            Data.Models.Metadata.Release[]? releases = machine.Release;
            Assert.NotNull(releases);
            Data.Models.Metadata.Release release = Assert.Single(releases);
            ValidateMetadataRelease(release);

            Data.Models.Metadata.Rom[]? roms = machine.Rom;
            Assert.NotNull(roms);
            Assert.Equal(2, roms.Length);
            Data.Models.Metadata.Rom rom = roms[0];
            ValidateMetadataRom(rom);

            Data.Models.Metadata.Sample[]? samples = machine.Sample;
            Assert.NotNull(samples);
            Data.Models.Metadata.Sample sample = Assert.Single(samples);
            ValidateMetadataSample(sample);

            Data.Models.Metadata.SharedFeat[]? sharedFeats = machine.SharedFeat;
            Assert.NotNull(sharedFeats);
            Data.Models.Metadata.SharedFeat sharedFeat = Assert.Single(sharedFeats);
            ValidateMetadataSharedFeat(sharedFeat);

            Data.Models.Metadata.Slot[]? slots = machine.Slot;
            Assert.NotNull(slots);
            Data.Models.Metadata.Slot slot = Assert.Single(slots);
            ValidateMetadataSlot(slot);

            Data.Models.Metadata.SoftwareList[]? softwareLists = machine.SoftwareList;
            Assert.NotNull(softwareLists);
            Data.Models.Metadata.SoftwareList softwareList = Assert.Single(softwareLists);
            ValidateMetadataSoftwareList(softwareList);

            Data.Models.Metadata.Sound? sound = machine.Sound;
            Assert.NotNull(sound);
            ValidateMetadataSound(sound);

            Data.Models.Metadata.Video[]? videos = machine.Video;
            Assert.NotNull(videos);
            Data.Models.Metadata.Video video = Assert.Single(videos);
            ValidateMetadataVideo(video);
        }

        private static void ValidateMetadataAdjuster(Data.Models.Metadata.Adjuster? adjuster)
        {
            Assert.NotNull(adjuster);
            Assert.True(adjuster.Default);
            Assert.Equal("name", adjuster.Name);

            Data.Models.Metadata.Condition? condition = adjuster.Condition;
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
            Assert.Equal("number", archive.Number);
            Assert.Equal("clone", archive.CloneTag);
            Assert.Equal("regparent", archive.RegParent);
            Assert.Equal("mergeof", archive.MergeOf);
            Assert.Equal("mergename", archive.MergeName);
            Assert.Equal("name", archive.Name);
            Assert.Equal("name_alt", archive.NameAlt);
            Assert.Equal("region", archive.Region);
            Assert.Equal("languages", archive.Languages);
            Assert.Equal(true, archive.ShowLang);
            Assert.Equal("langchecked", archive.LangChecked);
            Assert.Equal("version1", archive.Version1);
            Assert.Equal("version2", archive.Version2);
            Assert.Equal("devstatus", archive.DevStatus);
            Assert.Equal("additional", archive.Additional);
            Assert.Equal("special1", archive.Special1);
            Assert.Equal("special2", archive.Special2);
            Assert.Equal(true, archive.Alt);
            Assert.Equal("gameid1", archive.GameId1);
            Assert.Equal("gameid2", archive.GameId2);
            Assert.Equal("description", archive.Description);
            Assert.Equal(true, archive.Bios);
            Assert.Equal(true, archive.Licensed);
            Assert.Equal(true, archive.Pirate);
            Assert.Equal(true, archive.Physical);
            Assert.Equal(true, archive.Complete);
            Assert.Equal(true, archive.Adult);
            Assert.Equal(true, archive.Dat);
            Assert.Equal(true, archive.Listed);
            Assert.Equal(true, archive.Private);
            Assert.Equal("stickynote", archive.StickyNote);
            Assert.Equal("datternote", archive.DatterNote);
            Assert.Equal("categories", archive.Categories);
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

            ValidateMetadataCondition(configuration.Condition);

            Data.Models.Metadata.ConfLocation[]? confLocations = configuration.ConfLocation;
            Assert.NotNull(confLocations);
            Data.Models.Metadata.ConfLocation? confLocation = Assert.Single(confLocations);
            ValidateMetadataConfLocation(confLocation);

            Data.Models.Metadata.ConfSetting[]? confSettings = configuration.ConfSetting;
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

            ValidateMetadataCondition(confSetting.Condition);
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

            Data.Models.Metadata.Rom[]? roms = dataArea.Rom;
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

            Data.Models.Metadata.Extension[]? extensions = device.Extension;
            Assert.NotNull(extensions);
            Data.Models.Metadata.Extension? extension = Assert.Single(extensions);
            ValidateMetadataExtension(extension);

            ValidateMetadataInstance(device.Instance);
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

            ValidateMetadataCondition(dipSwitch.Condition);

            Data.Models.Metadata.DipLocation[]? dipLocations = dipSwitch.DipLocation;
            Assert.NotNull(dipLocations);
            Data.Models.Metadata.DipLocation? dipLocation = Assert.Single(dipLocations);
            ValidateMetadataDipLocation(dipLocation);

            Data.Models.Metadata.DipValue[]? dipValues = dipSwitch.DipValue;
            Assert.NotNull(dipValues);
            Data.Models.Metadata.DipValue? dipValue = Assert.Single(dipValues);
            ValidateMetadataDipValue(dipValue);

            string[]? entries = dipSwitch.Entry;
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

            ValidateMetadataCondition(dipValue.Condition);
        }

        private static void ValidateMetadataDisk(Data.Models.Metadata.Disk? disk)
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

        private static void ValidateMetadataDiskArea(Data.Models.Metadata.DiskArea? diskArea)
        {
            Assert.NotNull(diskArea);
            Assert.Equal("name", diskArea.Name);

            Data.Models.Metadata.Disk[]? disks = diskArea.Disk;
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
            Assert.Equal("controlattr", input.ControlAttr);
            Assert.Equal(12345, input.Players);
            Assert.True(input.Service);
            Assert.True(input.Tilt);

            Data.Models.Metadata.Control[]? controls = input.Control;
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
            Assert.Equal(HashType.MD5.ZeroString, media.MD5);
            Assert.Equal("name", media.Name);
            Assert.Equal(HashType.SHA1.ZeroString, media.SHA1);
            Assert.Equal(HashType.SHA256.ZeroString, media.SHA256);
            Assert.Equal(HashType.SpamSum.ZeroString, media.SpamSum);
        }

        private static void ValidateMetadataPart(Data.Models.Metadata.Part? part)
        {
            Assert.NotNull(part);
            Assert.Equal("interface", part.Interface);
            Assert.Equal("name", part.Name);

            Data.Models.Metadata.DataArea[]? dataAreas = part.DataArea;
            Assert.NotNull(dataAreas);
            Data.Models.Metadata.DataArea? dataArea = Assert.Single(dataAreas);
            ValidateMetadataDataArea(dataArea);

            Data.Models.Metadata.DiskArea[]? diskAreas = part.DiskArea;
            Assert.NotNull(diskAreas);
            Data.Models.Metadata.DiskArea? diskArea = Assert.Single(diskAreas);
            ValidateMetadataDiskArea(diskArea);

            Data.Models.Metadata.DipSwitch[]? dipSwitches = part.DipSwitch;
            Assert.NotNull(dipSwitches);
            Data.Models.Metadata.DipSwitch? dipSwitch = Assert.Single(dipSwitches);
            ValidateMetadataDipSwitch(dipSwitch);

            Data.Models.Metadata.Feature[]? features = part.Feature;
            Assert.NotNull(features);
            Data.Models.Metadata.Feature? feature = Assert.Single(features);
            ValidateMetadataFeature(feature);
        }

        private static void ValidateMetadataPort(Data.Models.Metadata.Port? port)
        {
            Assert.NotNull(port);
            Assert.Equal("tag", port.Tag);

            Data.Models.Metadata.Analog[]? dipValues = port.Analog;
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
            Assert.Equal("id", releaseDetails.Id);
            Assert.Equal("appendtonumber", releaseDetails.AppendToNumber);
            Assert.Equal("date", releaseDetails.Date);
            Assert.Equal("originalformat", releaseDetails.OriginalFormat);
            Assert.Equal("group", releaseDetails.Group);
            Assert.Equal("dirname", releaseDetails.DirName);
            Assert.Equal("nfoname", releaseDetails.NfoName);
            Assert.Equal("nfosize", releaseDetails.NfoSize);
            Assert.Equal("nfocrc", releaseDetails.NfoCRC);
            Assert.Equal("archivename", releaseDetails.ArchiveName);
            Assert.Equal("rominfo", releaseDetails.RomInfo);
            Assert.Equal("category", releaseDetails.Category);
            Assert.Equal("comment", releaseDetails.Comment);
            Assert.Equal("tool", releaseDetails.Tool);
            Assert.Equal("region", releaseDetails.Region);
            Assert.Equal("origin", releaseDetails.Origin);
        }

        private static void ValidateMetadataRom(Data.Models.Metadata.Rom? rom)
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
            Assert.Equal("original", rom.Original);
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
            Assert.Equal("source", rom.Source);
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

            Data.Models.Metadata.SlotOption[]? slotOptions = slot.SlotOption;
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
