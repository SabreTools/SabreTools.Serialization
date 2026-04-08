using System;
using System.Collections.Generic;
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
        #region Properties

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
            Header.DatFormat = DatFormat.ClrMamePro;
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
                    if (string.IsNullOrEmpty(release.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Release.Name));
                    if (string.IsNullOrEmpty(release.Region))
                        missingFields.Add(nameof(Data.Models.Metadata.Release.Region));
                    break;

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
                    if (string.IsNullOrEmpty(rom.CRC16)
                        && string.IsNullOrEmpty(rom.CRC32)
                        && string.IsNullOrEmpty(rom.CRC64)
                        && string.IsNullOrEmpty(rom.MD2)
                        && string.IsNullOrEmpty(rom.MD4)
                        && string.IsNullOrEmpty(rom.MD5)
                        && string.IsNullOrEmpty(rom.RIPEMD128)
                        && string.IsNullOrEmpty(rom.RIPEMD160)
                        && string.IsNullOrEmpty(rom.SHA1)
                        && string.IsNullOrEmpty(rom.SHA256)
                        && string.IsNullOrEmpty(rom.SHA384)
                        && string.IsNullOrEmpty(rom.SHA512)
                        && string.IsNullOrEmpty(rom.SpamSum))
                    {
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.SHA1));
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

                case Sample sample:
                    if (string.IsNullOrEmpty(sample.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Sample.Name));
                    break;

                case Archive archive:
                    if (string.IsNullOrEmpty(archive.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Archive.Name));
                    break;

                case Chip chip:
                    if (chip.ChipType is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Chip.ChipType));
                    if (string.IsNullOrEmpty(chip.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Chip.Name));
                    break;

                case Display display:
                    if (display.DisplayType is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Display.DisplayType));
                    if (display.Rotate is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Display.Rotate));
                    break;

                case Sound sound:
                    if (sound.Channels is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Sound.Channels));
                    break;

                case Input input:
                    if (input.Players is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Input.Players));
                    if (!input.ControlSpecified)
                        missingFields.Add(nameof(Data.Models.Metadata.Input.Control));
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrEmpty(dipswitch.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.DipSwitch.Name));
                    break;

                case Driver driver:
                    if (driver.Status is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Driver.Status));
                    if (driver.Emulation is null)
                        missingFields.Add(nameof(Data.Models.Metadata.Driver.Emulation));
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
