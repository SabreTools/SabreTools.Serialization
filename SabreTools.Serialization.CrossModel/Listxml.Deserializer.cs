using System;
using SabreTools.Data.Models.Listxml;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listxml : BaseMetadataSerializer<Mame>
    {
        /// <inheritdoc/>
        public override Mame? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var header = obj.Header;
            var mame = header is not null ? ConvertMameFromInternalModel(header) : new Mame();

            var machines = obj.Machine;
            if (machines is not null && machines.Length > 0)
                mame.Game = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return mame;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Mame"/>
        /// </summary>
        private static Mame ConvertMameFromInternalModel(Data.Models.Metadata.Header item)
        {
            var mame = new Mame
            {
                Build = item.Build,
                Debug = item.Debug,
                MameConfig = item.MameConfig,
            };

            return mame;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="GameBase"/>
        /// </summary>
        internal static GameBase ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var machine = new Machine
            {
                Name = item.Name,
                SourceFile = item.SourceFile,
                IsBios = item.IsBios,
                IsDevice = item.IsDevice,
                IsMechanical = item.IsMechanical,
                Runnable = item.Runnable,
                CloneOf = item.CloneOf,
                RomOf = item.RomOf,
                SampleOf = item.SampleOf,
                Description = item.Description,
                Year = item.Year,
                Manufacturer = item.Manufacturer,
                History = item.History,
            };

            var biosSets = item.BiosSet;
            if (biosSets is not null && biosSets.Length > 0)
                machine.BiosSet = Array.ConvertAll(biosSets, ConvertFromInternalModel);

            var roms = item.Rom;
            if (roms is not null && roms.Length > 0)
                machine.Rom = Array.ConvertAll(roms, ConvertFromInternalModel);

            var disks = item.Disk;
            if (disks is not null && disks.Length > 0)
                machine.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            var deviceRefs = item.DeviceRef;
            if (deviceRefs is not null && deviceRefs.Length > 0)
                machine.DeviceRef = Array.ConvertAll(deviceRefs, ConvertFromInternalModel);

            var samples = item.Sample;
            if (samples is not null && samples.Length > 0)
                machine.Sample = Array.ConvertAll(samples, ConvertFromInternalModel);

            var chips = item.Chip;
            if (chips is not null && chips.Length > 0)
                machine.Chip = Array.ConvertAll(chips, ConvertFromInternalModel);

            var displays = item.Display;
            if (displays is not null && displays.Length > 0)
                machine.Display = Array.ConvertAll(displays, ConvertFromInternalModel);

            var videos = item.Video;
            if (videos is not null && videos.Length > 0)
                machine.Video = Array.ConvertAll(videos, ConvertFromInternalModel);

            var sound = item.Sound;
            if (sound is not null)
                machine.Sound = ConvertFromInternalModel(sound);

            var input = item.Input;
            if (input is not null)
                machine.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.DipSwitch;
            if (dipSwitches is not null && dipSwitches.Length > 0)
                machine.DipSwitch = Array.ConvertAll(dipSwitches, ConvertFromInternalModel);

            var configurations = item.Configuration;
            if (configurations is not null && configurations.Length > 0)
                machine.Configuration = Array.ConvertAll(configurations, ConvertFromInternalModel);

            var ports = item.Port;
            if (ports is not null && ports.Length > 0)
                machine.Port = Array.ConvertAll(ports, ConvertFromInternalModel);

            var adjusters = item.Adjuster;
            if (adjusters is not null && adjusters.Length > 0)
                machine.Adjuster = Array.ConvertAll(adjusters, ConvertFromInternalModel);

            var driver = item.Driver;
            if (driver is not null)
                machine.Driver = ConvertFromInternalModel(driver);

            var features = item.Feature;
            if (features is not null && features.Length > 0)
                machine.Feature = Array.ConvertAll(features, ConvertFromInternalModel);

            var devices = item.Device;
            if (devices is not null && devices.Length > 0)
                machine.Device = Array.ConvertAll(devices, ConvertFromInternalModel);

            var slots = item.Slot;
            if (slots is not null && slots.Length > 0)
                machine.Slot = Array.ConvertAll(slots, ConvertFromInternalModel);

            var softwareLists = item.SoftwareList;
            if (softwareLists is not null && softwareLists.Length > 0)
                machine.SoftwareList = Array.ConvertAll(softwareLists, ConvertFromInternalModel);

            var ramOptions = item.RamOption;
            if (ramOptions is not null && ramOptions.Length > 0)
                machine.RamOption = Array.ConvertAll(ramOptions, ConvertFromInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Adjuster"/> to <see cref="Adjuster"/>
        /// </summary>
        private static Adjuster ConvertFromInternalModel(Data.Models.Metadata.Adjuster item)
        {
            var adjuster = new Adjuster
            {
                Name = item.Name,
                Default = item.Default,
            };

            var condition = item.Condition;
            if (condition is not null)
                adjuster.Condition = ConvertFromInternalModel(condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Analog"/> to <see cref="Analog"/>
        /// </summary>
        private static Analog ConvertFromInternalModel(Data.Models.Metadata.Analog item)
        {
            var analog = new Analog
            {
                Mask = item.Mask,
            };
            return analog;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.BiosSet"/> to <see cref="BiosSet"/>
        /// </summary>
        private static BiosSet ConvertFromInternalModel(Data.Models.Metadata.BiosSet item)
        {
            var biosset = new BiosSet
            {
                Name = item.Name,
                Description = item.Description,
                Default = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Chip"/> to <see cref="Chip"/>
        /// </summary>
        private static Chip ConvertFromInternalModel(Data.Models.Metadata.Chip item)
        {
            var chip = new Chip
            {
                Name = item.Name,
                Tag = item.Tag,
                Type = item.ChipType,
                SoundOnly = item.SoundOnly,
                Clock = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Condition"/> to <see cref="Condition"/>
        /// </summary>
        private static Condition ConvertFromInternalModel(Data.Models.Metadata.Condition item)
        {
            var condition = new Condition
            {
                Tag = item.Tag,
                Mask = item.Mask,
                Relation = item.Relation,
                Value = item.Value,
            };
            return condition;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Configuration"/> to <see cref="Configuration"/>
        /// </summary>
        private static Configuration ConvertFromInternalModel(Data.Models.Metadata.Configuration item)
        {
            var configuration = new Configuration
            {
                Name = item.Name,
                Tag = item.Tag,
                Mask = item.Mask,
            };

            var condition = item.Condition;
            if (condition is not null)
                configuration.Condition = ConvertFromInternalModel(condition);

            var confLocations = item.ConfLocation;
            if (confLocations is not null && confLocations.Length > 0)
                configuration.ConfLocation = Array.ConvertAll(confLocations, ConvertFromInternalModel);

            var confSettings = item.ConfSetting;
            if (confSettings is not null && confSettings.Length > 0)
                configuration.ConfSetting = Array.ConvertAll(confSettings, ConvertFromInternalModel);

            return configuration;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.ConfLocation"/> to <see cref="ConfLocation"/>
        /// </summary>
        private static ConfLocation ConvertFromInternalModel(Data.Models.Metadata.ConfLocation item)
        {
            var confLocation = new ConfLocation
            {
                Name = item.Name,
                Number = item.Number,
                Inverted = item.Inverted,
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.ConfSetting"/> to <see cref="ConfSetting"/>
        /// </summary>
        private static ConfSetting ConvertFromInternalModel(Data.Models.Metadata.ConfSetting item)
        {
            var confSetting = new ConfSetting
            {
                Name = item.Name,
                Value = item.Value,
                Default = item.Default,
            };

            var condition = item.Condition;
            if (condition is not null)
                confSetting.Condition = ConvertFromInternalModel(condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Control"/> to <see cref="Control"/>
        /// </summary>
        private static Control ConvertFromInternalModel(Data.Models.Metadata.Control item)
        {
            var control = new Control
            {
                Type = item.ControlType,
                Player = item.Player,
                Buttons = item.Buttons,
                ReqButtons = item.ReqButtons,
                Minimum = item.Minimum,
                Maximum = item.Maximum,
                Sensitivity = item.Sensitivity,
                KeyDelta = item.KeyDelta,
                Reverse = item.Reverse,
                Ways = item.Ways,
                Ways2 = item.Ways2,
                Ways3 = item.Ways3,
            };
            return control;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Device"/> to <see cref="Device"/>
        /// </summary>
        private static Device ConvertFromInternalModel(Data.Models.Metadata.Device item)
        {
            var device = new Device
            {
                Type = item.DeviceType,
                Tag = item.Tag,
                FixedImage = item.FixedImage,
                Mandatory = item.Mandatory,
                Interface = item.Interface,
            };

            var instance = item.Instance;
            if (instance is not null)
                device.Instance = ConvertFromInternalModel(instance);

            var extensions = item.Extension;
            if (extensions is not null && extensions.Length > 0)
                device.Extension = Array.ConvertAll(extensions, ConvertFromInternalModel);

            return device;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DeviceRef"/> to <see cref="DeviceRef"/>
        /// </summary>
        private static DeviceRef ConvertFromInternalModel(Data.Models.Metadata.DeviceRef item)
        {
            var deviceRef = new DeviceRef
            {
                Name = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DipLocation"/> to <see cref="DipLocation"/>
        /// </summary>
        private static DipLocation ConvertFromInternalModel(Data.Models.Metadata.DipLocation item)
        {
            var dipLocation = new DipLocation
            {
                Name = item.Name,
                Number = item.Number,
                Inverted = item.Inverted,
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DipSwitch"/> to <see cref="DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Data.Models.Metadata.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                Name = item.Name,
                Tag = item.Tag,
                Mask = item.Mask,
            };

            var condition = item.Condition;
            if (condition is not null)
                dipSwitch.Condition = ConvertFromInternalModel(condition);

            var dipLocations = item.DipLocation;
            if (dipLocations is not null && dipLocations.Length > 0)
                dipSwitch.DipLocation = Array.ConvertAll(dipLocations, ConvertFromInternalModel);

            var dipValues = item.DipValue;
            if (dipValues is not null && dipValues.Length > 0)
                dipSwitch.DipValue = Array.ConvertAll(dipValues, ConvertFromInternalModel);

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DipValue"/> to <see cref="DipValue"/>
        /// </summary>
        private static DipValue ConvertFromInternalModel(Data.Models.Metadata.DipValue item)
        {
            var dipValue = new DipValue
            {
                Name = item.Name,
                Value = item.Value,
                Default = item.Default,
            };

            var condition = item.Condition;
            if (condition is not null)
                dipValue.Condition = ConvertFromInternalModel(condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Disk"/> to <see cref="Disk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Data.Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.Name,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                Merge = item.Merge,
                Region = item.Region,
                Index = item.Index,
                Writable = item.Writable,
                Status = item.Status,
                Optional = item.Optional,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Display"/> to <see cref="Display"/>
        /// </summary>
        private static Display ConvertFromInternalModel(Data.Models.Metadata.Display item)
        {
            var display = new Display
            {
                Tag = item.Tag,
                Type = item.DisplayType,
                Rotate = item.Rotate,
                FlipX = item.FlipX,
                Width = item.Width,
                Height = item.Height,
                Refresh = item.Refresh,
                PixClock = item.PixClock,
                HTotal = item.HTotal,
                HBEnd = item.HBEnd,
                HBStart = item.HBStart,
                VTotal = item.VTotal,
                VBEnd = item.VBEnd,
                VBStart = item.VBStart,
            };
            return display;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Driver"/> to <see cref="Driver"/>
        /// </summary>
        private static Driver ConvertFromInternalModel(Data.Models.Metadata.Driver item)
        {
            var driver = new Driver
            {
                Status = item.Status,
                Color = item.Color,
                Sound = item.Sound,
                PaletteSize = item.PaletteSize,
                Emulation = item.Emulation,
                Cocktail = item.Cocktail,
                SaveState = item.SaveState,
                RequiresArtwork = item.RequiresArtwork,
                Unofficial = item.Unofficial,
                NoSoundHardware = item.NoSoundHardware,
                Incomplete = item.Incomplete,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Extension"/> to <see cref="Extension"/>
        /// </summary>
        private static Extension ConvertFromInternalModel(Data.Models.Metadata.Extension item)
        {
            var extension = new Extension
            {
                Name = item.Name,
            };
            return extension;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Feature"/> to <see cref="Feature"/>
        /// </summary>
        private static Feature ConvertFromInternalModel(Data.Models.Metadata.Feature item)
        {
            var feature = new Feature
            {
                Type = item.FeatureType,
                Status = item.Status,
                Overall = item.Overall,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Input"/> to <see cref="Input"/>
        /// </summary>
        private static Input ConvertFromInternalModel(Data.Models.Metadata.Input item)
        {
            var input = new Input
            {
                Service = item.Service,
                Tilt = item.Tilt,
                Players = item.Players,
                ControlAttr = item.ControlAttr,
                Buttons = item.Buttons,
                Coins = item.Coins,
            };

            var controls = item.Control;
            if (controls is not null && controls.Length > 0)
                input.Control = Array.ConvertAll(controls, ConvertFromInternalModel);

            return input;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Instance"/> to <see cref="Instance"/>
        /// </summary>
        private static Instance ConvertFromInternalModel(Data.Models.Metadata.Instance item)
        {
            var instance = new Instance
            {
                Name = item.Name,
                BriefName = item.BriefName,
            };
            return instance;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Port"/> to <see cref="Port"/>
        /// </summary>
        private static Port ConvertFromInternalModel(Data.Models.Metadata.Port item)
        {
            var port = new Port
            {
                Tag = item.Tag,
            };

            var analogs = item.Analog;
            if (analogs is not null && analogs.Length > 0)
                port.Analog = Array.ConvertAll(analogs, ConvertFromInternalModel);

            return port;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.RamOption"/> to <see cref="RamOption"/>
        /// </summary>
        private static RamOption ConvertFromInternalModel(Data.Models.Metadata.RamOption item)
        {
            var ramOption = new RamOption
            {
                Name = item.Name,
                Default = item.Default,
                Content = item.Content,
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.Name,
                Bios = item.ReadString(Data.Models.Metadata.Rom.BiosKey),
                Size = item.Size,
                CRC = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                Merge = item.ReadString(Data.Models.Metadata.Rom.MergeKey),
                Region = item.ReadString(Data.Models.Metadata.Rom.RegionKey),
                Offset = item.ReadString(Data.Models.Metadata.Rom.OffsetKey),
                Status = item.Status,
                Optional = item.Optional,
                Dispose = item.Dispose,
                SoundOnly = item.SoundOnly,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Sample"/> to <see cref="Sample"/>
        /// </summary>
        private static Sample ConvertFromInternalModel(Data.Models.Metadata.Sample item)
        {
            var sample = new Sample { Name = item.Name, };
            return sample;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Slot"/> to <see cref="Slot"/>
        /// </summary>
        private static Slot ConvertFromInternalModel(Data.Models.Metadata.Slot item)
        {
            var slot = new Slot
            {
                Name = item.Name,
            };

            var slotOptions = item.SlotOption;
            if (slotOptions is not null && slotOptions.Length > 0)
                slot.SlotOption = Array.ConvertAll(slotOptions, ConvertFromInternalModel);

            return slot;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.SlotOption"/> to <see cref="SlotOption"/>
        /// </summary>
        private static SlotOption ConvertFromInternalModel(Data.Models.Metadata.SlotOption item)
        {
            var slotOption = new SlotOption
            {
                Name = item.Name,
                DevName = item.DevName,
                Default = item.Default,
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.SoftwareList"/> to <see cref="Models.Listxml.SoftwareList"/>
        /// </summary>
        private static Data.Models.Listxml.SoftwareList ConvertFromInternalModel(Data.Models.Metadata.SoftwareList item)
        {
            var softwareList = new Data.Models.Listxml.SoftwareList
            {
                Tag = item.Tag,
                Name = item.Name,
                Status = item.Status,
                Filter = item.Filter,
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Sound"/> to <see cref="Sound"/>
        /// </summary>
        private static Sound ConvertFromInternalModel(Data.Models.Metadata.Sound item)
        {
            var sound = new Sound
            {
                Channels = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Video"/> to <see cref="Video"/>
        /// </summary>
        private static Video ConvertFromInternalModel(Data.Models.Metadata.Video item)
        {
            var video = new Video
            {
                Screen = item.Screen,
                Orientation = item.Orientation,
                Width = item.Width,
                Height = item.Height,
                AspectX = item.AspectX,
                AspectY = item.AspectY,
                Refresh = item.Refresh,
            };
            return video;
        }
    }
}
