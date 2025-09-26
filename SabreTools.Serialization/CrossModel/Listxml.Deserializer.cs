using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.Listxml;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listxml : IModelSerializer<Mame, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Mame? Deserialize(Serialization.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Serialization.Models.Metadata.Header>(Serialization.Models.Metadata.MetadataFile.HeaderKey);
            var mame = header != null ? ConvertMameFromInternalModel(header) : new Mame();

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                mame.Game = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return mame;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Header"/> to <see cref="Mame"/>
        /// </summary>
        private static Mame ConvertMameFromInternalModel(Serialization.Models.Metadata.Header item)
        {
            var mame = new Mame
            {
                Build = item.ReadString(Serialization.Models.Metadata.Header.BuildKey),
                Debug = item.ReadString(Serialization.Models.Metadata.Header.DebugKey),
                MameConfig = item.ReadString(Serialization.Models.Metadata.Header.MameConfigKey),
            };

            return mame;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Machine"/> to <see cref="GameBase"/>
        /// </summary>
        internal static GameBase ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item)
        {
            var machine = new Machine
            {
                Name = item.ReadString(Serialization.Models.Metadata.Machine.NameKey),
                SourceFile = item.ReadString(Serialization.Models.Metadata.Machine.SourceFileKey),
                IsBios = item.ReadString(Serialization.Models.Metadata.Machine.IsBiosKey),
                IsDevice = item.ReadString(Serialization.Models.Metadata.Machine.IsDeviceKey),
                IsMechanical = item.ReadString(Serialization.Models.Metadata.Machine.IsMechanicalKey),
                Runnable = item.ReadString(Serialization.Models.Metadata.Machine.RunnableKey),
                CloneOf = item.ReadString(Serialization.Models.Metadata.Machine.CloneOfKey),
                RomOf = item.ReadString(Serialization.Models.Metadata.Machine.RomOfKey),
                SampleOf = item.ReadString(Serialization.Models.Metadata.Machine.SampleOfKey),
                Description = item.ReadString(Serialization.Models.Metadata.Machine.DescriptionKey),
                Year = item.ReadString(Serialization.Models.Metadata.Machine.YearKey),
                Manufacturer = item.ReadString(Serialization.Models.Metadata.Machine.ManufacturerKey),
                History = item.ReadString(Serialization.Models.Metadata.Machine.HistoryKey),
            };

            var biosSets = item.Read<Serialization.Models.Metadata.BiosSet[]>(Serialization.Models.Metadata.Machine.BiosSetKey);
            if (biosSets != null && biosSets.Length > 0)
                machine.BiosSet = Array.ConvertAll(biosSets, ConvertFromInternalModel);

            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
                machine.Rom = Array.ConvertAll(roms, ConvertFromInternalModel);

            var disks = item.Read<Serialization.Models.Metadata.Disk[]>(Serialization.Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Length > 0)
                machine.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            var deviceRefs = item.Read<Serialization.Models.Metadata.DeviceRef[]>(Serialization.Models.Metadata.Machine.DeviceRefKey);
            if (deviceRefs != null && deviceRefs.Length > 0)
                machine.DeviceRef = Array.ConvertAll(deviceRefs, ConvertFromInternalModel);

            var samples = item.Read<Serialization.Models.Metadata.Sample[]>(Serialization.Models.Metadata.Machine.SampleKey);
            if (samples != null && samples.Length > 0)
                machine.Sample = Array.ConvertAll(samples, ConvertFromInternalModel);

            var chips = item.Read<Serialization.Models.Metadata.Chip[]>(Serialization.Models.Metadata.Machine.ChipKey);
            if (chips != null && chips.Length > 0)
                machine.Chip = Array.ConvertAll(chips, ConvertFromInternalModel);

            var displays = item.Read<Serialization.Models.Metadata.Display[]>(Serialization.Models.Metadata.Machine.DisplayKey);
            if (displays != null && displays.Length > 0)
                machine.Display = Array.ConvertAll(displays, ConvertFromInternalModel);

            var videos = item.Read<Serialization.Models.Metadata.Video[]>(Serialization.Models.Metadata.Machine.VideoKey);
            if (videos != null && videos.Length > 0)
                machine.Video = Array.ConvertAll(videos, ConvertFromInternalModel);

            var sound = item.Read<Serialization.Models.Metadata.Sound>(Serialization.Models.Metadata.Machine.SoundKey);
            if (sound != null)
                machine.Sound = ConvertFromInternalModel(sound);

            var input = item.Read<Serialization.Models.Metadata.Input>(Serialization.Models.Metadata.Machine.InputKey);
            if (input != null)
                machine.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.Read<Serialization.Models.Metadata.DipSwitch[]>(Serialization.Models.Metadata.Machine.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Length > 0)
                machine.DipSwitch = Array.ConvertAll(dipSwitches, ConvertFromInternalModel);

            var configurations = item.Read<Serialization.Models.Metadata.Configuration[]>(Serialization.Models.Metadata.Machine.ConfigurationKey);
            if (configurations != null && configurations.Length > 0)
                machine.Configuration = Array.ConvertAll(configurations, ConvertFromInternalModel);

            var ports = item.Read<Serialization.Models.Metadata.Port[]>(Serialization.Models.Metadata.Machine.PortKey);
            if (ports != null && ports.Length > 0)
                machine.Port = Array.ConvertAll(ports, ConvertFromInternalModel);

            var adjusters = item.Read<Serialization.Models.Metadata.Adjuster[]>(Serialization.Models.Metadata.Machine.AdjusterKey);
            if (adjusters != null && adjusters.Length > 0)
                machine.Adjuster = Array.ConvertAll(adjusters, ConvertFromInternalModel);

            var driver = item.Read<Serialization.Models.Metadata.Driver>(Serialization.Models.Metadata.Machine.DriverKey);
            if (driver != null)
                machine.Driver = ConvertFromInternalModel(driver);

            var features = item.Read<Serialization.Models.Metadata.Feature[]>(Serialization.Models.Metadata.Machine.FeatureKey);
            if (features != null && features.Length > 0)
                machine.Feature = Array.ConvertAll(features, ConvertFromInternalModel);

            var devices = item.Read<Serialization.Models.Metadata.Device[]>(Serialization.Models.Metadata.Machine.DeviceKey);
            if (devices != null && devices.Length > 0)
                machine.Device = Array.ConvertAll(devices, ConvertFromInternalModel);

            var slots = item.Read<Serialization.Models.Metadata.Slot[]>(Serialization.Models.Metadata.Machine.SlotKey);
            if (slots != null && slots.Length > 0)
                machine.Slot = Array.ConvertAll(slots, ConvertFromInternalModel);

            var softwareLists = item.Read<Serialization.Models.Metadata.SoftwareList[]>(Serialization.Models.Metadata.Machine.SoftwareListKey);
            if (softwareLists != null && softwareLists.Length > 0)
                machine.SoftwareList = Array.ConvertAll(softwareLists, ConvertFromInternalModel);

            var ramOptions = item.Read<Serialization.Models.Metadata.RamOption[]>(Serialization.Models.Metadata.Machine.RamOptionKey);
            if (ramOptions != null && ramOptions.Length > 0)
                machine.RamOption = Array.ConvertAll(ramOptions, ConvertFromInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Adjuster"/> to <see cref="Adjuster"/>
        /// </summary>
        private static Adjuster ConvertFromInternalModel(Serialization.Models.Metadata.Adjuster item)
        {
            var adjuster = new Adjuster
            {
                Name = item.ReadString(Serialization.Models.Metadata.Adjuster.NameKey),
                Default = item.ReadString(Serialization.Models.Metadata.Adjuster.DefaultKey),
            };

            var condition = item.Read<Serialization.Models.Metadata.Condition>(Serialization.Models.Metadata.Adjuster.ConditionKey);
            if (condition != null)
                adjuster.Condition = ConvertFromInternalModel(condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Analog"/> to <see cref="Analog"/>
        /// </summary>
        private static Analog ConvertFromInternalModel(Serialization.Models.Metadata.Analog item)
        {
            var analog = new Analog
            {
                Mask = item.ReadString(Serialization.Models.Metadata.Analog.MaskKey),
            };
            return analog;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.BiosSet"/> to <see cref="BiosSet"/>
        /// </summary>
        private static BiosSet ConvertFromInternalModel(Serialization.Models.Metadata.BiosSet item)
        {
            var biosset = new BiosSet
            {
                Name = item.ReadString(Serialization.Models.Metadata.BiosSet.NameKey),
                Description = item.ReadString(Serialization.Models.Metadata.BiosSet.DescriptionKey),
                Default = item.ReadString(Serialization.Models.Metadata.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Chip"/> to <see cref="Chip"/>
        /// </summary>
        private static Chip ConvertFromInternalModel(Serialization.Models.Metadata.Chip item)
        {
            var chip = new Chip
            {
                Name = item.ReadString(Serialization.Models.Metadata.Chip.NameKey),
                Tag = item.ReadString(Serialization.Models.Metadata.Chip.TagKey),
                Type = item.ReadString(Serialization.Models.Metadata.Chip.ChipTypeKey),
                SoundOnly = item.ReadString(Serialization.Models.Metadata.Chip.SoundOnlyKey),
                Clock = item.ReadString(Serialization.Models.Metadata.Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Condition"/> to <see cref="Condition"/>
        /// </summary>
        private static Condition ConvertFromInternalModel(Serialization.Models.Metadata.Condition item)
        {
            var condition = new Condition
            {
                Tag = item.ReadString(Serialization.Models.Metadata.Condition.TagKey),
                Mask = item.ReadString(Serialization.Models.Metadata.Condition.MaskKey),
                Relation = item.ReadString(Serialization.Models.Metadata.Condition.RelationKey),
                Value = item.ReadString(Serialization.Models.Metadata.Condition.ValueKey),
            };
            return condition;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Configuration"/> to <see cref="Configuration"/>
        /// </summary>
        private static Configuration ConvertFromInternalModel(Serialization.Models.Metadata.Configuration item)
        {
            var configuration = new Configuration
            {
                Name = item.ReadString(Serialization.Models.Metadata.Configuration.NameKey),
                Tag = item.ReadString(Serialization.Models.Metadata.Configuration.TagKey),
                Mask = item.ReadString(Serialization.Models.Metadata.Configuration.MaskKey),
            };

            var condition = item.Read<Serialization.Models.Metadata.Condition>(Serialization.Models.Metadata.Configuration.ConditionKey);
            if (condition != null)
                configuration.Condition = ConvertFromInternalModel(condition);

            var confLocations = item.Read<Serialization.Models.Metadata.ConfLocation[]>(Serialization.Models.Metadata.Configuration.ConfLocationKey);
            if (confLocations != null && confLocations.Length > 0)
                configuration.ConfLocation = Array.ConvertAll(confLocations, ConvertFromInternalModel);

            var confSettings = item.Read<Serialization.Models.Metadata.ConfSetting[]>(Serialization.Models.Metadata.Configuration.ConfSettingKey);
            if (confSettings != null && confSettings.Length > 0)
                configuration.ConfSetting = Array.ConvertAll(confSettings, ConvertFromInternalModel);

            return configuration;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.ConfLocation"/> to <see cref="ConfLocation"/>
        /// </summary>
        private static ConfLocation ConvertFromInternalModel(Serialization.Models.Metadata.ConfLocation item)
        {
            var confLocation = new ConfLocation
            {
                Name = item.ReadString(Serialization.Models.Metadata.ConfLocation.NameKey),
                Number = item.ReadString(Serialization.Models.Metadata.ConfLocation.NumberKey),
                Inverted = item.ReadString(Serialization.Models.Metadata.ConfLocation.InvertedKey),
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.ConfSetting"/> to <see cref="ConfSetting"/>
        /// </summary>
        private static ConfSetting ConvertFromInternalModel(Serialization.Models.Metadata.ConfSetting item)
        {
            var confSetting = new ConfSetting
            {
                Name = item.ReadString(Serialization.Models.Metadata.ConfSetting.NameKey),
                Value = item.ReadString(Serialization.Models.Metadata.ConfSetting.ValueKey),
                Default = item.ReadString(Serialization.Models.Metadata.ConfSetting.DefaultKey),
            };

            var condition = item.Read<Serialization.Models.Metadata.Condition>(Serialization.Models.Metadata.ConfSetting.ConditionKey);
            if (condition != null)
                confSetting.Condition = ConvertFromInternalModel(condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Control"/> to <see cref="Control"/>
        /// </summary>
        private static Control ConvertFromInternalModel(Serialization.Models.Metadata.Control item)
        {
            var control = new Control
            {
                Type = item.ReadString(Serialization.Models.Metadata.Control.ControlTypeKey),
                Player = item.ReadString(Serialization.Models.Metadata.Control.PlayerKey),
                Buttons = item.ReadString(Serialization.Models.Metadata.Control.ButtonsKey),
                ReqButtons = item.ReadString(Serialization.Models.Metadata.Control.ReqButtonsKey),
                Minimum = item.ReadString(Serialization.Models.Metadata.Control.MinimumKey),
                Maximum = item.ReadString(Serialization.Models.Metadata.Control.MaximumKey),
                Sensitivity = item.ReadString(Serialization.Models.Metadata.Control.SensitivityKey),
                KeyDelta = item.ReadString(Serialization.Models.Metadata.Control.KeyDeltaKey),
                Reverse = item.ReadString(Serialization.Models.Metadata.Control.ReverseKey),
                Ways = item.ReadString(Serialization.Models.Metadata.Control.WaysKey),
                Ways2 = item.ReadString(Serialization.Models.Metadata.Control.Ways2Key),
                Ways3 = item.ReadString(Serialization.Models.Metadata.Control.Ways3Key),
            };
            return control;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Device"/> to <see cref="Device"/>
        /// </summary>
        private static Device ConvertFromInternalModel(Serialization.Models.Metadata.Device item)
        {
            var device = new Device
            {
                Type = item.ReadString(Serialization.Models.Metadata.Device.DeviceTypeKey),
                Tag = item.ReadString(Serialization.Models.Metadata.Device.TagKey),
                FixedImage = item.ReadString(Serialization.Models.Metadata.Device.FixedImageKey),
                Mandatory = item.ReadString(Serialization.Models.Metadata.Device.MandatoryKey),
                Interface = item.ReadString(Serialization.Models.Metadata.Device.InterfaceKey),
            };

            var instance = item.Read<Serialization.Models.Metadata.Instance>(Serialization.Models.Metadata.Device.InstanceKey);
            if (instance != null)
                device.Instance = ConvertFromInternalModel(instance);

            var extensions = item.Read<Serialization.Models.Metadata.Extension[]>(Serialization.Models.Metadata.Device.ExtensionKey);
            if (extensions != null && extensions.Length > 0)
                device.Extension = Array.ConvertAll(extensions, ConvertFromInternalModel);

            return device;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.DeviceRef"/> to <see cref="DeviceRef"/>
        /// </summary>
        private static DeviceRef ConvertFromInternalModel(Serialization.Models.Metadata.DeviceRef item)
        {
            var deviceRef = new DeviceRef
            {
                Name = item.ReadString(Serialization.Models.Metadata.DeviceRef.NameKey),
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.DipLocation"/> to <see cref="DipLocation"/>
        /// </summary>
        private static DipLocation ConvertFromInternalModel(Serialization.Models.Metadata.DipLocation item)
        {
            var dipLocation = new DipLocation
            {
                Name = item.ReadString(Serialization.Models.Metadata.DipLocation.NameKey),
                Number = item.ReadString(Serialization.Models.Metadata.DipLocation.NumberKey),
                Inverted = item.ReadString(Serialization.Models.Metadata.DipLocation.InvertedKey),
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.DipSwitch"/> to <see cref="DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Serialization.Models.Metadata.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                Name = item.ReadString(Serialization.Models.Metadata.DipSwitch.NameKey),
                Tag = item.ReadString(Serialization.Models.Metadata.DipSwitch.TagKey),
                Mask = item.ReadString(Serialization.Models.Metadata.DipSwitch.MaskKey),
            };

            var condition = item.Read<Serialization.Models.Metadata.Condition>(Serialization.Models.Metadata.DipSwitch.ConditionKey);
            if (condition != null)
                dipSwitch.Condition = ConvertFromInternalModel(condition);

            var dipLocations = item.Read<Serialization.Models.Metadata.DipLocation[]>(Serialization.Models.Metadata.DipSwitch.DipLocationKey);
            if (dipLocations != null && dipLocations.Length > 0)
                dipSwitch.DipLocation = Array.ConvertAll(dipLocations, ConvertFromInternalModel);

            var dipValues = item.Read<Serialization.Models.Metadata.DipValue[]>(Serialization.Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues != null && dipValues.Length > 0)
                dipSwitch.DipValue = Array.ConvertAll(dipValues, ConvertFromInternalModel);

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.DipValue"/> to <see cref="DipValue"/>
        /// </summary>
        private static DipValue ConvertFromInternalModel(Serialization.Models.Metadata.DipValue item)
        {
            var dipValue = new DipValue
            {
                Name = item.ReadString(Serialization.Models.Metadata.DipValue.NameKey),
                Value = item.ReadString(Serialization.Models.Metadata.DipValue.ValueKey),
                Default = item.ReadString(Serialization.Models.Metadata.DipValue.DefaultKey),
            };

            var condition = item.Read<Serialization.Models.Metadata.Condition>(Serialization.Models.Metadata.DipValue.ConditionKey);
            if (condition != null)
                dipValue.Condition = ConvertFromInternalModel(condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Disk"/> to <see cref="Disk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Serialization.Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.ReadString(Serialization.Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Serialization.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Disk.SHA1Key),
                Merge = item.ReadString(Serialization.Models.Metadata.Disk.MergeKey),
                Region = item.ReadString(Serialization.Models.Metadata.Disk.RegionKey),
                Index = item.ReadString(Serialization.Models.Metadata.Disk.IndexKey),
                Writable = item.ReadString(Serialization.Models.Metadata.Disk.WritableKey),
                Status = item.ReadString(Serialization.Models.Metadata.Disk.StatusKey),
                Optional = item.ReadString(Serialization.Models.Metadata.Disk.OptionalKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Display"/> to <see cref="Display"/>
        /// </summary>
        private static Display ConvertFromInternalModel(Serialization.Models.Metadata.Display item)
        {
            var display = new Display
            {
                Tag = item.ReadString(Serialization.Models.Metadata.Display.TagKey),
                Type = item.ReadString(Serialization.Models.Metadata.Display.DisplayTypeKey),
                Rotate = item.ReadString(Serialization.Models.Metadata.Display.RotateKey),
                FlipX = item.ReadString(Serialization.Models.Metadata.Display.FlipXKey),
                Width = item.ReadString(Serialization.Models.Metadata.Display.WidthKey),
                Height = item.ReadString(Serialization.Models.Metadata.Display.HeightKey),
                Refresh = item.ReadString(Serialization.Models.Metadata.Display.RefreshKey),
                PixClock = item.ReadString(Serialization.Models.Metadata.Display.PixClockKey),
                HTotal = item.ReadString(Serialization.Models.Metadata.Display.HTotalKey),
                HBEnd = item.ReadString(Serialization.Models.Metadata.Display.HBEndKey),
                HBStart = item.ReadString(Serialization.Models.Metadata.Display.HBStartKey),
                VTotal = item.ReadString(Serialization.Models.Metadata.Display.VTotalKey),
                VBEnd = item.ReadString(Serialization.Models.Metadata.Display.VBEndKey),
                VBStart = item.ReadString(Serialization.Models.Metadata.Display.VBStartKey),
            };
            return display;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Driver"/> to <see cref="Driver"/>
        /// </summary>
        private static Driver ConvertFromInternalModel(Serialization.Models.Metadata.Driver item)
        {
            var driver = new Driver
            {
                Status = item.ReadString(Serialization.Models.Metadata.Driver.StatusKey),
                Color = item.ReadString(Serialization.Models.Metadata.Driver.ColorKey),
                Sound = item.ReadString(Serialization.Models.Metadata.Driver.SoundKey),
                PaletteSize = item.ReadString(Serialization.Models.Metadata.Driver.PaletteSizeKey),
                Emulation = item.ReadString(Serialization.Models.Metadata.Driver.EmulationKey),
                Cocktail = item.ReadString(Serialization.Models.Metadata.Driver.CocktailKey),
                SaveState = item.ReadString(Serialization.Models.Metadata.Driver.SaveStateKey),
                RequiresArtwork = item.ReadString(Serialization.Models.Metadata.Driver.RequiresArtworkKey),
                Unofficial = item.ReadString(Serialization.Models.Metadata.Driver.UnofficialKey),
                NoSoundHardware = item.ReadString(Serialization.Models.Metadata.Driver.NoSoundHardwareKey),
                Incomplete = item.ReadString(Serialization.Models.Metadata.Driver.IncompleteKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Extension"/> to <see cref="Extension"/>
        /// </summary>
        private static Extension ConvertFromInternalModel(Serialization.Models.Metadata.Extension item)
        {
            var extension = new Extension
            {
                Name = item.ReadString(Serialization.Models.Metadata.Extension.NameKey),
            };
            return extension;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Feature"/> to <see cref="Feature"/>
        /// </summary>
        private static Feature ConvertFromInternalModel(Serialization.Models.Metadata.Feature item)
        {
            var feature = new Feature
            {
                Type = item.ReadString(Serialization.Models.Metadata.Feature.FeatureTypeKey),
                Status = item.ReadString(Serialization.Models.Metadata.Feature.StatusKey),
                Overall = item.ReadString(Serialization.Models.Metadata.Feature.OverallKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Input"/> to <see cref="Input"/>
        /// </summary>
        private static Input ConvertFromInternalModel(Serialization.Models.Metadata.Input item)
        {
            var input = new Input
            {
                Service = item.ReadString(Serialization.Models.Metadata.Input.ServiceKey),
                Tilt = item.ReadString(Serialization.Models.Metadata.Input.TiltKey),
                Players = item.ReadString(Serialization.Models.Metadata.Input.PlayersKey),
                Buttons = item.ReadString(Serialization.Models.Metadata.Input.ButtonsKey),
                Coins = item.ReadString(Serialization.Models.Metadata.Input.CoinsKey),
            };

            var controlAttr = item.ReadString(Serialization.Models.Metadata.Input.ControlKey);
            if (controlAttr != null)
                input.ControlAttr = controlAttr;

            var controls = item.Read<Serialization.Models.Metadata.Control[]>(Serialization.Models.Metadata.Input.ControlKey);
            if (controls != null && controls.Length > 0)
                input.Control = Array.ConvertAll(controls, ConvertFromInternalModel);

            return input;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Instance"/> to <see cref="Instance"/>
        /// </summary>
        private static Instance ConvertFromInternalModel(Serialization.Models.Metadata.Instance item)
        {
            var instance = new Instance
            {
                Name = item.ReadString(Serialization.Models.Metadata.Instance.NameKey),
                BriefName = item.ReadString(Serialization.Models.Metadata.Instance.BriefNameKey),
            };
            return instance;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Port"/> to <see cref="Port"/>
        /// </summary>
        private static Port ConvertFromInternalModel(Serialization.Models.Metadata.Port item)
        {
            var port = new Port
            {
                Tag = item.ReadString(Serialization.Models.Metadata.Port.TagKey),
            };

            var analogs = item.Read<Serialization.Models.Metadata.Analog[]>(Serialization.Models.Metadata.Port.AnalogKey);
            if (analogs != null && analogs.Length > 0)
                port.Analog = Array.ConvertAll(analogs, ConvertFromInternalModel);

            return port;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.RamOption"/> to <see cref="RamOption"/>
        /// </summary>
        private static RamOption ConvertFromInternalModel(Serialization.Models.Metadata.RamOption item)
        {
            var ramOption = new RamOption
            {
                Name = item.ReadString(Serialization.Models.Metadata.RamOption.NameKey),
                Default = item.ReadString(Serialization.Models.Metadata.RamOption.DefaultKey),
                Content = item.ReadString(Serialization.Models.Metadata.RamOption.ContentKey),
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Serialization.Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.ReadString(Serialization.Models.Metadata.Rom.NameKey),
                Bios = item.ReadString(Serialization.Models.Metadata.Rom.BiosKey),
                Size = item.ReadString(Serialization.Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Serialization.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Rom.SHA1Key),
                Merge = item.ReadString(Serialization.Models.Metadata.Rom.MergeKey),
                Region = item.ReadString(Serialization.Models.Metadata.Rom.RegionKey),
                Offset = item.ReadString(Serialization.Models.Metadata.Rom.OffsetKey),
                Status = item.ReadString(Serialization.Models.Metadata.Rom.StatusKey),
                Optional = item.ReadString(Serialization.Models.Metadata.Rom.OptionalKey),
                Dispose = item.ReadString(Serialization.Models.Metadata.Rom.DisposeKey),
                SoundOnly = item.ReadString(Serialization.Models.Metadata.Rom.SoundOnlyKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Sample"/> to <see cref="Sample"/>
        /// </summary>
        private static Sample ConvertFromInternalModel(Serialization.Models.Metadata.Sample item)
        {
            var sample = new Sample
            {
                Name = item.ReadString(Serialization.Models.Metadata.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Slot"/> to <see cref="Slot"/>
        /// </summary>
        private static Slot ConvertFromInternalModel(Serialization.Models.Metadata.Slot item)
        {
            var slot = new Slot
            {
                Name = item.ReadString(Serialization.Models.Metadata.Slot.NameKey),
            };

            var slotOptions = item.Read<Serialization.Models.Metadata.SlotOption[]>(Serialization.Models.Metadata.Slot.SlotOptionKey);
            if (slotOptions != null && slotOptions.Length > 0)
                slot.SlotOption = Array.ConvertAll(slotOptions, ConvertFromInternalModel);

            return slot;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.SlotOption"/> to <see cref="SlotOption"/>
        /// </summary>
        private static SlotOption ConvertFromInternalModel(Serialization.Models.Metadata.SlotOption item)
        {
            var slotOption = new SlotOption
            {
                Name = item.ReadString(Serialization.Models.Metadata.SlotOption.NameKey),
                DevName = item.ReadString(Serialization.Models.Metadata.SlotOption.DevNameKey),
                Default = item.ReadString(Serialization.Models.Metadata.SlotOption.DefaultKey),
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.SoftwareList"/> to <see cref="SabreTools.Serialization.Models.Listxml.SoftwareList"/>
        /// </summary>
        private static SabreTools.Serialization.Models.Listxml.SoftwareList ConvertFromInternalModel(Serialization.Models.Metadata.SoftwareList item)
        {
            var softwareList = new SabreTools.Serialization.Models.Listxml.SoftwareList
            {
                Tag = item.ReadString(Serialization.Models.Metadata.SoftwareList.TagKey),
                Name = item.ReadString(Serialization.Models.Metadata.SoftwareList.NameKey),
                Status = item.ReadString(Serialization.Models.Metadata.SoftwareList.StatusKey),
                Filter = item.ReadString(Serialization.Models.Metadata.SoftwareList.FilterKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Sound"/> to <see cref="Sound"/>
        /// </summary>
        private static Sound ConvertFromInternalModel(Serialization.Models.Metadata.Sound item)
        {
            var sound = new Sound
            {
                Channels = item.ReadString(Serialization.Models.Metadata.Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Video"/> to <see cref="Video"/>
        /// </summary>
        private static Video ConvertFromInternalModel(Serialization.Models.Metadata.Video item)
        {
            var video = new Video
            {
                Screen = item.ReadString(Serialization.Models.Metadata.Video.ScreenKey),
                Orientation = item.ReadString(Serialization.Models.Metadata.Video.OrientationKey),
                Width = item.ReadString(Serialization.Models.Metadata.Video.WidthKey),
                Height = item.ReadString(Serialization.Models.Metadata.Video.HeightKey),
                AspectX = item.ReadString(Serialization.Models.Metadata.Video.AspectXKey),
                AspectY = item.ReadString(Serialization.Models.Metadata.Video.AspectYKey),
                Refresh = item.ReadString(Serialization.Models.Metadata.Video.RefreshKey),
            };
            return video;
        }
    }
}
