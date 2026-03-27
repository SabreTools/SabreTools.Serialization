using System.Collections.Generic;

namespace SabreTools.Metadata.DatFiles
{
    public static class Extensions
    {
        #region Private Maps

        /// <summary>
        /// Set of enum to string mappings for MergingFlag
        /// </summary>
        private static readonly Dictionary<string, MergingFlag> _toMergingFlagMap = Converters.GenerateToEnum<MergingFlag>();

        /// <summary>
        /// Set of string to enum mappings for MergingFlag
        /// </summary>
        private static readonly Dictionary<MergingFlag, string> _fromMergingFlagMap = Converters.GenerateToString<MergingFlag>(useSecond: false);

        /// <summary>
        /// Set of string to enum mappings for MergingFlag (secondary)
        /// </summary>
        private static readonly Dictionary<MergingFlag, string> _fromMergingFlagSecondaryMap = Converters.GenerateToString<MergingFlag>(useSecond: true);

        /// <summary>
        /// Set of enum to string mappings for NodumpFlag
        /// </summary>
        private static readonly Dictionary<string, NodumpFlag> _toNodumpFlagMap = Converters.GenerateToEnum<NodumpFlag>();

        /// <summary>
        /// Set of string to enum mappings for NodumpFlag
        /// </summary>
        private static readonly Dictionary<NodumpFlag, string> _fromNodumpFlagMap = Converters.GenerateToString<NodumpFlag>(useSecond: false);

        /// <summary>
        /// Set of enum to string mappings for PackingFlag
        /// </summary>
        private static readonly Dictionary<string, PackingFlag> _toPackingFlagMap = Converters.GenerateToEnum<PackingFlag>();

        /// <summary>
        /// Set of string to enum mappings for PackingFlag
        /// </summary>
        private static readonly Dictionary<PackingFlag, string> _fromPackingFlagMap = Converters.GenerateToString<PackingFlag>(useSecond: false);

        /// <summary>
        /// Set of string to enum mappings for PackingFlag (secondary)
        /// </summary>
        private static readonly Dictionary<PackingFlag, string> _fromPackingFlagSecondaryMap = Converters.GenerateToString<PackingFlag>(useSecond: true);

        #endregion

        #region String to Enum

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, default on error</returns>
        public static MergingFlag AsMergingFlag(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value is null)
                return default;

            // Try to get the value from the mappings
            if (_toMergingFlagMap.TryGetValue(value, out MergingFlag mergingFlag))
                return mergingFlag;

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, default on error</returns>
        public static NodumpFlag AsNodumpFlag(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value is null)
                return default;

            // Try to get the value from the mappings
            if (_toNodumpFlagMap.TryGetValue(value, out NodumpFlag nodumpFlag))
                return nodumpFlag;

            // Otherwise, return the default value for the enum
            return default;
        }

        /// <summary>
        /// Get the enum value for an input string, if possible
        /// </summary>
        /// <param name="value">String value to parse/param>
        /// <returns>Enum value representing the input, default on error</returns>
        public static PackingFlag AsPackingFlag(this string? value)
        {
            // Normalize the input value
            value = value?.ToLowerInvariant();
            if (value is null)
                return default;

            // Try to get the value from the mappings
            if (_toPackingFlagMap.TryGetValue(value, out PackingFlag packingFlag))
                return packingFlag;

            // Otherwise, return the default value for the enum
            return default;
        }

        #endregion

        #region Enum to String

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <param name="useSecond">True to use the second mapping option, if it exists</param>
        /// <returns>String value representing the input, default on error</returns>
        public static string? AsStringValue(this MergingFlag value, bool useSecond = false)
        {
            // Try to get the value from the mappings
            if (!useSecond && _fromMergingFlagMap.TryGetValue(value, out string? mergingFlag))
                return mergingFlag;
            else if (useSecond && _fromMergingFlagSecondaryMap.TryGetValue(value, out string? mergingFlagSecondary))
                return mergingFlagSecondary;

            // Otherwise, return null
            return null;
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <param name="useSecond">True to use the second mapping option, if it exists</param>
        /// <returns>String value representing the input, default on error</returns>
        public static string? AsStringValue(this NodumpFlag value)
        {
            // Try to get the value from the mappings
            if (_fromNodumpFlagMap.TryGetValue(value, out string? nodumpFlag))
                return nodumpFlag;

            // Otherwise, return null
            return null;
        }

        /// <summary>
        /// Get the string value for an input enum, if possible
        /// </summary>
        /// <param name="value">Enum value to parse/param>
        /// <param name="useSecond">True to use the second mapping option, if it exists</param>
        /// <returns>String value representing the input, default on error</returns>
        public static string? AsStringValue(this PackingFlag value, bool useSecond = false)
        {
            // Try to get the value from the mappings
            if (!useSecond && _fromPackingFlagMap.TryGetValue(value, out string? packingFlag))
                return packingFlag;
            else if (useSecond && _fromPackingFlagSecondaryMap.TryGetValue(value, out string? packingFlagSecondary))
                return packingFlagSecondary;

            // Otherwise, return null
            return null;
        }

        #endregion
    }
}
