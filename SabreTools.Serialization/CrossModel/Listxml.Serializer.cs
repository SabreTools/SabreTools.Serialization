using System;
using SabreTools.Data.Models.Listxml;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listxml : BaseMetadataSerializer<Mame>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(Mame? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Game != null && item.Game.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(item.Game, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Mame"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Mame item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.BuildKey] = item.Build,
                [Data.Models.Metadata.Header.DebugKey] = item.Debug,
                [Data.Models.Metadata.Header.MameConfigKey] = item.MameConfig,
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
                [Data.Models.Metadata.Machine.NameKey] = item.Name,
                [Data.Models.Metadata.Machine.SourceFileKey] = item.SourceFile,
                [Data.Models.Metadata.Machine.IsBiosKey] = item.IsBios,
                [Data.Models.Metadata.Machine.IsDeviceKey] = item.IsDevice,
                [Data.Models.Metadata.Machine.IsMechanicalKey] = item.IsMechanical,
                [Data.Models.Metadata.Machine.RunnableKey] = item.Runnable,
                [Data.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Data.Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Data.Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
                [Data.Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Data.Models.Metadata.Machine.YearKey] = item.Year,
                [Data.Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Data.Models.Metadata.Machine.HistoryKey] = item.History,
            };

            if (item.BiosSet != null && item.BiosSet.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.BiosSetKey]
                    = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);
            }

            if (item.Rom != null && item.Rom.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.RomKey]
                    = Array.ConvertAll(item.Rom, ConvertToInternalModel);
            }

            if (item.Disk != null && item.Disk.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.DiskKey]
                    = Array.ConvertAll(item.Disk, ConvertToInternalModel);
            }

            if (item.DeviceRef != null && item.DeviceRef.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.DeviceRefKey]
                    = Array.ConvertAll(item.DeviceRef, ConvertToInternalModel);
            }

            if (item.Sample != null && item.Sample.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.SampleKey]
                    = Array.ConvertAll(item.Sample, ConvertToInternalModel);
            }

            if (item.Chip != null && item.Chip.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.ChipKey]
                    = Array.ConvertAll(item.Chip, ConvertToInternalModel);
            }

            if (item.Display != null && item.Display.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.DisplayKey]
                    = Array.ConvertAll(item.Display, ConvertToInternalModel);
            }

            if (item.Video != null && item.Video.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.VideoKey]
                    = Array.ConvertAll(item.Video, ConvertToInternalModel);
            }

            if (item.Sound != null)
                machine[Data.Models.Metadata.Machine.SoundKey] = ConvertToInternalModel(item.Sound);

            if (item.Input != null)
                machine[Data.Models.Metadata.Machine.InputKey] = ConvertToInternalModel(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.DipSwitchKey]
                    = Array.ConvertAll(item.DipSwitch, ConvertToInternalModel);
            }

            if (item.Configuration != null && item.Configuration.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.ConfigurationKey]
                    = Array.ConvertAll(item.Configuration, ConvertToInternalModel);
            }

            if (item.Port != null && item.Port.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.PortKey]
                    = Array.ConvertAll(item.Port, ConvertToInternalModel);
            }

            if (item.Adjuster != null && item.Adjuster.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.AdjusterKey]
                    = Array.ConvertAll(item.Adjuster, ConvertToInternalModel);
            }

            if (item.Driver != null)
                machine[Data.Models.Metadata.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            if (item.Feature != null && item.Feature.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.FeatureKey]
                    = Array.ConvertAll(item.Feature, ConvertToInternalModel);
            }

            if (item.Device != null && item.Device.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.DeviceKey]
                    = Array.ConvertAll(item.Device, ConvertToInternalModel);
            }

            if (item.Slot != null && item.Slot.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.SlotKey]
                    = Array.ConvertAll(item.Slot, ConvertToInternalModel);
            }

            if (item.SoftwareList != null && item.SoftwareList.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.SoftwareListKey]
                    = Array.ConvertAll(item.SoftwareList, ConvertToInternalModel);
            }

            if (item.RamOption != null && item.RamOption.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.RamOptionKey]
                    = Array.ConvertAll(item.RamOption, ConvertToInternalModel);
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Adjuster"/> to <see cref="Models.Metadata.Adjuster"/>
        /// </summary>
        private static Data.Models.Metadata.Adjuster ConvertToInternalModel(Adjuster item)
        {
            var adjuster = new Data.Models.Metadata.Adjuster
            {
                [Data.Models.Metadata.Adjuster.NameKey] = item.Name,
                [Data.Models.Metadata.Adjuster.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                adjuster[Data.Models.Metadata.Adjuster.ConditionKey] = ConvertToInternalModel(item.Condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <see cref="Analog"/> to <see cref="Models.Metadata.Analog"/>
        /// </summary>
        private static Data.Models.Metadata.Analog ConvertToInternalModel(Analog item)
        {
            var analog = new Data.Models.Metadata.Analog
            {
                [Data.Models.Metadata.Analog.MaskKey] = item.Mask,
            };
            return analog;
        }

        /// <summary>
        /// Convert from <see cref="BiosSet"/> to <see cref="Models.Metadata.BiosSet"/>
        /// </summary>
        private static Data.Models.Metadata.BiosSet ConvertToInternalModel(BiosSet item)
        {
            var biosset = new Data.Models.Metadata.BiosSet
            {
                [Data.Models.Metadata.BiosSet.NameKey] = item.Name,
                [Data.Models.Metadata.BiosSet.DescriptionKey] = item.Description,
                [Data.Models.Metadata.BiosSet.DefaultKey] = item.Default,
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
                [Data.Models.Metadata.Chip.NameKey] = item.Name,
                [Data.Models.Metadata.Chip.TagKey] = item.Tag,
                [Data.Models.Metadata.Chip.ChipTypeKey] = item.Type,
                [Data.Models.Metadata.Chip.SoundOnlyKey] = item.SoundOnly,
                [Data.Models.Metadata.Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <see cref="Condition"/> to <see cref="Models.Metadata.Condition"/>
        /// </summary>
        private static Data.Models.Metadata.Condition ConvertToInternalModel(Condition item)
        {
            var condition = new Data.Models.Metadata.Condition
            {
                [Data.Models.Metadata.Condition.TagKey] = item.Tag,
                [Data.Models.Metadata.Condition.MaskKey] = item.Mask,
                [Data.Models.Metadata.Condition.RelationKey] = item.Relation,
                [Data.Models.Metadata.Condition.ValueKey] = item.Value,
            };
            return condition;
        }

        /// <summary>
        /// Convert from <see cref="Configuration"/> to <see cref="Models.Metadata.Configuration"/>
        /// </summary>
        private static Data.Models.Metadata.Configuration ConvertToInternalModel(Configuration item)
        {
            var configuration = new Data.Models.Metadata.Configuration
            {
                [Data.Models.Metadata.Configuration.NameKey] = item.Name,
                [Data.Models.Metadata.Configuration.TagKey] = item.Tag,
                [Data.Models.Metadata.Configuration.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                configuration[Data.Models.Metadata.Configuration.ConditionKey] = ConvertToInternalModel(item.Condition);

            if (item.ConfLocation != null && item.ConfLocation.Length > 0)
            {
                configuration[Data.Models.Metadata.Configuration.ConfLocationKey]
                    = Array.ConvertAll(item.ConfLocation, ConvertToInternalModel);
            }

            if (item.ConfSetting != null && item.ConfSetting.Length > 0)
            {
                configuration[Data.Models.Metadata.Configuration.ConfSettingKey]
                    = Array.ConvertAll(item.ConfSetting, ConvertToInternalModel);
            }

            return configuration;
        }

        /// <summary>
        /// Convert from <see cref="ConfLocation"/> to <see cref="Models.Metadata.ConfLocation"/>
        /// </summary>
        private static Data.Models.Metadata.ConfLocation ConvertToInternalModel(ConfLocation item)
        {
            var confLocation = new Data.Models.Metadata.ConfLocation
            {
                [Data.Models.Metadata.ConfLocation.NameKey] = item.Name,
                [Data.Models.Metadata.ConfLocation.NumberKey] = item.Number,
                [Data.Models.Metadata.ConfLocation.InvertedKey] = item.Inverted,
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
                [Data.Models.Metadata.ConfSetting.NameKey] = item.Name,
                [Data.Models.Metadata.ConfSetting.ValueKey] = item.Value,
                [Data.Models.Metadata.ConfSetting.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                confSetting[Data.Models.Metadata.ConfSetting.ConditionKey] = ConvertToInternalModel(item.Condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <see cref="Control"/> to <see cref="Models.Metadata.Control"/>
        /// </summary>
        private static Data.Models.Metadata.Control ConvertToInternalModel(Control item)
        {
            var control = new Data.Models.Metadata.Control
            {
                [Data.Models.Metadata.Control.ControlTypeKey] = item.Type,
                [Data.Models.Metadata.Control.PlayerKey] = item.Player,
                [Data.Models.Metadata.Control.ButtonsKey] = item.Buttons,
                [Data.Models.Metadata.Control.ReqButtonsKey] = item.ReqButtons,
                [Data.Models.Metadata.Control.MinimumKey] = item.Minimum,
                [Data.Models.Metadata.Control.MaximumKey] = item.Maximum,
                [Data.Models.Metadata.Control.SensitivityKey] = item.Sensitivity,
                [Data.Models.Metadata.Control.KeyDeltaKey] = item.KeyDelta,
                [Data.Models.Metadata.Control.ReverseKey] = item.Reverse,
                [Data.Models.Metadata.Control.WaysKey] = item.Ways,
                [Data.Models.Metadata.Control.Ways2Key] = item.Ways2,
                [Data.Models.Metadata.Control.Ways3Key] = item.Ways3,
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
                [Data.Models.Metadata.Device.DeviceTypeKey] = item.Type,
                [Data.Models.Metadata.Device.TagKey] = item.Tag,
                [Data.Models.Metadata.Device.FixedImageKey] = item.FixedImage,
                [Data.Models.Metadata.Device.MandatoryKey] = item.Mandatory,
                [Data.Models.Metadata.Device.InterfaceKey] = item.Interface,
            };

            if (item.Instance != null)
                device[Data.Models.Metadata.Device.InstanceKey] = ConvertToInternalModel(item.Instance);

            if (item.Extension != null && item.Extension.Length > 0)
            {
                device[Data.Models.Metadata.Device.ExtensionKey]
                    = Array.ConvertAll(item.Extension, ConvertToInternalModel);
            }

            return device;
        }

        /// <summary>
        /// Convert from <see cref="DeviceRef"/> to <see cref="Models.Metadata.DeviceRef"/>
        /// </summary>
        private static Data.Models.Metadata.DeviceRef ConvertToInternalModel(DeviceRef item)
        {
            var deviceRef = new Data.Models.Metadata.DeviceRef
            {
                [Data.Models.Metadata.DeviceRef.NameKey] = item.Name,
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
                [Data.Models.Metadata.DipLocation.NameKey] = item.Name,
                [Data.Models.Metadata.DipLocation.NumberKey] = item.Number,
                [Data.Models.Metadata.DipLocation.InvertedKey] = item.Inverted,
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
                [Data.Models.Metadata.DipSwitch.NameKey] = item.Name,
                [Data.Models.Metadata.DipSwitch.TagKey] = item.Tag,
                [Data.Models.Metadata.DipSwitch.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                dipSwitch[Data.Models.Metadata.DipSwitch.ConditionKey] = ConvertToInternalModel(item.Condition);

            if (item.DipLocation != null && item.DipLocation.Length > 0)
            {
                dipSwitch[Data.Models.Metadata.DipSwitch.DipLocationKey]
                    = Array.ConvertAll(item.DipLocation, ConvertToInternalModel);
            }

            if (item.DipValue != null && item.DipValue.Length > 0)
            {
                dipSwitch[Data.Models.Metadata.DipSwitch.DipValueKey]
                    = Array.ConvertAll(item.DipValue, ConvertToInternalModel);
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="DipValue"/> to <see cref="Models.Metadata.DipValue"/>
        /// </summary>
        private static Data.Models.Metadata.DipValue ConvertToInternalModel(DipValue item)
        {
            var dipValue = new Data.Models.Metadata.DipValue
            {
                [Data.Models.Metadata.DipValue.NameKey] = item.Name,
                [Data.Models.Metadata.DipValue.ValueKey] = item.Value,
                [Data.Models.Metadata.DipValue.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                dipValue[Data.Models.Metadata.DipValue.ConditionKey] = ConvertToInternalModel(item.Condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <see cref="Disk"/> to <see cref="Models.Metadata.Disk"/>
        /// </summary>
        private static Data.Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Data.Models.Metadata.Disk
            {
                [Data.Models.Metadata.Disk.NameKey] = item.Name,
                [Data.Models.Metadata.Disk.MD5Key] = item.MD5,
                [Data.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Disk.MergeKey] = item.Merge,
                [Data.Models.Metadata.Disk.RegionKey] = item.Region,
                [Data.Models.Metadata.Disk.IndexKey] = item.Index,
                [Data.Models.Metadata.Disk.WritableKey] = item.Writable,
                [Data.Models.Metadata.Disk.StatusKey] = item.Status,
                [Data.Models.Metadata.Disk.OptionalKey] = item.Optional,
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
                [Data.Models.Metadata.Display.TagKey] = item.Tag,
                [Data.Models.Metadata.Display.DisplayTypeKey] = item.Type,
                [Data.Models.Metadata.Display.RotateKey] = item.Rotate,
                [Data.Models.Metadata.Display.FlipXKey] = item.FlipX,
                [Data.Models.Metadata.Display.WidthKey] = item.Width,
                [Data.Models.Metadata.Display.HeightKey] = item.Height,
                [Data.Models.Metadata.Display.RefreshKey] = item.Refresh,
                [Data.Models.Metadata.Display.PixClockKey] = item.PixClock,
                [Data.Models.Metadata.Display.HTotalKey] = item.HTotal,
                [Data.Models.Metadata.Display.HBEndKey] = item.HBEnd,
                [Data.Models.Metadata.Display.HBStartKey] = item.HBStart,
                [Data.Models.Metadata.Display.VTotalKey] = item.VTotal,
                [Data.Models.Metadata.Display.VBEndKey] = item.VBEnd,
                [Data.Models.Metadata.Display.VBStartKey] = item.VBStart,
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
                [Data.Models.Metadata.Driver.StatusKey] = item.Status,
                [Data.Models.Metadata.Driver.ColorKey] = item.Color,
                [Data.Models.Metadata.Driver.SoundKey] = item.Sound,
                [Data.Models.Metadata.Driver.PaletteSizeKey] = item.PaletteSize,
                [Data.Models.Metadata.Driver.EmulationKey] = item.Emulation,
                [Data.Models.Metadata.Driver.CocktailKey] = item.Cocktail,
                [Data.Models.Metadata.Driver.SaveStateKey] = item.SaveState,
                [Data.Models.Metadata.Driver.RequiresArtworkKey] = item.RequiresArtwork,
                [Data.Models.Metadata.Driver.UnofficialKey] = item.Unofficial,
                [Data.Models.Metadata.Driver.NoSoundHardwareKey] = item.NoSoundHardware,
                [Data.Models.Metadata.Driver.IncompleteKey] = item.Incomplete,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <see cref="Extension"/> to <see cref="Models.Metadata.Extension"/>
        /// </summary>
        private static Data.Models.Metadata.Extension ConvertToInternalModel(Extension item)
        {
            var extension = new Data.Models.Metadata.Extension
            {
                [Data.Models.Metadata.Extension.NameKey] = item.Name,
            };
            return extension;
        }

        /// <summary>
        /// Convert from <see cref="Feature"/> to <see cref="Models.Metadata.Feature"/>
        /// </summary>
        private static Data.Models.Metadata.Feature ConvertToInternalModel(Feature item)
        {
            var feature = new Data.Models.Metadata.Feature
            {
                [Data.Models.Metadata.Feature.FeatureTypeKey] = item.Type,
                [Data.Models.Metadata.Feature.StatusKey] = item.Status,
                [Data.Models.Metadata.Feature.OverallKey] = item.Overall,
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
                [Data.Models.Metadata.Input.ServiceKey] = item.Service,
                [Data.Models.Metadata.Input.TiltKey] = item.Tilt,
                [Data.Models.Metadata.Input.PlayersKey] = item.Players,
                [Data.Models.Metadata.Input.ControlKey] = item.ControlAttr,
                [Data.Models.Metadata.Input.ButtonsKey] = item.Buttons,
                [Data.Models.Metadata.Input.CoinsKey] = item.Coins,
            };

            if (item.Control != null && item.Control.Length > 0)
            {
                input[Data.Models.Metadata.Input.ControlKey]
                    = Array.ConvertAll(item.Control, ConvertToInternalModel);
            }

            return input;
        }

        /// <summary>
        /// Convert from <see cref="Instance"/> to <see cref="Models.Metadata.Instance"/>
        /// </summary>
        private static Data.Models.Metadata.Instance ConvertToInternalModel(Instance item)
        {
            var instance = new Data.Models.Metadata.Instance
            {
                [Data.Models.Metadata.Instance.NameKey] = item.Name,
                [Data.Models.Metadata.Instance.BriefNameKey] = item.BriefName,
            };
            return instance;
        }

        /// <summary>
        /// Convert from <see cref="Port"/> to <see cref="Models.Metadata.Port"/>
        /// </summary>
        private static Data.Models.Metadata.Port ConvertToInternalModel(Port item)
        {
            var port = new Data.Models.Metadata.Port
            {
                [Data.Models.Metadata.Port.TagKey] = item.Tag,
            };

            if (item.Analog != null && item.Analog.Length > 0)
            {
                port[Data.Models.Metadata.Port.AnalogKey]
                    = Array.ConvertAll(item.Analog, ConvertToInternalModel);
            }

            return port;
        }

        /// <summary>
        /// Convert from <see cref="RamOption"/> to <see cref="Models.Metadata.RamOption"/>
        /// </summary>
        private static Data.Models.Metadata.RamOption ConvertToInternalModel(RamOption item)
        {
            var ramOption = new Data.Models.Metadata.RamOption
            {
                [Data.Models.Metadata.RamOption.NameKey] = item.Name,
                [Data.Models.Metadata.RamOption.DefaultKey] = item.Default,
                [Data.Models.Metadata.RamOption.ContentKey] = item.Content,
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
                [Data.Models.Metadata.Rom.NameKey] = item.Name,
                [Data.Models.Metadata.Rom.BiosKey] = item.Bios,
                [Data.Models.Metadata.Rom.SizeKey] = item.Size,
                [Data.Models.Metadata.Rom.CRCKey] = item.CRC,
                [Data.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Rom.MergeKey] = item.Merge,
                [Data.Models.Metadata.Rom.RegionKey] = item.Region,
                [Data.Models.Metadata.Rom.OffsetKey] = item.Offset,
                [Data.Models.Metadata.Rom.StatusKey] = item.Status,
                [Data.Models.Metadata.Rom.OptionalKey] = item.Optional,
                [Data.Models.Metadata.Rom.DisposeKey] = item.Dispose,
                [Data.Models.Metadata.Rom.SoundOnlyKey] = item.SoundOnly,
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
                [Data.Models.Metadata.Sample.NameKey] = item.Name,
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
                [Data.Models.Metadata.Slot.NameKey] = item.Name,
            };

            if (item.SlotOption != null && item.SlotOption.Length > 0)
            {
                slot[Data.Models.Metadata.Slot.SlotOptionKey]
                    = Array.ConvertAll(item.SlotOption, ConvertToInternalModel);
            }

            return slot;
        }

        /// <summary>
        /// Convert from <see cref="SlotOption"/> to <see cref="Models.Metadata.SlotOption"/>
        /// </summary>
        private static Data.Models.Metadata.SlotOption ConvertToInternalModel(SlotOption item)
        {
            var slotOption = new Data.Models.Metadata.SlotOption
            {
                [Data.Models.Metadata.SlotOption.NameKey] = item.Name,
                [Data.Models.Metadata.SlotOption.DevNameKey] = item.DevName,
                [Data.Models.Metadata.SlotOption.DefaultKey] = item.Default,
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
                [Data.Models.Metadata.SoftwareList.TagKey] = item.Tag,
                [Data.Models.Metadata.SoftwareList.NameKey] = item.Name,
                [Data.Models.Metadata.SoftwareList.StatusKey] = item.Status,
                [Data.Models.Metadata.SoftwareList.FilterKey] = item.Filter,
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
                [Data.Models.Metadata.Sound.ChannelsKey] = item.Channels,
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
                [Data.Models.Metadata.Video.ScreenKey] = item.Screen,
                [Data.Models.Metadata.Video.OrientationKey] = item.Orientation,
                [Data.Models.Metadata.Video.WidthKey] = item.Width,
                [Data.Models.Metadata.Video.HeightKey] = item.Height,
                [Data.Models.Metadata.Video.AspectXKey] = item.AspectX,
                [Data.Models.Metadata.Video.AspectYKey] = item.AspectY,
                [Data.Models.Metadata.Video.RefreshKey] = item.Refresh,
            };
            return video;
        }
    }
}
