using System;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Metadata
{
    public static class DictionaryBaseExtensions
    {
        #region Equality Checking

        /// <summary>
        /// Check equality of two Disk objects
        /// </summary>
        public static bool PartialEquals(this Disk self, Disk other)
        {
            ItemStatus? selfStatus = self.Status;
            ItemStatus? otherStatus = other.Status;

            string? selfName = self.Name;
            string? otherName = other.Name;

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if (selfStatus == ItemStatus.Nodump
                && otherStatus == ItemStatus.Nodump
                && string.Equals(selfName, otherName, StringComparison.OrdinalIgnoreCase)
                && !self.HasHashes()
                && !other.HasHashes())
            {
                return true;
            }

            // If we get a partial match
            if (self.HashMatch(other))
                return true;

            // All other cases fail
            return false;
        }

        /// <summary>
        /// Check equality of two Media objects
        /// </summary>
        public static bool PartialEquals(this Media self, Media other)
        {
            // If we get a partial match
            if (self.HashMatch(other))
                return true;

            // All other cases fail
            return false;
        }

        /// <summary>
        /// Check equality of two Rom objects
        /// </summary>
        public static bool PartialEquals(this Rom self, Rom other)
        {
            ItemStatus? selfStatus = self.Status;
            ItemStatus? otherStatus = other.Status;

            string? selfName = self.Name;
            string? otherName = other.Name;

            long? selfSize = self.Size;
            long? otherSize = other.Size;

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if (selfStatus == ItemStatus.Nodump
                && otherStatus == ItemStatus.Nodump
                && string.Equals(selfName, otherName, StringComparison.OrdinalIgnoreCase)
                && !self.HasHashes()
                && !other.HasHashes())
            {
                return true;
            }

            // If we have a file that has no known size, rely on the hashes only
            if (selfSize is null && self.HashMatch(other))
                return true;
            else if (otherSize is null && self.HashMatch(other))
                return true;

            // If we get a partial match
            if (selfSize == otherSize && self.HashMatch(other))
                return true;

            // All other cases fail
            return false;
        }

        #endregion
    }
}
