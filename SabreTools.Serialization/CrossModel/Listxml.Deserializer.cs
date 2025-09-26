using System;
using SabreTools.Data.Models.Listxml;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listxml : ICrossModel<Mame, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Mame? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            var mame = header != null ? ConvertMameFromInternalModel(header) : new Mame();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
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
                Build = item.ReadString(Data.Models.Metadata.Header.BuildKey),
                Debug = item.ReadString(Data.Models.Metadata.Header.DebugKey),
                MameConfig = item.ReadString(Data.Models.Metadata.Header.MameConfigKey),
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
                Name = item.ReadString(Data.Models.Metadata.Machine.NameKey),
                SourceFile = item.ReadString(Data.Models.Metadata.Machine.SourceFileKey),
                IsBios = item.ReadString(Data.Models.Metadata.Machine.IsBiosKey),
                IsDevice = item.ReadString(Data.Models.Metadata.Machine.IsDeviceKey),
                IsMechanical = item.ReadString(Data.Models.Metadata.Machine.IsMechanicalKey),
                Runnable = item.ReadString(Data.Models.Metadata.Machine.RunnableKey),
                CloneOf = item.ReadString(Data.Models.Metadata.Machine.CloneOfKey),
                RomOf = item.ReadString(Data.Models.Metadata.Machine.RomOfKey),
                SampleOf = item.ReadString(Data.Models.Metadata.Machine.SampleOfKey),
                Description = item.ReadString(Data.Models.Metadata.Machine.DescriptionKey),
                Year = item.ReadString(Data.Models.Metadata.Machine.YearKey),
                Manufacturer = item.ReadString(Data.Models.Metadata.Machine.ManufacturerKey),
                History = item.ReadString(Data.Models.Metadata.Machine.HistoryKey),
            };

            var biosSets = item.Read<Data.Models.Metadata.BiosSet[]>(Data.Models.Metadata.Machine.BiosSetKey);
            if (biosSets != null && biosSets.Length > 0)
                machine.BiosSet = Array.ConvertAll(biosSets, ConvertFromInternalModel);

            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
                machine.Rom = Array.ConvertAll(roms, ConvertFromInternalModel);

            var disks = item.Read<Data.Models.Metadata.Disk[]>(Data.Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Length > 0)
                machine.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            var deviceRefs = item.Read<Data.Models.Metadata.DeviceRef[]>(Data.Models.Metadata.Machine.DeviceRefKey);
            if (deviceRefs != null && deviceRefs.Length > 0)
                machine.DeviceRef = Array.ConvertAll(deviceRefs, ConvertFromInternalModel);

            var samples = item.Read<Data.Models.Metadata.Sample[]>(Data.Models.Metadata.Machine.SampleKey);
            if (samples != null && samples.Length > 0)
                machine.Sample = Array.ConvertAll(samples, ConvertFromInternalModel);

            var chips = item.Read<Data.Models.Metadata.Chip[]>(Data.Models.Metadata.Machine.ChipKey);
            if (chips != null && chips.Length > 0)
                machine.Chip = Array.ConvertAll(chips, ConvertFromInternalModel);

            var displays = item.Read<Data.Models.Metadata.Display[]>(Data.Models.Metadata.Machine.DisplayKey);
            if (displays != null && displays.Length > 0)
                machine.Display = Array.ConvertAll(displays, ConvertFromInternalModel);

            var videos = item.Read<Data.Models.Metadata.Video[]>(Data.Models.Metadata.Machine.VideoKey);
            if (videos != null && videos.Length > 0)
                machine.Video = Array.ConvertAll(videos, ConvertFromInternalModel);

            var sound = item.Read<Data.Models.Metadata.Sound>(Data.Models.Metadata.Machine.SoundKey);
            if (sound != null)
                machine.Sound = ConvertFromInternalModel(sound);

            var input = item.Read<Data.Models.Metadata.Input>(Data.Models.Metadata.Machine.InputKey);
            if (input != null)
                machine.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.Read<Data.Models.Metadata.DipSwitch[]>(Data.Models.Metadata.Machine.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Length > 0)
                machine.DipSwitch = Array.ConvertAll(dipSwitches, ConvertFromInternalModel);

            var configurations = item.Read<Data.Models.Metadata.Configuration[]>(Data.Models.Metadata.Machine.ConfigurationKey);
            if (configurations != null && configurations.Length > 0)
                machine.Configuration = Array.ConvertAll(configurations, ConvertFromInternalModel);

            var ports = item.Read<Data.Models.Metadata.Port[]>(Data.Models.Metadata.Machine.PortKey);
            if (ports != null && ports.Length > 0)
                machine.Port = Array.ConvertAll(ports, ConvertFromInternalModel);

            var adjusters = item.Read<Data.Models.Metadata.Adjuster[]>(Data.Models.Metadata.Machine.AdjusterKey);
            if (adjusters != null && adjusters.Length > 0)
                machine.Adjuster = Array.ConvertAll(adjusters, ConvertFromInternalModel);

            var driver = item.Read<Data.Models.Metadata.Driver>(Data.Models.Metadata.Machine.DriverKey);
            if (driver != null)
                machine.Driver = ConvertFromInternalModel(driver);

            var features = item.Read<Data.Models.Metadata.Feature[]>(Data.Models.Metadata.Machine.FeatureKey);
            if (features != null && features.Length > 0)
                machine.Feature = Array.ConvertAll(features, ConvertFromInternalModel);

            var devices = item.Read<Data.Models.Metadata.Device[]>(Data.Models.Metadata.Machine.DeviceKey);
            if (devices != null && devices.Length > 0)
                machine.Device = Array.ConvertAll(devices, ConvertFromInternalModel);

            var slots = item.Read<Data.Models.Metadata.Slot[]>(Data.Models.Metadata.Machine.SlotKey);
            if (slots != null && slots.Length > 0)
                machine.Slot = Array.ConvertAll(slots, ConvertFromInternalModel);

            var softwareLists = item.Read<Data.Models.Metadata.SoftwareList[]>(Data.Models.Metadata.Machine.SoftwareListKey);
            if (softwareLists != null && softwareLists.Length > 0)
                machine.SoftwareList = Array.ConvertAll(softwareLists, ConvertFromInternalModel);

            var ramOptions = item.Read<Data.Models.Metadata.RamOption[]>(Data.Models.Metadata.Machine.RamOptionKey);
            if (ramOptions != null && ramOptions.Length > 0)
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
                Name = item.ReadString(Data.Models.Metadata.Adjuster.NameKey),
                Default = item.ReadString(Data.Models.Metadata.Adjuster.DefaultKey),
            };

            var condition = item.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.Adjuster.ConditionKey);
            if (condition != null)
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
                Mask = item.ReadString(Data.Models.Metadata.Analog.MaskKey),
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
                Name = item.ReadString(Data.Models.Metadata.BiosSet.NameKey),
                Description = item.ReadString(Data.Models.Metadata.BiosSet.DescriptionKey),
                Default = item.ReadString(Data.Models.Metadata.BiosSet.DefaultKey),
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
                Name = item.ReadString(Data.Models.Metadata.Chip.NameKey),
                Tag = item.ReadString(Data.Models.Metadata.Chip.TagKey),
                Type = item.ReadString(Data.Models.Metadata.Chip.ChipTypeKey),
                SoundOnly = item.ReadString(Data.Models.Metadata.Chip.SoundOnlyKey),
                Clock = item.ReadString(Data.Models.Metadata.Chip.ClockKey),
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
                Tag = item.ReadString(Data.Models.Metadata.Condition.TagKey),
                Mask = item.ReadString(Data.Models.Metadata.Condition.MaskKey),
                Relation = item.ReadString(Data.Models.Metadata.Condition.RelationKey),
                Value = item.ReadString(Data.Models.Metadata.Condition.ValueKey),
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
                Name = item.ReadString(Data.Models.Metadata.Configuration.NameKey),
                Tag = item.ReadString(Data.Models.Metadata.Configuration.TagKey),
                Mask = item.ReadString(Data.Models.Metadata.Configuration.MaskKey),
            };

            var condition = item.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.Configuration.ConditionKey);
            if (condition != null)
                configuration.Condition = ConvertFromInternalModel(condition);

            var confLocations = item.Read<Data.Models.Metadata.ConfLocation[]>(Data.Models.Metadata.Configuration.ConfLocationKey);
            if (confLocations != null && confLocations.Length > 0)
                configuration.ConfLocation = Array.ConvertAll(confLocations, ConvertFromInternalModel);

            var confSettings = item.Read<Data.Models.Metadata.ConfSetting[]>(Data.Models.Metadata.Configuration.ConfSettingKey);
            if (confSettings != null && confSettings.Length > 0)
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
                Name = item.ReadString(Data.Models.Metadata.ConfLocation.NameKey),
                Number = item.ReadString(Data.Models.Metadata.ConfLocation.NumberKey),
                Inverted = item.ReadString(Data.Models.Metadata.ConfLocation.InvertedKey),
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
                Name = item.ReadString(Data.Models.Metadata.ConfSetting.NameKey),
                Value = item.ReadString(Data.Models.Metadata.ConfSetting.ValueKey),
                Default = item.ReadString(Data.Models.Metadata.ConfSetting.DefaultKey),
            };

            var condition = item.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.ConfSetting.ConditionKey);
            if (condition != null)
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
                Type = item.ReadString(Data.Models.Metadata.Control.ControlTypeKey),
                Player = item.ReadString(Data.Models.Metadata.Control.PlayerKey),
                Buttons = item.ReadString(Data.Models.Metadata.Control.ButtonsKey),
                ReqButtons = item.ReadString(Data.Models.Metadata.Control.ReqButtonsKey),
                Minimum = item.ReadString(Data.Models.Metadata.Control.MinimumKey),
                Maximum = item.ReadString(Data.Models.Metadata.Control.MaximumKey),
                Sensitivity = item.ReadString(Data.Models.Metadata.Control.SensitivityKey),
                KeyDelta = item.ReadString(Data.Models.Metadata.Control.KeyDeltaKey),
                Reverse = item.ReadString(Data.Models.Metadata.Control.ReverseKey),
                Ways = item.ReadString(Data.Models.Metadata.Control.WaysKey),
                Ways2 = item.ReadString(Data.Models.Metadata.Control.Ways2Key),
                Ways3 = item.ReadString(Data.Models.Metadata.Control.Ways3Key),
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
                Type = item.ReadString(Data.Models.Metadata.Device.DeviceTypeKey),
                Tag = item.ReadString(Data.Models.Metadata.Device.TagKey),
                FixedImage = item.ReadString(Data.Models.Metadata.Device.FixedImageKey),
                Mandatory = item.ReadString(Data.Models.Metadata.Device.MandatoryKey),
                Interface = item.ReadString(Data.Models.Metadata.Device.InterfaceKey),
            };

            var instance = item.Read<Data.Models.Metadata.Instance>(Data.Models.Metadata.Device.InstanceKey);
            if (instance != null)
                device.Instance = ConvertFromInternalModel(instance);

            var extensions = item.Read<Data.Models.Metadata.Extension[]>(Data.Models.Metadata.Device.ExtensionKey);
            if (extensions != null && extensions.Length > 0)
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
                Name = item.ReadString(Data.Models.Metadata.DeviceRef.NameKey),
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
                Name = item.ReadString(Data.Models.Metadata.DipLocation.NameKey),
                Number = item.ReadString(Data.Models.Metadata.DipLocation.NumberKey),
                Inverted = item.ReadString(Data.Models.Metadata.DipLocation.InvertedKey),
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
                Name = item.ReadString(Data.Models.Metadata.DipSwitch.NameKey),
                Tag = item.ReadString(Data.Models.Metadata.DipSwitch.TagKey),
                Mask = item.ReadString(Data.Models.Metadata.DipSwitch.MaskKey),
            };

            var condition = item.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.DipSwitch.ConditionKey);
            if (condition != null)
                dipSwitch.Condition = ConvertFromInternalModel(condition);

            var dipLocations = item.Read<Data.Models.Metadata.DipLocation[]>(Data.Models.Metadata.DipSwitch.DipLocationKey);
            if (dipLocations != null && dipLocations.Length > 0)
                dipSwitch.DipLocation = Array.ConvertAll(dipLocations, ConvertFromInternalModel);

            var dipValues = item.Read<Data.Models.Metadata.DipValue[]>(Data.Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues != null && dipValues.Length > 0)
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
                Name = item.ReadString(Data.Models.Metadata.DipValue.NameKey),
                Value = item.ReadString(Data.Models.Metadata.DipValue.ValueKey),
                Default = item.ReadString(Data.Models.Metadata.DipValue.DefaultKey),
            };

            var condition = item.Read<Data.Models.Metadata.Condition>(Data.Models.Metadata.DipValue.ConditionKey);
            if (condition != null)
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
                Name = item.ReadString(Data.Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Data.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Disk.SHA1Key),
                Merge = item.ReadString(Data.Models.Metadata.Disk.MergeKey),
                Region = item.ReadString(Data.Models.Metadata.Disk.RegionKey),
                Index = item.ReadString(Data.Models.Metadata.Disk.IndexKey),
                Writable = item.ReadString(Data.Models.Metadata.Disk.WritableKey),
                Status = item.ReadString(Data.Models.Metadata.Disk.StatusKey),
                Optional = item.ReadString(Data.Models.Metadata.Disk.OptionalKey),
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
                Tag = item.ReadString(Data.Models.Metadata.Display.TagKey),
                Type = item.ReadString(Data.Models.Metadata.Display.DisplayTypeKey),
                Rotate = item.ReadString(Data.Models.Metadata.Display.RotateKey),
                FlipX = item.ReadString(Data.Models.Metadata.Display.FlipXKey),
                Width = item.ReadString(Data.Models.Metadata.Display.WidthKey),
                Height = item.ReadString(Data.Models.Metadata.Display.HeightKey),
                Refresh = item.ReadString(Data.Models.Metadata.Display.RefreshKey),
                PixClock = item.ReadString(Data.Models.Metadata.Display.PixClockKey),
                HTotal = item.ReadString(Data.Models.Metadata.Display.HTotalKey),
                HBEnd = item.ReadString(Data.Models.Metadata.Display.HBEndKey),
                HBStart = item.ReadString(Data.Models.Metadata.Display.HBStartKey),
                VTotal = item.ReadString(Data.Models.Metadata.Display.VTotalKey),
                VBEnd = item.ReadString(Data.Models.Metadata.Display.VBEndKey),
                VBStart = item.ReadString(Data.Models.Metadata.Display.VBStartKey),
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
                Status = item.ReadString(Data.Models.Metadata.Driver.StatusKey),
                Color = item.ReadString(Data.Models.Metadata.Driver.ColorKey),
                Sound = item.ReadString(Data.Models.Metadata.Driver.SoundKey),
                PaletteSize = item.ReadString(Data.Models.Metadata.Driver.PaletteSizeKey),
                Emulation = item.ReadString(Data.Models.Metadata.Driver.EmulationKey),
                Cocktail = item.ReadString(Data.Models.Metadata.Driver.CocktailKey),
                SaveState = item.ReadString(Data.Models.Metadata.Driver.SaveStateKey),
                RequiresArtwork = item.ReadString(Data.Models.Metadata.Driver.RequiresArtworkKey),
                Unofficial = item.ReadString(Data.Models.Metadata.Driver.UnofficialKey),
                NoSoundHardware = item.ReadString(Data.Models.Metadata.Driver.NoSoundHardwareKey),
                Incomplete = item.ReadString(Data.Models.Metadata.Driver.IncompleteKey),
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
                Name = item.ReadString(Data.Models.Metadata.Extension.NameKey),
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
                Type = item.ReadString(Data.Models.Metadata.Feature.FeatureTypeKey),
                Status = item.ReadString(Data.Models.Metadata.Feature.StatusKey),
                Overall = item.ReadString(Data.Models.Metadata.Feature.OverallKey),
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
                Service = item.ReadString(Data.Models.Metadata.Input.ServiceKey),
                Tilt = item.ReadString(Data.Models.Metadata.Input.TiltKey),
                Players = item.ReadString(Data.Models.Metadata.Input.PlayersKey),
                Buttons = item.ReadString(Data.Models.Metadata.Input.ButtonsKey),
                Coins = item.ReadString(Data.Models.Metadata.Input.CoinsKey),
            };

            var controlAttr = item.ReadString(Data.Models.Metadata.Input.ControlKey);
            if (controlAttr != null)
                input.ControlAttr = controlAttr;

            var controls = item.Read<Data.Models.Metadata.Control[]>(Data.Models.Metadata.Input.ControlKey);
            if (controls != null && controls.Length > 0)
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
                Name = item.ReadString(Data.Models.Metadata.Instance.NameKey),
                BriefName = item.ReadString(Data.Models.Metadata.Instance.BriefNameKey),
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
                Tag = item.ReadString(Data.Models.Metadata.Port.TagKey),
            };

            var analogs = item.Read<Data.Models.Metadata.Analog[]>(Data.Models.Metadata.Port.AnalogKey);
            if (analogs != null && analogs.Length > 0)
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
                Name = item.ReadString(Data.Models.Metadata.RamOption.NameKey),
                Default = item.ReadString(Data.Models.Metadata.RamOption.DefaultKey),
                Content = item.ReadString(Data.Models.Metadata.RamOption.ContentKey),
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
                Name = item.ReadString(Data.Models.Metadata.Rom.NameKey),
                Bios = item.ReadString(Data.Models.Metadata.Rom.BiosKey),
                Size = item.ReadString(Data.Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                Merge = item.ReadString(Data.Models.Metadata.Rom.MergeKey),
                Region = item.ReadString(Data.Models.Metadata.Rom.RegionKey),
                Offset = item.ReadString(Data.Models.Metadata.Rom.OffsetKey),
                Status = item.ReadString(Data.Models.Metadata.Rom.StatusKey),
                Optional = item.ReadString(Data.Models.Metadata.Rom.OptionalKey),
                Dispose = item.ReadString(Data.Models.Metadata.Rom.DisposeKey),
                SoundOnly = item.ReadString(Data.Models.Metadata.Rom.SoundOnlyKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Sample"/> to <see cref="Sample"/>
        /// </summary>
        private static Sample ConvertFromInternalModel(Data.Models.Metadata.Sample item)
        {
            var sample = new Sample
            {
                Name = item.ReadString(Data.Models.Metadata.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Slot"/> to <see cref="Slot"/>
        /// </summary>
        private static Slot ConvertFromInternalModel(Data.Models.Metadata.Slot item)
        {
            var slot = new Slot
            {
                Name = item.ReadString(Data.Models.Metadata.Slot.NameKey),
            };

            var slotOptions = item.Read<Data.Models.Metadata.SlotOption[]>(Data.Models.Metadata.Slot.SlotOptionKey);
            if (slotOptions != null && slotOptions.Length > 0)
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
                Name = item.ReadString(Data.Models.Metadata.SlotOption.NameKey),
                DevName = item.ReadString(Data.Models.Metadata.SlotOption.DevNameKey),
                Default = item.ReadString(Data.Models.Metadata.SlotOption.DefaultKey),
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
                Tag = item.ReadString(Data.Models.Metadata.SoftwareList.TagKey),
                Name = item.ReadString(Data.Models.Metadata.SoftwareList.NameKey),
                Status = item.ReadString(Data.Models.Metadata.SoftwareList.StatusKey),
                Filter = item.ReadString(Data.Models.Metadata.SoftwareList.FilterKey),
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
                Channels = item.ReadString(Data.Models.Metadata.Sound.ChannelsKey),
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
                Screen = item.ReadString(Data.Models.Metadata.Video.ScreenKey),
                Orientation = item.ReadString(Data.Models.Metadata.Video.OrientationKey),
                Width = item.ReadString(Data.Models.Metadata.Video.WidthKey),
                Height = item.ReadString(Data.Models.Metadata.Video.HeightKey),
                AspectX = item.ReadString(Data.Models.Metadata.Video.AspectXKey),
                AspectY = item.ReadString(Data.Models.Metadata.Video.AspectYKey),
                Refresh = item.ReadString(Data.Models.Metadata.Video.RefreshKey),
            };
            return video;
        }
    }
}
