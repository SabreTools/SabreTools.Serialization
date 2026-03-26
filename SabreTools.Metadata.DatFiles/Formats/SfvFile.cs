using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

#pragma warning disable IDE0290 // Use primary constructor
namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an SFV (CRC-32) hashfile
    /// </summary>
    public sealed class SfvFile : Hashfile
    {
        /// <inheritdoc/>
        public override ItemType[] SupportedTypes
            => [
                ItemType.Rom,
            ];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SfvFile(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.CRC32;
            Header.Write(DatHeader.DatFormatKey, DatFormat.RedumpSFV);
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
                case Rom rom:
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
