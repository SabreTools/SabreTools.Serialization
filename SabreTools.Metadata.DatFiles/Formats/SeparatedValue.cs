using System;
using System.Collections.Generic;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using SabreTools.Metadata.Filter;

#pragma warning disable IDE0290 // Use primary constructor
namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents a value-separated DAT
    /// </summary>
    public abstract class SeparatedValue : SerializableDatFile<Data.Models.SeparatedValue.MetadataFile, Serialization.Readers.SeparatedValue, Serialization.Writers.SeparatedValue, Serialization.CrossModel.SeparatedValue>
    {
        #region Fields

        /// <inheritdoc/>
        public override Data.Models.Metadata.ItemType[] SupportedTypes
            => [
                Data.Models.Metadata.ItemType.Disk,
                Data.Models.Metadata.ItemType.Media,
                Data.Models.Metadata.ItemType.Rom,
            ];

        /// <summary>
        /// Represents the delimiter between fields
        /// </summary>
        protected char _delim;

        #endregion

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SeparatedValue(DatFile? datFile) : base(datFile)
        {
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
                var metadataFile = new Serialization.Readers.SeparatedValue().Deserialize(filename, _delim);
                var metadata = new Serialization.CrossModel.SeparatedValue().Serialize(metadataFile);

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

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(nameof(Data.Models.Metadata.Rom.Name));

            switch (datItem)
            {
                case Disk disk:
                    if (string.IsNullOrEmpty(disk.ReadString(Data.Models.Metadata.Disk.MD5Key))
                        && string.IsNullOrEmpty(disk.ReadString(Data.Models.Metadata.Disk.SHA1Key)))
                    {
                        missingFields.Add(Data.Models.Metadata.Disk.SHA1Key);
                    }

                    break;

                case Media media:
                    if (string.IsNullOrEmpty(media.ReadString(Data.Models.Metadata.Media.MD5Key))
                        && string.IsNullOrEmpty(media.ReadString(Data.Models.Metadata.Media.SHA1Key))
                        && string.IsNullOrEmpty(media.ReadString(Data.Models.Metadata.Media.SHA256Key))
                        && string.IsNullOrEmpty(media.ReadString(Data.Models.Metadata.Media.SpamSumKey)))
                    {
                        missingFields.Add(Data.Models.Metadata.Media.SHA1Key);
                    }

                    break;

                case Rom rom:
                    if (rom.ReadLong(Data.Models.Metadata.Rom.SizeKey) is null || rom.ReadLong(Data.Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Data.Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.CRCKey))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.MD5Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA1Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA256Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA384Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA512Key))
                        && string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SpamSumKey)))
                    {
                        missingFields.Add(Data.Models.Metadata.Rom.SHA1Key);
                    }

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
                var metadataFile = new Serialization.CrossModel.SeparatedValue().Deserialize(metadata);
                if (!new Serialization.Writers.SeparatedValue().SerializeFile(metadataFile, outfile, _delim, longHeader: false))
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
