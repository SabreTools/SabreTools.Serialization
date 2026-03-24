using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

#pragma warning disable IDE0290 // Use primary constructor
namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an SHA-1 hashfile
    /// </summary>
    public sealed class Sha1File : Hashfile
    {
        /// <inheritdoc/>
        public override ItemType[] SupportedTypes
            => [
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Sha1File(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.SHA1;
            Header.SetFieldValue(DatHeader.DatFormatKey, DatFormat.RedumpSHA1);
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
                case Disk disk:
                    if (string.IsNullOrEmpty(disk.GetStringFieldValue(Data.Models.Metadata.Disk.SHA1Key)))
                        missingFields.Add(Data.Models.Metadata.Disk.SHA1Key);
                    break;

                case Media medium:
                    if (string.IsNullOrEmpty(medium.GetStringFieldValue(Data.Models.Metadata.Media.SHA1Key)))
                        missingFields.Add(Data.Models.Metadata.Media.SHA1Key);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetStringFieldValue(Data.Models.Metadata.Rom.SHA1Key)))
                        missingFields.Add(Data.Models.Metadata.Rom.SHA1Key);
                    break;
            }
#pragma warning restore IDE0010

            return missingFields;
        }
    }
}
