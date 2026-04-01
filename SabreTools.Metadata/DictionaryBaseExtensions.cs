using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Metadata
{
    public static class DictionaryBaseExtensions
    {
        #region Equality Checking

        /// <summary>
        /// Check equality of two DictionaryBase objects
        /// </summary>
        public static bool EqualTo(this DictionaryBase self, DictionaryBase other)
        {
            // Check types first
            if (self.GetType() != other.GetType())
                return false;

            // Check based on the item type
#if NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            return (self, other) switch
            {
                (Disk diskSelf, Disk diskOther) => EqualsImpl(diskSelf, diskOther),
                (Media mediaSelf, Media mediaOther) => EqualsImpl(mediaSelf, mediaOther),
                (Rom romSelf, Rom romOther) => EqualsImpl(romSelf, romOther),
                _ => EqualsImpl(self, other),
            };
#else
            if (self is Disk diskSelf && other is Disk diskOther)
                return EqualsImpl(diskSelf, diskOther);
            else if (self is Media mediaSelf && other is Media mediaOther)
                return EqualsImpl(mediaSelf, mediaOther);
            else if (self is Rom romSelf && other is Rom romOther)
                return EqualsImpl(romSelf, romOther);
            else
                return EqualsImpl(self, other);
#endif
        }

        /// <summary>
        /// Check equality of two DictionaryBase objects
        /// </summary>
        private static bool EqualsImpl(this DictionaryBase self, DictionaryBase other)
        {
            // If the number of key-value pairs doesn't match, they can't match
            if (self.Count != other.Count)
                return false;

            // If any keys are missing on either side, they can't match
            var selfKeys = new HashSet<string>(self.Keys);
            var otherKeys = new HashSet<string>(other.Keys);
            if (!selfKeys.SetEquals(otherKeys))
                return false;

            // Check names
            if (self.GetName() != other.GetName())
                return false;

            // Check all pairs to see if they're equal
            foreach (var kvp in self)
            {
#if NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                switch (kvp.Value, other[kvp.Key])
                {
                    case (string selfString, string otherString):
                        if (!string.Equals(selfString, otherString, StringComparison.OrdinalIgnoreCase))
                            return false;
                        break;

                    case (ModelBackedItem selfMbi, ModelBackedItem otherMbi):
                        if (!selfMbi.Equals(otherMbi))
                            return false;
                        break;

                    case (DictionaryBase selfDb, DictionaryBase otherDb):
                        if (!selfDb.Equals(otherDb))
                            return false;
                        break;

                    // TODO: Make this case-insensitive
                    case (string[] selfStrArr, string[] otherStrArr):
                        if (selfStrArr.Length != otherStrArr.Length)
                            return false;
                        if (selfStrArr.Except(otherStrArr).Any())
                            return false;
                        if (otherStrArr.Except(selfStrArr).Any())
                            return false;
                        break;

                    // TODO: Fix the logic here for real equality
                    case (DictionaryBase[] selfDbArr, DictionaryBase[] otherDbArr):
                        if (selfDbArr.Length != otherDbArr.Length)
                            return false;
                        if (selfDbArr.Except(otherDbArr).Any())
                            return false;
                        if (otherDbArr.Except(selfDbArr).Any())
                            return false;
                        break;

                    default:
                        // Handle cases where a null is involved
                        if (kvp.Value is null && other[kvp.Key] is null)
                            return true;
                        else if (kvp.Value is null && other[kvp.Key] is not null)
                            return false;
                        else if (kvp.Value is not null && other[kvp.Key] is null)
                            return false;

                        // Try to rely on type hashes
                        else if (kvp.Value!.GetHashCode() != other[kvp.Key]!.GetHashCode())
                            return false;

                        break;
                }
#else
                if (kvp.Value is string selfString && other[kvp.Key] is string otherString)
                {
                    if (!string.Equals(selfString, otherString, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                else if (kvp.Value is ModelBackedItem selfMbi && other[kvp.Key] is ModelBackedItem otherMbi)
                {
                    if (!selfMbi.Equals(otherMbi))
                        return false;
                }
                else if (kvp.Value is DictionaryBase selfDb && other[kvp.Key] is DictionaryBase otherDb)
                {
                    if (!selfDb.Equals(otherDb))
                        return false;
                }
                else if (kvp.Value is string[] selfStrArr && other[kvp.Key] is string[] otherStrArr)
                {
                    // TODO: Make this case-insensitive
                    if (selfStrArr.Length != otherStrArr.Length)
                        return false;
                    if (selfStrArr.Except(otherStrArr).Any())
                        return false;
                    if (otherStrArr.Except(selfStrArr).Any())
                        return false;
                }
                else if (kvp.Value is DictionaryBase[] selfDbArr && other[kvp.Key] is DictionaryBase[] otherDbArr)
                {
                    // TODO: Fix the logic here for real equality
                    if (selfDbArr.Length != otherDbArr.Length)
                        return false;
                    if (selfDbArr.Except(otherDbArr).Any())
                        return false;
                    if (otherDbArr.Except(selfDbArr).Any())
                        return false;
                }
                else
                {
                    // Handle cases where a null is involved
                    if (kvp.Value is null && other[kvp.Key] is null)
                        return true;
                    else if (kvp.Value is null && other[kvp.Key] is not null)
                        return false;
                    else if (kvp.Value is not null && other[kvp.Key] is null)
                        return false;

                    // Try to rely on type hashes
                    else if (kvp.Value!.GetHashCode() != other[kvp.Key]!.GetHashCode())
                        return false;
                }
#endif
            }

            return true;
        }

        /// <summary>
        /// Check equality of two Disk objects
        /// </summary>
        private static bool EqualsImpl(this Disk self, Disk other)
        {
            string? selfStatus = self.ReadString(Disk.StatusKey);
            string? otherStatus = other.ReadString(Disk.StatusKey);

            string? selfName = self.Name;
            string? otherName = other.Name;

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if (string.Equals(selfStatus, "nodump", StringComparison.OrdinalIgnoreCase)
                && string.Equals(otherStatus, "nodump", StringComparison.OrdinalIgnoreCase)
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
        private static bool EqualsImpl(this Media self, Media other)
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
        private static bool EqualsImpl(this Rom self, Rom other)
        {
            string? selfStatus = self.ReadString(Rom.StatusKey);
            string? otherStatus = other.ReadString(Rom.StatusKey);

            string? selfName = self.Name;
            string? otherName = other.Name;

            long? selfSize = self.ReadLong(Rom.SizeKey);
            long? otherSize = other.ReadLong(Rom.SizeKey);

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if (string.Equals(selfStatus, "nodump", StringComparison.OrdinalIgnoreCase)
                && string.Equals(otherStatus, "nodump", StringComparison.OrdinalIgnoreCase)
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
