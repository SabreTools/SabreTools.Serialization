using Xunit;

namespace SabreTools.Serialization.CrossModel.Test
{
    public class ListxmlTests
    {
        [Fact]
        public void RoundTripGameTest()
        {
            // Get the cross-model serializer
            var serializer = new Listxml();

            // Build the data
            Data.Models.Listxml.Mame mame = Build(game: true);

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(mame);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.Listxml.Mame? newMame = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newMame);
            Assert.Equal("build", newMame.Build);
            Assert.Equal(true, newMame.Debug);
            Assert.Equal("mameconfig", newMame.MameConfig);

            Assert.NotNull(newMame.Game);
            var newGame = Assert.Single(newMame.Game);
            Validate(newGame);
        }

        [Fact]
        public void RoundTripMachineTest()
        {
            // Get the cross-model serializer
            var serializer = new Listxml();

            // Build the data
            Data.Models.Listxml.Mame mame = Build(game: false);

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(mame);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.Listxml.Mame? newMame = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newMame);
            Assert.Equal("build", newMame.Build);
            Assert.Equal(true, newMame.Debug);
            Assert.Equal("mameconfig", newMame.MameConfig);

            Assert.NotNull(newMame.Game);
            var newGame = Assert.Single(newMame.Game);
            Validate(newGame);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.Listxml.Mame Build(bool game)
        {
            var biosset = new Data.Models.Listxml.BiosSet
            {
                Name = "name",
                Description = "description",
                Default = true,
            };

            var rom = new Data.Models.Listxml.Rom
            {
                Name = "name",
                Bios = "bios",
                Size = 12345,
                CRC = "crc32",
                SHA1 = "sha1",
                Merge = "merge",
                Region = "region",
                Offset = "offset",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Optional = true,
                Dispose = true,
                SoundOnly = true,
            };

            var disk = new Data.Models.Listxml.Disk
            {
                Name = "name",
                MD5 = "md5",
                SHA1 = "sha1",
                Merge = "merge",
                Region = "region",
                Index = 12345,
                Writable = true,
                Status = Data.Models.Metadata.ItemStatus.Good,
                Optional = true,
            };

            var deviceref = new Data.Models.Listxml.DeviceRef
            {
                Name = "name",
            };

            var sample = new Data.Models.Listxml.Sample
            {
                Name = "name",
            };

            var chip = new Data.Models.Listxml.Chip
            {
                Name = "name",
                Tag = "tag",
                Type = Data.Models.Metadata.ChipType.CPU,
                SoundOnly = true,
                Clock = 12345,
            };

            var display = new Data.Models.Listxml.Display
            {
                Tag = "tag",
                Type = Data.Models.Metadata.DisplayType.Vector,
                Rotate = Data.Models.Metadata.Rotation.East,
                FlipX = true,
                Width = 12345,
                Height = 12345,
                Refresh = 123.45,
                PixClock = 12345,
                HTotal = 12345,
                HBEnd = 12345,
                HBStart = 12345,
                VTotal = 12345,
                VBEnd = 12345,
                VBStart = 12345,
            };

            var video = new Data.Models.Listxml.Video
            {
                Screen = Data.Models.Metadata.DisplayType.Vector,
                Orientation = Data.Models.Metadata.Rotation.East,
                Width = 12345,
                Height = 12345,
                AspectX = 12345,
                AspectY = 12345,
                Refresh = 123.45,
            };

            var sound = new Data.Models.Listxml.Sound
            {
                Channels = 12345,
            };

            var control = new Data.Models.Listxml.Control
            {
                Type = Data.Models.Metadata.ControlType.Lightgun,
                Player = 12345,
                Buttons = 12345,
                ReqButtons = 12345,
                Minimum = 12345,
                Maximum = 12345,
                Sensitivity = 12345,
                KeyDelta = 12345,
                Reverse = true,
                Ways = "ways",
                Ways2 = "ways2",
                Ways3 = "ways3",
            };

            var input = new Data.Models.Listxml.Input
            {
                Service = true,
                Tilt = true,
                Players = 12345,
                ControlAttr = "controlattr",
                Buttons = 12345,
                Coins = 12345,
                Control = [control],
            };

            var condition = new Data.Models.Listxml.Condition
            {
                Tag = "tag",
                Mask = "mask",
                Relation = Data.Models.Metadata.Relation.Equal,
                Value = "value",
            };

            var diplocation = new Data.Models.Listxml.DipLocation
            {
                Name = "name",
                Number = 12345,
                Inverted = true,
            };

            var dipvalue = new Data.Models.Listxml.DipValue
            {
                Name = "name",
                Value = "value",
                Default = true,
                Condition = condition,
            };

            var dipswitch = new Data.Models.Listxml.DipSwitch
            {
                Name = "name",
                Tag = "tag",
                Mask = "mask",
                Condition = condition,
                DipLocation = [diplocation],
                DipValue = [dipvalue],
            };

            var conflocation = new Data.Models.Listxml.ConfLocation
            {
                Name = "name",
                Number = 12345,
                Inverted = true,
            };

            var confsetting = new Data.Models.Listxml.ConfSetting
            {
                Name = "name",
                Value = "value",
                Default = true,
                Condition = condition,
            };

            var configuration = new Data.Models.Listxml.Configuration
            {
                Name = "name",
                Tag = "tag",
                Mask = "mask",
                Condition = condition,
                ConfLocation = [conflocation],
                ConfSetting = [confsetting],
            };

            var analog = new Data.Models.Listxml.Analog
            {
                Mask = "mask",
            };

            var port = new Data.Models.Listxml.Port
            {
                Tag = "tag",
                Analog = [analog],
            };

            var adjuster = new Data.Models.Listxml.Adjuster
            {
                Name = "name",
                Default = true,
                Condition = condition,
            };

            var driver = new Data.Models.Listxml.Driver
            {
                Status = Data.Models.Metadata.SupportStatus.Good,
                Color = Data.Models.Metadata.SupportStatus.Good,
                Sound = Data.Models.Metadata.SupportStatus.Good,
                PaletteSize = "palettesize",
                Emulation = Data.Models.Metadata.SupportStatus.Good,
                Cocktail = Data.Models.Metadata.SupportStatus.Good,
                SaveState = Data.Models.Metadata.Supported.Yes,
                RequiresArtwork = true,
                Unofficial = true,
                NoSoundHardware = true,
                Incomplete = true,
            };

            var feature = new Data.Models.Listxml.Feature
            {
                Type = Data.Models.Metadata.FeatureType.Protection,
                Status = Data.Models.Metadata.FeatureStatus.Imperfect,
                Overall = Data.Models.Metadata.FeatureStatus.Imperfect,
            };

            var instance = new Data.Models.Listxml.Instance
            {
                Name = "name",
                BriefName = "briefname",
            };

            var extension = new Data.Models.Listxml.Extension
            {
                Name = "name",
            };

            var device = new Data.Models.Listxml.Device
            {
                Type = Data.Models.Metadata.DeviceType.PunchTape,
                Tag = "tag",
                FixedImage = "fixedimage",
                Mandatory = true,
                Interface = "interface",
                Instance = instance,
                Extension = [extension],
            };

            var slotOption = new Data.Models.Listxml.SlotOption
            {
                Name = "name",
                DevName = "devname",
                Default = true,
            };

            var slot = new Data.Models.Listxml.Slot
            {
                Name = "name",
                SlotOption = [slotOption],
            };

            var softwarelist = new Data.Models.Listxml.SoftwareList
            {
                Tag = "tag",
                Name = "name",
                Status = Data.Models.Metadata.SoftwareListStatus.Original,
                Filter = "filter",
            };

            var ramoption = new Data.Models.Listxml.RamOption
            {
                Name = "name",
                Default = true,
                Content = "content",
            };

            Data.Models.Listxml.GameBase gameBase = game
                ? new Data.Models.Listxml.Game()
                : new Data.Models.Listxml.Machine();
            gameBase.Name = "name";
            gameBase.SourceFile = "sourcefile";
            gameBase.IsBios = true;
            gameBase.IsDevice = true;
            gameBase.IsMechanical = true;
            gameBase.Runnable = Data.Models.Metadata.Runnable.Yes;
            gameBase.CloneOf = "cloneof";
            gameBase.RomOf = "romof";
            gameBase.SampleOf = "sampleof";
            gameBase.Description = "description";
            gameBase.Year = "year";
            gameBase.Manufacturer = "manufacturer";
            gameBase.History = "history";
            gameBase.BiosSet = [biosset];
            gameBase.Rom = [rom];
            gameBase.Disk = [disk];
            gameBase.DeviceRef = [deviceref];
            gameBase.Sample = [sample];
            gameBase.Chip = [chip];
            gameBase.Display = [display];
            gameBase.Video = [video];
            gameBase.Sound = sound;
            gameBase.Input = input;
            gameBase.DipSwitch = [dipswitch];
            gameBase.Configuration = [configuration];
            gameBase.Port = [port];
            gameBase.Adjuster = [adjuster];
            gameBase.Driver = driver;
            gameBase.Feature = [feature];
            gameBase.Device = [device];
            gameBase.Slot = [slot];
            gameBase.SoftwareList = [softwarelist];
            gameBase.RamOption = [ramoption];

            return new Data.Models.Listxml.Mame
            {
                Build = "build",
                Debug = true,
                MameConfig = "mameconfig",
                Game = [gameBase],
            };
        }

