using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

#pragma warning disable IDE0290 // Use primary constructor
namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an SpamSum hashfile
    /// </summary>
    public sealed class SpamSumFile : Hashfile
    {
        /// <inheritdoc/>
        public override ItemType[] SupportedTypes
            => [
                ItemType.Media,
                ItemType.Rom,
            ];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SpamSumFile(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.SpamSum;
            Header.SetFieldValue(DatHeader.DatFormatKey, DatFormat.RedumpSpamSum);
        }

        /// <inheritdoc/>
        protected internal override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            List<string> missingFields = [];

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Data.Models.Metadata.Rom.NameKey);

#pragma warning disable IDE0010
            switch (datItem)
            {
                case Media medium:
                    if (string.IsNullOrEmpty(medium.GetStringFieldValue(Data.Models.Metadata.Media.SpamSumKey)))
                        missingFields.Add(Data.Models.Metadata.Media.SpamSumKey);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetStringFieldValue(Data.Models.Metadata.Rom.SpamSumKey)))
                        missingFields.Add(Data.Models.Metadata.Rom.SpamSumKey);
                    break;
            }
#pragma warning restore IDE0010

            return missingFields;
        }
    }
}
