using System.Collections.Generic;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an OfflineList XML DAT
    /// </summary>
    public sealed class OfflineList : SerializableDatFile<Data.Models.OfflineList.Dat, Serialization.Readers.OfflineList, Serialization.Writers.OfflineList, Serialization.CrossModel.OfflineList>
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
        public OfflineList(DatFile? datFile) : base(datFile)
        {
            Header.DatFormat = DatFormat.OfflineList;
        }

        /// <inheritdoc/>
        protected internal override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            List<string> missingFields = [];

            switch (datItem)
            {
                case Rom rom:
                    if (rom.Size is null || rom.Size < 0)
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.Size));
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