        /// <summary>
        /// Validate a GameBase
        /// </summary>
        private static void Validate(Data.Models.Listxml.GameBase? gb)
        {
            Assert.NotNull(gb);
            Assert.Equal("name", gb.Name);
            Assert.Equal("sourcefile", gb.SourceFile);
            Assert.Equal(true, gb.IsBios);
            Assert.Equal(true, gb.IsDevice);
            Assert.Equal(true, gb.IsMechanical);
            Assert.Equal(Data.Models.Metadata.Runnable.Yes, gb.Runnable);
            Assert.Equal("cloneof", gb.CloneOf);
            Assert.Equal("romof", gb.RomOf);
            Assert.Equal("sampleof", gb.SampleOf);
            Assert.Equal("description", gb.Description);
            Assert.Equal("year", gb.Year);
            Assert.Equal("manufacturer", gb.Manufacturer);
            Assert.Equal("history", gb.History);

            Assert.NotNull(gb.BiosSet);
            var biosset = Assert.Single(gb.BiosSet);
            Validate(biosset);

            Assert.NotNull(gb.Rom);
            var rom = Assert.Single(gb.Rom);
            Validate(rom);

            Assert.NotNull(gb.Disk);
            var disk = Assert.Single(gb.Disk);
            Validate(disk);

            Assert.NotNull(gb.DeviceRef);
            var deviceref = Assert.Single(gb.DeviceRef);
            Validate(deviceref);

            Assert.NotNull(gb.Sample);
            var sample = Assert.Single(gb.Sample);
            Validate(sample);

            Assert.NotNull(gb.Chip);
            var chip = Assert.Single(gb.Chip);
            Validate(chip);

            Assert.NotNull(gb.Display);
            var display = Assert.Single(gb.Display);
            Validate(display);

            Assert.NotNull(gb.Video);
            var video = Assert.Single(gb.Video);
            Validate(video);

            Validate(gb.Sound);
            Validate(gb.Input);

            Assert.NotNull(gb.DipSwitch);
            var dipswitch = Assert.Single(gb.DipSwitch);
            Validate(dipswitch);

            Assert.NotNull(gb.Configuration);
            var configuration = Assert.Single(gb.Configuration);
            Validate(configuration);

            Assert.NotNull(gb.Port);
            var port = Assert.Single(gb.Port);
            Validate(port);

            Assert.NotNull(gb.Adjuster);
            var adjuster = Assert.Single(gb.Adjuster);
            Validate(adjuster);

            Validate(gb.Driver);

            Assert.NotNull(gb.Feature);
            var feature = Assert.Single(gb.Feature);
            Validate(feature);

            Assert.NotNull(gb.Device);
            var device = Assert.Single(gb.Device);
            Validate(device);

            Assert.NotNull(gb.Slot);
            var slot = Assert.Single(gb.Slot);
            Validate(slot);

            Assert.NotNull(gb.SoftwareList);
            var softwarelist = Assert.Single(gb.SoftwareList);
            Validate(softwarelist);

            Assert.NotNull(gb.RamOption);
            var ramoption = Assert.Single(gb.RamOption);
            Validate(ramoption);
        }

