using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Listxml;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class Mess : BaseBinaryReader<Data.Models.Listxml.Mess>
    {
        /// <inheritdoc/>
        public override Data.Models.Listxml.Mess? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create the XmlTextReader
                var reader = new XmlTextReader(data);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                // Parse the XML, if possible
                Data.Models.Listxml.Mess? mess = null;
                while (reader.Read())
                {
                    // An ending element means exit
                    if (reader.NodeType == XmlNodeType.EndElement)
                        break;

                    // Only process starting elements
                    if (!reader.IsStartElement())
                        continue;

                    switch (reader.Name)
                    {
                        case "mess":
                            if (mess is not null && Debug)
                                Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                            mess = ParseMess(reader);
                            break;

                        default:
                            if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                            break;
                    }
                }

                return mess;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Mess
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Mess on success, null on error</returns>
        public Data.Models.Listxml.Mess ParseMess(XmlTextReader reader)
        {
            var obj = new Data.Models.Listxml.Mess();

            obj.Version = reader.GetAttribute("version");

            List<GameBase> games = [];
            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "machine":
                    case "game":
                        var game = ParseGameBase(reader);
                        if (game is not null)
                            games.Add(game);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            obj.Game = [.. games];

            return obj;
        }

        #region Items

        /// <summary>
        /// Parse from an XmlTextReader into a Adjuster
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Adjuster on success, null on error</returns>
        public Adjuster ParseAdjuster(XmlTextReader reader)
        {
            var obj = new Adjuster();

            obj.Name = reader.GetAttribute("name");
            obj.Default = reader.GetAttribute("default").AsYesNo();

            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "condition":
                        if (obj.Condition is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Condition = ParseCondition(reader);
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Analog
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Analog on success, null on error</returns>
        public Analog ParseAnalog(XmlTextReader reader)
        {
            var obj = new Analog();

            obj.Mask = reader.GetAttribute("mask");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a BiosSet
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled BiosSet on success, null on error</returns>
        public BiosSet ParseBiosSet(XmlTextReader reader)
        {
            var obj = new BiosSet();

            obj.Name = reader.GetAttribute("name");
            obj.Description = reader.GetAttribute("description");
            obj.Default = reader.GetAttribute("default").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Chip
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Chip on success, null on error</returns>
        public Chip ParseChip(XmlTextReader reader)
        {
            var obj = new Chip();

            obj.Name = reader.GetAttribute("name");
            obj.Tag = reader.GetAttribute("tag");
            obj.Type = reader.GetAttribute("type").AsChipType();
            obj.SoundOnly = reader.GetAttribute("soundonly").AsYesNo();
            obj.Clock = reader.GetAttribute("clock");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Condition
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Condition on success, null on error</returns>
        public Condition ParseCondition(XmlTextReader reader)
        {
            var obj = new Condition();

            obj.Tag = reader.GetAttribute("tag");
            obj.Mask = reader.GetAttribute("mask");
            obj.Relation = reader.GetAttribute("relation").AsRelation();
            obj.Value = reader.GetAttribute("value");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Configuration
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Configuration on success, null on error</returns>
        public Configuration ParseConfiguration(XmlTextReader reader)
        {
            var obj = new Configuration();

            obj.Name = reader.GetAttribute("name");
            obj.Tag = reader.GetAttribute("tag");
            obj.Mask = reader.GetAttribute("mask");

            List<ConfLocation> confLocations = [];
            List<ConfSetting> confSettings = [];

            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "condition":
                        if (obj.Condition is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Condition = ParseCondition(reader);
                        break;
                    case "conflocation":
                        var confLocation = ParseConfLocation(reader);
                        if (confLocation is not null)
                            confLocations.Add(confLocation);

                        break;
                    case "confsetting":
                        var confSetting = ParseConfSetting(reader);
                        if (confSetting is not null)
                            confSettings.Add(confSetting);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            if (confLocations.Count > 0)
                obj.ConfLocation = [.. confLocations];
            if (confSettings.Count > 0)
                obj.ConfSetting = [.. confSettings];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a ConfLocation
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled ConfLocation on success, null on error</returns>
        public ConfLocation ParseConfLocation(XmlTextReader reader)
        {
            var obj = new ConfLocation();

            obj.Name = reader.GetAttribute("name");
            obj.Number = reader.GetAttribute("number");
            obj.Inverted = reader.GetAttribute("inverted").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a ConfSetting
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled ConfSetting on success, null on error</returns>
        public ConfSetting ParseConfSetting(XmlTextReader reader)
        {
            var obj = new ConfSetting();

            obj.Name = reader.GetAttribute("name");
            obj.Value = reader.GetAttribute("value");
            obj.Default = reader.GetAttribute("default").AsYesNo();

            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "condition":
                        if (obj.Condition is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Condition = ParseCondition(reader);
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Control
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Control on success, null on error</returns>
        public Control ParseControl(XmlTextReader reader)
        {
            var obj = new Control();

            obj.Type = reader.GetAttribute("type").AsControlType();
            obj.Player = reader.GetAttribute("player");
            obj.Buttons = reader.GetAttribute("buttons");
            obj.ReqButtons = reader.GetAttribute("reqbuttons");
            obj.Minimum = reader.GetAttribute("minimum");
            obj.Maximum = reader.GetAttribute("maximum");
            obj.Sensitivity = reader.GetAttribute("sensitivity");
            obj.KeyDelta = reader.GetAttribute("keydelta");
            obj.Reverse = reader.GetAttribute("reverse").AsYesNo();
            obj.Ways = reader.GetAttribute("ways");
            obj.Ways2 = reader.GetAttribute("ways2");
            obj.Ways3 = reader.GetAttribute("ways3");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Device
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Device on success, null on error</returns>
        public Device ParseDevice(XmlTextReader reader)
        {
            var obj = new Device();

            obj.Type = reader.GetAttribute("type").AsDeviceType();
            obj.Tag = reader.GetAttribute("tag");
            obj.FixedImage = reader.GetAttribute("fixed_image");
            obj.Mandatory = reader.GetAttribute("mandatory").AsYesNo();
            obj.Interface = reader.GetAttribute("interface");

            List<Extension> extensions = [];

            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "instance":
                        if (obj.Instance is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Instance = ParseInstance(reader);
                        break;
                    case "extension":
                        var extension = ParseExtension(reader);
                        if (extension is not null)
                            extensions.Add(extension);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            if (extensions.Count > 0)
                obj.Extension = [.. extensions];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a DeviceRef
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled DeviceRef on success, null on error</returns>
        public DeviceRef ParseDeviceRef(XmlTextReader reader)
        {
            var obj = new DeviceRef();

            obj.Name = reader.GetAttribute("name");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a DipLocation
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled DipLocation on success, null on error</returns>
        public DipLocation ParseDipLocation(XmlTextReader reader)
        {
            var obj = new DipLocation();

            obj.Name = reader.GetAttribute("name");
            obj.Number = reader.GetAttribute("number");
            obj.Inverted = reader.GetAttribute("inverted").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a DipSwitch
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled DipSwitch on success, null on error</returns>
        public DipSwitch ParseDipSwitch(XmlTextReader reader)
        {
            var obj = new DipSwitch();

            obj.Name = reader.GetAttribute("name");
            obj.Tag = reader.GetAttribute("tag");
            obj.Mask = reader.GetAttribute("mask");

            List<DipLocation> dipLocations = [];
            List<DipValue> dipValues = [];

            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "condition":
                        if (obj.Condition is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Condition = ParseCondition(reader);
                        break;
                    case "diplocation":
                        var dipLocation = ParseDipLocation(reader);
                        if (dipLocation is not null)
                            dipLocations.Add(dipLocation);

                        break;
                    case "dipvalue":
                        var dipValue = ParseDipValue(reader);
                        if (dipValue is not null)
                            dipValues.Add(dipValue);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            if (dipLocations.Count > 0)
                obj.DipLocation = [.. dipLocations];
            if (dipValues.Count > 0)
                obj.DipValue = [.. dipValues];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a DipValue
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled DipValue on success, null on error</returns>
        public DipValue ParseDipValue(XmlTextReader reader)
        {
            var obj = new DipValue();

            obj.Name = reader.GetAttribute("name");
            obj.Value = reader.GetAttribute("value");
            obj.Default = reader.GetAttribute("default").AsYesNo();

            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "condition":
                        if (obj.Condition is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Condition = ParseCondition(reader);
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Disk
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Disk on success, null on error</returns>
        public Disk ParseDisk(XmlTextReader reader)
        {
            var obj = new Disk();

            obj.Name = reader.GetAttribute("name");
            obj.MD5 = reader.GetAttribute("md5");
            obj.SHA1 = reader.GetAttribute("sha1");
            obj.Merge = reader.GetAttribute("merge");
            obj.Region = reader.GetAttribute("region");
            obj.Index = reader.GetAttribute("index");
            obj.Writable = reader.GetAttribute("writable").AsYesNo();
            obj.Status = reader.GetAttribute("status").AsItemStatus();
            obj.Optional = reader.GetAttribute("optional").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Display
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Display on success, null on error</returns>
        public Display ParseDisplay(XmlTextReader reader)
        {
            var obj = new Display();

            obj.Tag = reader.GetAttribute("tag");
            obj.Type = reader.GetAttribute("type").AsDisplayType();
            obj.Rotate = reader.GetAttribute("rotate");
            obj.FlipX = reader.GetAttribute("flipx").AsYesNo();
            obj.Width = reader.GetAttribute("width");
            obj.Height = reader.GetAttribute("height");
            obj.Refresh = reader.GetAttribute("refresh");
            obj.PixClock = reader.GetAttribute("pixclock");
            obj.HTotal = reader.GetAttribute("htotal");
            obj.HBEnd = reader.GetAttribute("hbend");
            obj.HBStart = reader.GetAttribute("hbstart");
            obj.VTotal = reader.GetAttribute("vtotal");
            obj.VBEnd = reader.GetAttribute("vbend");
            obj.VBStart = reader.GetAttribute("vbstart");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Driver
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Driver on success, null on error</returns>
        public Driver ParseDriver(XmlTextReader reader)
        {
            var obj = new Driver();

            obj.Status = reader.GetAttribute("status").AsSupportStatus();
            obj.Color = reader.GetAttribute("color").AsSupportStatus();
            obj.Sound = reader.GetAttribute("sound").AsSupportStatus();
            obj.PaletteSize = reader.GetAttribute("palettesize");
            obj.Emulation = reader.GetAttribute("emulation").AsSupportStatus();
            obj.Cocktail = reader.GetAttribute("cocktail").AsSupportStatus();
            obj.SaveState = reader.GetAttribute("savestate").AsSupported();
            obj.RequiresArtwork = reader.GetAttribute("requiresartwork").AsYesNo();
            obj.Unofficial = reader.GetAttribute("unofficial").AsYesNo();
            obj.NoSoundHardware = reader.GetAttribute("nosoundhardware").AsYesNo();
            obj.Incomplete = reader.GetAttribute("incomplete").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Extension
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Extension on success, null on error</returns>
        public Extension ParseExtension(XmlTextReader reader)
        {
            var obj = new Extension();

            obj.Name = reader.GetAttribute("name");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Feature
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Feature on success, null on error</returns>
        public Feature ParseFeature(XmlTextReader reader)
        {
            var obj = new Feature();

            obj.Type = reader.GetAttribute("type").AsFeatureType();
            obj.Status = reader.GetAttribute("status").AsFeatureStatus();
            obj.Overall = reader.GetAttribute("overall").AsFeatureStatus();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a GameBase
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled GameBase on success, null on error</returns>
        public GameBase? ParseGameBase(XmlTextReader reader)
        {
            GameBase obj;
            if (reader.Name == "game")
                obj = new Game();
            else if (reader.Name == "machine")
                obj = new Machine();
            else
                return null;

            obj.Name = reader.GetAttribute("name");
            obj.SourceFile = reader.GetAttribute("sourcefile");
            obj.IsBios = reader.GetAttribute("isbios").AsYesNo();
            obj.IsDevice = reader.GetAttribute("isdevice").AsYesNo();
            obj.IsMechanical = reader.GetAttribute("ismechanical").AsYesNo();
            obj.Runnable = reader.GetAttribute("runnable").AsRunnable();
            obj.CloneOf = reader.GetAttribute("cloneof");
            obj.RomOf = reader.GetAttribute("romof");
            obj.SampleOf = reader.GetAttribute("sampleof");

            List<BiosSet> biosSets = [];
            List<Rom> roms = [];
            List<Disk> disks = [];
            List<DeviceRef> deviceRefs = [];
            List<Sample> samples = [];
            List<Chip> chips = [];
            List<Display> displays = [];
            List<Video> videos = [];
            List<DipSwitch> dipSwitches = [];
            List<Configuration> configurations = [];
            List<Port> ports = [];
            List<Adjuster> adjusters = [];
            List<Feature> features = [];
            List<Device> devices = [];
            List<Slot> slots = [];
            List<Data.Models.Listxml.SoftwareList> softwareLists = [];
            List<RamOption> ramOptions = [];

            reader.Read();
            while (!reader.EOF)
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "description":
                        if (obj.Description is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Description = reader.ReadElementContentAsString();
                        break;
                    case "year":
                        if (obj.Year is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Year = reader.ReadElementContentAsString();
                        break;
                    case "manufacturer":
                        if (obj.Manufacturer is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Manufacturer = reader.ReadElementContentAsString();
                        break;
                    case "history":
                        if (obj.History is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.History = reader.ReadElementContentAsString();
                        break;
                    case "biosset":
                        var biosSet = ParseBiosSet(reader);
                        if (biosSet is not null)
                            biosSets.Add(biosSet);

                        reader.Skip();
                        break;
                    case "rom":
                        var rom = ParseRom(reader);
                        if (rom is not null)
                            roms.Add(rom);

                        reader.Skip();
                        break;
                    case "disk":
                        var disk = ParseDisk(reader);
                        if (disk is not null)
                            disks.Add(disk);

                        reader.Skip();
                        break;
                    case "device_ref":
                        var deviceRef = ParseDeviceRef(reader);
                        if (deviceRef is not null)
                            deviceRefs.Add(deviceRef);

                        reader.Skip();
                        break;
                    case "sample":
                        var sample = ParseSample(reader);
                        if (sample is not null)
                            samples.Add(sample);

                        reader.Skip();
                        break;
                    case "chip":
                        var chip = ParseChip(reader);
                        if (chip is not null)
                            chips.Add(chip);

                        reader.Skip();
                        break;
                    case "display":
                        var display = ParseDisplay(reader);
                        if (display is not null)
                            displays.Add(display);

                        reader.Skip();
                        break;
                    case "video":
                        var video = ParseVideo(reader);
                        if (video is not null)
                            videos.Add(video);

                        reader.Skip();
                        break;
                    case "sound":
                        if (obj.Sound is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Sound = ParseSound(reader);
                        reader.Skip();
                        break;
                    case "input":
                        if (obj.Input is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Input = ParseInput(reader);
                        reader.Skip();
                        break;
                    case "dipswitch":
                        var dipSwitch = ParseDipSwitch(reader);
                        if (dipSwitch is not null)
                            dipSwitches.Add(dipSwitch);

                        reader.Skip();
                        break;
                    case "configuration":
                        var configuration = ParseConfiguration(reader);
                        if (configuration is not null)
                            configurations.Add(configuration);

                        reader.Skip();
                        break;
                    case "port":
                        var port = ParsePort(reader);
                        if (port is not null)
                            ports.Add(port);

                        reader.Skip();
                        break;
                    case "adjuster":
                        var adjuster = ParseAdjuster(reader);
                        if (adjuster is not null)
                            adjusters.Add(adjuster);

                        reader.Skip();
                        break;
                    case "driver":
                        if (obj.Driver is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Driver = ParseDriver(reader);
                        reader.Skip();
                        break;
                    case "feature":
                        var feature = ParseFeature(reader);
                        if (feature is not null)
                            features.Add(feature);

                        reader.Skip();
                        break;
                    case "device":
                        var device = ParseDevice(reader);
                        if (device is not null)
                            devices.Add(device);

                        reader.Skip();
                        break;
                    case "slot":
                        var slot = ParseSlot(reader);
                        if (slot is not null)
                            slots.Add(slot);

                        reader.Skip();
                        break;
                    case "softwarelist":
                        var softwareList = ParseSoftwareList(reader);
                        if (softwareList is not null)
                            softwareLists.Add(softwareList);

                        reader.Skip();
                        break;
                    case "ramoption":
                        var ramOption = ParseRamOption(reader);
                        if (ramOption is not null)
                            ramOptions.Add(ramOption);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (biosSets.Count > 0)
                obj.BiosSet = [.. biosSets];
            if (roms.Count > 0)
                obj.Rom = [.. roms];
            if (disks.Count > 0)
                obj.Disk = [.. disks];
            if (deviceRefs.Count > 0)
                obj.DeviceRef = [.. deviceRefs];
            if (samples.Count > 0)
                obj.Sample = [.. samples];
            if (chips.Count > 0)
                obj.Chip = [.. chips];
            if (displays.Count > 0)
                obj.Display = [.. displays];
            if (videos.Count > 0)
                obj.Video = [.. videos];
            if (dipSwitches.Count > 0)
                obj.DipSwitch = [.. dipSwitches];
            if (configurations.Count > 0)
                obj.Configuration = [.. configurations];
            if (ports.Count > 0)
                obj.Port = [.. ports];
            if (adjusters.Count > 0)
                obj.Adjuster = [.. adjusters];
            if (features.Count > 0)
                obj.Feature = [.. features];
            if (devices.Count > 0)
                obj.Device = [.. devices];
            if (slots.Count > 0)
                obj.Slot = [.. slots];
            if (softwareLists.Count > 0)
                obj.SoftwareList = [.. softwareLists];
            if (ramOptions.Count > 0)
                obj.RamOption = [.. ramOptions];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Input
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Input on success, null on error</returns>
        public Input ParseInput(XmlTextReader reader)
        {
            var obj = new Input();

            obj.Service = reader.GetAttribute("service").AsYesNo();
            obj.Tilt = reader.GetAttribute("tilt").AsYesNo();
            obj.Players = reader.GetAttribute("players");
            obj.ControlAttr = reader.GetAttribute("control");
            obj.Buttons = reader.GetAttribute("buttons");
            obj.Coins = reader.GetAttribute("coins");

            List<Control> controls = [];

            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "control":
                        var control = ParseControl(reader);
                        if (control is not null)
                            controls.Add(control);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            if (controls.Count > 0)
                obj.Control = [.. controls];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Instance
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Instance on success, null on error</returns>
        public Instance ParseInstance(XmlTextReader reader)
        {
            var obj = new Instance();

            obj.Name = reader.GetAttribute("name");
            obj.BriefName = reader.GetAttribute("briefname");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Port
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Port on success, null on error</returns>
        public Port ParsePort(XmlTextReader reader)
        {
            var obj = new Port();

            obj.Tag = reader.GetAttribute("tag");

            List<Analog> analogs = [];

            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "analog":
                        var analog = ParseAnalog(reader);
                        if (analog is not null)
                            analogs.Add(analog);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            if (analogs.Count > 0)
                obj.Analog = [.. analogs];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a RamOption
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled RamOption on success, null on error</returns>
        public RamOption ParseRamOption(XmlTextReader reader)
        {
            var obj = new RamOption();

            obj.Name = reader.GetAttribute("name");
            obj.Default = reader.GetAttribute("default").AsYesNo();
            obj.Content = reader.ReadElementContentAsString();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Rom
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Rom on success, null on error</returns>
        public Rom ParseRom(XmlTextReader reader)
        {
            var obj = new Rom();

            obj.Name = reader.GetAttribute("name");
            obj.Bios = reader.GetAttribute("bios");
            obj.Size = reader.GetAttribute("size");
            obj.CRC = reader.GetAttribute("crc");
            obj.SHA1 = reader.GetAttribute("sha1");
            obj.Merge = reader.GetAttribute("merge");
            obj.Region = reader.GetAttribute("region");
            obj.Offset = reader.GetAttribute("offset");
            obj.Status = reader.GetAttribute("status").AsItemStatus();
            obj.Optional = reader.GetAttribute("optional").AsYesNo();
            obj.Dispose = reader.GetAttribute("dispose").AsYesNo();
            obj.SoundOnly = reader.GetAttribute("soundonly").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Sample
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Sample on success, null on error</returns>
        public Sample ParseSample(XmlTextReader reader)
        {
            var obj = new Sample();

            obj.Name = reader.GetAttribute("name");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Slot
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Slot on success, null on error</returns>
        public Slot ParseSlot(XmlTextReader reader)
        {
            var obj = new Slot();

            obj.Name = reader.GetAttribute("name");

            List<SlotOption> slotOptions = [];

            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "slotoption":
                        var slotOption = ParseSlotOption(reader);
                        if (slotOption is not null)
                            slotOptions.Add(slotOption);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            if (slotOptions.Count > 0)
                obj.SlotOption = [.. slotOptions];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a SlotOption
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled SlotOption on success, null on error</returns>
        public SlotOption ParseSlotOption(XmlTextReader reader)
        {
            var obj = new SlotOption();

            obj.Name = reader.GetAttribute("name");
            obj.DevName = reader.GetAttribute("devname");
            obj.Default = reader.GetAttribute("default").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a SoftwareList
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled SoftwareList on success, null on error</returns>
        public Data.Models.Listxml.SoftwareList ParseSoftwareList(XmlTextReader reader)
        {
            var obj = new Data.Models.Listxml.SoftwareList();

            obj.Tag = reader.GetAttribute("tag");
            obj.Name = reader.GetAttribute("name");
            obj.Status = reader.GetAttribute("status").AsSoftwareListStatus();
            obj.Filter = reader.GetAttribute("filter");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Sound
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Sound on success, null on error</returns>
        public Sound ParseSound(XmlTextReader reader)
        {
            var obj = new Sound();

            obj.Channels = NumberHelper.ConvertToInt64(reader.GetAttribute("channels"));

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Video
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Video on success, null on error</returns>
        public Video ParseVideo(XmlTextReader reader)
        {
            var obj = new Video();

            obj.Screen = reader.GetAttribute("screen").AsDisplayType();
            obj.Orientation = reader.GetAttribute("orientation");
            obj.Width = reader.GetAttribute("width");
            obj.Height = reader.GetAttribute("height");
            obj.AspectX = reader.GetAttribute("aspectx");
            obj.AspectY = reader.GetAttribute("aspecty");
            obj.Refresh = reader.GetAttribute("refresh");

            return obj;
        }

        #endregion
    }
}
