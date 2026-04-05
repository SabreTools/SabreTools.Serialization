using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an SHA-384 hashfile
    /// </summary>
    public sealed class Sha384File : Hashfile
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
        public Sha384File(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.SHA384;
            Header.DatFormat = DatFormat.RedumpSHA384;
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
                    if (string.IsNullOrEmpty(rom.SHA384))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.SHA384));
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
