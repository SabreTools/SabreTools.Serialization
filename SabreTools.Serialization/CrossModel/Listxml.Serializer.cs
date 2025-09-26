using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.Listxml;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listxml : IModelSerializer<Mame, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(Mame? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile
            {
                [Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Game != null && item.Game.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(item.Game, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Mame"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(Mame item)
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.BuildKey] = item.Build,
                [Serialization.Models.Metadata.Header.DebugKey] = item.Debug,
                [Serialization.Models.Metadata.Header.MameConfigKey] = item.MameConfig,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="GameBase"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        internal static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(GameBase item)
        {
            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.NameKey] = item.Name,
                [Serialization.Models.Metadata.Machine.SourceFileKey] = item.SourceFile,
                [Serialization.Models.Metadata.Machine.IsBiosKey] = item.IsBios,
                [Serialization.Models.Metadata.Machine.IsDeviceKey] = item.IsDevice,
                [Serialization.Models.Metadata.Machine.IsMechanicalKey] = item.IsMechanical,
                [Serialization.Models.Metadata.Machine.RunnableKey] = item.Runnable,
                [Serialization.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Serialization.Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Serialization.Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
                [Serialization.Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Serialization.Models.Metadata.Machine.YearKey] = item.Year,
                [Serialization.Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Serialization.Models.Metadata.Machine.HistoryKey] = item.History,
            };

            if (item.BiosSet != null && item.BiosSet.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.BiosSetKey]
                    = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);
            }

            if (item.Rom != null && item.Rom.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.RomKey]
                    = Array.ConvertAll(item.Rom, ConvertToInternalModel);
            }

            if (item.Disk != null && item.Disk.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.DiskKey]
                    = Array.ConvertAll(item.Disk, ConvertToInternalModel);
            }

            if (item.DeviceRef != null && item.DeviceRef.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.DeviceRefKey]
                    = Array.ConvertAll(item.DeviceRef, ConvertToInternalModel);
            }

            if (item.Sample != null && item.Sample.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.SampleKey]
                    = Array.ConvertAll(item.Sample, ConvertToInternalModel);
            }

            if (item.Chip != null && item.Chip.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.ChipKey]
                    = Array.ConvertAll(item.Chip, ConvertToInternalModel);
            }

            if (item.Display != null && item.Display.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.DisplayKey]
                    = Array.ConvertAll(item.Display, ConvertToInternalModel);
            }

            if (item.Video != null && item.Video.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.VideoKey]
                    = Array.ConvertAll(item.Video, ConvertToInternalModel);
            }

            if (item.Sound != null)
                machine[Serialization.Models.Metadata.Machine.SoundKey] = ConvertToInternalModel(item.Sound);

            if (item.Input != null)
                machine[Serialization.Models.Metadata.Machine.InputKey] = ConvertToInternalModel(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.DipSwitchKey]
                    = Array.ConvertAll(item.DipSwitch, ConvertToInternalModel);
            }

            if (item.Configuration != null && item.Configuration.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.ConfigurationKey]
                    = Array.ConvertAll(item.Configuration, ConvertToInternalModel);
            }

            if (item.Port != null && item.Port.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.PortKey]
                    = Array.ConvertAll(item.Port, ConvertToInternalModel);
            }

            if (item.Adjuster != null && item.Adjuster.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.AdjusterKey]
                    = Array.ConvertAll(item.Adjuster, ConvertToInternalModel);
            }

            if (item.Driver != null)
                machine[Serialization.Models.Metadata.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            if (item.Feature != null && item.Feature.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.FeatureKey]
                    = Array.ConvertAll(item.Feature, ConvertToInternalModel);
            }

            if (item.Device != null && item.Device.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.DeviceKey]
                    = Array.ConvertAll(item.Device, ConvertToInternalModel);
            }

            if (item.Slot != null && item.Slot.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.SlotKey]
                    = Array.ConvertAll(item.Slot, ConvertToInternalModel);
            }

            if (item.SoftwareList != null && item.SoftwareList.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.SoftwareListKey]
                    = Array.ConvertAll(item.SoftwareList, ConvertToInternalModel);
            }

            if (item.RamOption != null && item.RamOption.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.RamOptionKey]
                    = Array.ConvertAll(item.RamOption, ConvertToInternalModel);
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Adjuster"/> to <see cref="Serialization.Models.Metadata.Adjuster"/>
        /// </summary>
        private static Serialization.Models.Metadata.Adjuster ConvertToInternalModel(Adjuster item)
        {
            var adjuster = new Serialization.Models.Metadata.Adjuster
            {
                [Serialization.Models.Metadata.Adjuster.NameKey] = item.Name,
                [Serialization.Models.Metadata.Adjuster.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                adjuster[Serialization.Models.Metadata.Adjuster.ConditionKey] = ConvertToInternalModel(item.Condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <see cref="Analog"/> to <see cref="Serialization.Models.Metadata.Analog"/>
        /// </summary>
        private static Serialization.Models.Metadata.Analog ConvertToInternalModel(Analog item)
        {
            var analog = new Serialization.Models.Metadata.Analog
            {
                [Serialization.Models.Metadata.Analog.MaskKey] = item.Mask,
            };
            return analog;
        }

        /// <summary>
        /// Convert from <see cref="BiosSet"/> to <see cref="Serialization.Models.Metadata.BiosSet"/>
        /// </summary>
        private static Serialization.Models.Metadata.BiosSet ConvertToInternalModel(BiosSet item)
        {
            var biosset = new Serialization.Models.Metadata.BiosSet
            {
                [Serialization.Models.Metadata.BiosSet.NameKey] = item.Name,
                [Serialization.Models.Metadata.BiosSet.DescriptionKey] = item.Description,
                [Serialization.Models.Metadata.BiosSet.DefaultKey] = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <see cref="Chip"/> to <see cref="Serialization.Models.Metadata.Chip"/>
        /// </summary>
        private static Serialization.Models.Metadata.Chip ConvertToInternalModel(Chip item)
        {
            var chip = new Serialization.Models.Metadata.Chip
            {
                [Serialization.Models.Metadata.Chip.NameKey] = item.Name,
                [Serialization.Models.Metadata.Chip.TagKey] = item.Tag,
                [Serialization.Models.Metadata.Chip.ChipTypeKey] = item.Type,
                [Serialization.Models.Metadata.Chip.SoundOnlyKey] = item.SoundOnly,
                [Serialization.Models.Metadata.Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <see cref="Condition"/> to <see cref="Serialization.Models.Metadata.Condition"/>
        /// </summary>
        private static Serialization.Models.Metadata.Condition ConvertToInternalModel(Condition item)
        {
            var condition = new Serialization.Models.Metadata.Condition
            {
                [Serialization.Models.Metadata.Condition.TagKey] = item.Tag,
                [Serialization.Models.Metadata.Condition.MaskKey] = item.Mask,
                [Serialization.Models.Metadata.Condition.RelationKey] = item.Relation,
                [Serialization.Models.Metadata.Condition.ValueKey] = item.Value,
            };
            return condition;
        }

        /// <summary>
        /// Convert from <see cref="Configuration"/> to <see cref="Serialization.Models.Metadata.Configuration"/>
        /// </summary>
        private static Serialization.Models.Metadata.Configuration ConvertToInternalModel(Configuration item)
        {
            var configuration = new Serialization.Models.Metadata.Configuration
            {
                [Serialization.Models.Metadata.Configuration.NameKey] = item.Name,
                [Serialization.Models.Metadata.Configuration.TagKey] = item.Tag,
                [Serialization.Models.Metadata.Configuration.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                configuration[Serialization.Models.Metadata.Configuration.ConditionKey] = ConvertToInternalModel(item.Condition);

            if (item.ConfLocation != null && item.ConfLocation.Length > 0)
            {
                configuration[Serialization.Models.Metadata.Configuration.ConfLocationKey]
                    = Array.ConvertAll(item.ConfLocation, ConvertToInternalModel);
            }

            if (item.ConfSetting != null && item.ConfSetting.Length > 0)
            {
                configuration[Serialization.Models.Metadata.Configuration.ConfSettingKey]
                    = Array.ConvertAll(item.ConfSetting, ConvertToInternalModel);
            }

            return configuration;
        }

        /// <summary>
        /// Convert from <see cref="ConfLocation"/> to <see cref="Serialization.Models.Metadata.ConfLocation"/>
        /// </summary>
        private static Serialization.Models.Metadata.ConfLocation ConvertToInternalModel(ConfLocation item)
        {
            var confLocation = new Serialization.Models.Metadata.ConfLocation
            {
                [Serialization.Models.Metadata.ConfLocation.NameKey] = item.Name,
                [Serialization.Models.Metadata.ConfLocation.NumberKey] = item.Number,
                [Serialization.Models.Metadata.ConfLocation.InvertedKey] = item.Inverted,
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <see cref="ConfSetting"/> to <see cref="Serialization.Models.Metadata.ConfSetting"/>
        /// </summary>
        private static Serialization.Models.Metadata.ConfSetting ConvertToInternalModel(ConfSetting item)
        {
            var confSetting = new Serialization.Models.Metadata.ConfSetting
            {
                [Serialization.Models.Metadata.ConfSetting.NameKey] = item.Name,
                [Serialization.Models.Metadata.ConfSetting.ValueKey] = item.Value,
                [Serialization.Models.Metadata.ConfSetting.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                confSetting[Serialization.Models.Metadata.ConfSetting.ConditionKey] = ConvertToInternalModel(item.Condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <see cref="Control"/> to <see cref="Serialization.Models.Metadata.Control"/>
        /// </summary>
        private static Serialization.Models.Metadata.Control ConvertToInternalModel(Control item)
        {
            var control = new Serialization.Models.Metadata.Control
            {
                [Serialization.Models.Metadata.Control.ControlTypeKey] = item.Type,
                [Serialization.Models.Metadata.Control.PlayerKey] = item.Player,
                [Serialization.Models.Metadata.Control.ButtonsKey] = item.Buttons,
                [Serialization.Models.Metadata.Control.ReqButtonsKey] = item.ReqButtons,
                [Serialization.Models.Metadata.Control.MinimumKey] = item.Minimum,
                [Serialization.Models.Metadata.Control.MaximumKey] = item.Maximum,
                [Serialization.Models.Metadata.Control.SensitivityKey] = item.Sensitivity,
                [Serialization.Models.Metadata.Control.KeyDeltaKey] = item.KeyDelta,
                [Serialization.Models.Metadata.Control.ReverseKey] = item.Reverse,
                [Serialization.Models.Metadata.Control.WaysKey] = item.Ways,
                [Serialization.Models.Metadata.Control.Ways2Key] = item.Ways2,
                [Serialization.Models.Metadata.Control.Ways3Key] = item.Ways3,
            };
            return control;
        }

        /// <summary>
        /// Convert from <see cref="Device"/> to <see cref="Serialization.Models.Metadata.Device"/>
        /// </summary>
        private static Serialization.Models.Metadata.Device ConvertToInternalModel(Device item)
        {
            var device = new Serialization.Models.Metadata.Device
            {
                [Serialization.Models.Metadata.Device.DeviceTypeKey] = item.Type,
                [Serialization.Models.Metadata.Device.TagKey] = item.Tag,
                [Serialization.Models.Metadata.Device.FixedImageKey] = item.FixedImage,
                [Serialization.Models.Metadata.Device.MandatoryKey] = item.Mandatory,
                [Serialization.Models.Metadata.Device.InterfaceKey] = item.Interface,
            };

            if (item.Instance != null)
                device[Serialization.Models.Metadata.Device.InstanceKey] = ConvertToInternalModel(item.Instance);

            if (item.Extension != null && item.Extension.Length > 0)
            {
                device[Serialization.Models.Metadata.Device.ExtensionKey]
                    = Array.ConvertAll(item.Extension, ConvertToInternalModel);
            }

            return device;
        }

        /// <summary>
        /// Convert from <see cref="DeviceRef"/> to <see cref="Serialization.Models.Metadata.DeviceRef"/>
        /// </summary>
        private static Serialization.Models.Metadata.DeviceRef ConvertToInternalModel(DeviceRef item)
        {
            var deviceRef = new Serialization.Models.Metadata.DeviceRef
            {
                [Serialization.Models.Metadata.DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <see cref="DipLocation"/> to <see cref="Serialization.Models.Metadata.DipLocation"/>
        /// </summary>
        private static Serialization.Models.Metadata.DipLocation ConvertToInternalModel(DipLocation item)
        {
            var dipLocation = new Serialization.Models.Metadata.DipLocation
            {
                [Serialization.Models.Metadata.DipLocation.NameKey] = item.Name,
                [Serialization.Models.Metadata.DipLocation.NumberKey] = item.Number,
                [Serialization.Models.Metadata.DipLocation.InvertedKey] = item.Inverted,
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <see cref="DipSwitch"/> to <see cref="Serialization.Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Serialization.Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipSwitch = new Serialization.Models.Metadata.DipSwitch
            {
                [Serialization.Models.Metadata.DipSwitch.NameKey] = item.Name,
                [Serialization.Models.Metadata.DipSwitch.TagKey] = item.Tag,
                [Serialization.Models.Metadata.DipSwitch.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                dipSwitch[Serialization.Models.Metadata.DipSwitch.ConditionKey] = ConvertToInternalModel(item.Condition);

            if (item.DipLocation != null && item.DipLocation.Length > 0)
            {
                dipSwitch[Serialization.Models.Metadata.DipSwitch.DipLocationKey]
                    = Array.ConvertAll(item.DipLocation, ConvertToInternalModel);
            }

            if (item.DipValue != null && item.DipValue.Length > 0)
            {
                dipSwitch[Serialization.Models.Metadata.DipSwitch.DipValueKey]
                    = Array.ConvertAll(item.DipValue, ConvertToInternalModel);
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <see cref="DipValue"/> to <see cref="Serialization.Models.Metadata.DipValue"/>
        /// </summary>
        private static Serialization.Models.Metadata.DipValue ConvertToInternalModel(DipValue item)
        {
            var dipValue = new Serialization.Models.Metadata.DipValue
            {
                [Serialization.Models.Metadata.DipValue.NameKey] = item.Name,
                [Serialization.Models.Metadata.DipValue.ValueKey] = item.Value,
                [Serialization.Models.Metadata.DipValue.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                dipValue[Serialization.Models.Metadata.DipValue.ConditionKey] = ConvertToInternalModel(item.Condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <see cref="Disk"/> to <see cref="Serialization.Models.Metadata.Disk"/>
        /// </summary>
        private static Serialization.Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Serialization.Models.Metadata.Disk
            {
                [Serialization.Models.Metadata.Disk.NameKey] = item.Name,
                [Serialization.Models.Metadata.Disk.MD5Key] = item.MD5,
                [Serialization.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Serialization.Models.Metadata.Disk.MergeKey] = item.Merge,
                [Serialization.Models.Metadata.Disk.RegionKey] = item.Region,
                [Serialization.Models.Metadata.Disk.IndexKey] = item.Index,
                [Serialization.Models.Metadata.Disk.WritableKey] = item.Writable,
                [Serialization.Models.Metadata.Disk.StatusKey] = item.Status,
                [Serialization.Models.Metadata.Disk.OptionalKey] = item.Optional,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Display"/> to <see cref="Serialization.Models.Metadata.Display"/>
        /// </summary>
        private static Serialization.Models.Metadata.Display ConvertToInternalModel(Display item)
        {
            var display = new Serialization.Models.Metadata.Display
            {
                [Serialization.Models.Metadata.Display.TagKey] = item.Tag,
                [Serialization.Models.Metadata.Display.DisplayTypeKey] = item.Type,
                [Serialization.Models.Metadata.Display.RotateKey] = item.Rotate,
                [Serialization.Models.Metadata.Display.FlipXKey] = item.FlipX,
                [Serialization.Models.Metadata.Display.WidthKey] = item.Width,
                [Serialization.Models.Metadata.Display.HeightKey] = item.Height,
                [Serialization.Models.Metadata.Display.RefreshKey] = item.Refresh,
                [Serialization.Models.Metadata.Display.PixClockKey] = item.PixClock,
                [Serialization.Models.Metadata.Display.HTotalKey] = item.HTotal,
                [Serialization.Models.Metadata.Display.HBEndKey] = item.HBEnd,
                [Serialization.Models.Metadata.Display.HBStartKey] = item.HBStart,
                [Serialization.Models.Metadata.Display.VTotalKey] = item.VTotal,
                [Serialization.Models.Metadata.Display.VBEndKey] = item.VBEnd,
                [Serialization.Models.Metadata.Display.VBStartKey] = item.VBStart,
            };
            return display;
        }

        /// <summary>
        /// Convert from <see cref="Driver"/> to <see cref="Serialization.Models.Metadata.Driver"/>
        /// </summary>
        private static Serialization.Models.Metadata.Driver ConvertToInternalModel(Driver item)
        {
            var driver = new Serialization.Models.Metadata.Driver
            {
                [Serialization.Models.Metadata.Driver.StatusKey] = item.Status,
                [Serialization.Models.Metadata.Driver.ColorKey] = item.Color,
                [Serialization.Models.Metadata.Driver.SoundKey] = item.Sound,
                [Serialization.Models.Metadata.Driver.PaletteSizeKey] = item.PaletteSize,
                [Serialization.Models.Metadata.Driver.EmulationKey] = item.Emulation,
                [Serialization.Models.Metadata.Driver.CocktailKey] = item.Cocktail,
                [Serialization.Models.Metadata.Driver.SaveStateKey] = item.SaveState,
                [Serialization.Models.Metadata.Driver.RequiresArtworkKey] = item.RequiresArtwork,
                [Serialization.Models.Metadata.Driver.UnofficialKey] = item.Unofficial,
                [Serialization.Models.Metadata.Driver.NoSoundHardwareKey] = item.NoSoundHardware,
                [Serialization.Models.Metadata.Driver.IncompleteKey] = item.Incomplete,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <see cref="Extension"/> to <see cref="Serialization.Models.Metadata.Extension"/>
        /// </summary>
        private static Serialization.Models.Metadata.Extension ConvertToInternalModel(Extension item)
        {
            var extension = new Serialization.Models.Metadata.Extension
            {
                [Serialization.Models.Metadata.Extension.NameKey] = item.Name,
            };
            return extension;
        }

        /// <summary>
        /// Convert from <see cref="Feature"/> to <see cref="Serialization.Models.Metadata.Feature"/>
        /// </summary>
        private static Serialization.Models.Metadata.Feature ConvertToInternalModel(Feature item)
        {
            var feature = new Serialization.Models.Metadata.Feature
            {
                [Serialization.Models.Metadata.Feature.FeatureTypeKey] = item.Type,
                [Serialization.Models.Metadata.Feature.StatusKey] = item.Status,
                [Serialization.Models.Metadata.Feature.OverallKey] = item.Overall,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <see cref="Input"/> to <see cref="Serialization.Models.Metadata.Input"/>
        /// </summary>
        private static Serialization.Models.Metadata.Input ConvertToInternalModel(Input item)
        {
            var input = new Serialization.Models.Metadata.Input
            {
                [Serialization.Models.Metadata.Input.ServiceKey] = item.Service,
                [Serialization.Models.Metadata.Input.TiltKey] = item.Tilt,
                [Serialization.Models.Metadata.Input.PlayersKey] = item.Players,
                [Serialization.Models.Metadata.Input.ControlKey] = item.ControlAttr,
                [Serialization.Models.Metadata.Input.ButtonsKey] = item.Buttons,
                [Serialization.Models.Metadata.Input.CoinsKey] = item.Coins,
            };

            if (item.Control != null && item.Control.Length > 0)
            {
                input[Serialization.Models.Metadata.Input.ControlKey]
                    = Array.ConvertAll(item.Control, ConvertToInternalModel);
            }

            return input;
        }

        /// <summary>
        /// Convert from <see cref="Instance"/> to <see cref="Serialization.Models.Metadata.Instance"/>
        /// </summary>
        private static Serialization.Models.Metadata.Instance ConvertToInternalModel(Instance item)
        {
            var instance = new Serialization.Models.Metadata.Instance
            {
                [Serialization.Models.Metadata.Instance.NameKey] = item.Name,
                [Serialization.Models.Metadata.Instance.BriefNameKey] = item.BriefName,
            };
            return instance;
        }

        /// <summary>
        /// Convert from <see cref="Port"/> to <see cref="Serialization.Models.Metadata.Port"/>
        /// </summary>
        private static Serialization.Models.Metadata.Port ConvertToInternalModel(Port item)
        {
            var port = new Serialization.Models.Metadata.Port
            {
                [Serialization.Models.Metadata.Port.TagKey] = item.Tag,
            };

            if (item.Analog != null && item.Analog.Length > 0)
            {
                port[Serialization.Models.Metadata.Port.AnalogKey]
                    = Array.ConvertAll(item.Analog, ConvertToInternalModel);
            }

            return port;
        }

        /// <summary>
        /// Convert from <see cref="RamOption"/> to <see cref="Serialization.Models.Metadata.RamOption"/>
        /// </summary>
        private static Serialization.Models.Metadata.RamOption ConvertToInternalModel(RamOption item)
        {
            var ramOption = new Serialization.Models.Metadata.RamOption
            {
                [Serialization.Models.Metadata.RamOption.NameKey] = item.Name,
                [Serialization.Models.Metadata.RamOption.DefaultKey] = item.Default,
                [Serialization.Models.Metadata.RamOption.ContentKey] = item.Content,
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <see cref="Rom"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        private static Serialization.Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Serialization.Models.Metadata.Rom
            {
                [Serialization.Models.Metadata.Rom.NameKey] = item.Name,
                [Serialization.Models.Metadata.Rom.BiosKey] = item.Bios,
                [Serialization.Models.Metadata.Rom.SizeKey] = item.Size,
                [Serialization.Models.Metadata.Rom.CRCKey] = item.CRC,
                [Serialization.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Serialization.Models.Metadata.Rom.MergeKey] = item.Merge,
                [Serialization.Models.Metadata.Rom.RegionKey] = item.Region,
                [Serialization.Models.Metadata.Rom.OffsetKey] = item.Offset,
                [Serialization.Models.Metadata.Rom.StatusKey] = item.Status,
                [Serialization.Models.Metadata.Rom.OptionalKey] = item.Optional,
                [Serialization.Models.Metadata.Rom.DisposeKey] = item.Dispose,
                [Serialization.Models.Metadata.Rom.SoundOnlyKey] = item.SoundOnly,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Sample"/> to <see cref="Serialization.Models.Metadata.Sample"/>
        /// </summary>
        private static Serialization.Models.Metadata.Sample ConvertToInternalModel(Sample item)
        {
            var sample = new Serialization.Models.Metadata.Sample
            {
                [Serialization.Models.Metadata.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <see cref="Slot"/> to <see cref="Serialization.Models.Metadata.Slot"/>
        /// </summary>
        private static Serialization.Models.Metadata.Slot ConvertToInternalModel(Slot item)
        {
            var slot = new Serialization.Models.Metadata.Slot
            {
                [Serialization.Models.Metadata.Slot.NameKey] = item.Name,
            };

            if (item.SlotOption != null && item.SlotOption.Length > 0)
            {
                slot[Serialization.Models.Metadata.Slot.SlotOptionKey]
                    = Array.ConvertAll(item.SlotOption, ConvertToInternalModel);
            }

            return slot;
        }

        /// <summary>
        /// Convert from <see cref="SlotOption"/> to <see cref="Serialization.Models.Metadata.SlotOption"/>
        /// </summary>
        private static Serialization.Models.Metadata.SlotOption ConvertToInternalModel(SlotOption item)
        {
            var slotOption = new Serialization.Models.Metadata.SlotOption
            {
                [Serialization.Models.Metadata.SlotOption.NameKey] = item.Name,
                [Serialization.Models.Metadata.SlotOption.DevNameKey] = item.DevName,
                [Serialization.Models.Metadata.SlotOption.DefaultKey] = item.Default,
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <see cref="SabreTools.Serialization.Models.Listxml.SoftwareList"/> to <see cref="Serialization.Models.Metadata.SoftwareList"/>
        /// </summary>
        private static Serialization.Models.Metadata.SoftwareList ConvertToInternalModel(SabreTools.Serialization.Models.Listxml.SoftwareList item)
        {
            var softwareList = new Serialization.Models.Metadata.SoftwareList
            {
                [Serialization.Models.Metadata.SoftwareList.TagKey] = item.Tag,
                [Serialization.Models.Metadata.SoftwareList.NameKey] = item.Name,
                [Serialization.Models.Metadata.SoftwareList.StatusKey] = item.Status,
                [Serialization.Models.Metadata.SoftwareList.FilterKey] = item.Filter,
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <see cref="Sound"/> to <see cref="Serialization.Models.Metadata.Sound"/>
        /// </summary>
        private static Serialization.Models.Metadata.Sound ConvertToInternalModel(Sound item)
        {
            var sound = new Serialization.Models.Metadata.Sound
            {
                [Serialization.Models.Metadata.Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <see cref="Video"/> to <see cref="Serialization.Models.Metadata.Video"/>
        /// </summary>
        private static Serialization.Models.Metadata.Video ConvertToInternalModel(Video item)
        {
            var video = new Serialization.Models.Metadata.Video
            {
                [Serialization.Models.Metadata.Video.ScreenKey] = item.Screen,
                [Serialization.Models.Metadata.Video.OrientationKey] = item.Orientation,
                [Serialization.Models.Metadata.Video.WidthKey] = item.Width,
                [Serialization.Models.Metadata.Video.HeightKey] = item.Height,
                [Serialization.Models.Metadata.Video.AspectXKey] = item.AspectX,
                [Serialization.Models.Metadata.Video.AspectYKey] = item.AspectY,
                [Serialization.Models.Metadata.Video.RefreshKey] = item.Refresh,
            };
            return video;
        }
    }
}