        /// <summary>
        /// Validate a BiosSet
        /// </summary>
        private static void Validate(Data.Models.Listxml.BiosSet? biosset)
        {
            Assert.NotNull(biosset);
            Assert.Equal("name", biosset.Name);
            Assert.Equal("description", biosset.Description);
            Assert.Equal(true, biosset.Default);
        }

        /// <summary>
        /// Validate a Rom
        /// </summary>
        private static void Validate(Data.Models.Listxml.Rom? rom)
        {
            Assert.NotNull(rom);
            Assert.Equal("name", rom.Name);
            Assert.Equal("bios", rom.Bios);
            Assert.Equal(12345, rom.Size);
            Assert.Equal("crc32", rom.CRC);
            Assert.Equal("sha1", rom.SHA1);
            Assert.Equal("merge", rom.Merge);
            Assert.Equal("region", rom.Region);
            Assert.Equal("offset", rom.Offset);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, rom.Status);
            Assert.Equal(true, rom.Optional);
            Assert.Equal(true, rom.Dispose);
            Assert.Equal(true, rom.SoundOnly);
        }

        /// <summary>
        /// Validate a Disk
        /// </summary>
        private static void Validate(Data.Models.Listxml.Disk? disk)
        {
            Assert.NotNull(disk);
            Assert.Equal("name", disk.Name);
            Assert.Equal("md5", disk.MD5);
            Assert.Equal("sha1", disk.SHA1);
            Assert.Equal("merge", disk.Merge);
            Assert.Equal("region", disk.Region);
            Assert.Equal(12345, disk.Index);
            Assert.Equal(true, disk.Writable);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, disk.Status);
            Assert.Equal(true, disk.Optional);
        }

        /// <summary>
        /// Validate a DeviceRef
        /// </summary>
        private static void Validate(Data.Models.Listxml.DeviceRef? deviceref)
        {
            Assert.NotNull(deviceref);
            Assert.Equal("name", deviceref.Name);
        }

        /// <summary>
        /// Validate a Sample
        /// </summary>
        private static void Validate(Data.Models.Listxml.Sample? sample)
        {
            Assert.NotNull(sample);
            Assert.Equal("name", sample.Name);
        }

        /// <summary>
        /// Validate a Chip
        /// </summary>
        private static void Validate(Data.Models.Listxml.Chip? chip)
        {
            Assert.NotNull(chip);
            Assert.Equal("name", chip.Name);
            Assert.Equal("tag", chip.Tag);
            Assert.Equal(Data.Models.Metadata.ChipType.CPU, chip.Type);
            Assert.Equal(true, chip.SoundOnly);
            Assert.Equal(12345, chip.Clock);
        }

        /// <summary>
        /// Validate a Display
        /// </summary>
        private static void Validate(Data.Models.Listxml.Display? display)
        {
            Assert.NotNull(display);
            Assert.Equal("tag", display.Tag);
            Assert.Equal(Data.Models.Metadata.DisplayType.Vector, display.Type);
            Assert.Equal(Data.Models.Metadata.Rotation.East, display.Rotate);
            Assert.Equal(true, display.FlipX);
            Assert.Equal(12345, display.Width);
            Assert.Equal(12345, display.Height);
            Assert.Equal(123.45, display.Refresh);
            Assert.Equal(12345, display.PixClock);
            Assert.Equal(12345, display.HTotal);
            Assert.Equal(12345, display.HBEnd);
            Assert.Equal(12345, display.HBStart);
            Assert.Equal(12345, display.VTotal);
            Assert.Equal(12345, display.VBEnd);
            Assert.Equal(12345, display.VBStart);
        }

        /// <summary>
        /// Validate a Video
        /// </summary>
        private static void Validate(Data.Models.Listxml.Video? video)
        {
            Assert.NotNull(video);
            Assert.Equal(Data.Models.Metadata.DisplayType.Vector, video.Screen);
            Assert.Equal(Data.Models.Metadata.Rotation.East, video.Orientation);
            Assert.Equal(12345, video.Width);
            Assert.Equal(12345, video.Height);
            Assert.Equal(12345, video.AspectX);
            Assert.Equal(12345, video.AspectY);
            Assert.Equal(123.45, video.Refresh);
        }

        /// <summary>
        /// Validate a Sound
        /// </summary>
        private static void Validate(Data.Models.Listxml.Sound? sound)
        {
            Assert.NotNull(sound);
            Assert.Equal(12345, sound.Channels);
        }

        /// <summary>
        /// Validate a Input
        /// </summary>
        private static void Validate(Data.Models.Listxml.Input? input)
        {
            Assert.NotNull(input);
            Assert.Equal(true, input.Service);
            Assert.Equal(true, input.Tilt);
            Assert.Equal(12345, input.Players);
            Assert.Equal("controlattr", input.ControlAttr);
            Assert.Equal(12345, input.Buttons);
            Assert.Equal(12345, input.Coins);

            Assert.NotNull(input.Control);
            var control = Assert.Single(input.Control);
            Validate(control);
        }

        /// <summary>
        /// Validate a Control
        /// </summary>
        private static void Validate(Data.Models.Listxml.Control? control)
        {
            Assert.NotNull(control);
            Assert.Equal(Data.Models.Metadata.ControlType.Lightgun, control.Type);
            Assert.Equal(12345, control.Player);
            Assert.Equal(12345, control.Buttons);
            Assert.Equal(12345, control.ReqButtons);
            Assert.Equal(12345, control.Minimum);
            Assert.Equal(12345, control.Maximum);
            Assert.Equal(12345, control.Sensitivity);
            Assert.Equal(12345, control.KeyDelta);
            Assert.Equal(true, control.Reverse);
            Assert.Equal("ways", control.Ways);
            Assert.Equal("ways2", control.Ways2);
            Assert.Equal("ways3", control.Ways3);
        }

        /// <summary>
        /// Validate a DipSwitch
        /// </summary>
        private static void Validate(Data.Models.Listxml.DipSwitch? dipswitch)
        {
            Assert.NotNull(dipswitch);
            Assert.Equal("name", dipswitch.Name);
            Assert.Equal("tag", dipswitch.Tag);
            Assert.Equal("mask", dipswitch.Mask);
            Validate(dipswitch.Condition);

            Assert.NotNull(dipswitch.DipLocation);
            var diplocation = Assert.Single(dipswitch.DipLocation);
            Validate(diplocation);

            Assert.NotNull(dipswitch.DipValue);
            var dipvalue = Assert.Single(dipswitch.DipValue);
            Validate(dipvalue);
        }

        /// <summary>
        /// Validate a Condition
        /// </summary>
        private static void Validate(Data.Models.Listxml.Condition? condition)
        {
            Assert.NotNull(condition);
            Assert.Equal("tag", condition.Tag);
            Assert.Equal("mask", condition.Mask);
            Assert.Equal(Data.Models.Metadata.Relation.Equal, condition.Relation);
            Assert.Equal("value", condition.Value);
        }

        /// <summary>
        /// Validate a DipLocation
        /// </summary>
        private static void Validate(Data.Models.Listxml.DipLocation? diplocation)
        {
            Assert.NotNull(diplocation);
            Assert.Equal("name", diplocation.Name);
            Assert.Equal(12345, diplocation.Number);
            Assert.Equal(true, diplocation.Inverted);
        }

        /// <summary>
        /// Validate a DipValue
        /// </summary>
        private static void Validate(Data.Models.Listxml.DipValue? dipvalue)
        {
            Assert.NotNull(dipvalue);
            Assert.Equal("name", dipvalue.Name);
            Assert.Equal("value", dipvalue.Value);
            Assert.Equal(true, dipvalue.Default);
            Validate(dipvalue.Condition);
        }

        /// <summary>
        /// Validate a Configuration
        /// </summary>
        private static void Validate(Data.Models.Listxml.Configuration? configuration)
        {
            Assert.NotNull(configuration);
            Assert.Equal("name", configuration.Name);
            Assert.Equal("tag", configuration.Tag);
            Assert.Equal("mask", configuration.Mask);
            Validate(configuration.Condition);

            Assert.NotNull(configuration.ConfLocation);
            var conflocation = Assert.Single(configuration.ConfLocation);
            Validate(conflocation);

            Assert.NotNull(configuration.ConfSetting);
            var confsetting = Assert.Single(configuration.ConfSetting);
            Validate(confsetting);
        }

        /// <summary>
        /// Validate a ConfLocation
        /// </summary>
        private static void Validate(Data.Models.Listxml.ConfLocation? conflocation)
        {
            Assert.NotNull(conflocation);
            Assert.Equal("name", conflocation.Name);
            Assert.Equal(12345, conflocation.Number);
            Assert.Equal(true, conflocation.Inverted);
        }

        /// <summary>
        /// Validate a ConfSetting
        /// </summary>
        private static void Validate(Data.Models.Listxml.ConfSetting? confsetting)
        {
            Assert.NotNull(confsetting);
            Assert.Equal("name", confsetting.Name);
            Assert.Equal("value", confsetting.Value);
            Assert.Equal(true, confsetting.Default);
            Validate(confsetting.Condition);
        }

        /// <summary>
        /// Validate a Port
        /// </summary>
        private static void Validate(Data.Models.Listxml.Port? port)
        {
            Assert.NotNull(port);
            Assert.Equal("tag", port.Tag);

            Assert.NotNull(port.Analog);
            var analog = Assert.Single(port.Analog);
            Validate(analog);
        }

        /// <summary>
        /// Validate a Analog
        /// </summary>
        private static void Validate(Data.Models.Listxml.Analog? analog)
        {
            Assert.NotNull(analog);
            Assert.Equal("mask", analog.Mask);
        }

        /// <summary>
        /// Validate a Adjuster
        /// </summary>
        private static void Validate(Data.Models.Listxml.Adjuster? adjuster)
        {
            Assert.NotNull(adjuster);
            Assert.Equal("name", adjuster.Name);
            Assert.Equal(true, adjuster.Default);
            Validate(adjuster.Condition);
        }

        /// <summary>
        /// Validate a Driver
        /// </summary>
        private static void Validate(Data.Models.Listxml.Driver? driver)
        {
            Assert.NotNull(driver);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Status);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Color);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Sound);
            Assert.Equal("palettesize", driver.PaletteSize);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Emulation);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Cocktail);
            Assert.Equal(Data.Models.Metadata.Supported.Yes, driver.SaveState);
            Assert.Equal(true, driver.RequiresArtwork);
            Assert.Equal(true, driver.Unofficial);
            Assert.Equal(true, driver.NoSoundHardware);
            Assert.Equal(true, driver.Incomplete);
        }

        /// <summary>
        /// Validate a Feature
        /// </summary>
        private static void Validate(Data.Models.Listxml.Feature? feature)
        {
            Assert.NotNull(feature);
            Assert.Equal(Data.Models.Metadata.FeatureType.Protection, feature.Type);
            Assert.Equal(Data.Models.Metadata.FeatureStatus.Imperfect, feature.Status);
            Assert.Equal(Data.Models.Metadata.FeatureStatus.Imperfect, feature.Overall);
        }

        /// <summary>
        /// Validate a Device
        /// </summary>
        private static void Validate(Data.Models.Listxml.Device? device)
        {
            Assert.NotNull(device);
            Assert.Equal(Data.Models.Metadata.DeviceType.PunchTape, device.Type);
            Assert.Equal("tag", device.Tag);
            Assert.Equal("fixedimage", device.FixedImage);
            Assert.Equal(true, device.Mandatory);
            Assert.Equal("interface", device.Interface);
            Validate(device.Instance);

            Assert.NotNull(device.Extension);
            var extension = Assert.Single(device.Extension);
            Validate(extension);
        }

        /// <summary>
        /// Validate a Instance
        /// </summary>
        private static void Validate(Data.Models.Listxml.Instance? instance)
        {
            Assert.NotNull(instance);
            Assert.Equal("name", instance.Name);
            Assert.Equal("briefname", instance.BriefName);
        }

        /// <summary>
        /// Validate a Extension
        /// </summary>
        private static void Validate(Data.Models.Listxml.Extension? extension)
        {
            Assert.NotNull(extension);
            Assert.Equal("name", extension.Name);
        }

        /// <summary>
        /// Validate a Slot
        /// </summary>
        private static void Validate(Data.Models.Listxml.Slot? slot)
        {
            Assert.NotNull(slot);
            Assert.Equal("name", slot.Name);

            Assert.NotNull(slot.SlotOption);
            var slotoption = Assert.Single(slot.SlotOption);
            Validate(slotoption);
        }

        /// <summary>
        /// Validate a SlotOption
        /// </summary>
        private static void Validate(Data.Models.Listxml.SlotOption? slotoption)
        {
            Assert.NotNull(slotoption);
            Assert.Equal("name", slotoption.Name);
            Assert.Equal("devname", slotoption.DevName);
            Assert.Equal(true, slotoption.Default);
        }

        /// <summary>
        /// Validate a SoftwareList
        /// </summary>
        private static void Validate(Data.Models.Listxml.SoftwareList? softwarelist)
        {
            Assert.NotNull(softwarelist);
            Assert.Equal("tag", softwarelist.Tag);
            Assert.Equal("name", softwarelist.Name);
            Assert.Equal(Data.Models.Metadata.SoftwareListStatus.Original, softwarelist.Status);
            Assert.Equal("filter", softwarelist.Filter);
        }

        /// <summary>
        /// Validate a RamOption
        /// </summary>
        private static void Validate(Data.Models.Listxml.RamOption? ramoption)
        {
            Assert.NotNull(ramoption);
            Assert.Equal("name", ramoption.Name);
            Assert.Equal(true, ramoption.Default);
            Assert.Equal("content", ramoption.Content);
        }
    }
}
