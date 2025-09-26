using SabreTools.Serialization.CrossModel;
using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
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
            Assert.Equal("XXXXXX", newMame.Build);
            Assert.Equal("XXXXXX", newMame.Debug);
            Assert.Equal("XXXXXX", newMame.MameConfig);

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
            Assert.Equal("XXXXXX", newMame.Build);
            Assert.Equal("XXXXXX", newMame.Debug);
            Assert.Equal("XXXXXX", newMame.MameConfig);

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
                Name = "XXXXXX",
                Description = "XXXXXX",
                Default = "XXXXXX",
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
                Status = "XXXXXX",
                Optional = "XXXXXX",
                Dispose = "XXXXXX",
                SoundOnly = "XXXXXX",
            };

            var disk = new Data.Models.Listxml.Disk
            {
                Name = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                Merge = "XXXXXX",
                Region = "XXXXXX",
                Index = "XXXXXX",
                Writable = "XXXXXX",
                Status = "XXXXXX",
                Optional = "XXXXXX",
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
                Type = "XXXXXX",
                SoundOnly = "XXXXXX",
                Clock = "XXXXXX",
            };

            var display = new Data.Models.Listxml.Display
            {
                Tag = "XXXXXX",
                Type = "XXXXXX",
                Rotate = "XXXXXX",
                FlipX = "XXXXXX",
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
                Screen = "XXXXXX",
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
                Type = "XXXXXX",
                Player = "XXXXXX",
                Buttons = "XXXXXX",
                ReqButtons = "XXXXXX",
                Minimum = "XXXXXX",
                Maximum = "XXXXXX",
                Sensitivity = "XXXXXX",
                KeyDelta = "XXXXXX",
                Reverse = "XXXXXX",
                Ways = "XXXXXX",
                Ways2 = "XXXXXX",
                Ways3 = "XXXXXX",
            };

            var input = new Data.Models.Listxml.Input
            {
                Service = "XXXXXX",
                Tilt = "XXXXXX",
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
                Relation = "XXXXXX",
                Value = "XXXXXX",
            };

            var diplocation = new Data.Models.Listxml.DipLocation
            {
                Name = "XXXXXX",
                Number = "XXXXXX",
                Inverted = "XXXXXX",
            };

            var dipvalue = new Data.Models.Listxml.DipValue
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
                Default = "XXXXXX",
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
                Inverted = "XXXXXX",
            };

            var confsetting = new Data.Models.Listxml.ConfSetting
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
                Default = "XXXXXX",
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
                Default = "XXXXXX",
                Condition = condition,
            };

            var driver = new Data.Models.Listxml.Driver
            {
                Status = "XXXXXX",
                Color = "XXXXXX",
                Sound = "XXXXXX",
                PaletteSize = "XXXXXX",
                Emulation = "XXXXXX",
                Cocktail = "XXXXXX",
                SaveState = "XXXXXX",
                RequiresArtwork = "XXXXXX",
                Unofficial = "XXXXXX",
                NoSoundHardware = "XXXXXX",
                Incomplete = "XXXXXX",
            };

            var feature = new Data.Models.Listxml.Feature
            {
                Type = "XXXXXX",
                Status = "XXXXXX",
                Overall = "XXXXXX",
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
                Type = "XXXXXX",
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
                Default = "XXXXXX",
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
                Status = "XXXXXX",
                Filter = "XXXXXX",
            };

            var ramoption = new Data.Models.Listxml.RamOption
            {
                Name = "XXXXXX",
                Default = "XXXXXX",
                Content = "XXXXXX",
            };

            Data.Models.Listxml.GameBase gameBase = game
                ? new Data.Models.Listxml.Game()
                : new Data.Models.Listxml.Machine();
            gameBase.Name = "XXXXXX";
            gameBase.SourceFile = "XXXXXX";
            gameBase.IsBios = "XXXXXX";
            gameBase.IsDevice = "XXXXXX";
            gameBase.IsMechanical = "XXXXXX";
            gameBase.Runnable = "XXXXXX";
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

            return new Data.Models.Listxml.Mame
            {
                Build = "XXXXXX",
                Debug = "XXXXXX",
                MameConfig = "XXXXXX",
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
            Assert.Equal("XXXXXX", gb.IsBios);
            Assert.Equal("XXXXXX", gb.IsDevice);
            Assert.Equal("XXXXXX", gb.IsMechanical);
            Assert.Equal("XXXXXX", gb.Runnable);
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
            Assert.Equal("XXXXXX", biosset.Default);
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
            Assert.Equal("XXXXXX", rom.Status);
            Assert.Equal("XXXXXX", rom.Optional);
            Assert.Equal("XXXXXX", rom.Dispose);
            Assert.Equal("XXXXXX", rom.SoundOnly);
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
            Assert.Equal("XXXXXX", disk.Writable);
            Assert.Equal("XXXXXX", disk.Status);
            Assert.Equal("XXXXXX", disk.Optional);
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
            Assert.Equal("XXXXXX", chip.Type);
            Assert.Equal("XXXXXX", chip.SoundOnly);
            Assert.Equal("XXXXXX", chip.Clock);
        }

        /// <summary>
        /// Validate a Display
        /// </summary>
        private static void Validate(Data.Models.Listxml.Display? display)
        {
            Assert.NotNull(display);
            Assert.Equal("XXXXXX", display.Tag);
            Assert.Equal("XXXXXX", display.Type);
            Assert.Equal("XXXXXX", display.Rotate);
            Assert.Equal("XXXXXX", display.FlipX);
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
            Assert.Equal("XXXXXX", video.Screen);
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
            Assert.Equal("XXXXXX", input.Service);
            Assert.Equal("XXXXXX", input.Tilt);
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
            Assert.Equal("XXXXXX", control.Type);
            Assert.Equal("XXXXXX", control.Player);
            Assert.Equal("XXXXXX", control.Buttons);
            Assert.Equal("XXXXXX", control.ReqButtons);
            Assert.Equal("XXXXXX", control.Minimum);
            Assert.Equal("XXXXXX", control.Maximum);
            Assert.Equal("XXXXXX", control.Sensitivity);
            Assert.Equal("XXXXXX", control.KeyDelta);
            Assert.Equal("XXXXXX", control.Reverse);
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
            Assert.Equal("XXXXXX", condition.Relation);
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
            Assert.Equal("XXXXXX", diplocation.Inverted);
        }

        /// <summary>
        /// Validate a DipValue
        /// </summary>
        private static void Validate(Data.Models.Listxml.DipValue? dipvalue)
        {
            Assert.NotNull(dipvalue);
            Assert.Equal("XXXXXX", dipvalue.Name);
            Assert.Equal("XXXXXX", dipvalue.Value);
            Assert.Equal("XXXXXX", dipvalue.Default);
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
            Assert.Equal("XXXXXX", conflocation.Inverted);
        }

        /// <summary>
        /// Validate a ConfSetting
        /// </summary>
        private static void Validate(Data.Models.Listxml.ConfSetting? confsetting)
        {
            Assert.NotNull(confsetting);
            Assert.Equal("XXXXXX", confsetting.Name);
            Assert.Equal("XXXXXX", confsetting.Value);
            Assert.Equal("XXXXXX", confsetting.Default);
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
            Assert.Equal("XXXXXX", adjuster.Default);
            Validate(adjuster.Condition);
        }

        /// <summary>
        /// Validate a Driver
        /// </summary>
        private static void Validate(Data.Models.Listxml.Driver? driver)
        {
            Assert.NotNull(driver);
            Assert.Equal("XXXXXX", driver.Status);
            Assert.Equal("XXXXXX", driver.Color);
            Assert.Equal("XXXXXX", driver.Sound);
            Assert.Equal("XXXXXX", driver.PaletteSize);
            Assert.Equal("XXXXXX", driver.Emulation);
            Assert.Equal("XXXXXX", driver.Cocktail);
            Assert.Equal("XXXXXX", driver.SaveState);
            Assert.Equal("XXXXXX", driver.RequiresArtwork);
            Assert.Equal("XXXXXX", driver.Unofficial);
            Assert.Equal("XXXXXX", driver.NoSoundHardware);
            Assert.Equal("XXXXXX", driver.Incomplete);
        }

        /// <summary>
        /// Validate a Feature
        /// </summary>
        private static void Validate(Data.Models.Listxml.Feature? feature)
        {
            Assert.NotNull(feature);
            Assert.Equal("XXXXXX", feature.Type);
            Assert.Equal("XXXXXX", feature.Status);
            Assert.Equal("XXXXXX", feature.Overall);
        }

        /// <summary>
        /// Validate a Device
        /// </summary>
        private static void Validate(Data.Models.Listxml.Device? device)
        {
            Assert.NotNull(device);
            Assert.Equal("XXXXXX", device.Type);
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
            Assert.Equal("XXXXXX", slotoption.Default);
        }

        /// <summary>
        /// Validate a SoftwareList
        /// </summary>
        private static void Validate(Data.Models.Listxml.SoftwareList? softwarelist)
        {
            Assert.NotNull(softwarelist);
            Assert.Equal("XXXXXX", softwarelist.Tag);
            Assert.Equal("XXXXXX", softwarelist.Name);
            Assert.Equal("XXXXXX", softwarelist.Status);
            Assert.Equal("XXXXXX", softwarelist.Filter);
        }

        /// <summary>
        /// Validate a RamOption
        /// </summary>
        private static void Validate(Data.Models.Listxml.RamOption? ramoption)
        {
            Assert.NotNull(ramoption);
            Assert.Equal("XXXXXX", ramoption.Name);
            Assert.Equal("XXXXXX", ramoption.Default);
            Assert.Equal("XXXXXX", ramoption.Content);
        }
    }
}