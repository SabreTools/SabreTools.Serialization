using System;
using System.Collections.Generic;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using SabreTools.Metadata.Filter;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents a MAME/M1 XML DAT
    /// </summary>
    public sealed class Listxml : SerializableDatFile<Data.Models.Listxml.Mame, Serialization.Readers.Listxml, Serialization.Writers.Listxml, Serialization.CrossModel.Listxml>
    {
        #region Constants

        /// <summary>
        /// DTD for original MAME XML DATs
        /// </summary>
        internal const string MAMEDTD = @"<!DOCTYPE mame [
<!ELEMENT mame (machine+)>
	<!ATTLIST mame build CDATA #IMPLIED>
	<!ATTLIST mame debug (yes|no) ""no"">
	<!ATTLIST mame mameconfig CDATA #REQUIRED>
	<!ELEMENT machine (description, year?, manufacturer?, biosset*, rom*, disk*, device_ref*, sample*, chip*, display*, sound?, input?, dipswitch*, configuration*, port*, adjuster*, driver?, feature*, device*, slot*, softwarelist*, ramoption*)>
		<!ATTLIST machine name CDATA #REQUIRED>
		<!ATTLIST machine sourcefile CDATA #IMPLIED>
		<!ATTLIST machine isbios (yes|no) ""no"">
		<!ATTLIST machine isdevice (yes|no) ""no"">
		<!ATTLIST machine ismechanical (yes|no) ""no"">
		<!ATTLIST machine runnable (yes|no) ""yes"">
		<!ATTLIST machine cloneof CDATA #IMPLIED>
		<!ATTLIST machine romof CDATA #IMPLIED>
		<!ATTLIST machine sampleof CDATA #IMPLIED>
		<!ELEMENT description (#PCDATA)>
		<!ELEMENT year (#PCDATA)>
		<!ELEMENT manufacturer (#PCDATA)>
		<!ELEMENT biosset EMPTY>
			<!ATTLIST biosset name CDATA #REQUIRED>
			<!ATTLIST biosset description CDATA #REQUIRED>
			<!ATTLIST biosset default (yes|no) ""no"">
		<!ELEMENT rom EMPTY>
			<!ATTLIST rom name CDATA #REQUIRED>
			<!ATTLIST rom bios CDATA #IMPLIED>
			<!ATTLIST rom size CDATA #REQUIRED>
			<!ATTLIST rom crc CDATA #IMPLIED>
			<!ATTLIST rom sha1 CDATA #IMPLIED>
			<!ATTLIST rom merge CDATA #IMPLIED>
			<!ATTLIST rom region CDATA #IMPLIED>
			<!ATTLIST rom offset CDATA #IMPLIED>
			<!ATTLIST rom status (baddump|nodump|good) ""good"">
			<!ATTLIST rom optional (yes|no) ""no"">
		<!ELEMENT disk EMPTY>
			<!ATTLIST disk name CDATA #REQUIRED>
			<!ATTLIST disk sha1 CDATA #IMPLIED>
			<!ATTLIST disk merge CDATA #IMPLIED>
			<!ATTLIST disk region CDATA #IMPLIED>
			<!ATTLIST disk index CDATA #IMPLIED>
			<!ATTLIST disk writable (yes|no) ""no"">
			<!ATTLIST disk status (baddump|nodump|good) ""good"">
			<!ATTLIST disk optional (yes|no) ""no"">
		<!ELEMENT device_ref EMPTY>
			<!ATTLIST device_ref name CDATA #REQUIRED>
		<!ELEMENT sample EMPTY>
			<!ATTLIST sample name CDATA #REQUIRED>
		<!ELEMENT chip EMPTY>
			<!ATTLIST chip name CDATA #REQUIRED>
			<!ATTLIST chip tag CDATA #IMPLIED>
			<!ATTLIST chip type (cpu|audio) #REQUIRED>
			<!ATTLIST chip clock CDATA #IMPLIED>
		<!ELEMENT display EMPTY>
			<!ATTLIST display tag CDATA #IMPLIED>
			<!ATTLIST display type (raster|vector|lcd|svg|unknown) #REQUIRED>
			<!ATTLIST display rotate (0|90|180|270) #IMPLIED>
			<!ATTLIST display flipx (yes|no) ""no"">
			<!ATTLIST display width CDATA #IMPLIED>
			<!ATTLIST display height CDATA #IMPLIED>
			<!ATTLIST display refresh CDATA #REQUIRED>
			<!ATTLIST display pixclock CDATA #IMPLIED>
			<!ATTLIST display htotal CDATA #IMPLIED>
			<!ATTLIST display hbend CDATA #IMPLIED>
			<!ATTLIST display hbstart CDATA #IMPLIED>
			<!ATTLIST display vtotal CDATA #IMPLIED>
			<!ATTLIST display vbend CDATA #IMPLIED>
			<!ATTLIST display vbstart CDATA #IMPLIED>
		<!ELEMENT sound EMPTY>
			<!ATTLIST sound channels CDATA #REQUIRED>
		<!ELEMENT condition EMPTY>
			<!ATTLIST condition tag CDATA #REQUIRED>
			<!ATTLIST condition mask CDATA #REQUIRED>
			<!ATTLIST condition relation (eq|ne|gt|le|lt|ge) #REQUIRED>
			<!ATTLIST condition value CDATA #REQUIRED>
		<!ELEMENT input (control*)>
			<!ATTLIST input service (yes|no) ""no"">
			<!ATTLIST input tilt (yes|no) ""no"">
			<!ATTLIST input players CDATA #REQUIRED>
			<!ATTLIST input coins CDATA #IMPLIED>
			<!ELEMENT control EMPTY>
				<!ATTLIST control type CDATA #REQUIRED>
				<!ATTLIST control player CDATA #IMPLIED>
				<!ATTLIST control buttons CDATA #IMPLIED>
				<!ATTLIST control reqbuttons CDATA #IMPLIED>
				<!ATTLIST control minimum CDATA #IMPLIED>
				<!ATTLIST control maximum CDATA #IMPLIED>
				<!ATTLIST control sensitivity CDATA #IMPLIED>
				<!ATTLIST control keydelta CDATA #IMPLIED>
				<!ATTLIST control reverse (yes|no) ""no"">
				<!ATTLIST control ways CDATA #IMPLIED>
				<!ATTLIST control ways2 CDATA #IMPLIED>
				<!ATTLIST control ways3 CDATA #IMPLIED>
		<!ELEMENT dipswitch (condition?, diplocation*, dipvalue*)>
			<!ATTLIST dipswitch name CDATA #REQUIRED>
			<!ATTLIST dipswitch tag CDATA #REQUIRED>
			<!ATTLIST dipswitch mask CDATA #REQUIRED>
			<!ELEMENT diplocation EMPTY>
				<!ATTLIST diplocation name CDATA #REQUIRED>
				<!ATTLIST diplocation number CDATA #REQUIRED>
				<!ATTLIST diplocation inverted (yes|no) ""no"">
			<!ELEMENT dipvalue (condition?)>
				<!ATTLIST dipvalue name CDATA #REQUIRED>
				<!ATTLIST dipvalue value CDATA #REQUIRED>
				<!ATTLIST dipvalue default (yes|no) ""no"">
		<!ELEMENT configuration (condition?, conflocation*, confsetting*)>
			<!ATTLIST configuration name CDATA #REQUIRED>
			<!ATTLIST configuration tag CDATA #REQUIRED>
			<!ATTLIST configuration mask CDATA #REQUIRED>
			<!ELEMENT conflocation EMPTY>
				<!ATTLIST conflocation name CDATA #REQUIRED>
				<!ATTLIST conflocation number CDATA #REQUIRED>
				<!ATTLIST conflocation inverted (yes|no) ""no"">
			<!ELEMENT confsetting (condition?)>
				<!ATTLIST confsetting name CDATA #REQUIRED>
				<!ATTLIST confsetting value CDATA #REQUIRED>
				<!ATTLIST confsetting default (yes|no) ""no"">
		<!ELEMENT port (analog*)>
			<!ATTLIST port tag CDATA #REQUIRED>
			<!ELEMENT analog EMPTY>
				<!ATTLIST analog mask CDATA #REQUIRED>
		<!ELEMENT adjuster (condition?)>
			<!ATTLIST adjuster name CDATA #REQUIRED>
			<!ATTLIST adjuster default CDATA #REQUIRED>
		<!ELEMENT driver EMPTY>
			<!ATTLIST driver status (good|imperfect|preliminary) #REQUIRED>
			<!ATTLIST driver emulation (good|imperfect|preliminary) #REQUIRED>
			<!ATTLIST driver cocktail (good|imperfect|preliminary) #IMPLIED>
			<!ATTLIST driver savestate (supported|unsupported) #REQUIRED>
			<!ATTLIST driver requiresartwork (yes|no) ""no"">
			<!ATTLIST driver unofficial (yes|no) ""no"">
			<!ATTLIST driver nosoundhardware (yes|no) ""no"">
			<!ATTLIST driver incomplete (yes|no) ""no"">
		<!ELEMENT feature EMPTY>
			<!ATTLIST feature type (protection|timing|graphics|palette|sound|capture|camera|microphone|controls|keyboard|mouse|media|disk|printer|tape|punch|drum|rom|comms|lan|wan) #REQUIRED>
			<!ATTLIST feature status (unemulated|imperfect) #IMPLIED>
			<!ATTLIST feature overall (unemulated|imperfect) #IMPLIED>
		<!ELEMENT device (instance?, extension*)>
			<!ATTLIST device type CDATA #REQUIRED>
			<!ATTLIST device tag CDATA #IMPLIED>
			<!ATTLIST device fixed_image CDATA #IMPLIED>
			<!ATTLIST device mandatory CDATA #IMPLIED>
			<!ATTLIST device interface CDATA #IMPLIED>
			<!ELEMENT instance EMPTY>
				<!ATTLIST instance name CDATA #REQUIRED>
				<!ATTLIST instance briefname CDATA #REQUIRED>
			<!ELEMENT extension EMPTY>
				<!ATTLIST extension name CDATA #REQUIRED>
		<!ELEMENT slot (slotoption*)>
			<!ATTLIST slot name CDATA #REQUIRED>
			<!ELEMENT slotoption EMPTY>
				<!ATTLIST slotoption name CDATA #REQUIRED>
				<!ATTLIST slotoption devname CDATA #REQUIRED>
				<!ATTLIST slotoption default (yes|no) ""no"">
		<!ELEMENT softwarelist EMPTY>
			<!ATTLIST softwarelist tag CDATA #REQUIRED>
			<!ATTLIST softwarelist name CDATA #REQUIRED>
			<!ATTLIST softwarelist status (original|compatible) #REQUIRED>
			<!ATTLIST softwarelist filter CDATA #IMPLIED>
		<!ELEMENT ramoption (#PCDATA)>
			<!ATTLIST ramoption name CDATA #REQUIRED>
			<!ATTLIST ramoption default CDATA #IMPLIED>
]>
";

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override Data.Models.Metadata.ItemType[] SupportedTypes
            => [
                Data.Models.Metadata.ItemType.Adjuster,
                Data.Models.Metadata.ItemType.BiosSet,
                Data.Models.Metadata.ItemType.Chip,
                Data.Models.Metadata.ItemType.Condition,
                Data.Models.Metadata.ItemType.Configuration,
                Data.Models.Metadata.ItemType.Device,
                Data.Models.Metadata.ItemType.DeviceRef,
                Data.Models.Metadata.ItemType.DipSwitch,
                Data.Models.Metadata.ItemType.Disk,
                Data.Models.Metadata.ItemType.Display,
                Data.Models.Metadata.ItemType.Driver,
                Data.Models.Metadata.ItemType.Feature,
                Data.Models.Metadata.ItemType.Input,
                Data.Models.Metadata.ItemType.Port,
                Data.Models.Metadata.ItemType.RamOption,
                Data.Models.Metadata.ItemType.Rom,
                Data.Models.Metadata.ItemType.Sample,
                Data.Models.Metadata.ItemType.Slot,
                Data.Models.Metadata.ItemType.SoftwareList,
                Data.Models.Metadata.ItemType.Sound,
            ];

        #endregion

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Listxml(DatFile? datFile) : base(datFile)
        {
            Header.DatFormat = DatFormat.Listxml;
        }

        /// <inheritdoc/>
        public override void ParseFile(string filename,
            int indexId,
            bool keep,
            bool statsOnly = false,
            FilterRunner? filterRunner = null,
            bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var mame = new Serialization.Readers.Listxml().Deserialize(filename);
                Data.Models.Metadata.MetadataFile? metadata;
                if (mame is null)
                {
                    var m1 = new Serialization.Readers.M1().Deserialize(filename);
                    metadata = new Serialization.CrossModel.M1().Serialize(m1);
                }
                else
                {
                    metadata = new Serialization.CrossModel.Listxml().Serialize(mame);
                }

                // Convert to the internal format
                ConvertFromMetadata(metadata, filename, indexId, keep, statsOnly, filterRunner);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                _logger.Error(ex, message);
            }
        }

        /// <inheritdoc/>
        protected internal override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            List<string> missingFields = [];

            switch (datItem)
            {
                case BiosSet biosset:
                    if (string.IsNullOrEmpty(biosset.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.BiosSet.Name));
                    if (string.IsNullOrEmpty(biosset.Description))
                        missingFields.Add(nameof(Data.Models.Metadata.BiosSet.Description));
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.Name));
                    if (rom.Size is null || rom.Size < 0)
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.Size));
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.CRCKey))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA1Key)))
                    {
                        missingFields.Add(Data.Models.Metadata.Rom.SHA1Key);
                    }

                    break;

                case Disk disk:
                    if (string.IsNullOrEmpty(disk.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Disk.Name));
                    if (string.IsNullOrEmpty(disk.MD5)
                        && string.IsNullOrEmpty(disk.SHA1))
                    {
                        missingFields.Add(nameof(Data.Models.Metadata.Disk.SHA1));
                    }

                    break;

                case DeviceRef deviceref:
                    if (string.IsNullOrEmpty(deviceref.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.DeviceRef.Name));
                    break;

                case Sample sample:
                    if (string.IsNullOrEmpty(sample.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Sample.Name));
                    break;

                case Chip chip:
                    if (string.IsNullOrEmpty(chip.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Chip.Name));
                    if (chip.ChipType is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Chip.ChipType));
                    break;

                case Display display:
                    if (display.DisplayType is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Display.DisplayType));
                    if (display.Refresh is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Display.Refresh));
                    break;

                case Sound sound:
                    if (sound.Channels is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Sound.Channels));
                    break;

                case Input input:
                    if (input.Players is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Input.Players));
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrEmpty(dipswitch.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.DipSwitch.Name));
                    if (string.IsNullOrEmpty(dipswitch.Tag))
                        missingFields.Add(nameof(Data.Models.Metadata.DipSwitch.Tag));
                    break;

                case Configuration configuration:
                    if (string.IsNullOrEmpty(configuration.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Configuration.Name));
                    if (string.IsNullOrEmpty(configuration.Tag))
                        missingFields.Add(nameof(Data.Models.Metadata.Configuration.Tag));
                    break;

                case Port port:
                    if (string.IsNullOrEmpty(port.Tag))
                        missingFields.Add(nameof(Data.Models.Metadata.Port.Tag));
                    break;

                case Adjuster adjuster:
                    if (string.IsNullOrEmpty(adjuster.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Adjuster.Name));
                    break;

                case Driver driver:
                    if (driver.Status is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Driver.Status));
                    if (driver.Emulation is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Driver.Emulation));
                    if (driver.Cocktail is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Driver.Cocktail));
                    if (driver.SaveState is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Driver.SaveState));
                    break;

                case Feature feature:
                    if (feature.FeatureType is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Feature.FeatureType));
                    break;

                case Device device:
                    if (device.DeviceType is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Device.DeviceType));
                    break;

                case Slot slot:
                    if (string.IsNullOrEmpty(slot.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Slot.Name));
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrEmpty(softwarelist.Tag))
                        missingFields.Add(nameof(Data.Models.Metadata.SoftwareList.Tag));
                    if (string.IsNullOrEmpty(softwarelist.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.SoftwareList.Name));
                    if (softwarelist.Status == null)
                        missingFields.Add(nameof(Data.Models.Metadata.SoftwareList.Status));
                    break;

                case RamOption ramoption:
                    if (string.IsNullOrEmpty(ramoption.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.RamOption.Name));
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
