using System.Collections.Generic;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents a RomCenter INI file
    /// </summary>
    public sealed class RomCenter : SerializableDatFile<Data.Models.RomCenter.MetadataFile, Serialization.Readers.RomCenter, Serialization.Writers.RomCenter, Serialization.CrossModel.RomCenter>
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
        public RomCenter(DatFile? datFile) : base(datFile)
        {
            Header.Write(DatHeader.DatFormatKey, DatFormat.RomCenter);
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
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.CRCKey)))
                        missingFields.Add(Data.Models.Metadata.Rom.CRCKey);
                    if (rom.ReadLong(Data.Models.Metadata.Rom.SizeKey) is null || rom.ReadLong(Data.Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Data.Models.Metadata.Rom.SizeKey);
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
