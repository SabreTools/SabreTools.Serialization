using System.IO;
using System.Text;
using System.Xml;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Listxml;
using SabreTools.IO.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class Mess : BaseBinaryWriter<Data.Models.Listxml.Mess>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Data.Models.Listxml.Mess? obj)
        {
            // If the metadata file is null
            if (obj is null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                IndentChar = '\t',
                Indentation = 1
            };
            writer.Settings?.CheckCharacters = false;
            writer.Settings?.NewLineChars = "\n";

            // Write document start
            writer.WriteStartDocument();

            // Write the Mess, if it exists
            WriteMess(obj, writer);
            writer.Flush();

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write a Mess to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Mess to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteMess(Data.Models.Listxml.Mess obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("mess");

            writer.WriteOptionalAttributeString("version", obj.Version);

            if (obj.Game is not null && obj.Game.Length > 0)
            {
                foreach (var gameBase in obj.Game)
                {
                    WriteGameBase(gameBase, writer);
                }
            }

            writer.WriteEndElement();
        }

        #region Items

        /// <summary>
        /// Write a Adjuster to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Adjuster to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteAdjuster(Adjuster obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("adjuster");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("default", obj.Default.FromYesNo());

            if (obj.Condition is not null)
                WriteCondition(obj.Condition, writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Analog to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Analog to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteAnalog(Analog obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("analog");

            writer.WriteRequiredAttributeString("mask", obj.Mask);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a BiosSet to an XmlTextWriter
        /// </summary>
        /// <param name="obj">BiosSet to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteBiosSet(BiosSet obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("biosset");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("description", obj.Description);
            writer.WriteOptionalAttributeString("default", obj.Default.FromYesNo());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Chip to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Chip to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteChip(Chip obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("chip");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("tag", obj.Tag);
            writer.WriteRequiredAttributeString("type", obj.Type?.AsStringValue());
            writer.WriteOptionalAttributeString("soundonly", obj.SoundOnly.FromYesNo());
            writer.WriteOptionalAttributeString("clock", obj.Clock);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Condition to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Condition to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteCondition(Condition obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("condition");

            writer.WriteRequiredAttributeString("tag", obj.Tag);
            writer.WriteRequiredAttributeString("mask", obj.Mask);
            writer.WriteRequiredAttributeString("relation", obj.Relation?.AsStringValue());
            writer.WriteRequiredAttributeString("value", obj.Value);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Configuration to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Configuration to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteConfiguration(Configuration obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("configuration");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("tag", obj.Tag);
            writer.WriteOptionalAttributeString("mask", obj.Mask);

            if (obj.Condition is not null)
                WriteCondition(obj.Condition, writer);

            if (obj.ConfLocation is not null && obj.ConfLocation.Length > 0)
            {
                foreach (var confLocation in obj.ConfLocation)
                {
                    WriteConfLocation(confLocation, writer);
                }
            }

            if (obj.ConfSetting is not null && obj.ConfSetting.Length > 0)
            {
                foreach (var confSetting in obj.ConfSetting)
                {
                    WriteConfSetting(confSetting, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a ConfLocation to an XmlTextWriter
        /// </summary>
        /// <param name="obj">ConfLocation to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteConfLocation(ConfLocation obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("conflocation");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("number", obj.Number);
            writer.WriteOptionalAttributeString("inverted", obj.Inverted.FromYesNo());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a ConfSetting to an XmlTextWriter
        /// </summary>
        /// <param name="obj">ConfSetting to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteConfSetting(ConfSetting obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("confsetting");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("value", obj.Value);
            writer.WriteOptionalAttributeString("default", obj.Default.FromYesNo());

            if (obj.Condition is not null)
                WriteCondition(obj.Condition, writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Control to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Control to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteControl(Control obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("control");

            writer.WriteRequiredAttributeString("type", obj.Type?.AsStringValue());
            writer.WriteOptionalAttributeString("player", obj.Player);
            writer.WriteOptionalAttributeString("buttons", obj.Buttons);
            writer.WriteOptionalAttributeString("reqbuttons", obj.ReqButtons);
            writer.WriteOptionalAttributeString("minimum", obj.Minimum);
            writer.WriteOptionalAttributeString("maximum", obj.Maximum);
            writer.WriteOptionalAttributeString("sensitivity", obj.Sensitivity);
            writer.WriteOptionalAttributeString("keydelta", obj.KeyDelta);
            writer.WriteOptionalAttributeString("reverse", obj.Reverse.FromYesNo());
            writer.WriteOptionalAttributeString("ways", obj.Ways);
            writer.WriteOptionalAttributeString("ways2", obj.Ways2);
            writer.WriteOptionalAttributeString("ways3", obj.Ways3);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Device to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Device to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDevice(Device obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("device");

            writer.WriteRequiredAttributeString("type", obj.Type?.AsStringValue());
            writer.WriteOptionalAttributeString("tag", obj.Tag);
            writer.WriteOptionalAttributeString("fixed_image", obj.FixedImage);
            if (obj.Mandatory is not null)
                writer.WriteOptionalAttributeString("mandatory", obj.Mandatory.Value ? "1" : "0");
            writer.WriteOptionalAttributeString("interface", obj.Interface);

            if (obj.Instance is not null)
                WriteInstance(obj.Instance, writer);

            if (obj.Extension is not null && obj.Extension.Length > 0)
            {
                foreach (var extension in obj.Extension)
                {
                    WriteExtension(extension, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a DeviceRef to an XmlTextWriter
        /// </summary>
        /// <param name="obj">DeviceRef to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDeviceRef(DeviceRef obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("device_ref");

            writer.WriteRequiredAttributeString("name", obj.Name);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a DipLocation to an XmlTextWriter
        /// </summary>
        /// <param name="obj">DipLocation to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDipLocation(DipLocation obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("diplocation");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("number", obj.Number);
            writer.WriteOptionalAttributeString("inverted", obj.Inverted.FromYesNo());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a DipSwitch to an XmlTextWriter
        /// </summary>
        /// <param name="obj">DipSwitch to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDipSwitch(DipSwitch obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("dipswitch");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("tag", obj.Tag);
            writer.WriteOptionalAttributeString("mask", obj.Mask);

            if (obj.Condition is not null)
                WriteCondition(obj.Condition, writer);

            if (obj.DipLocation is not null && obj.DipLocation.Length > 0)
            {
                foreach (var dipLocation in obj.DipLocation)
                {
                    WriteDipLocation(dipLocation, writer);
                }
            }

            if (obj.DipValue is not null && obj.DipValue.Length > 0)
            {
                foreach (var dipValue in obj.DipValue)
                {
                    WriteDipValue(dipValue, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a DipValue to an XmlTextWriter
        /// </summary>
        /// <param name="obj">DipValue to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDipValue(DipValue obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("dipvalue");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("value", obj.Value);
            writer.WriteOptionalAttributeString("default", obj.Default.FromYesNo());

            if (obj.Condition is not null)
                WriteCondition(obj.Condition, writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Disk to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Disk to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDisk(Disk obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("disk");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("md5", obj.MD5);
            writer.WriteOptionalAttributeString("sha1", obj.SHA1);
            writer.WriteOptionalAttributeString("merge", obj.Merge);
            writer.WriteOptionalAttributeString("region", obj.Region);
            writer.WriteOptionalAttributeString("index", obj.Index);
            writer.WriteOptionalAttributeString("writable", obj.Writable.FromYesNo());
            writer.WriteOptionalAttributeString("status", obj.Status?.AsStringValue());
            writer.WriteOptionalAttributeString("optional", obj.Optional.FromYesNo());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Display to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Display to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDisplay(Display obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("display");

            writer.WriteOptionalAttributeString("tag", obj.Tag);
            writer.WriteRequiredAttributeString("type", obj.Type?.AsStringValue());
            writer.WriteOptionalAttributeString("rotate", obj.Rotate);
            writer.WriteOptionalAttributeString("flipx", obj.FlipX.FromYesNo());
            writer.WriteOptionalAttributeString("width", obj.Width);
            writer.WriteOptionalAttributeString("height", obj.Height);
            writer.WriteRequiredAttributeString("refresh", obj.Refresh);
            writer.WriteOptionalAttributeString("pixclock", obj.PixClock);
            writer.WriteOptionalAttributeString("htotal", obj.HTotal);
            writer.WriteOptionalAttributeString("hbend", obj.HBEnd);
            writer.WriteOptionalAttributeString("hbstart", obj.HBStart);
            writer.WriteOptionalAttributeString("vtotal", obj.VTotal);
            writer.WriteOptionalAttributeString("vbend", obj.VBEnd);
            writer.WriteOptionalAttributeString("vbstart", obj.VBStart);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Driver to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Driver to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDriver(Driver obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("driver");

            writer.WriteRequiredAttributeString("status", obj.Status?.AsStringValue());
            writer.WriteOptionalAttributeString("color", obj.Color?.AsStringValue());
            writer.WriteOptionalAttributeString("sound", obj.Sound?.AsStringValue());
            writer.WriteOptionalAttributeString("palettesize", obj.PaletteSize);
            writer.WriteRequiredAttributeString("emulation", obj.Emulation?.AsStringValue());
            writer.WriteRequiredAttributeString("cocktail", obj.Cocktail?.AsStringValue());
            writer.WriteRequiredAttributeString("savestate", obj.SaveState?.AsStringValue());
            writer.WriteOptionalAttributeString("requiresartwork", obj.RequiresArtwork.FromYesNo());
            writer.WriteOptionalAttributeString("unofficial", obj.Unofficial.FromYesNo());
            writer.WriteOptionalAttributeString("nosoundhardware", obj.NoSoundHardware.FromYesNo());
            writer.WriteOptionalAttributeString("incomplete", obj.Incomplete.FromYesNo());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Extension to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Extension to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteExtension(Extension obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("extension");

            writer.WriteRequiredAttributeString("name", obj.Name);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Feature to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Feature to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteFeature(Feature obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("feature");

            writer.WriteRequiredAttributeString("type", obj.Type?.AsStringValue());
            writer.WriteOptionalAttributeString("status", obj.Status?.AsStringValue());
            writer.WriteOptionalAttributeString("overall", obj.Overall?.AsStringValue());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a GameBase to an XmlTextWriter
        /// </summary>
        /// <param name="obj">GameBase to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteGameBase(GameBase obj, XmlTextWriter writer)
        {
            if (obj is Game)
                writer.WriteStartElement("game");
            else if (obj is Machine)
                writer.WriteStartElement("machine");
            else
                return;

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("sourcefile", obj.SourceFile);
            writer.WriteOptionalAttributeString("isbios", obj.IsBios.FromYesNo());
            writer.WriteOptionalAttributeString("isdevice", obj.IsDevice.FromYesNo());
            writer.WriteOptionalAttributeString("ismechanical", obj.IsMechanical.FromYesNo());
            writer.WriteOptionalAttributeString("runnable", obj.Runnable?.AsStringValue());
            writer.WriteOptionalAttributeString("cloneof", obj.CloneOf);
            writer.WriteOptionalAttributeString("romof", obj.RomOf);
            writer.WriteOptionalAttributeString("sampleof", obj.SampleOf);

            writer.WriteRequiredElementString("description", obj.Description);
            writer.WriteOptionalElementString("year", obj.Year);
            writer.WriteOptionalElementString("manufacturer", obj.Manufacturer);
            writer.WriteOptionalElementString("history", obj.History);

            if (obj.BiosSet is not null && obj.BiosSet.Length > 0)
            {
                foreach (var biosSet in obj.BiosSet)
                {
                    WriteBiosSet(biosSet, writer);
                }
            }

            if (obj.Rom is not null && obj.Rom.Length > 0)
            {
                foreach (var rom in obj.Rom)
                {
                    WriteRom(rom, writer);
                }
            }

            if (obj.Disk is not null && obj.Disk.Length > 0)
            {
                foreach (var disk in obj.Disk)
                {
                    WriteDisk(disk, writer);
                }
            }

            if (obj.DeviceRef is not null && obj.DeviceRef.Length > 0)
            {
                foreach (var deviceRef in obj.DeviceRef)
                {
                    WriteDeviceRef(deviceRef, writer);
                }
            }

            if (obj.Sample is not null && obj.Sample.Length > 0)
            {
                foreach (var sample in obj.Sample)
                {
                    WriteSample(sample, writer);
                }
            }

            if (obj.Chip is not null && obj.Chip.Length > 0)
            {
                foreach (var chip in obj.Chip)
                {
                    WriteChip(chip, writer);
                }
            }

            if (obj.Display is not null && obj.Display.Length > 0)
            {
                foreach (var display in obj.Display)
                {
                    WriteDisplay(display, writer);
                }
            }

            if (obj.Video is not null && obj.Video.Length > 0)
            {
                foreach (var video in obj.Video)
                {
                    WriteVideo(video, writer);
                }
            }

            if (obj.Sound is not null)
                WriteSound(obj.Sound, writer);

            if (obj.Input is not null)
                WriteInput(obj.Input, writer);

            if (obj.DipSwitch is not null && obj.DipSwitch.Length > 0)
            {
                foreach (var dipSwitch in obj.DipSwitch)
                {
                    WriteDipSwitch(dipSwitch, writer);
                }
            }

            if (obj.Configuration is not null && obj.Configuration.Length > 0)
            {
                foreach (var configuration in obj.Configuration)
                {
                    WriteConfiguration(configuration, writer);
                }
            }

            if (obj.Port is not null && obj.Port.Length > 0)
            {
                foreach (var port in obj.Port)
                {
                    WritePort(port, writer);
                }
            }

            if (obj.Adjuster is not null && obj.Adjuster.Length > 0)
            {
                foreach (var adjuster in obj.Adjuster)
                {
                    WriteAdjuster(adjuster, writer);
                }
            }

            if (obj.Driver is not null)
                WriteDriver(obj.Driver, writer);

            if (obj.Feature is not null && obj.Feature.Length > 0)
            {
                foreach (var feature in obj.Feature)
                {
                    WriteFeature(feature, writer);
                }
            }

            if (obj.Device is not null && obj.Device.Length > 0)
            {
                foreach (var device in obj.Device)
                {
                    WriteDevice(device, writer);
                }
            }

            if (obj.Slot is not null && obj.Slot.Length > 0)
            {
                foreach (var slot in obj.Slot)
                {
                    WriteSlot(slot, writer);
                }
            }

            if (obj.SoftwareList is not null && obj.SoftwareList.Length > 0)
            {
                foreach (var softwareList in obj.SoftwareList)
                {
                    WriteSoftwareList(softwareList, writer);
                }
            }

            if (obj.RamOption is not null && obj.RamOption.Length > 0)
            {
                foreach (var ramOption in obj.RamOption)
                {
                    WriteRamOption(ramOption, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Input to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Input to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteInput(Input obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("input");

            writer.WriteOptionalAttributeString("service", obj.Service.FromYesNo());
            writer.WriteOptionalAttributeString("tilt", obj.Tilt.FromYesNo());
            writer.WriteRequiredAttributeString("players", obj.Players);
            writer.WriteOptionalAttributeString("control", obj.ControlAttr);
            writer.WriteOptionalAttributeString("buttons", obj.Buttons);
            writer.WriteOptionalAttributeString("coins", obj.Coins);

            if (obj.Control is not null && obj.Control.Length > 0)
            {
                foreach (var control in obj.Control)
                {
                    WriteControl(control, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Instance to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Instance to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteInstance(Instance obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("instance");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("briefname", obj.BriefName);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Port to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Port to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WritePort(Port obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("port");

            writer.WriteRequiredAttributeString("tag", obj.Tag);

            if (obj.Analog is not null && obj.Analog.Length > 0)
            {
                foreach (var analog in obj.Analog)
                {
                    WriteAnalog(analog, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a RamOption to an XmlTextWriter
        /// </summary>
        /// <param name="obj">RamOption to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteRamOption(RamOption obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("ramoption");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("default", obj.Default.FromYesNo());

            if (obj.Content is not null)
                writer.WriteValue(obj.Content);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Rom to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Rom to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteRom(Rom obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("rom");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("bios", obj.Bios);
            writer.WriteRequiredAttributeString("size", obj.Size);
            writer.WriteOptionalAttributeString("crc", obj.CRC);
            writer.WriteOptionalAttributeString("sha1", obj.SHA1);
            writer.WriteOptionalAttributeString("merge", obj.Merge);
            writer.WriteOptionalAttributeString("region", obj.Region);
            writer.WriteOptionalAttributeString("offset", obj.Offset);
            writer.WriteOptionalAttributeString("status", obj.Status?.AsStringValue());
            writer.WriteOptionalAttributeString("optional", obj.Optional.FromYesNo());
            writer.WriteOptionalAttributeString("dispose", obj.Dispose.FromYesNo());
            writer.WriteOptionalAttributeString("soundonly", obj.SoundOnly.FromYesNo());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Sample to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Sample to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSample(Sample obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("sample");

            writer.WriteRequiredAttributeString("name", obj.Name);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Slot to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Slot to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSlot(Slot obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("slot");

            writer.WriteRequiredAttributeString("name", obj.Name);

            if (obj.SlotOption is not null && obj.SlotOption.Length > 0)
            {
                foreach (var slotOption in obj.SlotOption)
                {
                    WriteSlotOption(slotOption, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a SlotOption to an XmlTextWriter
        /// </summary>
        /// <param name="obj">SlotOption to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSlotOption(SlotOption obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("slotoption");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("devname", obj.DevName);
            writer.WriteOptionalAttributeString("default", obj.Default.FromYesNo());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a SoftwareList to an XmlTextWriter
        /// </summary>
        /// <param name="obj">SoftwareList to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSoftwareList(Data.Models.Listxml.SoftwareList obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("softwarelist");

            writer.WriteRequiredAttributeString("tag", obj.Tag);
            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("status", obj.Status?.AsStringValue());
            writer.WriteOptionalAttributeString("filter", obj.Filter);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Sound to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Sound to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSound(Sound obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("sound");

            writer.WriteRequiredAttributeString("channels", obj.Channels);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Video to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Video to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteVideo(Video obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("video");

            writer.WriteRequiredAttributeString("screen", obj.Screen?.AsStringValue());
            writer.WriteRequiredAttributeString("orientation", obj.Orientation);
            writer.WriteOptionalAttributeString("width", obj.Width);
            writer.WriteOptionalAttributeString("height", obj.Height);
            writer.WriteOptionalAttributeString("aspectx", obj.AspectX);
            writer.WriteOptionalAttributeString("aspecty", obj.AspectY);
            writer.WriteRequiredAttributeString("refresh", obj.Refresh);

            writer.WriteEndElement();
        }

        #endregion
    }
}
