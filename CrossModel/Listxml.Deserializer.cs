using System.Linq;
using SabreTools.Models.Listxml;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listxml : IModelSerializer<Mame, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public Mame Deserialize(Models.Metadata.MetadataFile obj)
#else
        public Mame? Deserialize(Models.Metadata.MetadataFile? obj)
#endif
        {
            if (obj == null)
                return null;

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var mame = header != null ? ConvertMameFromInternalModel(header) : new Mame();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                mame.Game = machines
                    .Where(m => m != null)
                    .Select(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return mame;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.MetadataFile"/> to <cref="Models.Listxml.Mame"/>
        /// </summary>
#if NET48
        private static Mame ConvertMameFromInternalModel(Models.Metadata.MetadataFile item)
#else
        public static Mame? ConvertMameFromInternalModel(Models.Metadata.MetadataFile? item)
#endif
        {
            if (item == null)
                return null;

            var header = item.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var mame = header != null ? ConvertMameFromInternalModel(header) : new Mame();

            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                mame.Game = machines
                    .Where(m => m != null)
                    .Select(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return mame;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.Header"/> to <cref="Models.Listxml.Mame"/>
        /// </summary>
        private static Mame ConvertMameFromInternalModel(Models.Metadata.Header item)
        {
            var mame = new Mame
            {
                Build = item.ReadString(Models.Metadata.Header.BuildKey),
                Debug = item.ReadString(Models.Metadata.Header.DebugKey),
                MameConfig = item.ReadString(Models.Metadata.Header.MameConfigKey),
            };

            return mame;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.Machine"/> to <cref="Models.Listxml.GameBase"/>
        /// </summary>
        internal static GameBase ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var machine = new Machine
            {
                Name = item.ReadString(Models.Metadata.Machine.NameKey),
                SourceFile = item.ReadString(Models.Metadata.Machine.SourceFileKey),
                IsBios = item.ReadString(Models.Metadata.Machine.IsBiosKey),
                IsDevice = item.ReadString(Models.Metadata.Machine.IsDeviceKey),
                IsMechanical = item.ReadString(Models.Metadata.Machine.IsMechanicalKey),
                Runnable = item.ReadString(Models.Metadata.Machine.RunnableKey),
                CloneOf = item.ReadString(Models.Metadata.Machine.CloneOfKey),
                RomOf = item.ReadString(Models.Metadata.Machine.RomOfKey),
                SampleOf = item.ReadString(Models.Metadata.Machine.SampleOfKey),
                Description = item.ReadString(Models.Metadata.Machine.DescriptionKey),
                Year = item.ReadString(Models.Metadata.Machine.YearKey),
                Manufacturer = item.ReadString(Models.Metadata.Machine.ManufacturerKey),
                History = item.ReadString(Models.Metadata.Machine.HistoryKey),
            };

            var biosSets = item.Read<Models.Metadata.BiosSet[]>(Models.Metadata.Machine.BiosSetKey);
            if (biosSets != null && biosSets.Any())
            {
                machine.BiosSet = biosSets
                    .Where(b => b != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Any())
            {
                machine.Rom = roms
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Any())
            {
                machine.Disk = disks
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var deviceRefs = item.Read<Models.Metadata.DeviceRef[]>(Models.Metadata.Machine.DeviceRefKey);
            if (deviceRefs != null && deviceRefs.Any())
            {
                machine.DeviceRef = deviceRefs
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var samples = item.Read<Models.Metadata.Sample[]>(Models.Metadata.Machine.SampleKey);
            if (samples != null && samples.Any())
            {
                machine.Sample = samples
                    .Where(s => s != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var chips = item.Read<Models.Metadata.Chip[]>(Models.Metadata.Machine.ChipKey);
            if (chips != null && chips.Any())
            {
                machine.Chip = chips
                    .Where(c => c != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var displays = item.Read<Models.Metadata.Display[]>(Models.Metadata.Machine.DisplayKey);
            if (displays != null && displays.Any())
            {
                machine.Display = displays
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var videos = item.Read<Models.Metadata.Video[]>(Models.Metadata.Machine.VideoKey);
            if (videos != null && videos.Any())
            {
                machine.Video = videos
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var sound = item.Read<Models.Metadata.Sound>(Models.Metadata.Machine.SoundKey);
            if (sound != null)
                machine.Sound = ConvertFromInternalModel(sound);

            var input = item.Read<Models.Metadata.Input>(Models.Metadata.Machine.InputKey);
            if (input != null)
                machine.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.Read<Models.Metadata.DipSwitch[]>(Models.Metadata.Machine.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Any())
            {
                machine.DipSwitch = dipSwitches
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var configurations = item.Read<Models.Metadata.Configuration[]>(Models.Metadata.Machine.ConfigurationKey);
            if (configurations != null && configurations.Any())
            {
                machine.Configuration = configurations
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var ports = item.Read<Models.Metadata.Port[]>(Models.Metadata.Machine.PortKey);
            if (ports != null && ports.Any())
            {
                machine.Port = ports
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var adjusters = item.Read<Models.Metadata.Adjuster[]>(Models.Metadata.Machine.AdjusterKey);
            if (adjusters != null && adjusters.Any())
            {
                machine.Adjuster = adjusters
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var driver = item.Read<Models.Metadata.Driver>(Models.Metadata.Machine.DriverKey);
            if (driver != null)
                machine.Driver = ConvertFromInternalModel(driver);

            var features = item.Read<Models.Metadata.Feature[]>(Models.Metadata.Machine.FeatureKey);
            if (features != null && features.Any())
            {
                machine.Feature = features
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var devices = item.Read<Models.Metadata.Device[]>(Models.Metadata.Machine.DeviceKey);
            if (devices != null && devices.Any())
            {
                machine.Device = devices
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var slots = item.Read<Models.Metadata.Slot[]>(Models.Metadata.Machine.SlotKey);
            if (slots != null && slots.Any())
            {
                machine.Slot = slots
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var softwareLists = item.Read<Models.Metadata.SoftwareList[]>(Models.Metadata.Machine.SoftwareListKey);
            if (softwareLists != null && softwareLists.Any())
            {
                machine.SoftwareList = softwareLists
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var ramOptions = item.Read<Models.Metadata.RamOption[]>(Models.Metadata.Machine.RamOptionKey);
            if (ramOptions != null && ramOptions.Any())
            {
                machine.RamOption = ramOptions
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.Adjuster"/> to <cref="Models.Listxml.Adjuster"/>
        /// </summary>
        private static Adjuster ConvertFromInternalModel(Models.Metadata.Adjuster item)
        {
            var adjuster = new Adjuster
            {
                Name = item.ReadString(Models.Metadata.Adjuster.NameKey),
                Default = item.ReadString(Models.Metadata.Adjuster.DefaultKey),
            };

            var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.Adjuster.ConditionKey);
            if (condition != null)
                adjuster.Condition = ConvertFromInternalModel(condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.Analog"/> to <cref="Models.Listxml.Analog"/>
        /// </summary>
        private static Analog ConvertFromInternalModel(Models.Metadata.Analog item)
        {
            var analog = new Analog
            {
                Mask = item.ReadString(Models.Metadata.Analog.MaskKey),
            };
            return analog;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.BiosSet"/> to <cref="Models.Listxml.BiosSet"/>
        /// </summary>
        private static BiosSet ConvertFromInternalModel(Models.Metadata.BiosSet item)
        {
            var biosset = new BiosSet
            {
                Name = item.ReadString(Models.Metadata.BiosSet.NameKey),
                Description = item.ReadString(Models.Metadata.BiosSet.DescriptionKey),
                Default = item.ReadString(Models.Metadata.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.Chip"/> to <cref="Models.Listxml.Chip"/>
        /// </summary>
        private static Chip ConvertFromInternalModel(Models.Metadata.Chip item)
        {
            var chip = new Chip
            {
                Name = item.ReadString(Models.Metadata.Chip.NameKey),
                Tag = item.ReadString(Models.Metadata.Chip.TagKey),
                Type = item.ReadString(Models.Metadata.Chip.TypeKey),
                SoundOnly = item.ReadString(Models.Metadata.Chip.SoundOnlyKey),
                Clock = item.ReadString(Models.Metadata.Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.Condition"/> to <cref="Models.Listxml.Condition"/>
        /// </summary>
        private static Condition ConvertFromInternalModel(Models.Metadata.Condition item)
        {
            var condition = new Condition
            {
                Tag = item.ReadString(Models.Metadata.Condition.TagKey),
                Mask = item.ReadString(Models.Metadata.Condition.MaskKey),
                Relation = item.ReadString(Models.Metadata.Condition.RelationKey),
                Value = item.ReadString(Models.Metadata.Condition.ValueKey),
            };
            return condition;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.Configuration"/> to <cref="Models.Listxml.Configuration"/>
        /// </summary>
        private static Configuration ConvertFromInternalModel(Models.Metadata.Configuration item)
        {
            var configuration = new Configuration
            {
                Name = item.ReadString(Models.Metadata.Configuration.NameKey),
                Tag = item.ReadString(Models.Metadata.Configuration.TagKey),
                Mask = item.ReadString(Models.Metadata.Configuration.MaskKey),
            };

            var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.Configuration.ConditionKey);
            if (condition != null)
                configuration.Condition = ConvertFromInternalModel(condition);

            var confLocations = item.Read<Models.Metadata.ConfLocation[]>(Models.Metadata.Configuration.ConfLocationKey);
            if (confLocations != null && confLocations.Any())
            {
                configuration.ConfLocation = confLocations
                    .Where(c => c != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var confSettings = item.Read<Models.Metadata.ConfSetting[]>(Models.Metadata.Configuration.ConfSettingKey);
            if (confSettings != null && confSettings.Any())
            {
                configuration.ConfSetting = confSettings
                    .Where(c => c != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return configuration;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.ConfLocation"/> to <cref="Models.Listxml.ConfLocation"/>
        /// </summary>
        private static ConfLocation ConvertFromInternalModel(Models.Metadata.ConfLocation item)
        {
            var confLocation = new ConfLocation
            {
                Name = item.ReadString(Models.Metadata.ConfLocation.NameKey),
                Number = item.ReadString(Models.Metadata.ConfLocation.NumberKey),
                Inverted = item.ReadString(Models.Metadata.ConfLocation.InvertedKey),
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.ConfSetting"/> to <cref="Models.Listxml.ConfSetting"/>
        /// </summary>
        private static ConfSetting ConvertFromInternalModel(Models.Metadata.ConfSetting item)
        {
            var confSetting = new ConfSetting
            {
                Name = item.ReadString(Models.Metadata.ConfSetting.NameKey),
                Value = item.ReadString(Models.Metadata.ConfSetting.ValueKey),
                Default = item.ReadString(Models.Metadata.ConfSetting.DefaultKey),
            };

            var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.ConfSetting.ConditionKey);
            if (condition != null)
                confSetting.Condition = ConvertFromInternalModel(condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.Control"/> to <cref="Models.Listxml.Control"/>
        /// </summary>
        private static Control ConvertFromInternalModel(Models.Metadata.Control item)
        {
            var control = new Control
            {
                Type = item.ReadString(Models.Metadata.Control.TypeKey),
                Player = item.ReadString(Models.Metadata.Control.PlayerKey),
                Buttons = item.ReadString(Models.Metadata.Control.ButtonsKey),
                ReqButtons = item.ReadString(Models.Metadata.Control.ReqButtonsKey),
                Minimum = item.ReadString(Models.Metadata.Control.MinimumKey),
                Maximum = item.ReadString(Models.Metadata.Control.MaximumKey),
                Sensitivity = item.ReadString(Models.Metadata.Control.SensitivityKey),
                KeyDelta = item.ReadString(Models.Metadata.Control.KeyDeltaKey),
                Reverse = item.ReadString(Models.Metadata.Control.ReverseKey),
                Ways = item.ReadString(Models.Metadata.Control.WaysKey),
                Ways2 = item.ReadString(Models.Metadata.Control.Ways2Key),
                Ways3 = item.ReadString(Models.Metadata.Control.Ways3Key),
            };
            return control;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Models.Metadata.Device"/> to <cref="Models.Listxml.Device"/>
        /// </summary>
        private static Device ConvertFromInternalModel(Models.Metadata.Device item)
        {
            var device = new Device
            {
                Type = item.ReadString(Models.Metadata.Device.TypeKey),
                Tag = item.ReadString(Models.Metadata.Device.TagKey),
                FixedImage = item.ReadString(Models.Metadata.Device.FixedImageKey),
                Mandatory = item.ReadString(Models.Metadata.Device.MandatoryKey),
                Interface = item.ReadString(Models.Metadata.Device.InterfaceKey),
            };

            var instance = item.Read<Models.Metadata.Instance>(Models.Metadata.Device.InstanceKey);
            if (instance != null)
                device.Instance = ConvertFromInternalModel(instance);

            var extensions = item.Read<Models.Metadata.Extension[]>(Models.Metadata.Device.ExtensionKey);
            if (extensions != null && extensions.Any())
            {
                device.Extension = extensions
                    .Where(e => e != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return device;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.DeviceRef"/> to <cref="Models.Listxml.DeviceRef"/>
        /// </summary>
        private static DeviceRef ConvertFromInternalModel(Models.Metadata.DeviceRef item)
        {
            var deviceRef = new DeviceRef
            {
                Name = item.ReadString(Models.Metadata.DeviceRef.NameKey),
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.DipLocation"/> to <cref="Models.Listxml.DipLocation"/>
        /// </summary>
        private static DipLocation ConvertFromInternalModel(Models.Metadata.DipLocation item)
        {
            var dipLocation = new DipLocation
            {
                Name = item.ReadString(Models.Metadata.DipLocation.NameKey),
                Number = item.ReadString(Models.Metadata.DipLocation.NumberKey),
                Inverted = item.ReadString(Models.Metadata.DipLocation.InvertedKey),
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.DipSwitch"/> to <cref="Models.Listxml.DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Models.Metadata.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                Name = item.ReadString(Models.Metadata.DipSwitch.NameKey),
                Tag = item.ReadString(Models.Metadata.DipSwitch.TagKey),
                Mask = item.ReadString(Models.Metadata.DipSwitch.MaskKey),
            };

            var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.DipSwitch.ConditionKey);
            if (condition != null)
                dipSwitch.Condition = ConvertFromInternalModel(condition);

            var dipLocations = item.Read<Models.Metadata.DipLocation[]>(Models.Metadata.DipSwitch.DipLocationKey);
            if (dipLocations != null && dipLocations.Any())
            {
                dipSwitch.DipLocation = dipLocations
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var dipValues = item.Read<Models.Metadata.DipValue[]>(Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues != null && dipValues.Any())
            {
                dipSwitch.DipValue = dipValues
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.DipValue"/> to <cref="Models.Listxml.DipValue"/>
        /// </summary>
        private static DipValue ConvertFromInternalModel(Models.Metadata.DipValue item)
        {
            var dipValue = new DipValue
            {
                Name = item.ReadString(Models.Metadata.DipValue.NameKey),
                Value = item.ReadString(Models.Metadata.DipValue.ValueKey),
                Default = item.ReadString(Models.Metadata.DipValue.DefaultKey),
            };

            var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.DipValue.ConditionKey);
            if (condition != null)
                dipValue.Condition = ConvertFromInternalModel(condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Disk"/> to <cref="Models.Listxml.Disk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.ReadString(Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Disk.SHA1Key),
                Merge = item.ReadString(Models.Metadata.Disk.MergeKey),
                Region = item.ReadString(Models.Metadata.Disk.RegionKey),
                Index = item.ReadString(Models.Metadata.Disk.IndexKey),
                Writable = item.ReadString(Models.Metadata.Disk.WritableKey),
                Status = item.ReadString(Models.Metadata.Disk.StatusKey),
                Optional = item.ReadString(Models.Metadata.Disk.OptionalKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Display"/> to <cref="Models.Listxml.Display"/>
        /// </summary>
        private static Display ConvertFromInternalModel(Models.Metadata.Display item)
        {
            var display = new Display
            {
                Tag = item.ReadString(Models.Metadata.Display.TagKey),
                Type = item.ReadString(Models.Metadata.Display.TypeKey),
                Rotate = item.ReadString(Models.Metadata.Display.RotateKey),
                FlipX = item.ReadString(Models.Metadata.Display.FlipXKey),
                Width = item.ReadString(Models.Metadata.Display.WidthKey),
                Height = item.ReadString(Models.Metadata.Display.HeightKey),
                Refresh = item.ReadString(Models.Metadata.Display.RefreshKey),
                PixClock = item.ReadString(Models.Metadata.Display.PixClockKey),
                HTotal = item.ReadString(Models.Metadata.Display.HTotalKey),
                HBEnd = item.ReadString(Models.Metadata.Display.HBEndKey),
                HBStart = item.ReadString(Models.Metadata.Display.HBStartKey),
                VTotal = item.ReadString(Models.Metadata.Display.VTotalKey),
                VBEnd = item.ReadString(Models.Metadata.Display.VBEndKey),
                VBStart = item.ReadString(Models.Metadata.Display.VBStartKey),
            };
            return display;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Driver"/> to <cref="Models.Listxml.Driver"/>
        /// </summary>
        private static Driver ConvertFromInternalModel(Models.Metadata.Driver item)
        {
            var driver = new Driver
            {
                Status = item.ReadString(Models.Metadata.Driver.StatusKey),
                Color = item.ReadString(Models.Metadata.Driver.ColorKey),
                Sound = item.ReadString(Models.Metadata.Driver.SoundKey),
                PaletteSize = item.ReadString(Models.Metadata.Driver.PaletteSizeKey),
                Emulation = item.ReadString(Models.Metadata.Driver.EmulationKey),
                Cocktail = item.ReadString(Models.Metadata.Driver.CocktailKey),
                SaveState = item.ReadString(Models.Metadata.Driver.SaveStateKey),
                RequiresArtwork = item.ReadString(Models.Metadata.Driver.RequiresArtworkKey),
                Unofficial = item.ReadString(Models.Metadata.Driver.UnofficialKey),
                NoSoundHardware = item.ReadString(Models.Metadata.Driver.NoSoundHardwareKey),
                Incomplete = item.ReadString(Models.Metadata.Driver.IncompleteKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Extension"/> to <cref="Models.Listxml.Extension"/>
        /// </summary>
        private static Extension ConvertFromInternalModel(Models.Metadata.Extension item)
        {
            var extension = new Extension
            {
                Name = item.ReadString(Models.Metadata.Extension.NameKey),
            };
            return extension;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Feature"/> to <cref="Models.Listxml.Feature"/>
        /// </summary>
        private static Feature ConvertFromInternalModel(Models.Metadata.Feature item)
        {
            var feature = new Feature
            {
                Type = item.ReadString(Models.Metadata.Feature.TypeKey),
                Status = item.ReadString(Models.Metadata.Feature.StatusKey),
                Overall = item.ReadString(Models.Metadata.Feature.OverallKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Input"/> to <cref="Models.Listxml.Input"/>
        /// </summary>
        private static Input ConvertFromInternalModel(Models.Metadata.Input item)
        {
            var input = new Input
            {
                Service = item.ReadString(Models.Metadata.Input.ServiceKey),
                Tilt = item.ReadString(Models.Metadata.Input.TiltKey),
                Players = item.ReadString(Models.Metadata.Input.PlayersKey),
                ControlAttr = item.ReadString(Models.Metadata.Input.ControlKey),
                Buttons = item.ReadString(Models.Metadata.Input.ButtonsKey),
                Coins = item.ReadString(Models.Metadata.Input.CoinsKey),
            };

            var controls = item.Read<Models.Metadata.Control[]>(Models.Metadata.Input.ControlKey);
            if (controls != null && controls.Any())
            {
                input.Control = controls
                    .Where(c => c != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Instance"/> to <cref="Models.Listxml.Instance"/>
        /// </summary>
        private static Instance ConvertFromInternalModel(Models.Metadata.Instance item)
        {
            var instance = new Instance
            {
                Name = item.ReadString(Models.Metadata.Instance.NameKey),
                BriefName = item.ReadString(Models.Metadata.Instance.BriefNameKey),
            };
            return instance;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Port"/> to <cref="Models.Listxml.Port"/>
        /// </summary>
        private static Port ConvertFromInternalModel(Models.Metadata.Port item)
        {
            var port = new Port
            {
                Tag = item.ReadString(Models.Metadata.Port.TagKey),
            };

            var analogs = item.Read<Models.Metadata.Analog[]>(Models.Metadata.Port.AnalogKey);
            if (analogs != null && analogs.Any())
            {
                port.Analog = analogs
                    .Where(a => a != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return port;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.RamOption"/> to <cref="Models.Listxml.RamOption"/>
        /// </summary>
        private static RamOption ConvertFromInternalModel(Models.Metadata.RamOption item)
        {
            var ramOption = new RamOption
            {
                Name = item.ReadString(Models.Metadata.RamOption.NameKey),
                Default = item.ReadString(Models.Metadata.RamOption.DefaultKey),
                Content = item.ReadString(Models.Metadata.RamOption.ContentKey),
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Listxml.Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                Bios = item.ReadString(Models.Metadata.Rom.BiosKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
                Merge = item.ReadString(Models.Metadata.Rom.MergeKey),
                Region = item.ReadString(Models.Metadata.Rom.RegionKey),
                Offset = item.ReadString(Models.Metadata.Rom.OffsetKey),
                Status = item.ReadString(Models.Metadata.Rom.StatusKey),
                Optional = item.ReadString(Models.Metadata.Rom.OptionalKey),
                Dispose = item.ReadString(Models.Metadata.Rom.DisposeKey),
                SoundOnly = item.ReadString(Models.Metadata.Rom.SoundOnlyKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Sample"/> to <cref="Models.Listxml.Sample"/>
        /// </summary>
        private static Sample ConvertFromInternalModel(Models.Metadata.Sample item)
        {
            var sample = new Sample
            {
                Name = item.ReadString(Models.Metadata.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Slot"/> to <cref="Models.Listxml.Slot"/>
        /// </summary>
        private static Slot ConvertFromInternalModel(Models.Metadata.Slot item)
        {
            var slot = new Slot
            {
                Name = item.ReadString(Models.Metadata.Slot.NameKey),
            };

            var slotOptions = item.Read<Models.Metadata.SlotOption[]>(Models.Metadata.Slot.SlotOptionKey);
            if (slotOptions != null && slotOptions.Any())
            {
                slot.SlotOption = slotOptions
                    .Where(s => s != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return slot;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.SlotOption"/> to <cref="Models.Listxml.SlotOption"/>
        /// </summary>
        private static SlotOption ConvertFromInternalModel(Models.Metadata.SlotOption item)
        {
            var slotOption = new SlotOption
            {
                Name = item.ReadString(Models.Metadata.SlotOption.NameKey),
                DevName = item.ReadString(Models.Metadata.SlotOption.DevNameKey),
                Default = item.ReadString(Models.Metadata.SlotOption.DefaultKey),
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.SoftwareList"/> to <cref="Models.Listxml.SoftwareList"/>
        /// </summary>
        private static Models.Listxml.SoftwareList ConvertFromInternalModel(Models.Metadata.SoftwareList item)
        {
            var softwareList = new Models.Listxml.SoftwareList
            {
                Tag = item.ReadString(Models.Metadata.SoftwareList.TagKey),
                Name = item.ReadString(Models.Metadata.SoftwareList.NameKey),
                Status = item.ReadString(Models.Metadata.SoftwareList.StatusKey),
                Filter = item.ReadString(Models.Metadata.SoftwareList.FilterKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Sound"/> to <cref="Models.Listxml.Sound"/>
        /// </summary>
        private static Sound ConvertFromInternalModel(Models.Metadata.Sound item)
        {
            var sound = new Sound
            {
                Channels = item.ReadString(Models.Metadata.Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Video"/> to <cref="Models.Listxml.Video"/>
        /// </summary>
        private static Video ConvertFromInternalModel(Models.Metadata.Video item)
        {
            var video = new Video
            {
                Screen = item.ReadString(Models.Metadata.Video.ScreenKey),
                Orientation = item.ReadString(Models.Metadata.Video.OrientationKey),
                Width = item.ReadString(Models.Metadata.Video.WidthKey),
                Height = item.ReadString(Models.Metadata.Video.HeightKey),
                AspectX = item.ReadString(Models.Metadata.Video.AspectXKey),
                AspectY = item.ReadString(Models.Metadata.Video.AspectYKey),
                Refresh = item.ReadString(Models.Metadata.Video.RefreshKey),
            };
            return video;
        }
    }
}