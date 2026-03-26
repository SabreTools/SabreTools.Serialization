using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

#pragma warning disable IDE0290 // Use primary constructor
namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an SHA-512 hashfile
    /// </summary>
    public sealed class Sha512File : Hashfile
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
        public Sha512File(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.SHA512;
            Header.Write(DatHeader.DatFormatKey, DatFormat.RedumpSHA512);
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
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA512Key)))
                        missingFields.Add(Data.Models.Metadata.Rom.SHA512Key);
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
