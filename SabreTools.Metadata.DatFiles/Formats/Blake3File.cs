using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an BLAKE3 hashfile
    /// </summary>
    public sealed class Blake3File : Hashfile
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
        public Blake3File(DatFile? datFile) : base(datFile)
        {
#if NET7_0_OR_GREATER
            _hash = HashType.BLAKE3;
#else
            // HACK because BLAKE3 is not supported below .NET 7
            _hash = HashType.CRC1_ZERO;
#endif
            Header.DatFormat = DatFormat.RedumpBLAKE3;
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
                    if (string.IsNullOrEmpty(rom.BLAKE3))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.BLAKE3));
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
