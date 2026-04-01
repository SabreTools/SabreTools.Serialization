using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an SpamSum hashfile
    /// </summary>
    public sealed class SpamSumFile : Hashfile
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.ItemType[] SupportedTypes
            => [
                Data.Models.Metadata.ItemType.Media,
                Data.Models.Metadata.ItemType.Rom,
            ];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SpamSumFile(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.SpamSum;
            Header.Write(DatHeader.DatFormatKey, DatFormat.RedumpSpamSum);
        }

        /// <inheritdoc/>
        protected internal override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            List<string> missingFields = [];

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Data.Models.Metadata.Rom.NameKey);

            switch (datItem)
            {
                case Media medium:
                    if (string.IsNullOrEmpty(medium.ReadString(Data.Models.Metadata.Media.SpamSumKey)))
                        missingFields.Add(Data.Models.Metadata.Media.SpamSumKey);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SpamSumKey)))
                        missingFields.Add(Data.Models.Metadata.Rom.SpamSumKey);
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
