using System;
using System.Collections.Generic;
using SabreTools.Data.Extensions;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using SabreTools.Metadata.Filter;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents a ClrMamePro DAT
    /// </summary>
    public sealed class ClrMamePro : SerializableDatFile<Data.Models.ClrMamePro.MetadataFile, Serialization.Readers.ClrMamePro, Serialization.Writers.ClrMamePro, Serialization.CrossModel.ClrMamePro>
    {
        #region Fields

        /// <inheritdoc/>
        public override Data.Models.Metadata.ItemType[] SupportedTypes
            => [
                Data.Models.Metadata.ItemType.Archive,
                Data.Models.Metadata.ItemType.BiosSet,
                Data.Models.Metadata.ItemType.Chip,
                Data.Models.Metadata.ItemType.DipSwitch,
                Data.Models.Metadata.ItemType.Disk,
                Data.Models.Metadata.ItemType.Display,
                Data.Models.Metadata.ItemType.Driver,
                Data.Models.Metadata.ItemType.Input,
                Data.Models.Metadata.ItemType.Media,
                Data.Models.Metadata.ItemType.Release,
                Data.Models.Metadata.ItemType.Rom,
                Data.Models.Metadata.ItemType.Sample,
                Data.Models.Metadata.ItemType.Sound,
            ];

        #endregion

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public ClrMamePro(DatFile? datFile) : base(datFile)
        {
            Header.Write(DatHeader.DatFormatKey, DatFormat.ClrMamePro);
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
                var metadataFile = new Serialization.Readers.ClrMamePro().Deserialize(filename, quotes: true);
                var metadata = new Serialization.CrossModel.ClrMamePro().Serialize(metadataFile);

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
                case Release release:
                    if (string.IsNullOrEmpty(release.GetName()))
                        missingFields.Add(Data.Models.Metadata.Release.NameKey);
                    if (string.IsNullOrEmpty(release.ReadString(Data.Models.Metadata.Release.RegionKey)))
                        missingFields.Add(Data.Models.Metadata.Release.RegionKey);
                    break;

                case BiosSet biosset:
                    if (string.IsNullOrEmpty(biosset.GetName()))
                        missingFields.Add(Data.Models.Metadata.BiosSet.NameKey);
                    if (string.IsNullOrEmpty(biosset.ReadString(Data.Models.Metadata.BiosSet.DescriptionKey)))
                        missingFields.Add(Data.Models.Metadata.BiosSet.DescriptionKey);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetName()))
                        missingFields.Add(Data.Models.Metadata.Rom.NameKey);
                    if (rom.ReadLong(Data.Models.Metadata.Rom.SizeKey) is null || rom.ReadLong(Data.Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Data.Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.CRC16Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.CRCKey))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.CRC64Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.MD2Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.MD4Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.MD5Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.RIPEMD128Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.RIPEMD160Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA1Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA256Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA384Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA512Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SpamSumKey)))
                    {
                        missingFields.Add(Data.Models.Metadata.Rom.SHA1Key);
                    }

                    break;

                case Disk disk:
                    if (string.IsNullOrEmpty(disk.GetName()))
                        missingFields.Add(Data.Models.Metadata.Disk.NameKey);
                    if (string.IsNullOrEmpty(disk.ReadString(Data.Models.Metadata.Disk.MD5Key))
                        && string.IsNullOrEmpty(disk.ReadString(Data.Models.Metadata.Disk.SHA1Key)))
                    {
                        missingFields.Add(Data.Models.Metadata.Disk.SHA1Key);
                    }

                    break;

                case Sample sample:
                    if (string.IsNullOrEmpty(sample.GetName()))
                        missingFields.Add(Data.Models.Metadata.Sample.NameKey);
                    break;

                case Archive archive:
                    if (string.IsNullOrEmpty(archive.GetName()))
                        missingFields.Add(Data.Models.Metadata.Archive.NameKey);
                    break;

                case Chip chip:
                    if (chip.ReadString(Data.Models.Metadata.Chip.ChipTypeKey).AsChipType() is null)
                        missingFields.Add(Data.Models.Metadata.Chip.ChipTypeKey);
                    if (string.IsNullOrEmpty(chip.GetName()))
                        missingFields.Add(Data.Models.Metadata.Chip.NameKey);
                    break;

                case Display display:
                    if (display.ReadString(Data.Models.Metadata.Display.DisplayTypeKey).AsDisplayType() is null)
                        missingFields.Add(Data.Models.Metadata.Display.DisplayTypeKey);
                    if (display.ReadLong(Data.Models.Metadata.Display.RotateKey) is null)
                        missingFields.Add(Data.Models.Metadata.Display.RotateKey);
                    break;

                case Sound sound:
                    if (sound.ReadLong(Data.Models.Metadata.Sound.ChannelsKey) is null)
                        missingFields.Add(Data.Models.Metadata.Sound.ChannelsKey);
                    break;

                case Input input:
                    if (input.ReadLong(Data.Models.Metadata.Input.PlayersKey) is null)
                        missingFields.Add(Data.Models.Metadata.Input.PlayersKey);
                    if (!input.ControlsSpecified)
                        missingFields.Add(Data.Models.Metadata.Input.ControlKey);
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrEmpty(dipswitch.GetName()))
                        missingFields.Add(Data.Models.Metadata.DipSwitch.NameKey);
                    break;

                case Driver driver:
                    if (driver.ReadString(Data.Models.Metadata.Driver.StatusKey).AsSupportStatus() is null)
                        missingFields.Add(Data.Models.Metadata.Driver.StatusKey);
                    if (driver.ReadString(Data.Models.Metadata.Driver.EmulationKey).AsSupportStatus() is null)
                        missingFields.Add(Data.Models.Metadata.Driver.EmulationKey);
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                _logger.User($"Writing to '{outfile}'...");

                // Serialize the input file
                var metadata = ConvertToMetadata(ignoreblanks);
                var metadataFile = new Serialization.CrossModel.ClrMamePro().Deserialize(metadata);
                if (!new Serialization.Writers.ClrMamePro().SerializeFile(metadataFile, outfile, quotes: true))
                {
                    _logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Error(ex);
                return false;
            }

            _logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }
    }
}
