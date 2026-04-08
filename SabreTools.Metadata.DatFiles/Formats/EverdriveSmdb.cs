using System.Collections.Generic;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of an Everdrive SMDB file
    /// </summary>
    public sealed class EverdriveSMDB : SerializableDatFile<Data.Models.EverdriveSMDB.MetadataFile, Serialization.Readers.EverdriveSMDB, Serialization.Writers.EverdriveSMDB, Serialization.CrossModel.EverdriveSMDB>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.ItemType[] SupportedTypes
            => [
                Data.Models.Metadata.ItemType.Rom,
            ];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public EverdriveSMDB(DatFile? datFile) : base(datFile)
        {
            Header.DatFormat = DatFormat.EverdriveSMDB;
        }

        /// <inheritdoc/>
        protected internal override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            List<string> missingFields = [];

            switch (datItem)
            {
                case Rom rom:
                    if (string.IsNullOrEmpty(rom.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.Name));
                    if (string.IsNullOrEmpty(rom.SHA256))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.SHA256));
                    if (string.IsNullOrEmpty(rom.SHA1))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.SHA1));
                    if (string.IsNullOrEmpty(rom.MD5))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.MD5));
                    if (string.IsNullOrEmpty(rom.CRC32))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.CRC32));
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
