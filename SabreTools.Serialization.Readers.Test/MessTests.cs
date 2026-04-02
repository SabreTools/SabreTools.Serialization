using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class MessTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new Mess();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new Mess();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new Mess();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new Mess();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new Mess();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new Mess();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripGameTest()
        {
            // Get the serializer and deserializer
            var deserializer = new Mess();
            var serializer = new Writers.Mess();

            // Build the data
            Data.Models.Listxml.Mess m1 = Build(game: true);

            // Serialize to generic model
            Stream? metadata = serializer.SerializeStream(m1);
            Assert.NotNull(metadata);

            // Serialize to stream
            Data.Models.Listxml.Mess? newMess = deserializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newMess);
            Assert.Equal("XXXXXX", newMess.Version);

            Assert.NotNull(newMess.Game);
            var newGame = Assert.Single(newMess.Game);
            Validate(newGame);
        }

        [Fact]
        public void RoundTripMachineTest()
        {
            // Get the serializer and deserializer
            var deserializer = new Mess();
            var serializer = new Writers.Mess();

            // Build the data
            Data.Models.Listxml.Mess m1 = Build(game: false);

            // Serialize to generic model
            Stream? metadata = serializer.SerializeStream(m1);
            Assert.NotNull(metadata);

            // Serialize to stream
            Data.Models.Listxml.Mess? newMess = deserializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newMess);
            Assert.Equal("XXXXXX", newMess.Version);

            Assert.NotNull(newMess.Game);
            var newGame = Assert.Single(newMess.Game);
            Validate(newGame);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.Listxml.Mess Build(bool game)
        {
            var biosset = new Data.Models.Listxml.BiosSet
            {
                Name = "XXXXXX",
                Description = "XXXXXX",
                Default = true,
            };

            var rom = new Data.Models.Listxml.Rom
            {
                Name = "XXXXXX",
                Bios = "XXXXXX",
                Size = "XXXXXX",
                CRC = "XXXXXX",
                SHA1 = "XXXXXX",
                Merge = "XXXXXX",
                Region = "XXXXXX",
                Offset = "XXXXXX",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Optional = true,
                Dispose = true,
                SoundOnly = true,
            };

            var disk = new Data.Models.Listxml.Disk
            {
                Name = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                Merge = "XXXXXX",
                Region = "XXXXXX",
                Index = "XXXXXX",
                Writable = true,
                Status = Data.Models.Metadata.ItemStatus.Good,
                Optional = true,
            };

            var deviceref = new Data.Models.Listxml.DeviceRef
            {
                Name = "XXXXXX",
            };

            var sample = new Data.Models.Listxml.Sample
            {
                Name = "XXXXXX",
            };

            var chip = new Data.Models.Listxml.Chip
            {
                Name = "XXXXXX",
                Tag = "XXXXXX",
                Type = Data.Models.Metadata.ChipType.CPU,
                SoundOnly = true,
                Clock = "XXXXXX",
            };

            var display = new Data.Models.Listxml.Display
            {
                Tag = "XXXXXX",
                Type = Data.Models.Metadata.DisplayType.Vector,
                Rotate = "XXXXXX",
                FlipX = true,
                Width = "XXXXXX",
                Height = "XXXXXX",
                Refresh = "XXXXXX",
                PixClock = "XXXXXX",
                HTotal = "XXXXXX",
                HBEnd = "XXXXXX",
                HBStart = "XXXXXX",
                VTotal = "XXXXXX",
                VBEnd = "XXXXXX",
                VBStart = "XXXXXX",
            };

            var video = new Data.Models.Listxml.Video
            {
                Screen = Data.Models.Metadata.DisplayType.Vector,
                Orientation = "XXXXXX",
                Width = "XXXXXX",
                Height = "XXXXXX",
                AspectX = "XXXXXX",
                AspectY = "XXXXXX",
                Refresh = "XXXXXX",
            };

            var sound = new Data.Models.Listxml.Sound
            {
                Channels = "XXXXXX",
            };

            var control = new Data.Models.Listxml.Control
            {
                Type = Data.Models.Metadata.ControlType.Lightgun,
                Player = "XXXXXX",
                Buttons = "XXXXXX",
                ReqButtons = "XXXXXX",
                Minimum = "XXXXXX",
                Maximum = "XXXXXX",
                Sensitivity = "XXXXXX",
                KeyDelta = "XXXXXX",
                Reverse = true,
                Ways = "XXXXXX",
                Ways2 = "XXXXXX",
                Ways3 = "XXXXXX",
            };

            var input = new Data.Models.Listxml.Input
            {
                Service = true,
                Tilt = true,
                Players = "XXXXXX",
                //ControlAttr = "XXXXXX", // Mututally exclusive with input.Control
                Buttons = "XXXXXX",
                Coins = "XXXXXX",
                Control = [control],
            };

            var condition = new Data.Models.Listxml.Condition
            {
                Tag = "XXXXXX",
                Mask = "XXXXXX",
                Relation = Data.Models.Metadata.Relation.Equal,
                Value = "XXXXXX",
            };

            var diplocation = new Data.Models.Listxml.DipLocation
            {
                Name = "XXXXXX",
                Number = "XXXXXX",
                Inverted = true,
            };

            var dipvalue = new Data.Models.Listxml.DipValue
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
                Default = true,
                Condition = condition,
            };

            var dipswitch = new Data.Models.Listxml.DipSwitch
            {
                Name = "XXXXXX",
                Tag = "XXXXXX",
                Mask = "XXXXXX",
                Condition = condition,
                DipLocation = [diplocation],
                DipValue = [dipvalue],
            };

            var conflocation = new Data.Models.Listxml.ConfLocation
            {
                Name = "XXXXXX",
                Number = "XXXXXX",
                Inverted = true,
            };

            var confsetting = new Data.Models.Listxml.ConfSetting
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
                Default = true,
                Condition = condition,
            };

            var configuration = new Data.Models.Listxml.Configuration
            {
                Name = "XXXXXX",
                Tag = "XXXXXX",
                Mask = "XXXXXX",
                Condition = condition,
                ConfLocation = [conflocation],
                ConfSetting = [confsetting],
            };

            var analog = new Data.Models.Listxml.Analog
            {
                Mask = "XXXXXX",
            };

            var port = new Data.Models.Listxml.Port
            {
                Tag = "XXXXXX",
                Analog = [analog],
            };

            var adjuster = new Data.Models.Listxml.Adjuster
            {
                Name = "XXXXXX",
                Default = true,
                Condition = condition,
            };

            var driver = new Data.Models.Listxml.Driver
            {
                Status = Data.Models.Metadata.SupportStatus.Good,
                Color = Data.Models.Metadata.SupportStatus.Good,
                Sound = Data.Models.Metadata.SupportStatus.Good,
                PaletteSize = "XXXXXX",
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
                Name = "XXXXXX",
                BriefName = "XXXXXX",
            };

            var extension = new Data.Models.Listxml.Extension
            {
                Name = "XXXXXX",
            };

            var device = new Data.Models.Listxml.Device
            {
                Type = Data.Models.Metadata.DeviceType.PunchTape,
                Tag = "XXXXXX",
                FixedImage = "XXXXXX",
                Mandatory = "XXXXXX",
                Interface = "XXXXXX",
                Instance = instance,
                Extension = [extension],
            };

            var slotOption = new Data.Models.Listxml.SlotOption
            {
                Name = "XXXXXX",
                DevName = "XXXXXX",
                Default = true,
            };

            var slot = new Data.Models.Listxml.Slot
            {
                Name = "XXXXXX",
                SlotOption = [slotOption],
            };

            var softwarelist = new Data.Models.Listxml.SoftwareList
            {
                Tag = "XXXXXX",
                Name = "XXXXXX",
                Status = Data.Models.Metadata.SoftwareListStatus.Original,
                Filter = "XXXXXX",
            };

            var ramoption = new Data.Models.Listxml.RamOption
            {
                Name = "XXXXXX",
                Default = true,
                Content = "XXXXXX",
            };

            Data.Models.Listxml.GameBase gameBase = game
                ? new Data.Models.Listxml.Game()
                : new Data.Models.Listxml.Machine();
            gameBase.Name = "XXXXXX";
            gameBase.SourceFile = "XXXXXX";
            gameBase.IsBios = true;
            gameBase.IsDevice = true;
            gameBase.IsMechanical = true;
            gameBase.Runnable = Data.Models.Metadata.Runnable.Yes;
            gameBase.CloneOf = "XXXXXX";
            gameBase.RomOf = "XXXXXX";
            gameBase.SampleOf = "XXXXXX";
            gameBase.Description = "XXXXXX";
            gameBase.Year = "XXXXXX";
            gameBase.Manufacturer = "XXXXXX";
            gameBase.History = "XXXXXX";
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

            return new Data.Models.Listxml.Mess
            {
                Version = "XXXXXX",
                Game = [gameBase],
            };
        }

        /// <summary>
        /// Validate a GameBase
        /// </summary>
        private static void Validate(Data.Models.Listxml.GameBase? gb)
        {
            Assert.NotNull(gb);
            Assert.Equal("XXXXXX", gb.Name);
            Assert.Equal("XXXXXX", gb.SourceFile);
            Assert.Equal(true, gb.IsBios);
            Assert.Equal(true, gb.IsDevice);
            Assert.Equal(true, gb.IsMechanical);
            Assert.Equal(Data.Models.Metadata.Runnable.Yes, gb.Runnable);
            Assert.Equal("XXXXXX", gb.CloneOf);
            Assert.Equal("XXXXXX", gb.RomOf);
            Assert.Equal("XXXXXX", gb.SampleOf);
            Assert.Equal("XXXXXX", gb.Description);
            Assert.Equal("XXXXXX", gb.Year);
            Assert.Equal("XXXXXX", gb.Manufacturer);
            Assert.Equal("XXXXXX", gb.History);

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
            Assert.Equal("XXXXXX", biosset.Name);
            Assert.Equal("XXXXXX", biosset.Description);
            Assert.Equal(true, biosset.Default);
        }

        /// <summary>
        /// Validate a Rom
        /// </summary>
        private static void Validate(Data.Models.Listxml.Rom? rom)
        {
            Assert.NotNull(rom);
            Assert.Equal("XXXXXX", rom.Name);
            Assert.Equal("XXXXXX", rom.Bios);
            Assert.Equal("XXXXXX", rom.Size);
            Assert.Equal("XXXXXX", rom.CRC);
            Assert.Equal("XXXXXX", rom.SHA1);
            Assert.Equal("XXXXXX", rom.Merge);
            Assert.Equal("XXXXXX", rom.Region);
            Assert.Equal("XXXXXX", rom.Offset);
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
            Assert.Equal("XXXXXX", disk.Name);
            Assert.Equal("XXXXXX", disk.MD5);
            Assert.Equal("XXXXXX", disk.SHA1);
            Assert.Equal("XXXXXX", disk.Merge);
            Assert.Equal("XXXXXX", disk.Region);
            Assert.Equal("XXXXXX", disk.Index);
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
            Assert.Equal("XXXXXX", deviceref.Name);
        }

        /// <summary>
        /// Validate a Sample
        /// </summary>
        private static void Validate(Data.Models.Listxml.Sample? sample)
        {
            Assert.NotNull(sample);
            Assert.Equal("XXXXXX", sample.Name);
        }

        /// <summary>
        /// Validate a Chip
        /// </summary>
        private static void Validate(Data.Models.Listxml.Chip? chip)
        {
            Assert.NotNull(chip);
            Assert.Equal("XXXXXX", chip.Name);
            Assert.Equal("XXXXXX", chip.Tag);
            Assert.Equal(Data.Models.Metadata.ChipType.CPU, chip.Type);
            Assert.Equal(true, chip.SoundOnly);
            Assert.Equal("XXXXXX", chip.Clock);
        }

        /// <summary>
        /// Validate a Display
        /// </summary>
        private static void Validate(Data.Models.Listxml.Display? display)
        {
            Assert.NotNull(display);
            Assert.Equal("XXXXXX", display.Tag);
            Assert.Equal(Data.Models.Metadata.DisplayType.Vector, display.Type);
            Assert.Equal("XXXXXX", display.Rotate);
            Assert.Equal(true, display.FlipX);
            Assert.Equal("XXXXXX", display.Width);
            Assert.Equal("XXXXXX", display.Height);
            Assert.Equal("XXXXXX", display.Refresh);
            Assert.Equal("XXXXXX", display.PixClock);
            Assert.Equal("XXXXXX", display.HTotal);
            Assert.Equal("XXXXXX", display.HBEnd);
            Assert.Equal("XXXXXX", display.HBStart);
            Assert.Equal("XXXXXX", display.VTotal);
            Assert.Equal("XXXXXX", display.VBEnd);
            Assert.Equal("XXXXXX", display.VBStart);
        }

        /// <summary>
        /// Validate a Video
        /// </summary>
        private static void Validate(Data.Models.Listxml.Video? video)
        {
            Assert.NotNull(video);
            Assert.Equal(Data.Models.Metadata.DisplayType.Vector, video.Screen);
            Assert.Equal("XXXXXX", video.Orientation);
            Assert.Equal("XXXXXX", video.Width);
            Assert.Equal("XXXXXX", video.Height);
            Assert.Equal("XXXXXX", video.AspectX);
            Assert.Equal("XXXXXX", video.AspectY);
            Assert.Equal("XXXXXX", video.Refresh);
        }

        /// <summary>
        /// Validate a Sound
        /// </summary>
        private static void Validate(Data.Models.Listxml.Sound? sound)
        {
            Assert.NotNull(sound);
            Assert.Equal("XXXXXX", sound.Channels);
        }

        /// <summary>
        /// Validate a Input
        /// </summary>
        private static void Validate(Data.Models.Listxml.Input? input)
        {
            Assert.NotNull(input);
            Assert.Equal(true, input.Service);
            Assert.Equal(true, input.Tilt);
            Assert.Equal("XXXXXX", input.Players);
            //Assert.Equal("XXXXXX", input.ControlAttr); // Mututally exclusive with input.Control
            Assert.Equal("XXXXXX", input.Buttons);
            Assert.Equal("XXXXXX", input.Coins);

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
            Assert.Equal("XXXXXX", control.Player);
            Assert.Equal("XXXXXX", control.Buttons);
            Assert.Equal("XXXXXX", control.ReqButtons);
            Assert.Equal("XXXXXX", control.Minimum);
            Assert.Equal("XXXXXX", control.Maximum);
            Assert.Equal("XXXXXX", control.Sensitivity);
            Assert.Equal("XXXXXX", control.KeyDelta);
            Assert.Equal(true, control.Reverse);
            Assert.Equal("XXXXXX", control.Ways);
            Assert.Equal("XXXXXX", control.Ways2);
            Assert.Equal("XXXXXX", control.Ways3);
        }

        /// <summary>
        /// Validate a DipSwitch
        /// </summary>
        private static void Validate(Data.Models.Listxml.DipSwitch? dipswitch)
        {
            Assert.NotNull(dipswitch);
            Assert.Equal("XXXXXX", dipswitch.Name);
            Assert.Equal("XXXXXX", dipswitch.Tag);
            Assert.Equal("XXXXXX", dipswitch.Mask);
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
            Assert.Equal("XXXXXX", condition.Tag);
            Assert.Equal("XXXXXX", condition.Mask);
            Assert.Equal(Data.Models.Metadata.Relation.Equal, condition.Relation);
            Assert.Equal("XXXXXX", condition.Value);
        }

        /// <summary>
        /// Validate a DipLocation
        /// </summary>
        private static void Validate(Data.Models.Listxml.DipLocation? diplocation)
        {
            Assert.NotNull(diplocation);
            Assert.Equal("XXXXXX", diplocation.Name);
            Assert.Equal("XXXXXX", diplocation.Number);
            Assert.Equal(true, diplocation.Inverted);
        }

        /// <summary>
        /// Validate a DipValue
        /// </summary>
        private static void Validate(Data.Models.Listxml.DipValue? dipvalue)
        {
            Assert.NotNull(dipvalue);
            Assert.Equal("XXXXXX", dipvalue.Name);
            Assert.Equal("XXXXXX", dipvalue.Value);
            Assert.Equal(true, dipvalue.Default);
            Validate(dipvalue.Condition);
        }

        /// <summary>
        /// Validate a Configuration
        /// </summary>
        private static void Validate(Data.Models.Listxml.Configuration? configuration)
        {
            Assert.NotNull(configuration);
            Assert.Equal("XXXXXX", configuration.Name);
            Assert.Equal("XXXXXX", configuration.Tag);
            Assert.Equal("XXXXXX", configuration.Mask);
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
            Assert.Equal("XXXXXX", conflocation.Name);
            Assert.Equal("XXXXXX", conflocation.Number);
            Assert.Equal(true, conflocation.Inverted);
        }

        /// <summary>
        /// Validate a ConfSetting
        /// </summary>
        private static void Validate(Data.Models.Listxml.ConfSetting? confsetting)
        {
            Assert.NotNull(confsetting);
            Assert.Equal("XXXXXX", confsetting.Name);
            Assert.Equal("XXXXXX", confsetting.Value);
            Assert.Equal(true, confsetting.Default);
            Validate(confsetting.Condition);
        }

        /// <summary>
        /// Validate a Port
        /// </summary>
        private static void Validate(Data.Models.Listxml.Port? port)
        {
            Assert.NotNull(port);
            Assert.Equal("XXXXXX", port.Tag);

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
            Assert.Equal("XXXXXX", analog.Mask);
        }

        /// <summary>
        /// Validate a Adjuster
        /// </summary>
        private static void Validate(Data.Models.Listxml.Adjuster? adjuster)
        {
            Assert.NotNull(adjuster);
            Assert.Equal("XXXXXX", adjuster.Name);
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
            Assert.Equal("XXXXXX", driver.PaletteSize);
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
            Assert.Equal("XXXXXX", device.Tag);
            Assert.Equal("XXXXXX", device.FixedImage);
            Assert.Equal("XXXXXX", device.Mandatory);
            Assert.Equal("XXXXXX", device.Interface);
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
            Assert.Equal("XXXXXX", instance.Name);
            Assert.Equal("XXXXXX", instance.BriefName);
        }

        /// <summary>
        /// Validate a Extension
        /// </summary>
        private static void Validate(Data.Models.Listxml.Extension? extension)
        {
            Assert.NotNull(extension);
            Assert.Equal("XXXXXX", extension.Name);
        }

        /// <summary>
        /// Validate a Slot
        /// </summary>
        private static void Validate(Data.Models.Listxml.Slot? slot)
        {
            Assert.NotNull(slot);
            Assert.Equal("XXXXXX", slot.Name);

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
            Assert.Equal("XXXXXX", slotoption.Name);
            Assert.Equal("XXXXXX", slotoption.DevName);
            Assert.Equal(true, slotoption.Default);
        }

        /// <summary>
        /// Validate a SoftwareList
        /// </summary>
        private static void Validate(Data.Models.Listxml.SoftwareList? softwarelist)
        {
            Assert.NotNull(softwarelist);
            Assert.Equal("XXXXXX", softwarelist.Tag);
            Assert.Equal("XXXXXX", softwarelist.Name);
            Assert.Equal(Data.Models.Metadata.SoftwareListStatus.Original, softwarelist.Status);
            Assert.Equal("XXXXXX", softwarelist.Filter);
        }

        /// <summary>
        /// Validate a RamOption
        /// </summary>
        private static void Validate(Data.Models.Listxml.RamOption? ramoption)
        {
            Assert.NotNull(ramoption);
            Assert.Equal("XXXXXX", ramoption.Name);
            Assert.Equal(true, ramoption.Default);
            Assert.Equal("XXXXXX", ramoption.Content);
        }
    }
}
