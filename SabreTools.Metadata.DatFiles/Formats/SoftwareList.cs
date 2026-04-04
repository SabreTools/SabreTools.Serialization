using System;
using System.Collections.Generic;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a SoftwareList
    /// </summary>
    public sealed class SoftwareList : SerializableDatFile<Data.Models.SoftwareList.SoftwareList, Serialization.Readers.SoftwareList, Serialization.Writers.SoftwareList, Serialization.CrossModel.SoftwareList>
    {
        #region Constants

        /// <summary>
        /// DTD for original MAME Software List DATs
        /// </summary>
        /// <remarks>
        /// TODO: See if there's an updated DTD and then check for required fields
        /// </remarks>
        internal const string SoftwareListDTD = @"<!ELEMENT softwarelist (notes?, software+)>
	<!ATTLIST softwarelist name CDATA #REQUIRED>
	<!ATTLIST softwarelist description CDATA #IMPLIED>
	<!ELEMENT notes (#PCDATA)>
	<!ELEMENT software (description, year, publisher, notes?, info*, sharedfeat*, part*)>
		<!ATTLIST software name CDATA #REQUIRED>
		<!ATTLIST software cloneof CDATA #IMPLIED>
		<!ATTLIST software supported (yes|partial|no) ""yes"">
		<!ELEMENT description (#PCDATA)>
		<!ELEMENT year (#PCDATA)>
		<!ELEMENT publisher (#PCDATA)>
		<!ELEMENT info EMPTY>
			<!ATTLIST info name CDATA #REQUIRED>
			<!ATTLIST info value CDATA #IMPLIED>
		<!ELEMENT sharedfeat EMPTY>
			<!ATTLIST sharedfeat name CDATA #REQUIRED>
			<!ATTLIST sharedfeat value CDATA #IMPLIED>
		<!ELEMENT part (feature*, dataarea*, diskarea*, dipswitch*)>
			<!ATTLIST part name CDATA #REQUIRED>
			<!ATTLIST part interface CDATA #REQUIRED>
			<!-- feature is used to store things like pcb-type, mapper type, etc. Specific values depend on the system. -->
			<!ELEMENT feature EMPTY>
				<!ATTLIST feature name CDATA #REQUIRED>
				<!ATTLIST feature value CDATA #IMPLIED>
			<!ELEMENT dataarea (rom*)>
				<!ATTLIST dataarea name CDATA #REQUIRED>
				<!ATTLIST dataarea size CDATA #REQUIRED>
				<!ATTLIST dataarea width (8|16|32|64) ""8"">
				<!ATTLIST dataarea endianness (big|little) ""little"">
				<!ELEMENT rom EMPTY>
					<!ATTLIST rom name CDATA #IMPLIED>
					<!ATTLIST rom size CDATA #IMPLIED>
					<!ATTLIST rom crc CDATA #IMPLIED>
					<!ATTLIST rom sha1 CDATA #IMPLIED>
					<!ATTLIST rom offset CDATA #IMPLIED>
					<!ATTLIST rom value CDATA #IMPLIED>
					<!ATTLIST rom status (baddump|nodump|good) ""good"">
					<!ATTLIST rom loadflag (load16_byte|load16_word|load16_word_swap|load32_byte|load32_word|load32_word_swap|load32_dword|load64_word|load64_word_swap|reload|fill|continue|reload_plain|ignore) #IMPLIED>
			<!ELEMENT diskarea (disk*)>
				<!ATTLIST diskarea name CDATA #REQUIRED>
				<!ELEMENT disk EMPTY>
					<!ATTLIST disk name CDATA #REQUIRED>
					<!ATTLIST disk sha1 CDATA #IMPLIED>
					<!ATTLIST disk status (baddump|nodump|good) ""good"">
					<!ATTLIST disk writeable (yes|no) ""no"">
			<!ELEMENT dipswitch (dipvalue*)>
				<!ATTLIST dipswitch name CDATA #REQUIRED>
				<!ATTLIST dipswitch tag CDATA #REQUIRED>
				<!ATTLIST dipswitch mask CDATA #REQUIRED>
				<!ELEMENT dipvalue EMPTY>
					<!ATTLIST dipvalue name CDATA #REQUIRED>
					<!ATTLIST dipvalue value CDATA #REQUIRED>
					<!ATTLIST dipvalue default (yes|no) ""no"">
";

        #endregion

        #region Fields

        /// <inheritdoc/>
        public override Data.Models.Metadata.ItemType[] SupportedTypes
            => [
                Data.Models.Metadata.ItemType.DipSwitch,
                Data.Models.Metadata.ItemType.Disk,
                Data.Models.Metadata.ItemType.Info,
                Data.Models.Metadata.ItemType.PartFeature,
                Data.Models.Metadata.ItemType.Rom,
                Data.Models.Metadata.ItemType.SharedFeat,
            ];

        #endregion

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SoftwareList(DatFile? datFile) : base(datFile)
        {
            Header.DatFormat = DatFormat.SoftwareList;
        }

        /// <inheritdoc/>
        protected internal override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            List<string> missingFields = [];

            switch (datItem)
            {
                case DipSwitch dipSwitch:
                    if (!dipSwitch.PartSpecified)
                    {
                        missingFields.Add(nameof(Data.Models.Metadata.Part.Name));
                        missingFields.Add(nameof(Data.Models.Metadata.Part.Interface));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(dipSwitch.Part!.Name))
                            missingFields.Add(nameof(Data.Models.Metadata.Part.Name));
                        if (string.IsNullOrEmpty(dipSwitch.Part!.Interface))
                            missingFields.Add(nameof(Data.Models.Metadata.Part.Interface));
                    }

                    if (string.IsNullOrEmpty(dipSwitch.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.DipSwitch.Name));
                    if (string.IsNullOrEmpty(dipSwitch.Tag))
                        missingFields.Add(nameof(Data.Models.Metadata.DipSwitch.Tag));
                    if (string.IsNullOrEmpty(dipSwitch.Mask))
                        missingFields.Add(nameof(Data.Models.Metadata.DipSwitch.Mask));
                    if (dipSwitch.DipValueSpecified)
                    {
                        var dipValues = dipSwitch.DipValue;
                        if (Array.Find(dipValues!, dv => string.IsNullOrEmpty(dv.Name)) is not null)
                            missingFields.Add(nameof(Data.Models.Metadata.DipValue.Name));
                        if (Array.Find(dipValues!, dv => string.IsNullOrEmpty(dv.Value)) is not null)
                            missingFields.Add(nameof(Data.Models.Metadata.DipValue.Value));
                    }

                    break;

                case Disk disk:
                    if (!disk.PartSpecified)
                    {
                        missingFields.Add(nameof(Data.Models.Metadata.Part.Name));
                        missingFields.Add(nameof(Data.Models.Metadata.Part.Interface));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(disk.Part!.Name))
                            missingFields.Add(nameof(Data.Models.Metadata.Part.Name));
                        if (string.IsNullOrEmpty(disk.Part.Interface))
                            missingFields.Add(nameof(Data.Models.Metadata.Part.Interface));
                    }

                    if (!disk.DiskAreaSpecified)
                    {
                        missingFields.Add(nameof(Data.Models.Metadata.DiskArea.Name));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(disk.DiskArea!.Name))
                            missingFields.Add(nameof(Data.Models.Metadata.DiskArea.Name));
                    }

                    if (string.IsNullOrEmpty(disk.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Disk.Name));
                    break;

                case Info info:
                    if (string.IsNullOrEmpty(info.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Info.Name));
                    break;

                case Rom rom:
                    if (!rom.PartSpecified)
                    {
                        missingFields.Add(nameof(Data.Models.Metadata.Part.Name));
                        missingFields.Add(nameof(Data.Models.Metadata.Part.Interface));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(rom.Part!.Name))
                            missingFields.Add(nameof(Data.Models.Metadata.Part.Name));
                        if (string.IsNullOrEmpty(rom.Part!.Interface))
                            missingFields.Add(nameof(Data.Models.Metadata.Part.Interface));
                    }

                    if (!rom.DataAreaSpecified)
                    {
                        missingFields.Add(nameof(Data.Models.Metadata.DataArea.Name));
                        missingFields.Add(nameof(Data.Models.Metadata.DataArea.Size));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(rom.DataArea!.Name))
                            missingFields.Add(nameof(Data.Models.Metadata.DataArea.Name));
                        if (rom.DataArea!.Size is null)
                            missingFields.Add(nameof(Data.Models.Metadata.DataArea.Size));
                    }

                    break;

                case SharedFeat sharedFeat:
                    if (string.IsNullOrEmpty(sharedFeat.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.SharedFeat.Name));
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
