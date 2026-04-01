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
            Header.Write(DatHeader.DatFormatKey, DatFormat.EverdriveSMDB);
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
                case Rom rom:
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA256Key)))
                        missingFields.Add(Data.Models.Metadata.Rom.SHA256Key);
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA1Key)))
                        missingFields.Add(Data.Models.Metadata.Rom.SHA1Key);
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.MD5Key)))
                        missingFields.Add(Data.Models.Metadata.Rom.MD5Key);
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.CRCKey)))
                        missingFields.Add(Data.Models.Metadata.Rom.CRCKey);
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
