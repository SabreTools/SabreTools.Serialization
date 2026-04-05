using System.Collections.Generic;

namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Specialized dictionary base for item types
    /// </summary>
    public abstract class DictionaryBase : Dictionary<string, object?>
    {
        #region Read

        /// <summary>
        /// Read a key as the specified type, returning null on error
        /// </summary>
        public T? Read<T>(string key)
        {
            try
            {
                if (!ValidateReadKey(key))
                    return default;
                if (this[key] is not T)
                    return default;
                return (T?)this[key];
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Read a key as a bool, returning null on error
        /// </summary>
        /// TODO: Determine if this can be removed
        public bool? ReadBool(string key)
        {
            if (!ValidateReadKey(key))
                return null;

            bool? asBool = Read<bool?>(key);
            if (asBool is not null)
                return asBool;

            string? asString = Read<string>(key);
            return asString?.ToLowerInvariant() switch
            {
                "true" or "yes" => true,
                "false" or "no" => false,
                _ => null,
            };
        }

        /// <summary>
        /// Read a key as a double, returning null on error
        /// </summary>
        /// TODO: Determine if this can be removed
        public double? ReadDouble(string key)
        {
            if (!ValidateReadKey(key))
                return null;

            double? asDouble = Read<double?>(key);
            if (asDouble is not null)
                return asDouble;

            float? asFloat = Read<float?>(key);
            if (asFloat is not null)
                return asFloat;

#if NET5_0_OR_GREATER
            System.Half? asHalf = Read<System.Half?>(key);
            if (asHalf is not null)
                return (double?)asHalf;
#endif

            string? asString = Read<string>(key);
            if (asString is not null && double.TryParse(asString, out double asStringDouble))
                return asStringDouble;

            return null;
        }

        /// <summary>
        /// Read a key as a long, returning null on error
        /// </summary>
        /// <remarks>TODO: Add logic to convert SI suffixes and hex</remarks>
        /// TODO: Determine if this can be removed
        public long? ReadLong(string key)
        {
            if (!ValidateReadKey(key))
                return null;

            long? asLong = Read<long?>(key);
            if (asLong is not null)
                return asLong;

            int? asInt = Read<int?>(key);
            if (asInt is not null)
                return asInt;

            short? asShort = Read<short?>(key);
            if (asShort is not null)
                return asShort;

            string? asString = Read<string>(key);
            if (asString is not null && long.TryParse(asString, out long asStringLong))
                return asStringLong;

            return null;
        }

        /// <summary>
        /// Read a key as a string, returning null on error
        /// </summary>
        public string? ReadString(string key)
        {
            if (!ValidateReadKey(key))
                return null;

            string? asString = Read<string>(key);
            if (asString is not null)
                return asString;

            string[]? asArray = Read<string[]>(key);
            if (asArray is not null)
#if NETFRAMEWORK || NETSTANDARD2_0
                return string.Join(",", asArray);
#else
                return string.Join(',', asArray);
#endif

            // TODO: Add byte array conversion here
            // TODO: Add byte array read helper

            return this[key]!.ToString();
        }

        /// <summary>
        /// Read a key as a T[], returning null on error
        /// </summary>
        public T[]? ReadArray<T>(string key)
        {
            if (!ValidateReadKey(key))
                return null;

            var items = Read<T[]>(key);
            if (items is not null)
                return items;

            var single = Read<T>(key);
            if (single is not null)
                return [single];

            return null;
        }

        /// <summary>
        /// Read a key as a string[], returning null on error
        /// </summary>
        /// TODO: Determine if this can be removed
        public string[]? ReadStringArray(string key)
        {
            if (!ValidateReadKey(key))
                return null;

            string[]? asArray = Read<string[]>(key);
            if (asArray is not null)
                return asArray;

            string? asString = Read<string>(key);
            if (asString is not null)
                return [asString];

            asString = this[key]!.ToString();
            if (asString is not null)
                return [asString];

            return null;
        }

        /// <summary>
        /// Check if a key is valid for read
        /// </summary>
        private bool ValidateReadKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;
            else if (!ContainsKey(key))
                return false;
            else if (this[key] is null)
                return false;

            return true;
        }

        #endregion

        #region Write

        /// <summary>
        /// Remove a key, if possible
        /// </summary>
        public new bool Remove(string? fieldName)
        {
            // If the item or field name are missing, we can't do anything
            if (string.IsNullOrEmpty(fieldName))
                return false;

            // If the key doesn't exist, then it's already removed
            if (!ContainsKey(fieldName!))
                return true;

            // Remove the key
            base.Remove(fieldName!);
            return true;
        }

        /// <summary>
        /// Replace a field from another item
        /// </summary>
        public bool Replace(DictionaryBase? from, string fieldName)
        {
            // If the source item is invalid
            if (from is null)
                return false;

            // If the field name is missing, we can't do anything
            if (string.IsNullOrEmpty(fieldName))
                return false;

            // If the types of the items are not the same, we can't do anything
            if (from.GetType() != GetType())
                return false;

            // If the key doesn't exist in the source, we can't do anything
            if (!from.TryGetValue(fieldName!, out var value))
                return false;

            // Set the key
            this[fieldName!] = value;
            return true;
        }

        /// <summary>
        /// Write a key as the specified type, returning false on error
        /// </summary>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">Value to set</param>
        /// <returns>True if the value was set, false otherwise</returns>
        public bool Write<T>(string fieldName, T? value)
        {
            // Invalid field cannot be processed
            if (fieldName is null)
                return false;

            // Set the value based on the type
            this[fieldName] = value;
            return true;
        }

        #endregion
    }
}
