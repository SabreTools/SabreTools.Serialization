using System;
using SabreTools.Data.Models.Listxml;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listxml : BaseMetadataSerializer<Mame>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(Mame? item)
        {
            if (item is null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                Header = ConvertHeaderToInternalModel(item),
            };

            if (item?.Game is not null && item.Game.Length > 0)
                metadataFile.Machine = Array.ConvertAll(item.Game, ConvertMachineToInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Mame"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Mame item)
        {
            var header = new Data.Models.Metadata.Header
            {
                Build = item.Build,
                Debug = item.Debug,
                MameConfig = item.MameConfig,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="GameBase"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        internal static Data.Models.Metadata.Machine ConvertMachineToInternalModel(GameBase item)
        {
            var machine = new Data.Models.Metadata.Machine
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

            if (item.BiosSet is not null && item.BiosSet.Length > 0)
                machine.BiosSet = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);

            if (item.Rom is not null && item.Rom.Length > 0)
                machine.Rom = Array.ConvertAll(item.Rom, ConvertToInternalModel);

            if (item.Disk is not null && item.Disk.Length > 0)
                machine.Disk = Array.ConvertAll(item.Disk, ConvertToInternalModel);

            if (item.DeviceRef is not null && item.DeviceRef.Length > 0)
                machine.DeviceRef = Array.ConvertAll(item.DeviceRef, ConvertToInternalModel);

            if (item.Sample is not null && item.Sample.Length > 0)
                machine.Sample = Array.ConvertAll(item.Sample, ConvertToInternalModel);

            if (item.Chip is not null && item.Chip.Length > 0)
                machine.Chip = Array.ConvertAll(item.Chip, ConvertToInternalModel);

            if (item.Display is not null && item.Display.Length > 0)
                machine.Display = Array.ConvertAll(item.Display, ConvertToInternalModel);

            if (item.Video is not null && item.Video.Length > 0)
                machine.Video = Array.ConvertAll(item.Video, ConvertToInternalModel);

            if (item.Sound is not null)
                machine.Sound = ConvertToInternalModel(item.Sound);

            if (item.Input is not null)
                machine.Input = ConvertToInternalModel(item.Input);

            if (item.DipSwitch is not null && item.DipSwitch.Length > 0)
                machine.DipSwitch = Array.ConvertAll(item.DipSwitch, ConvertToInternalModel);

            if (item.Configuration is not null && item.Configuration.Length > 0)
                machine.Configuration = Array.ConvertAll(item.Configuration, ConvertToInternalModel);

            if (item.Port is not null && item.Port.Length > 0)
                machine.Port = Array.ConvertAll(item.Port, ConvertToInternalModel);

            if (item.Adjuster is not null && item.Adjuster.Length > 0)
                machine.Adjuster = Array.ConvertAll(item.Adjuster, ConvertToInternalModel);

            if (item.Driver is not null)
                machine.Driver = ConvertToInternalModel(item.Driver);

            if (item.Feature is not null && item.Feature.Length > 0)
                machine.Feature = Array.ConvertAll(item.Feature, ConvertToInternalModel);

            if (item.Device is not null && item.Device.Length > 0)
                machine.Device = Array.ConvertAll(item.Device, ConvertToInternalModel);

            if (item.Slot is not null && item.Slot.Length > 0)
                machine.Slot = Array.ConvertAll(item.Slot, ConvertToInternalModel);

            if (item.SoftwareList is not null && item.SoftwareList.Length > 0)
                machine.SoftwareList = Array.ConvertAll(item.SoftwareList, ConvertToInternalModel);

            if (item.RamOption is not null && item.RamOption.Length > 0)
                machine.RamOption = Array.ConvertAll(item.RamOption, ConvertToInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Adjuster"/> to <see cref="Models.Metadata.Adjuster"/>
        /// </summary>
        private static Data.Models.Metadata.Adjuster ConvertToInternalModel(Adjuster item)
        {
            var adjuster = new Data.Models.Metadata.Adjuster
            {
                ConditionMask = item.Condition?.Mask,
                ConditionRelation = item.Condition?.Relation,
                ConditionTag = item.Condition?.Tag,
                ConditionValue = item.Condition?.Value,
                Name = item.Name,
                Default = item.Default,
            };

            return adjuster;
        }

        /// <summary>
        /// Convert from <see cref="BiosSet"/> to <see cref="Models.Metadata.BiosSet"/>
        /// </summary>
        private static Data.Models.Metadata.BiosSet ConvertToInternalModel(BiosSet item)
        {
            var biosset = new Data.Models.Metadata.BiosSet
            {
                Name = item.Name,
                Description = item.Description,
                Default = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <see cref="Chip"/> to <see cref="Models.Metadata.Chip"/>
        /// </summary>
        private static Data.Models.Metadata.Chip ConvertToInternalModel(Chip item)
        {
            var chip = new Data.Models.Metadata.Chip
            {
                Name = item.Name,
                Tag = item.Tag,
                ChipType = item.Type,
                SoundOnly = item.SoundOnly,
                Clock = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <see cref="Configuration"/> to <see cref="Models.Metadata.Configuration"/>
        /// </summary>
        private static Data.Models.Metadata.Configuration ConvertToInternalModel(Configuration item)
        {
            var configuration = new Data.Models.Metadata.Configuration
            {
                ConditionMask = item.Condition?.Mask,
                ConditionRelation = item.Condition?.Relation,
                ConditionTag = item.Condition?.Tag,
                ConditionValue = item.Condition?.Value,
                Name = item.Name,
                Tag = item.Tag,
                Mask = item.Mask,
            };

            if (item.ConfLocation is not null && item.ConfLocation.Length > 0)
                configuration.ConfLocation = Array.ConvertAll(item.ConfLocation, ConvertToInternalModel);

            if (item.ConfSetting is not null && item.ConfSetting.Length > 0)
                configuration.ConfSetting = Array.ConvertAll(item.ConfSetting, ConvertToInternalModel);

            return configuration;
        }

        /// <summary>
        /// Convert from <see cref="ConfLocation"/> to <see cref="Models.Metadata.ConfLocation"/>
        /// </summary>
        private static Data.Models.Metadata.ConfLocation ConvertToInternalModel(ConfLocation item)
        {
            var confLocation = new Data.Models.Metadata.ConfLocation
            {
                Name = item.Name,
                Number = item.Number,
                Inverted = item.Inverted,
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <see cref="ConfSetting"/> to <see cref="Models.Metadata.ConfSetting"/>
        /// </summary>
        private static Data.Models.Metadata.ConfSetting ConvertToInternalModel(ConfSetting item)
        {
            var confSetting = new Data.Models.Metadata.ConfSetting
            {
                ConditionMask = item.Condition?.Mask,
                ConditionRelation = item.Condition?.Relation,
                ConditionTag = item.Condition?.Tag,
                ConditionValue = item.Condition?.Value,
                Name = item.Name,
                Value = item.Value,
                Default = item.Default,
            };

            return confSetting;
        }

        /// <summary>
        /// Convert from <see cref="Control"/> to <see cref="Models.Metadata.Control"/>
        /// </summary>
        private static Data.Models.Metadata.Control ConvertToInternalModel(Control item)
        {
            var control = new Data.Models.Metadata.Control
            {
                ControlType = item.Type,
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
        /// Convert from <see cref="Device"/> to <see cref="Models.Metadata.Device"/>
        /// </summary>
        private static Data.Models.Metadata.Device ConvertToInternalModel(Device item)
        {
            var device = new Data.Models.Metadata.Device
            {
                DeviceType = item.Type,
                FixedImage = item.FixedImage,
                InstanceBriefName = item.Instance?.BriefName,
                InstanceName = item.Instance?.Name,
                Interface = item.Interface,
                Mandatory = item.Mandatory,
                Tag = item.Tag,
            };

            if (item.Extension is not null && item.Extension.Length > 0)
                device.ExtensionName = Array.ConvertAll(item.Extension, extension => extension.Name ?? string.Empty);

            return device;
        }

        /// <summary>
        /// Convert from <see cref="DeviceRef"/> to <see cref="Models.Metadata.DeviceRef"/>
        /// </summary>
        private static Data.Models.Metadata.DeviceRef ConvertToInternalModel(DeviceRef item)
        {
            var deviceRef = new Data.Models.Metadata.DeviceRef
            {
                Name = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <see cref="DipLocation"/> to <see cref="Models.Metadata.DipLocation"/>
        /// </summary>
        private static Data.Models.Metadata.DipLocation ConvertToInternalModel(DipLocation item)
        {
            var dipLocation = new Data.Models.Metadata.DipLocation
            {
                Name = item.Name,
                Number = item.Number,
                Inverted = item.Inverted,
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <see cref="DipSwitch"/> to <see cref="Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Data.Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipSwitch = new Data.Models.Metadata.DipSwitch
            {
                ConditionMask = item.Condition?.Mask,
                ConditionRelation = item.Condition?.Relation,
                ConditionTag = item.Condition?.Tag,
                ConditionValue = item.Condition?.Value,
                Name = item.Name,
                Tag = item.Tag,
                Mask = item.Mask,
            };

            if (item.DipLocation is not null && item.DipLocation.Length > 0)
                dipSwitch.DipLocation = Array.ConvertAll(item.DipLocation, ConvertToInternalModel);

            if (item.DipValue is not null && item.DipValue.Length > 0)
                dipSwitch.DipValue = Array.ConvertAll(item.DipValue, ConvertToInternalModel);

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="DipValue"/> to <see cref="Models.Metadata.DipValue"/>
        /// </summary>
        private static Data.Models.Metadata.DipValue ConvertToInternalModel(DipValue item)
        {
            var dipValue = new Data.Models.Metadata.DipValue
            {
                ConditionMask = item.Condition?.Mask,
                ConditionRelation = item.Condition?.Relation,
                ConditionTag = item.Condition?.Tag,
                ConditionValue = item.Condition?.Value,
                Name = item.Name,
                Value = item.Value,
                Default = item.Default,
            };

            return dipValue;
        }

        /// <summary>
        /// Convert from <see cref="Disk"/> to <see cref="Models.Metadata.Disk"/>
        /// </summary>
        private static Data.Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Data.Models.Metadata.Disk
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
        /// Convert from <see cref="Display"/> to <see cref="Models.Metadata.Display"/>
        /// </summary>
        private static Data.Models.Metadata.Display ConvertToInternalModel(Display item)
        {
            var display = new Data.Models.Metadata.Display
            {
                Tag = item.Tag,
                DisplayType = item.Type,
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
        /// Convert from <see cref="Driver"/> to <see cref="Models.Metadata.Driver"/>
        /// </summary>
        private static Data.Models.Metadata.Driver ConvertToInternalModel(Driver item)
        {
            var driver = new Data.Models.Metadata.Driver
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
        /// Convert from <see cref="Feature"/> to <see cref="Models.Metadata.Feature"/>
        /// </summary>
        private static Data.Models.Metadata.Feature ConvertToInternalModel(Feature item)
        {
            var feature = new Data.Models.Metadata.Feature
            {
                FeatureType = item.Type,
                Status = item.Status,
                Overall = item.Overall,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <see cref="Input"/> to <see cref="Models.Metadata.Input"/>
        /// </summary>
        private static Data.Models.Metadata.Input ConvertToInternalModel(Input item)
        {
            var input = new Data.Models.Metadata.Input
            {
                Service = item.Service,
                Tilt = item.Tilt,
                Players = item.Players,
                ControlAttr = item.ControlAttr,
                Buttons = item.Buttons,
                Coins = item.Coins,
            };

            if (item.Control is not null && item.Control.Length > 0)
                input.Control = Array.ConvertAll(item.Control, ConvertToInternalModel);

            return input;
        }

        /// <summary>
        /// Convert from <see cref="Port"/> to <see cref="Models.Metadata.Port"/>
        /// </summary>
        private static Data.Models.Metadata.Port ConvertToInternalModel(Port item)
        {
            var port = new Data.Models.Metadata.Port
            {
                Tag = item.Tag,
            };

            if (item.Analog is not null && item.Analog.Length > 0)
                port.AnalogMask = Array.ConvertAll(item.Analog, analog => analog.Mask ?? string.Empty);

            return port;
        }

        /// <summary>
        /// Convert from <see cref="RamOption"/> to <see cref="Models.Metadata.RamOption"/>
        /// </summary>
        private static Data.Models.Metadata.RamOption ConvertToInternalModel(RamOption item)
        {
            var ramOption = new Data.Models.Metadata.RamOption
            {
                Name = item.Name,
                Default = item.Default,
                Content = item.Content,
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <see cref="Rom"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                Name = item.Name,
                Bios = item.Bios,
                Size = item.Size,
                CRC32 = item.CRC,
                SHA1 = item.SHA1,
                Merge = item.Merge,
                Region = item.Region,
                Offset = item.Offset,
                Status = item.Status,
                Optional = item.Optional,
                Dispose = item.Dispose,
                SoundOnly = item.SoundOnly,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Sample"/> to <see cref="Models.Metadata.Sample"/>
        /// </summary>
        private static Data.Models.Metadata.Sample ConvertToInternalModel(Sample item)
        {
            var sample = new Data.Models.Metadata.Sample
            {
                Name = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <see cref="Slot"/> to <see cref="Models.Metadata.Slot"/>
        /// </summary>
        private static Data.Models.Metadata.Slot ConvertToInternalModel(Slot item)
        {
            var slot = new Data.Models.Metadata.Slot
            {
                Name = item.Name,
            };

            if (item.SlotOption is not null && item.SlotOption.Length > 0)
                slot.SlotOption = Array.ConvertAll(item.SlotOption, ConvertToInternalModel);

            return slot;
        }

        /// <summary>
        /// Convert from <see cref="SlotOption"/> to <see cref="Models.Metadata.SlotOption"/>
        /// </summary>
        private static Data.Models.Metadata.SlotOption ConvertToInternalModel(SlotOption item)
        {
            var slotOption = new Data.Models.Metadata.SlotOption
            {
                Name = item.Name,
                DevName = item.DevName,
                Default = item.Default,
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <see cref="Models.Listxml.SoftwareList"/> to <see cref="Models.Metadata.SoftwareList"/>
        /// </summary>
        private static Data.Models.Metadata.SoftwareList ConvertToInternalModel(Data.Models.Listxml.SoftwareList item)
        {
            var softwareList = new Data.Models.Metadata.SoftwareList
            {
                Tag = item.Tag,
                Name = item.Name,
                Status = item.Status,
                Filter = item.Filter,
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <see cref="Sound"/> to <see cref="Models.Metadata.Sound"/>
        /// </summary>
        private static Data.Models.Metadata.Sound ConvertToInternalModel(Sound item)
        {
            var sound = new Data.Models.Metadata.Sound
            {
                Channels = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <see cref="Video"/> to <see cref="Models.Metadata.Video"/>
        /// </summary>
        private static Data.Models.Metadata.Video ConvertToInternalModel(Video item)
        {
            var video = new Data.Models.Metadata.Video
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
