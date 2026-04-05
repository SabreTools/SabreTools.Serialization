using System.Collections.Generic;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a DosCenter DAT
    /// </summary>
    public sealed class DosCenter : SerializableDatFile<Data.Models.DosCenter.MetadataFile, Serialization.Readers.DosCenter, Serialization.Writers.DosCenter, Serialization.CrossModel.DosCenter>
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
        public DosCenter(DatFile? datFile) : base(datFile)
        {
            Header.DatFormat = DatFormat.DOSCenter;
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
                    if (rom.Size is null || rom.Size < 0)
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.Size));
                    // if (string.IsNullOrEmpty(rom.Date))
                    //     missingFields.Add(nameof(Data.Models.Metadata.Rom.Date));
                    if (string.IsNullOrEmpty(rom.CRC))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.CRC));
                    // if (string.IsNullOrEmpty(rom.GetStringFieldValue(Data.Models.Metadata.Rom.SHA1Key)))
                    //     missingFields.Add(nameof(Data.Models.Metadata.Rom.SHA1));
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
