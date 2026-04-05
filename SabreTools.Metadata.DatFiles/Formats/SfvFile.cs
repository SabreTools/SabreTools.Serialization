using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an SFV (CRC-32) hashfile
    /// </summary>
    public sealed class SfvFile : Hashfile
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
        public SfvFile(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.CRC32;
            Header.DatFormat = DatFormat.RedumpSFV;
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
                    if (string.IsNullOrEmpty(rom.CRC))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.CRC));
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
